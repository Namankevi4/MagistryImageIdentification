using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Magistry_Image_Identification.Models
{
    public class SearchResultViewModel
    {
        public bool IsFound { get; set; }
        public string RelativeImgPath { get; set; }
        [DisplayName("Time spent")]
        public TimeSpan ElapsedTime { get; set; }
        [DisplayName("Count of image in store")]
        public long CountOfImages { get; set; }
    }
}
