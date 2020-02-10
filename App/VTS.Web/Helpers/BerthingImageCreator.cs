using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace VTS.Web.Helpers
{
    public class BerthingImageCreator
    {
        public static (string FileName, bool Result) CreateImage(List<VTS.Shared.VesselData> Data, int Width = 900, int Height = 480)
        {
            bool result = false;
            string ImagePath = Path.GetTempFileName() + ".jpg";

            var resized = new Bitmap(Width, Height);
            using (var graphics = Graphics.FromImage(resized))
            {
                graphics.Clear(Color.White);
                graphics.CompositingQuality = CompositingQuality.HighSpeed;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.DrawImage(resized, 0, 0, Width, Height);
                
               
            }
            resized.Save(ImagePath, ImageFormat.Png);
            Console.WriteLine($"Saving image -> {ImagePath}");

            return (ImagePath, result);
        }
    }
}
