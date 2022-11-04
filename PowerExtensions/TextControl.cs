namespace PowerExtensions
{
    public class TextControl : Control
    {
        public bool IsHCentered = false;
        public bool IsVCentered = false;
        public override void Draw(IntPtr hWnd, IntPtr hdc)
        {
            RECT rect;
            WinAPI.GetClientRect(hWnd, out rect);
            uint drawArgs = Win32DTConstant.DT_SINGLELINE;
            if (IsHCentered) { drawArgs |= Win32DTConstant.DT_CENTER; }
            else { rect.X = Location.X; }
            if (IsVCentered) { drawArgs |= Win32DTConstant.DT_VCENTER; }
            else { rect.Y = Location.Y; }
            WinAPI.DrawText(hdc, Text, -1, ref rect, drawArgs);
        }
    }
}