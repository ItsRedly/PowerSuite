using System.Drawing;
using PowerAPI;

namespace PowerTalk {
    public static class Program {
        public static void Main() {
            Window window = new Window();
            window.BackgroundColor = Color.Black;
            window.Title = "PowerTalk";
            window.Controls.Add(new TextControl("lol") { IsHCentered = true });
            window.Show();
        }
    }
}