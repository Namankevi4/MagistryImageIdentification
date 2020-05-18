using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Configuration;
using Infrasctructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Magistry_Image_Identification.Controllers
{
    public class ActionsController : Controller
    {
        private readonly ImageService _imageService;
        private readonly IOptionsMonitor<ConfigSettings> _settings;

        public ActionsController(ImageService imageService, IOptionsMonitor<ConfigSettings> settings)
        {
            _settings = settings;
            _imageService = imageService;
        }

        public IActionResult Index()
        {
            ViewData["imageStorePath"] = _settings.CurrentValue.PathToImagesFolder + "/";
            ViewData["countOfImages"] = _imageService.GetCount();
            return View();
        }

        [HttpPost]
        public IActionResult AddImages(string imageOrFolderPath)
        {
            FileAttributes attr = System.IO.File.GetAttributes(imageOrFolderPath);

            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                _imageService.InsertAllImagesFromFolder(imageOrFolderPath);
            }
            else
            {
                _imageService.InsertImage(imageOrFolderPath);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteImage(string imageName)
        {
            _imageService.DeleteByImagePath(imageName);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult DeleteAll()
        {
            _imageService.DeleteAll();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public PartialViewResult ActionView(string actionType)
        {
            if (actionType == "Add")
            {
                return PartialView("AddImages");
            }

            if (actionType == "Delete")
            {
                return PartialView("DeleteImages");
            }

            return null;
        }
    }
}