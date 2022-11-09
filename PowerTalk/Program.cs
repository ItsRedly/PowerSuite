using System.Drawing;
using System.Linq.Expressions;
using PowerAPI.GUI;

namespace PowerTalk {
    public static class Program {
        public static void Main() {
            Window window = new Window("PowerTalk");
            window.BackgroundColor = Color.LightBlue;
            window.Controls.Add(new TextControl(window, window.Title) { Anchor = (AnchorX.Center, AnchorY.Top) });
            window.Controls.Add(new TextControl(window, "UR MUM!!!!") { Location = new(69, 69) });
            window.Controls.Add(new TextControl(window, "X") { Location = new(0, 64), Anchor = (AnchorX.Left, AnchorY.Bottom) });
            Bitmap b = new Bitmap(350, 350);
            Graphics g = Graphics.FromImage(b);
            g.DrawEllipse(new Pen(Color.AliceBlue), 0, 0, b.Width, b.Height);
            window.Controls.Add(new ImageControl(window, b) { Size = new(350, 350), Location = new(50, 50) });
            window.Show();
        }
    }
}