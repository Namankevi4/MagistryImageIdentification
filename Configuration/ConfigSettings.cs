using System;
using System.Collections.Generic;
using System.Text;

namespace Configuration
{
    public class ConfigSettings
    {
        public ConfigSettings()
        {

        }

        public string PathToImagesFolder { get; set; }
        public string DescriptorMatcherType { get; set; }
        public string ConnectionStringDb { get; set; }
    }
}
