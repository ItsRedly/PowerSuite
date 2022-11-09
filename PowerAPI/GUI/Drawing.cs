using System;
using System.Drawing;
using System.Reflection;
using System.Reflection.Emit;
using PowerAPI.Constants;
using PowerAPI.Extensions;

namespace PowerAPI.GUI
{
    public static class Drawing {
        public static void DrawText(IntPtr hDC, string text, Point location) {
            PInvokes.GetTextExtentPoint32(hDC, text, text.Length, out Size size);
            Rectangle rect = new Rectangle(location, size);
            rect.Location = location;
            PInvokes.DrawText(hDC, text, -1, ref rect, Win32DTConstants.DT_SINGLELINE);
        }

        public static void DrawRect(IntPtr hDC, Point location, Size size, Color color) {
            Rectangle rect = new Rectangle(location, size);
            PInvokes.FillRect(hDC, ref rect, PInvokes.CreateSolidBrush(new ColorReference(color)));
        }

        public static void DrawImage(IntPtr hDC, Point location, Size size, Bitmap image) {
            Rectangle rect = new Rectangle(location, size);
            PInvokes.FillRect(hDC, ref rect, PInvokes.CreatePatternBrush(image.GetHbitmap()));
        }
    }
}