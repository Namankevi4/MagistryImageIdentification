using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Magistry_Image_Identification.Models
{
    public class AddImageViewModel
    {
        [DisplayName("Enter image or folder path")]
        public string ImageOrFolderPath { get; set; }
    }
}
