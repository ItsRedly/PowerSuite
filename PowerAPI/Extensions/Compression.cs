using ICSharpCode.SharpZipLib.Zip;
using System.IO;

namespace PowerAPI.Extensions
{
    public static class Compression
    {
        public static void ExtractZipFile(string archivePath, string outFolder, string password = "")
        {
            FastZip fz = new FastZip();
            if (!string.IsNullOrEmpty(password)) { fz.Password = password; }
            fz.ExtractZip(archivePath, outFolder, password);
        }
    }
}