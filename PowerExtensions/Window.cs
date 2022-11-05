using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PowerExtensions
{
    public class Window
    {
        IntPtr hwnd;
        public List<Control> Controls = new List<Control>();
        public Point Location = new(50, 50);
        public Size Size = new(500, 500);
        public string Title = "Untitled window";
        public bool Running = false;

        public Window(string title = "Untitled window")
        {
            Title = title;
            IntPtr hInstance = Process.GetCurrentProcess().Handle;
            WNDCLASSEX wndClass = new WNDCLASSEX();
            wndClass.cbSize = Marshal.SizeOf(typeof(WNDCLASSEX));
            wndClass.style = (int)(ClassStyles.HorizontalRedraw | ClassStyles.VerticalRedraw);
            wndClass.lpfnWndProc = Marshal.GetFunctionPointerForDelegate((WndProc)((hWnd, message, wParam, lParam) => {
                try
                {
                    IntPtr result = HandleMessage((WM)message, hWnd);
                    return result == new IntPtr(-1) ? WinAPI.DefWindowProc(hWnd, (WM)message, wParam, lParam) : result;
                }
                catch { return WinAPI.DefWindowProc(hWnd, (WM)message, wParam, lParam); }
            }));
            wndClass.cbClsExtra = 0;
            wndClass.cbWndExtra = 0;
            wndClass.hInstance = hInstance;
            wndClass.hCursor = WinAPI.LoadCursor(IntPtr.Zero, (int)Win32IDCConstants.IDC_ARROW);
            wndClass.hbrBackground = WinAPI.GetStockObject(StockObjects.WHITE_BRUSH);
            wndClass.lpszMenuName = null;
            wndClass.lpszClassName = Title;
            UInt16 regRest = WinAPI.RegisterClassEx2(ref wndClass);
            hwnd = WinAPI.CreateWindowEx2(0, regRest, title, WindowStyles.WS_OVERLAPPEDWINDOW, -1, -1, -1, -1, IntPtr.Zero, IntPtr.Zero, hInstance, IntPtr.Zero);
        }

        public Window(Point startLoc, string title = "Untitled window") : this(title) { Location = startLoc; }

        public IntPtr HandleMessage(WM message, IntPtr hWnd)
        {
            switch (message)
            {
                case WM.PAINT:
                    PAINTSTRUCT ps;
                    IntPtr hdc = WinAPI.BeginPaint(hWnd, out ps);
                    Controls.ForEach((Control control) =>
                    {
                        if (control.IsEnabled) { control.Draw(hWnd, hdc); }
                    });
                    WinAPI.EndPaint(hWnd, ref ps);
                    return IntPtr.Zero;
                case WM.DESTROY:
                    WinAPI.PostQuitMessage(0);
                    return IntPtr.Zero;
            }
            return new IntPtr(-1);
        }

        public void Show()
        {
            Running = true;
            WinAPI.ShowWindow(hwnd, ShowWindowCommands.Normal);
            WinAPI.UpdateWindow(hwnd);
            WinAPI.UpdateWindow(hwnd);
            WinAPI.SetWindowPos(hwnd, new IntPtr(0), Location.X, Location.Y, Size.Width, Size.Height, 0);
            MSG msg;
            while (WinAPI.GetMessage(out msg, IntPtr.Zero, 0, 0) != 0 && Running)
            {
                WinAPI.SetWindowText(hwnd, Title);
                WinAPI.TranslateMessage(ref msg);
                WinAPI.DispatchMessage(ref msg);
            }
        }

        public void Hide()
        {
            Running = false;
            WinAPI.ShowWindow(hwnd, ShowWindowCommands.Hide);
        }
    }
}