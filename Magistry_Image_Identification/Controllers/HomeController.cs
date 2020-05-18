using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Configuration;
using Infrasctructure.OpenCvFunctionality;
using Infrasctructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Magistry_Image_Identification.Models;
using Microsoft.Extensions.Options;
using System.Timers;

namespace Magistry_Image_Identification.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ImageService _imageService;
        private readonly OpenCVMeasures _openCvMeasures;
        private readonly IOptionsMonitor<ConfigSettings> _settings;

        public HomeController(ILogger<HomeController> logger, ImageService imageService, OpenCVMeasures openCvMeasures, IOptionsMonitor<ConfigSettings> settings)
        {
            _settings = settings;
            _openCvMeasures = openCvMeasures;
            _imageService = imageService;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult FindImage(string imagePath)
        {
            Stopwatch stopWatch = new Stopwatch();

            stopWatch.Start();
            var identifyingImagePath = _openCvMeasures.IdentifyImage(imagePath);
            stopWatch.Stop();

            var elapsedTime = stopWatch.Elapsed;

            var searchResultModel = new SearchResultViewModel();

            if (!string.IsNullOrEmpty(identifyingImagePath))
            {
                var imageName = Path.GetFileName(identifyingImagePath);
                var relativeImgPath = "~/" + _settings.CurrentValue.PathToImagesFolder + "/" + imageName;
                searchResultModel.IsFound = true;
                searchResultModel.RelativeImgPath = relativeImgPath;
                searchResultModel.ElapsedTime = elapsedTime;
                searchResultModel.CountOfImages = _imageService.GetCount();
            }

            return View("SearchResult", searchResultModel);
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
