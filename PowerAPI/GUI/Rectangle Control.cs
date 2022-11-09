using System.Drawing;
using PowerAPI.Extensions;

namespace PowerAPI.GUI
{
    public class RectangleControl : Control {
        public Color Color;
        public RectangleControl(Window parent) : base(parent) { }
        public override void Draw(IntPtr hWnd, IntPtr hDC, uint drawArgs) { Drawing.DrawRect(hDC, Location, Size, Color); }
    }
}