using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrasctructure.Converters;
using Infrasctructure.Models;
using Infrasctructure.Services;

namespace Infrasctructure.OpenCvFunctionality
{ 
    public class OpenCVMeasures
    {
        private DescriptorMatherWrapper _descriptorMatherWrapper;
        private ImageService _imageService;
        public OpenCVMeasures(DescriptorMatherWrapper descriptorMatherWrapper, ImageService imageService)
        {
            _descriptorMatherWrapper = descriptorMatherWrapper;
            _imageService = imageService;
        }

        public string IdentifyImage(string imagePath)
        {
            var imageObject = _imageService.ConvertImagePathToImageModel(imagePath);

            var matches = _descriptorMatherWrapper.Match(imageObject.Descriptor);
            var imgIndx = matches.Select(x => x.ImgIdx).ToList();

            var imgIndxGrouped = imgIndx.GroupBy(x => x).ToList();

            imgIndx = imgIndxGrouped.Select(x => x.Key).Take(20).ToList();

            var imgIds =_descriptorMatherWrapper.GetImgIdByImgIndex(imgIndx);

            var imageScenes = _imageService.GetAllByIds(imgIds);

            var bestImagePath = _descriptorMatherWrapper.FindBestMatchFromScopeOfImages(imageObject, imageScenes);

            return bestImagePath;
        }
    }
}