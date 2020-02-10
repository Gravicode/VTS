using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace VTS.Web.Helpers
{
    public class ImageConverter
    {
        public static string getByteImageHTML(string ImagePath)
        {

            // Get image path  
            if (!File.Exists(ImagePath)) return null;
            // Convert image to byte array  
            byte[] byteData = System.IO.File.ReadAllBytes(ImagePath);
            // Convert byte arry to base64string   
            string imreBase64Data = Convert.ToBase64String(byteData);
            string imgDataURL = string.Format("data:image/png;base64,{0}", imreBase64Data);
            // Passing image data in viewbag to view  
            return imgDataURL;


        }
    }
}
