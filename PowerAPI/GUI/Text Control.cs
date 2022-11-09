using System;
using PowerAPI.Extensions;

namespace PowerAPI.GUI
{
    public class TextControl : Control
    {
        public TextControl(Window parent, string text = "TextControl") : base(parent) { Text = text; }

        public override void Draw(IntPtr hWnd, IntPtr hDC, uint drawArgs) { Drawing.DrawText(hWnd, Text, Location); }
    }
}