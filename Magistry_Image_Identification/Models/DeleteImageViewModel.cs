using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Magistry_Image_Identification.Models
{
    public class DeleteImageViewModel
    {
        [DisplayName("Enter image name for delete")]
        public string ImageName { get; set; }
    }
}
