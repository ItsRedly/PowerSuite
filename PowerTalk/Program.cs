using System.Drawing;
using PowerAPI;

namespace PowerTalk {
    public static class Program {
        public static void Main() {
            Window window = new Window("PowerTalk");
            window.BackgroundColor = Color.LightBlue;
            window.Controls.Add(new TextControl("lol") { IsHCentered = true });
            window.Controls.Add(new TextControl("TUA MÃE!!!!") { Location = new Point(69, 69) });
            window.Show();
        }
    }
}