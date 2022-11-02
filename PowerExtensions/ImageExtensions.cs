namespace PowerExtensions
{
    public static class ImageExtensions
    {
        public static Image ConvertBase64ToImage(this string base64string)
        {
            byte[] imageBytes = Convert.FromBase64String(base64string);
            using (MemoryStream ms = new(imageBytes, 0, imageBytes.Length))
            {
                ms.Write(imageBytes, 0, imageBytes.Length);
                return Image.FromStream(ms, true);
            }
        }

        public static string ConvertImageToBase64(this Image image)
        {
            using (MemoryStream memoryStream = new())
            {
                image.Save(memoryStream, image.RawFormat);
                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }
    }
}
