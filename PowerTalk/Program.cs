using System.Drawing;
using PowerAPI;

namespace PowerTalk {
    public static class Program {
        public static void Main() {
            Window window = new Window("PowerTalk", false, false, new(0, 0), new());
            window.BackgroundColor = Color.LightBlue;
            window.Controls.Add(new TextControl(window.Title) { IsCentered = true });
            window.Controls.Add(new TextControl("UR MUM!!!!") { Location = new Point(69, 69) });
            window.Controls.Add(new TextControl("X") { Location = new Point(64, 64)});
            window.Show();
        }
    }
}