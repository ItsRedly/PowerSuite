using System;
using System.Drawing;
using System.Runtime.InteropServices;
using PowerAPI.GUI;

namespace PowerAPI.Extensions
{
    internal class PInvokes
    {
        [DllImport("gdi32.dll")]
        public static extern bool GetTextExtentPoint32(IntPtr hDC, string lpString, int cbString, out Size lpSize);
        [DllImport("user32.dll")]
        public static extern IntPtr DispatchMessage([In] ref Message lpMsg);
        [DllImport("user32.dll")]
        public static extern bool TranslateMessage([In] ref Message lpMsg);
        [DllImport("user32.dll")]
        public static extern sbyte GetMessage(out Message lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);
        [DllImport("user32.dll", SetLastError = true, EntryPoint = "CreateWindowEx")]
        public static extern IntPtr CreateWindowEx(WindowStylesEx dwExStyle, string lpClassName, string lpWindowName, WindowStyles dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);
        [DllImport("user32.dll", SetLastError = true, EntryPoint = "CreateWindowEx")]
        public static extern IntPtr CreateWindowEx2(WindowStylesEx dwExStyle, UInt16 lpClassName, string lpWindowName, WindowStyles dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);
        [DllImport("user32.dll")]
        public static extern ushort RegisterClass([In] ref WndClass lpWndClass);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(IntPtr hWnd, ShowWindowType nCmdShow);
        [DllImport("user32.dll")]
        public static extern bool CloseWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern IntPtr BeginPaint(IntPtr hWnd, out PaintStruct lpPaint);
        [DllImport("user32.dll")]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, MessageType uMsg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hWnd, out Rectangle lpRect);
        [DllImport("user32.dll")]
        public static extern int DrawText(IntPtr hDC, string lpString, int nCount, ref Rectangle lpRect, uint uFormat);
        [DllImport("user32.dll")]
        public static extern int FillRect(IntPtr hDC, [In] ref Rectangle lprc, IntPtr hbr);
        [DllImport("user32.dll")]
        public static extern bool EndPaint(IntPtr hWnd, [In] ref PaintStruct lpPaint);
        [DllImport("user32.dll")]
        public static extern void PostQuitMessage(int nExitCode);
        [DllImport("user32.dll")]
        public static extern IntPtr LoadIcon(IntPtr hInstance, string lpIconName);
        [DllImport("user32.dll")]
        public static extern IntPtr LoadIcon(IntPtr hInstance, IntPtr lpIConName);
        [DllImport("user32.dll")]
        public static extern MessageBoxResult MessageBox(IntPtr hWnd, string text, string caption, MessageBoxOptions options);
        [DllImport("user32.dll")]
        public static extern bool UpdateWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);
        [DllImport("user32.dll")]
        public static extern short RegisterClassEx([In] ref WndClassEx lpwcx);
        [DllImport("user32.dll", SetLastError = true, EntryPoint = "RegisterClassEx")]
        public static extern UInt16 RegisterClassEx2([In] ref WndClassEx lpwcx);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SetWindowText(IntPtr hWnd, String lpString);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, UInt32 uFlags);
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);
        [DllImport("user32.dll")]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("gdi32.dll", EntryPoint="CreateSolidBrush", SetLastError=true)]
        public static extern IntPtr CreateSolidBrush(ColorReference crColor);
        [DllImport("gdi32.dll")]
        public static extern uint SetClassLongPtr32(IntPtr hWnd, ClassLongFlags nIndex, uint dwNewLong);
        [DllImport("user32.dll", EntryPoint="SetClassLongPtr")]
        public static extern IntPtr SetClassLongPtr64(IntPtr hWnd, ClassLongFlags nIndex, IntPtr dwNewLong);
        [DllImport("user32.dll")]
        public extern static void InvalidateRect(IntPtr handle, Rectangle rect, bool erase);
        [DllImport("gdi32.dll")]
        public static extern int SetBkMode(IntPtr hdc, int iBkMode);
        [DllImport("gdi32.dll")]
        public static extern uint SetBkColor(IntPtr hdc, ColorReference crColor);
        [DllImport("user32.dll", SetLastError = false)]
        public static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreatePatternBrush(IntPtr hbmp);
        public static IntPtr SetClassLongPtr(IntPtr hWnd, ClassLongFlags nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size > 4) { return SetClassLongPtr64(hWnd, nIndex, dwNewLong); }
            else { return new IntPtr(SetClassLongPtr32(hWnd, nIndex, unchecked((uint)dwNewLong.ToInt32()))); }
        }

        public static Point GetPointFromInt(IntPtr value)
        {
            uint xy = unchecked(IntPtr.Size == 8 ? (uint)value.ToInt64() : (uint)value.ToInt32());
            int x = unchecked((short)xy);
            int y = unchecked((short)(xy >> 16));
            return new(x, y);
        }
    }
}