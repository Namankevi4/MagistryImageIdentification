using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ImageLogic
{
    public static class ImageHelper
    {
        public static bool[] GetImageHash(string imagePath)
        {
            Bitmap bmpSource = new Bitmap(imagePath);
            List<bool> lResult = new List<bool>();
            //create new image with 16x16 pixel
            Bitmap bmpMin = new Bitmap(bmpSource, new Size(16, 16));
            for (int j = 0; j < bmpMin.Height; j++)
            {
                for (int i = 0; i < bmpMin.Width; i++)
                {
                    //reduce colors to true / false                
                    lResult.Add(bmpMin.GetPixel(i, j).GetBrightness() < 0.5f);
                }
            }
            return lResult.ToArray();
        }
    }
}
