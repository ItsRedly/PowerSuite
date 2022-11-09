using System.Diagnostics;
using System.IO;
using System.Security.Principal;

namespace PowerAPI.Extensions
{
    public static class ApplicationExtensions {
        public static string GetApplicationLocation() { return Path.GetDirectoryName(GetApplicationPath()); }
        public static string GetApplicationFileName() { return Path.GetFileName(GetApplicationPath()); }
        public static string GetApplicationFileNameWithoutExtension() { return Path.GetFileNameWithoutExtension(GetApplicationPath()); }
        public static string GetApplicationPath() { return Process.GetCurrentProcess().MainModule.FileName; }
        
        public static bool IsRunningAsAdmin() {
            try { return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator); }
            catch { return false; }
        }
    }
}