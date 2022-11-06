using System.Drawing;
using PowerAPI;

namespace PowerTalk {
    public static class Program {
        public static void Main() {
            Window window = new Window("PowerTalk");
            window.BackgroundColor = Color.LightBlue;
            window.Controls.Add(new TextControl(window.Title) { IsHCentered = true });
            window.Controls.Add(new TextControl("UR MUM!!!!") { Location = new Point(69, 69) });
            window.Controls.Add(new TextControl("UR MUM!!!!") { Location = new Point(420, 420) });
            window.Show();
        }
    }
}

//