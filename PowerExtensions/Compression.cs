using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace PowerExtensions
{
    public static class Compression
    {
        public static void ExtractZipFile(string archivePath, string outFolder, string password = "")
        {
            using (FileStream fsInput = File.OpenRead(archivePath))
            using (ZipFile zf = new(fsInput))
            {
                if (!string.IsNullOrEmpty(password)) { zf.Password = password; }
                foreach (ZipEntry zipEntry in zf)
                {
                    if (!zipEntry.IsFile) { continue; }
                    string entryFileName = zipEntry.Name;
                    var fullZipToPath = Path.Combine(outFolder, entryFileName);
                    var directoryName = Path.GetDirectoryName(fullZipToPath);
                    if (directoryName.Length > 0) { Directory.CreateDirectory(directoryName); }
                    byte[] buffer = new byte[4096];
                    using (var zipStream = zf.GetInputStream(zipEntry))
                    using (Stream fsOutput = File.Create(fullZipToPath)) { StreamUtils.Copy(zipStream, fsOutput, buffer);}
                }
            }
        }
    }
}