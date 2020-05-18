using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Configuration;
using DatabaseLogic;
using Infrasctructure.Converters;
using Infrasctructure.Models;
using Infrasctructure.Services;
using Microsoft.Extensions.Options;
using OpenCvSharp;

namespace Infrasctructure.OpenCvFunctionality
{
    public class DescriptorMatherWrapper
    {
        private readonly IOptionsMonitor<ConfigSettings> _settings;
        private readonly DescriptorMatcher _matcher;

        private readonly object _lockObjectCalculating = new object();

        private readonly IDictionary<int, string> _indexIdMapImage;

        private int _index;

        public DescriptorMatherWrapper(IOptionsMonitor<ConfigSettings> settings)
        {
            _indexIdMapImage = new Dictionary<int, string>();

            _settings = settings;
            _matcher = DescriptorMatcher.Create(_settings.CurrentValue.DescriptorMatcherType);
        }

        public IList<string> GetImgIdByImgIndex(IList<int> imgIndx)
        {
            var imgIds = _indexIdMapImage.Where(x => imgIndx.Contains(x.Key)).Select(x => x.Value).ToList();
            return imgIds;
        }

        public DMatch[] Match(float[,] descriptorImage)
        {
            var descriptor = ConvertOpenCVTypes.ConvertFloatArrayToMat(descriptorImage);
            if (!_matcher.Empty())
            {
                _matcher.Train();
            }

            return _matcher.Match(descriptor);
        }

        public string FindBestMatchFromScopeOfImages(ImageModel imageObject, IList<ImageModel> imageScenes)
        {
            IList<DMatch> bestMatches = new List<DMatch>();
            string imagePath = string.Empty;

            var descriptorObject = ConvertOpenCVTypes.ConvertFloatArrayToMat(imageObject.Descriptor);

            Parallel.ForEach(imageScenes, (imageScene) =>
            {
                var descriptorScene = ConvertOpenCVTypes.ConvertFloatArrayToMat(imageScene.Descriptor);

                var knnMatches = _matcher.KnnMatch(descriptorObject, descriptorScene, 2).ToList();

                const float ratioThresh = 0.7f;
                List<DMatch> goodMatches = new List<DMatch>();

                for (int i = 0; i < knnMatches.Count; i++)
                {
                    if (knnMatches[i][0].Distance < ratioThresh * knnMatches[i][1].Distance)
                    {
                        goodMatches.Add(knnMatches[i][0]);
                    }
                }
                lock (_lockObjectCalculating)
                {
                    if (goodMatches.Count > 100 && goodMatches.Count > bestMatches.Count)
                    {
                        bestMatches = goodMatches;
                        imagePath = imageScene.ImagePath;
                    }
                }
            });

            return imagePath;
        }

        public void Startup(IList<ImageModel> imagesForTraining)
        {
            if (!imagesForTraining.Any())
            {
                return;
            }

            AddImagesToTrain(imagesForTraining);
        }

        public void AddImagesToTrain(IList<ImageModel> models)
        {
            var descriptionsScene =
                models.Select(x =>
                {
                    _indexIdMapImage.Add(_index, x.Id);
                    _index++;
                    return ConvertOpenCVTypes.ConvertFloatArrayToMat(x.Descriptor);
                });
            _matcher.Add(descriptionsScene);
        }

        public void ClearTrainCollection()
        {
            _matcher.Clear();
            _index = 0;
            _indexIdMapImage.Clear();
        }
    }
}
