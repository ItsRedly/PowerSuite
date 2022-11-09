using PowerAPI;
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Runtime.Versioning;

namespace PowerWin {
    public static class Program {
        public static void Main() {
            Window window = new Window("PowerWin", true, true);
            window.UseWholeMonitor = true;
            window.BackgroundColor = Color.LightBlue;
            window.Controls.Add(new ButtonControl(window, "Shutdown") { Location = new(12, 412), Size = new(86, 28) }) ;
            window.Controls.Add(new TextControl(window, "AccountName") { Location = new(368, 235) });
            window.Controls.Add(new TextControl(window, "Password") { Location = new(350, 267), Size = new(137, 20) });
            window.Controls.Add(new ImageControl(window, "AccountIcon") { Location = new(350, 100), Size = new(137, 129) });
            window.Controls.Add(new ImageControl(window, "BackgroundImg") { Location = new(-35, 0), Size = new(869, 452), Properties.Resources });
            window.Show();
        }
    }
}