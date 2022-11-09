using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace PowerAPI.Extensions
{
    public static class BitmapExtensions
    {
        public static Bitmap ConvertBase64ToBitmap(this string base64string)
        {
            byte[] imageBytes = Convert.FromBase64String(base64string);
            using (MemoryStream ms = new(imageBytes, 0, imageBytes.Length))
            {
                ms.Write(imageBytes, 0, imageBytes.Length);
                return new Bitmap(ms);
            }
        }

        public static string ConvertBitmapToBase64(this Bitmap image)
        {
            using (MemoryStream memoryStream = new())
            {
                image.Save(memoryStream, ImageFormat.Jpeg);
                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }
    }
}