using System.Drawing;

namespace PowerAPI.GUI
{
    public class ImageControl : Control {
        Bitmap Image;
        public ImageControl(Window parent, Bitmap image) : base(parent) { Image = image; }

        public override void Draw(IntPtr hWnd, IntPtr hDC, uint drawArgs) { Drawing.DrawImage(hDC, Location, Size, Image); }
    }
}