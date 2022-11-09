using System;
using System.Drawing;
using System.Collections.ObjectModel;
using System.Diagnostics;
using PowerAPI.Extensions;
using PowerAPI.Constants;
using System.Runtime.InteropServices;

namespace PowerAPI.GUI
{
    public class Window
    {
        IntPtr hWnd;
        public ObservableCollection<Control> Controls = new();
        public Point Location = new(0, 0);
        public Size Size = new(500, 500);
        public string Title = "Untitled Window";
        public bool Running = false;
        public bool IsFullscreen = false;
        public Color BackgroundColor = Color.White;

        public Window(string title = "Untitled Window") {
            Size = new(500, 500);
            Location = Point.Empty;
            Title = title;
            WndClassEx wndClass = new() { cbSize = Marshal.SizeOf(typeof(WndClassEx)), style = (int)(ClassStyles.HorizontalRedraw | ClassStyles.VerticalRedraw), lpfnWndProc = Marshal.GetFunctionPointerForDelegate((WndProc)((hWnd, message, wParam, lParam) => {
                try
                {
                    IntPtr result = HandleMessage((MessageType)message, hWnd, wParam, lParam);
                    return result == new IntPtr(-1) ? PInvokes.DefWindowProc(hWnd, (MessageType)message, wParam, lParam) : result;
                }
                catch { return PInvokes.DefWindowProc(hWnd, (MessageType)message, wParam, lParam); }
            })), cbClsExtra = 0, cbWndExtra = 0, hInstance = Process.GetCurrentProcess().Handle, hbrBackground = PInvokes.CreateSolidBrush(new ColorReference(BackgroundColor)), hCursor = PInvokes.LoadCursor(IntPtr.Zero, (int)Win32IDCConstants.IDC_ARROW), lpszClassName = Title, lpszMenuName = null };
            hWnd = PInvokes.CreateWindowEx2(0, PInvokes.RegisterClassEx2(ref wndClass), title, WindowStyles.WS_OVERLAPPED, -1, -1, -1, -1, IntPtr.Zero, Process.GetCurrentProcess().Handle, IntPtr.Zero, IntPtr.Zero);
        }

        public IntPtr HandleMessage(MessageType message, IntPtr hWnd, IntPtr wParam, IntPtr lParam)
        {
            PInvokes.GetClientRect(hWnd, out Rectangle windowRect);
            switch (message)
            {
                case MessageType.PAINT:
                    IntPtr hDC = PInvokes.BeginPaint(hWnd, out PaintStruct ps);
                    int i = 0;
                    Controls.ToList().ForEach((Control control) =>
                    {
                        PInvokes.SetBkMode(hDC, control.BackgroundColor == Color.Transparent ? 1 : 2);
                        if (control.BackgroundColor != Color.Transparent) { PInvokes.SetBkColor(hDC, new ColorReference(control.BackgroundColor)); }
                        if (control.IsEnabled) { control.Draw(hWnd, hDC, control.GetDefaultDrawArgs()); }
                        i ++;
                    });
                    PInvokes.EndPaint(hWnd, ref ps);
                    return IntPtr.Zero;
                case MessageType.SIZING:
                    Size = windowRect.Size;
                    Location = windowRect.Location;
                    return IntPtr.Zero;
                case MessageType.SYSCOMMAND:
                    if ((SystemCommands)wParam == SystemCommands.SC_MINIMIZE || (SystemCommands)wParam == SystemCommands.SC_MAXIMIZE) {
                        Size = windowRect.Size;
                        Location = windowRect.Location;
                    }
                    return new IntPtr(-1);
                case MessageType.KEYUP:
                    Controls.ToList().ForEach((Control control) => {
                        if (control.HasFocus) { control.GotKey.Invoke((Key)wParam); }
                    });
                    return IntPtr.Zero;
                case MessageType.SYSKEYUP:
                    Controls.ToList().ForEach((Control control) => {
                        if (control.HasFocus) { control.GotSystemKey.Invoke((Key)wParam); }
                    });
                    return IntPtr.Zero;
                case MessageType.LBUTTONUP:
                    Controls.ToList().ForEach((Control control) => {
                        if (new Rectangle(control.Location, control.Size).IntersectsWith(new(PInvokes.GetPointFromInt(lParam), new(1, 1))) && control.Clicked != null) {
                            Controls.ToList().ForEach((Control control2) => { control2.HasFocus = false; });
                            control.HasFocus = true;
                            control.Clicked.Invoke(MouseButton.Left);
                        }
                    });
                    return IntPtr.Zero;
                case MessageType.RBUTTONUP:
                    Controls.ToList().ForEach((Control control) => {
                        if (new Rectangle(control.Location, control.Size).IntersectsWith(new(PInvokes.GetPointFromInt(lParam), new(1, 1))) && control.Clicked != null) {
                            Controls.ToList().ForEach((Control control2) => { control2.HasFocus = false; });
                            control.HasFocus = true;
                            control.Clicked.Invoke(MouseButton.Right);
                        }
                    });
                    return IntPtr.Zero;
                case MessageType.MBUTTONUP:
                    Controls.ToList().ForEach((Control control) => {
                        if (new Rectangle(control.Location, control.Size).IntersectsWith(new(PInvokes.GetPointFromInt(lParam), new(1, 1))) && control.Clicked != null) {
                            Controls.ToList().ForEach((Control control2) => { control2.HasFocus = false; });
                            control.HasFocus = true;
                            control.Clicked.Invoke(MouseButton.Middle);
                        }
                    });
                    return IntPtr.Zero;
                case MessageType.DESTROY:
                    PInvokes.PostQuitMessage(0);
                    return IntPtr.Zero;
            }
            return new(-1);
        }

        public void Show()
        {
            Running = true;
            Size = new(500, 500);
            Location = Point.Empty;
            PInvokes.ShowWindow(hWnd, ShowWindowType.Normal);
            PInvokes.GetClientRect(PInvokes.GetDesktopWindow(), out Rectangle screenSize);
            PInvokes.SetWindowPos(hWnd, IntPtr.Zero, Location.X, Location.Y, IsFullscreen ? screenSize.Width : Size.Width, IsFullscreen ? screenSize.Height : Size.Height, 0);
            while (true)
            {
                if (PInvokes.GetMessage(out Message msg, IntPtr.Zero, 0, 0) == 0) { break; }
                if (!Running) { PInvokes.PostQuitMessage(0); }
                PInvokes.GetClientRect(hWnd, out Rectangle rect);
                PInvokes.InvalidateRect(hWnd, rect, true);
                PInvokes.UpdateWindow(hWnd);
                PInvokes.SetClassLongPtr(hWnd, ClassLongFlags.GCLP_HBRBACKGROUND, PInvokes.CreateSolidBrush(new ColorReference(BackgroundColor)));
                PInvokes.SetWindowText(hWnd, Title);
                PInvokes.TranslateMessage(ref msg);
                PInvokes.DispatchMessage(ref msg);
            }
        }

        public void Hide() { PInvokes.ShowWindow(hWnd, ShowWindowType.Hide); }

        public void Close() { Running = false; }
    }
}