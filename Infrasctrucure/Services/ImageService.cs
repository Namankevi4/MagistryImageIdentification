using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Configuration;
using DatabaseLogic;
using ImageLogic;
using Infrasctructure.Converters;
using Infrasctructure.Models;
using Infrasctructure.OpenCvFunctionality;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using OpenCvSharp;
using OpenCvSharp.XFeatures2D;

namespace Infrasctructure.Services
{
    public class ImageService
    {
        private readonly ImageDatabase _imageDatabase;
        private readonly DescriptorMatherWrapper _descriptorMatherWrapper;
        private readonly IOptionsMonitor<ConfigSettings> _settings;
        private readonly IHostingEnvironment _hostingEnvironment;

        public ImageService(ImageDatabase imageDatabase, DescriptorMatherWrapper descriptorMatherWrapper, IOptionsMonitor<ConfigSettings> settings,
            IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            _imageDatabase = imageDatabase;
            _settings = settings;
            _descriptorMatherWrapper = descriptorMatherWrapper;
        }

        public IList<ImageModel> GetAll()
        {
            return Task.Run(async () => await _imageDatabase.GetAll()).Result;
        }

        public long GetCount()
        {
            return Task.Run(async () => await _imageDatabase.GetCount()).Result;
        }

        public void DeleteAll()
        {
            Task.Run(async () => await _imageDatabase.DeleteAll()).Wait();

            var imageStorage = Path.Combine(_hostingEnvironment.WebRootPath, _settings.CurrentValue.PathToImagesFolder);

            System.IO.DirectoryInfo di = new DirectoryInfo(imageStorage);

            foreach (FileInfo file in di.GetFiles("*.jpg*"))
            {
                file.Delete();
            }

            _descriptorMatherWrapper.ClearTrainCollection();
        }

        public void DeleteByImagePath(string fileName)
        {
            var imageStorage = Path.Combine(_hostingEnvironment.WebRootPath, _settings.CurrentValue.PathToImagesFolder);

            var filePath = Path.Combine(imageStorage, fileName);

            Task.Run(async () => await _imageDatabase.DeleteByPath(filePath)).Wait();

            File.Delete(filePath);
        }

        public void InsertAllImagesFromFolder(string folderPath)
        {
            var filePaths = Directory.GetFiles(folderPath, "*.jpg*", SearchOption.AllDirectories)
                .ToList();

            foreach (var filePath in filePaths)
            {
                InsertImage(filePath);
            }
        }

        public void InsertImage(string imagePath)
        {
            var fileName = Path.GetFileName(imagePath);

            var imageStorage = Path.Combine(_hostingEnvironment.WebRootPath, _settings.CurrentValue.PathToImagesFolder);

            var newFilePath = Path.Combine(imageStorage, fileName);

            if (File.Exists(newFilePath))
            {
                return;
            }

            var imageModel = ConvertImagePathToImageModel(imagePath);
            imageModel.ImagePath = newFilePath;

            var isCreated = Task.Run(async () => await _imageDatabase.Create(imageModel)).Result;

            if (isCreated)
            {
                _descriptorMatherWrapper.AddImagesToTrain(new List<ImageModel>() { imageModel });
                File.Copy(imagePath, newFilePath);
            }
        }

        public IList<ImageModel> GetAllByIds(IList<string> ids)
        {
            return Task.Run(async () => await _imageDatabase.GetAllByIds(ids)).Result;
        }

        public ImageModel ConvertImagePathToImageModel(string imagePath)
        {
            Mat imgObject = Cv2.ImRead(imagePath, ImreadModes.Grayscale);

            Mat descriptorsObject = new Mat();
            double minHessian = 400;

            SURF detector = SURF.Create(minHessian, extended: false);

            var keypointsObject = detector.Detect(imgObject).Take(1000).ToArray();

            detector.Compute(imgObject, ref keypointsObject, descriptorsObject);

            var imageModel = new ImageModel()
            {
                ImagePath = imagePath,
                ImageHash = ImageHelper.GetImageHash(imagePath),
                Descriptor = ConvertOpenCVTypes.ConvertMatToFloatArray(descriptorsObject)
            };

            return imageModel;
        }
    }
}
