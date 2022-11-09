using System.Runtime.InteropServices;
using System.Drawing;

namespace PowerAPI.GUI
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ColorReference
    {
        public uint ColorDWORD;
        public ColorReference(Color color) { ColorDWORD = (uint) color.R + (((uint) color.G) << 8) + (((uint) color.B) << 16); }
        public Color GetColor() { return Color.FromArgb((int) (0x000000FFU & ColorDWORD), (int) (0x0000FF00U & ColorDWORD) >> 8, (int) (0x00FF0000U & ColorDWORD) >> 16); }
        public void SetColor(Color color) { ColorDWORD = (uint) color.R + (((uint) color.G) << 8) + (((uint) color.B) << 16); }
    }
}