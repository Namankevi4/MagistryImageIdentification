using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Magistry_Image_Identification.Models
{
    public class ImageViewModel
    {
        [DisplayName("Enter image path")]
        public string ImagePath { get; set; }
    }
}
