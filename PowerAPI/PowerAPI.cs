using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace PowerAPI
{
    public static class API
    {
        [DllImport("user32.dll")]
        public static extern IntPtr DispatchMessage([In] ref Msg lpmsg);
        [DllImport("user32.dll")]
        public static extern bool TranslateMessage([In] ref Msg lpMsg);
        [DllImport("user32.dll")]
        public static extern sbyte GetMessage(out Msg lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);
        [DllImport("user32.dll", SetLastError = true, EntryPoint = "CreateWindowEx")]
        public static extern IntPtr CreateWindowEx(WindowStylesEx dwExStyle, string lpClassName, string lpWindowName, WindowStyles dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);
        [DllImport("user32.dll", SetLastError = true, EntryPoint = "CreateWindowEx")]
        public static extern IntPtr CreateWindowEx2(WindowStylesEx dwExStyle, UInt16 lpClassName, string lpWindowName, WindowStyles dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);
        [DllImport("user32.dll")]
        public static extern ushort RegisterClass([In] ref WndClass lpWndClass);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(IntPtr hWnd, ShowWindowCommands nCmdShow);
        [DllImport("user32.dll")]
        public static extern bool CloseWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr BeginPaint(IntPtr hwnd, out PaintStruct lpPaint);

        [DllImport("user32.dll")]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, MessageType uMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hWnd, out Rectangle lpRect);

        [DllImport("user32.dll")]
        public static extern int DrawText(IntPtr hDC, string lpString, int nCount, ref Rectangle lpRect, uint uFormat);

        [DllImport("user32.dll")]
        public static extern bool EndPaint(IntPtr hWnd, [In] ref PaintStruct lpPaint);

        [DllImport("user32.dll")]
        public static extern void PostQuitMessage(int nExitCode);

        [DllImport("user32.dll")]
        public static extern IntPtr LoadIcon(IntPtr hInstance, string lpIconName);

        [DllImport("user32.dll")]
        public static extern IntPtr LoadIcon(IntPtr hInstance, IntPtr lpIConName);

        [DllImport("gdi32.dll")]
        public static extern IntPtr GetStockObject(StockObjects fnObject);

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
        public static extern bool SetWindowText(IntPtr hwnd, String lpString);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, UInt32 uFlags);
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        public static Point GetPointFromInt(IntPtr value)
        {
            uint xy = unchecked(IntPtr.Size == 8 ? (uint)value.ToInt64() : (uint)value.ToInt32());
            int x = unchecked((short)xy);
            int y = unchecked((short)(xy >> 16));
            return new Point(x, y);
        }
    }

    public static class Win32DTConstant
    {
        public const int DT_TOP = 0x00000000;
        public const int DT_LEFT = 0x00000000;
        public const int DT_CENTER = 0x00000001;
        public const int DT_RIGHT = 0x00000002;
        public const int DT_VCENTER = 0x00000004;
        public const int DT_BOTTOM = 0x00000008;
        public const int DT_WORDBREAK = 0x00000010;
        public const int DT_SINGLELINE = 0x00000020;
        public const int DT_EXPANDTABS = 0x00000040;
        public const int DT_TABSTOP = 0x00000080;
        public const int DT_NOCLIP = 0x00000100;
        public const int DT_EXTERNALLEADING = 0x00000200;
        public const int DT_CALCRECT = 0x00000400;
        public const int DT_NOPREFIX = 0x00000800;
        public const int DT_INTERNAL = 0x00001000;
    }

    public enum StockObjects
    {
        WHITE_BRUSH = 0,
        LTGRAY_BRUSH = 1,
        GRAY_BRUSH = 2,
        DKGRAY_BRUSH = 3,
        BLACK_BRUSH = 4,
        NULL_BRUSH = 5,
        HOLLOW_BRUSH = NULL_BRUSH,
        WHITE_PEN = 6,
        BLACK_PEN = 7,
        NULL_PEN = 8,
        OEM_FIXED_FONT = 10,
        ANSI_FIXED_FONT = 11,
        ANSI_VAR_FONT = 12,
        SYSTEM_FONT = 13,
        DEVICE_DEFAULT_FONT = 14,
        DEFAULT_PALETTE = 15,
        SYSTEM_FIXED_FONT = 16,
        DEFAULT_GUI_FONT = 17,
        DC_BRUSH = 18,
        DC_PEN = 19,
    }

    public static class Win32IDCConstants
    {
        public const int IDC_ARROW = 32512;
        public const int IDC_IBEAM = 32513;
        public const int IDC_WAIT = 32514;
        public const int IDC_CROSS = 32515;
        public const int IDC_UPARROW = 32516;
        public const int IDC_SIZE = 32640;
        public const int IDC_ICON = 32641;
        public const int IDC_SIZENWSE = 32642;
        public const int IDC_SIZENESW = 32643;
        public const int IDC_SIZEWE = 32644;
        public const int IDC_SIZENS = 32645;
        public const int IDC_SIZEALL = 32646;
        public const int IDC_NO = 32648;
        public const int IDC_HAND = 32649;
        public const int IDC_APPSTARTING = 32650;
        public const int IDC_HELP = 32651;
    }

    [Flags]
    public enum MessageBoxOptions : uint
    {
        OkOnly = 0x000000,
        OkCancel = 0x000001,
        AbortRetryIgnore = 0x000002,
        YesNoCancel = 0x000003,
        YesNo = 0x000004,
        RetryCancel = 0x000005,
        CancelTryContinue = 0x000006,
        IconHand = 0x000010,
        IconQuestion = 0x000020,
        IconExclamation = 0x000030,
        IconAsterisk = 0x000040,
        UserIcon = 0x000080,
        IconWarning = IconExclamation,
        IconError = IconHand,
        IconInformation = IconAsterisk,
        IconStop = IconHand,
        DefButton1 = 0x000000,
        DefButton2 = 0x000100,
        DefButton3 = 0x000200,
        DefButton4 = 0x000300,
        ApplicationModal = 0x000000,
        SystemModal = 0x001000,
        TaskModal = 0x002000,
        Help = 0x004000,
        NoFocus = 0x008000,
        SetForeground = 0x010000,
        DefaultDesktopOnly = 0x020000,
        Topmost = 0x040000,
        Right = 0x080000,
        RTLReading = 0x100000
    }

    public enum MessageBoxResult : uint
    {
        Ok = 1,
        Cancel,
        Abort,
        Retry,
        Ignore,
        Yes,
        No,
        Close,
        Help,
        TryAgain,
        Continue,
        Timeout = 32000
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct Msg
    {
        public IntPtr hwnd;
        public UInt32 message;
        public UIntPtr wParam;
        public UIntPtr lParam;
        public UInt32 time;
        public Rectangle pt;
    }

    public struct WndClass
    {
        public ClassStyles style;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public WndProc lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public IntPtr hInstance;
        public IntPtr hIcon;
        public IntPtr hCursor;
        public IntPtr hbrBackground;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpszMenuName;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpszClassName;
    }

    public delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
    public delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct WndClassEx
    {
        [MarshalAs(UnmanagedType.U4)]
        public int cbSize;
        [MarshalAs(UnmanagedType.U4)]
        public int style;
        public IntPtr lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public IntPtr hInstance;
        public IntPtr hIcon;
        public IntPtr hCursor;
        public IntPtr hbrBackground;
        public string lpszMenuName;
        public string lpszClassName;
        public IntPtr hIconSm;
    }

    [Flags()]
    public enum WindowStyles : uint
    {
        WS_BORDER = 0x800000,
        WS_CAPTION = 0xc00000,
        WS_CHILD = 0x40000000,
        WS_CLIPCHILDREN = 0x2000000,
        WS_CLIPSIBLINGS = 0x4000000,
        WS_DISABLED = 0x8000000,
        WS_DLGFRAME = 0x400000,
        WS_GROUP = 0x20000,
        WS_HSCROLL = 0x100000,
        WS_MAXIMIZE = 0x1000000,
        WS_MAXIMIZEBOX = 0x10000,
        WS_MINIMIZE = 0x20000000,
        WS_MINIMIZEBOX = 0x20000,
        WS_OVERLAPPED = 0x0,
        WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_SIZEFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
        WS_POPUP = 0x80000000u,
        WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,
        WS_SIZEFRAME = 0x40000,
        WS_SYSMENU = 0x80000,
        WS_TABSTOP = 0x10000,
        WS_VISIBLE = 0x10000000,
        WS_VSCROLL = 0x200000
    }

    [Flags]
    public enum WindowStylesEx : uint
    {
        WS_EX_ACCEPTFILES = 0x00000010,
        WS_EX_APPWINDOW = 0x00040000,
        WS_EX_CLIENTEDGE = 0x00000200,
        WS_EX_COMPOSITED = 0x02000000,
        WS_EX_CONTEXTHELP = 0x00000400,
        WS_EX_CONTROLPARENT = 0x00010000,
        WS_EX_DLGMODALFRAME = 0x00000001,
        WS_EX_LAYERED = 0x00080000,
        WS_EX_LAYOUTRTL = 0x00400000,
        WS_EX_LEFT = 0x00000000,
        WS_EX_LEFTSCROLLBAR = 0x00004000,
        WS_EX_LTRREADING = 0x00000000,
        WS_EX_MDICHILD = 0x00000040,
        WS_EX_NOACTIVATE = 0x08000000,
        WS_EX_NOINHERITLAYOUT = 0x00100000,
        WS_EX_NOPARENTNOTIFY = 0x00000004,
        WS_EX_OVERLAPPEDWINDOW = WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE,
        WS_EX_PALETTEWINDOW = WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST,
        WS_EX_RIGHT = 0x00001000,
        WS_EX_RIGHTSCROLLBAR = 0x00000000,
        WS_EX_RTLREADING = 0x00002000,
        WS_EX_STATICEDGE = 0x00020000,
        WS_EX_TOOLWINDOW = 0x00000080,
        WS_EX_TOPMOST = 0x00000008,
        WS_EX_TRANSPARENT = 0x00000020,
        WS_EX_WINDOWEDGE = 0x00000100
    }

    public enum ShowWindowCommands : int
    {
        Hide = 0,
        Normal = 1,
        ShowMinimized = 2,
        Maximize = 3,
        ShowMaximized = 3,
        ShowNoActivate = 4,
        Show = 5,
        Minimize = 6,
        ShowMinNoActive = 7,
        ShowNA = 8,
        Restore = 9,
        ShowDefault = 10,
        ForceMinimize = 11
    }

    public enum ClassStyles : uint
    {
        ByteAlignClient = 0x1000,
        ByteAlignWindow = 0x2000,
        ClassDC = 0x40,
        DoubleClicks = 0x8,
        DropShadow = 0x20000,
        GlobalClass = 0x4000,
        HorizontalRedraw = 0x2,
        NoClose = 0x200,
        OwnDC = 0x20,
        ParentDC = 0x80,
        SaveBits = 0x800,
        VerticalRedraw = 0x1
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PaintStruct
    {
        public IntPtr hdc;
        public bool fErase;
        public Rectangle rcPaint;
        public bool fRestore;
        public bool fIncUpdate;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] rgbReserved;
    }

    public enum MessageType : uint
    {
        NULL = 0x0000,
        CREATE = 0x0001,
        DESTROY = 0x0002,
        MOVE = 0x0003,
        SIZE = 0x0005,
        ACTIVATE = 0x0006,
        SETFOCUS = 0x0007,
        KILLFOCUS = 0x0008,
        ENABLE = 0x000A,
        SETREDRAW = 0x000B,
        SETTEXT = 0x000C,
        GETTEXT = 0x000D,
        GETTEXTLENGTH = 0x000E,
        PAINT = 0x000F,
        CLOSE = 0x0010,
        QUERYENDSESSION = 0x0011,
        QUERYOPEN = 0x0013,
        ENDSESSION = 0x0016,
        QUIT = 0x0012,
        ERASEBKGND = 0x0014,
        SYSCOLORCHANGE = 0x0015,
        SHOWWINDOW = 0x0018,
        WININICHANGE = 0x001A,
        SETTINGCHANGE = WININICHANGE,
        DEVMODECHANGE = 0x001B,
        ACTIVATEAPP = 0x001C,
        FONTCHANGE = 0x001D,
        TIMECHANGE = 0x001E,
        CANCELMODE = 0x001F,
        SETCURSOR = 0x0020,
        MOUSEACTIVATE = 0x0021,
        CHILDACTIVATE = 0x0022,
        QUEUESYNC = 0x0023,
        GETMINMAXINFO = 0x0024,
        PAINTICON = 0x0026,
        ICONERASEBKGND = 0x0027,
        NEXTDLGCTL = 0x0028,
        SPOOLERSTATUS = 0x002A,
        DRAWITEM = 0x002B,
        MEASUREITEM = 0x002C,
        DELETEITEM = 0x002D,
        VKEYTOITEM = 0x002E,
        CHARTOITEM = 0x002F,
        SETFONT = 0x0030,
        GETFONT = 0x0031,
        SETHOTKEY = 0x0032,
        GETHOTKEY = 0x0033,
        QUERYDRAGICON = 0x0037,
        COMPAREITEM = 0x0039,
        GETOBJECT = 0x003D,
        COMPACTING = 0x0041,
        [Obsolete]
        COMMNOTIFY = 0x0044,
        WINDOWPOSCHANGING = 0x0046,
        WINDOWPOSCHANGED = 0x0047,
        [Obsolete]
        POWER = 0x0048,
        COPYDATA = 0x004A,
        CANCELJOURNAL = 0x004B,
        NOTIFY = 0x004E,
        INPUTLANGCHANGEREQUEST = 0x0050,
        INPUTLANGCHANGE = 0x0051,
        TCARD = 0x0052,
        HELP = 0x0053,
        USERCHANGED = 0x0054,
        NOTIFYFORMAT = 0x0055,
        CONTEXTMENU = 0x007B,
        STYLECHANGING = 0x007C,
        STYLECHANGED = 0x007D,
        DISPLAYCHANGE = 0x007E,
        GETICON = 0x007F,
        SETICON = 0x0080,
        NCCREATE = 0x0081,
        NCDESTROY = 0x0082,
        NCCALCSIZE = 0x0083,
        NCHITTEST = 0x0084,
        NCPAINT = 0x0085,
        NCACTIVATE = 0x0086,
        GETDLGCODE = 0x0087,
        SYNCPAINT = 0x0088,
        NCMOUSEMOVE = 0x00A0,
        NCLBUTTONDOWN = 0x00A1,
        NCLBUTTONUP = 0x00A2,
        NCLBUTTONDBLCLK = 0x00A3,
        NCRBUTTONDOWN = 0x00A4,
        NCRBUTTONUP = 0x00A5,
        NCRBUTTONDBLCLK = 0x00A6,
        NCMBUTTONDOWN = 0x00A7,
        NCMBUTTONUP = 0x00A8,
        NCMBUTTONDBLCLK = 0x00A9,
        NCXBUTTONDOWN = 0x00AB,
        NCXBUTTONUP = 0x00AC,
        NCXBUTTONDBLCLK = 0x00AD,
        INPUT_DEVICE_CHANGE = 0x00FE,
        INPUT = 0x00FF,
        KEYFIRST = 0x0100,
        KEYDOWN = 0x0100,
        KEYUP = 0x0101,
        CHAR = 0x0102,
        DEADCHAR = 0x0103,
        SYSKEYDOWN = 0x0104,
        SYSKEYUP = 0x0105,
        SYSCHAR = 0x0106,
        SYSDEADCHAR = 0x0107,
        UNICHAR = 0x0109,
        KEYLAST = 0x0109,
        IME_STARTCOMPOSITION = 0x010D,
        IME_ENDCOMPOSITION = 0x010E,
        IME_COMPOSITION = 0x010F,
        IME_KEYLAST = 0x010F,
        INITDIALOG = 0x0110,
        COMMAND = 0x0111,
        SYSCOMMAND = 0x0112,
        TIMER = 0x0113,
        HSCROLL = 0x0114,
        VSCROLL = 0x0115,
        INITMENU = 0x0116,
        INITMENUPOPUP = 0x0117,
        MENUSELECT = 0x011F,
        MENUCHAR = 0x0120,
        ENTERIDLE = 0x0121,
        MENURBUTTONUP = 0x0122,
        MENUDRAG = 0x0123,
        MENUGETOBJECT = 0x0124,
        UNINITMENUPOPUP = 0x0125,
        MENUCOMMAND = 0x0126,
        CHANGEUISTATE = 0x0127,
        UPDATEUISTATE = 0x0128,
        QUERYUISTATE = 0x0129,
        CTLCOLORMSGBOX = 0x0132,
        CTLCOLOREDIT = 0x0133,
        CTLCOLORLISTBOX = 0x0134,
        CTLCOLORBTN = 0x0135,
        CTLCOLORDLG = 0x0136,
        CTLCOLORSCROLLBAR = 0x0137,
        CTLCOLORSTATIC = 0x0138,
        MOUSEFIRST = 0x0200,
        MOUSEMOVE = 0x0200,
        LBUTTONDOWN = 0x0201,
        LBUTTONUP = 0x0202,
        LBUTTONDBLCLK = 0x0203,
        RBUTTONDOWN = 0x0204,
        RBUTTONUP = 0x0205,
        RBUTTONDBLCLK = 0x0206,
        MBUTTONDOWN = 0x0207,
        MBUTTONUP = 0x0208,
        MBUTTONDBLCLK = 0x0209,
        MOUSEWHEEL = 0x020A,
        XBUTTONDOWN = 0x020B,
        XBUTTONUP = 0x020C,
        XBUTTONDBLCLK = 0x020D,
        MOUSEHWHEEL = 0x020E,
        MOUSELAST = 0x020E,
        PARENTNOTIFY = 0x0210,
        ENTERMENULOOP = 0x0211,
        EXITMENULOOP = 0x0212,
        NEXTMENU = 0x0213,
        SIZING = 0x0214,
        CAPTURECHANGED = 0x0215,
        MOVING = 0x0216,
        POWERBROADCAST = 0x0218,
        DEVICECHANGE = 0x0219,
        MDICREATE = 0x0220,
        MDIDESTROY = 0x0221,
        MDIACTIVATE = 0x0222,
        MDIRESTORE = 0x0223,
        MDINEXT = 0x0224,
        MDIMAXIMIZE = 0x0225,
        MDITILE = 0x0226,
        MDICASCADE = 0x0227,
        MDIICONARRANGE = 0x0228,
        MDIGETACTIVE = 0x0229,
        MDISETMENU = 0x0230,
        ENTERSIZEMOVE = 0x0231,
        EXITSIZEMOVE = 0x0232,
        DROPFILES = 0x0233,
        MDIREFRESHMENU = 0x0234,
        IME_SETCONTEXT = 0x0281,
        IME_NOTIFY = 0x0282,
        IME_CONTROL = 0x0283,
        IME_COMPOSITIONFULL = 0x0284,
        IME_SELECT = 0x0285,
        IME_CHAR = 0x0286,
        IME_REQUEST = 0x0288,
        IME_KEYDOWN = 0x0290,
        IME_KEYUP = 0x0291,
        MOUSEHOVER = 0x02A1,
        MOUSELEAVE = 0x02A3,
        NCMOUSEHOVER = 0x02A0,
        NCMOUSELEAVE = 0x02A2,
        WTSSESSION_CHANGE = 0x02B1,
        TABLET_FIRST = 0x02c0,
        TABLET_LAST = 0x02df,
        CUT = 0x0300,
        COPY = 0x0301,
        PASTE = 0x0302,
        CLEAR = 0x0303,
        UNDO = 0x0304,
        RENDERFORMAT = 0x0305,
        RENDERALLFORMATS = 0x0306,
        DESTROYCLIPBOARD = 0x0307,
        DRAWCLIPBOARD = 0x0308,
        PAINTCLIPBOARD = 0x0309,
        VSCROLLCLIPBOARD = 0x030A,
        SIZECLIPBOARD = 0x030B,
        ASKCBFORMATNAME = 0x030C,
        CHANGECBCHAIN = 0x030D,
        HSCROLLCLIPBOARD = 0x030E,
        QUERYNEWPALETTE = 0x030F,
        PALETTEISCHANGING = 0x0310,
        PALETTECHANGED = 0x0311,
        HOTKEY = 0x0312,
        PRINT = 0x0317,
        PRINTCLIENT = 0x0318,
        APPCOMMAND = 0x0319,
        THEMECHANGED = 0x031A,
        CLIPBOARDUPDATE = 0x031D,
        DWMCOMPOSITIONCHANGED = 0x031E,
        DWMNCRENDERINGCHANGED = 0x031F,
        DWMCOLORIZATIONCOLORCHANGED = 0x0320,
        DWMWINDOWMAXIMIZEDCHANGE = 0x0321,
        GETTITLEBARINFOEX = 0x033F,
        HANDHELDFIRST = 0x0358,
        HANDHELDLAST = 0x035F,
        AFXFIRST = 0x0360,
        AFXLAST = 0x037F,
        PENWINFIRST = 0x0380,
        PENWINLAST = 0x038F,
        APP = 0x8000,
        USER = 0x0400,
        CPL_LAUNCH = USER + 0x1000,
        CPL_LAUNCHED = USER + 0x1001,
        SYSTIMER = 0x118
    }

    public enum MouseButton
    {
        Left,
        Right,
        Middle
    }

    public static class Compression
    {
        public static void ExtractZipFile(string archivePath, string outFolder, string password = "")
        {
            using (FileStream fsInput = File.OpenRead(archivePath))
            using (ZipFile zf = new(fsInput))
            {
                if (!string.IsNullOrEmpty(password)) { zf.Password = password; }
                foreach (ZipEntry zipEntry in zf)
                {
                    if (!zipEntry.IsFile) { continue; }
                    string entryFileName = zipEntry.Name;
                    var fullZipToPath = Path.Combine(outFolder, entryFileName);
                    var directoryName = Path.GetDirectoryName(fullZipToPath);
                    if (directoryName.Length > 0) { Directory.CreateDirectory(directoryName); }
                    byte[] buffer = new byte[4096];
                    using (var zipStream = zf.GetInputStream(zipEntry))
                    using (Stream fsOutput = File.Create(fullZipToPath)) { StreamUtils.Copy(zipStream, fsOutput, buffer); }
                }
            }
        }
    }

    public class ConsoleTools : TextWriter
    {
        TextWriter stdOutWriter;
        TextWriter Logs { get; set; }
        List<string> Inputs = new();
        public override Encoding Encoding { get { return Encoding.ASCII; } }

        public ConsoleTools(string appTitle = "Console Application")
        {
            Console.Title = appTitle;
            this.stdOutWriter = Console.Out;
            Console.SetOut(this);
            Logs = new StringWriter();
            API.DeleteMenu(API.GetSystemMenu(API.GetConsoleWindow(), false), 0xF030, 0x00000000);
            API.DeleteMenu(API.GetSystemMenu(API.GetConsoleWindow(), false), 0xF000, 0x00000000);
        }

        public void Hide() { API.ShowWindow(API.GetConsoleWindow(), ShowWindowCommands.Hide); }

        public void Show() { API.ShowWindow(API.GetConsoleWindow(), ShowWindowCommands.Normal); }

        public void Write(string str)
        {
            Logs.Write(str);
            stdOutWriter.Write(str);
        }

        public void Write(string str, ConsoleColor foregroundColor)
        {
            ConsoleColor foregroundBackup = Console.ForegroundColor;
            Console.ForegroundColor = foregroundColor;
            Write(str);
            Console.ForegroundColor = foregroundBackup;
        }

        public void Write(string str, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            ConsoleColor foregroundBackup = Console.ForegroundColor;
            ConsoleColor backgroundBackup = Console.BackgroundColor;
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
            Write(str);
            Console.ForegroundColor = foregroundBackup;
            Console.BackgroundColor = backgroundBackup;
        }

        public void WriteLine(string str)
        {
            Logs.WriteLine(str);
            stdOutWriter.WriteLine(str);
        }

        public void WriteLine(string str, ConsoleColor foregroundColor)
        {
            ConsoleColor foregroundBackup = Console.ForegroundColor;
            Console.ForegroundColor = foregroundColor;
            WriteLine(str);
            Console.ForegroundColor = foregroundBackup;
        }

        public void WriteLine(string str, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            ConsoleColor foregroundBackup = Console.ForegroundColor;
            ConsoleColor backgroundBackup = Console.BackgroundColor;
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
            WriteLine(str);
            Console.ForegroundColor = foregroundBackup;
            Console.BackgroundColor = backgroundBackup;
        }

        public string GetInput()
        {
            string input = "";
            int startWriteI = Console.CursorTop - 1;
            int currentInputI = 0;
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                Write(key.KeyChar);
                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        currentInputI = Math.Max(currentInputI - 1, 0);
                        input = Inputs[currentInputI];
                        Clear(startWriteI);
                        Write(input);
                        break;

                    case ConsoleKey.DownArrow:
                        currentInputI = Math.Min(currentInputI + 1, Inputs.Count);
                        input = Inputs[currentInputI];
                        Clear(startWriteI);
                        Write(input);
                        break;

                    case ConsoleKey.Enter:
                        Write("\n");
                        Inputs.Add(input);
                        return input;
                }
            }
        }

        public string GetLogs() { return Logs.ToString(); }
        public string GetLogs(int startIndex) { return string.Join("\n", Logs.ToString().Split("\n").Skip(startIndex)); }
        public string GetLogs(int startIndex, int endIndex) { return string.Join("\n", Logs.ToString().Split("\n").Skip(startIndex).Take(endIndex)); }

        public void Clear() { Console.Clear(); }
        public void Clear(int startIndex)
        {
            Clear();
            WriteLine(GetLogs(0, startIndex));
        }

        public string Prompt(string text)
        {
            Write(string.IsNullOrEmpty(text) ? ">" : text + ">");
            string line = GetInput();
            Logs.WriteLine(line);
            return line;
        }

        public string GetCenteredString(string content, string decorationString = "")
        {
            int windowWidth = Console.WindowWidth - (2 * decorationString.Length);
            string returnString = "";
            foreach (string text in content.Split("\n")) { returnString += string.Format(decorationString + "{0," + ((windowWidth / 2) + (text.Length / 2)) + "}{1," + (windowWidth - (windowWidth / 2) - (text.Length / 2) + decorationString.Length) + "}", text, decorationString) + "\n"; }
            return returnString.Substring(0, returnString.Length - 1);
        }

        public void WaitForKeyPress()
        {
            Write("Press any key to continue...");
            Console.ReadKey(true);
        }

        public void Header(string title, string subtitle = "")
        {
            WriteLine("╔" + new string('═', Console.WindowWidth - 2) + "╗");
            WriteLine(GetCenteredString(title, "║"));
            if (!string.IsNullOrEmpty(subtitle)) { WriteLine(GetCenteredString(subtitle, "║")); }
            WriteLine("╚" + new string('═', Console.WindowWidth - 2) + "╝");
        }

        public void Help(string title, string[] helpInstructions)
        {
            WriteLine("╔" + new string('═', Console.WindowWidth - 2) + "╗");
            WriteLine(GetCenteredString(title, "║"));
            WriteLine(GetCenteredString("Help instructions", "║"));
            foreach (string helpInstruction in helpInstructions) { WriteLine(GetCenteredString(helpInstruction, "║")); }
            WriteLine("╚" + new string('═', Console.WindowWidth - 2) + "╝");
        }

        public int Select(string[] elements)
        {
            int selectedI = 1;
            while (true)
            {
                int curTop = Console.CursorTop;
                int i = 1;
                WriteLine("╔" + new string('═', Console.WindowWidth - 2) + "╗");
                foreach (string element in elements)
                {
                    WriteLine(GetCenteredString((selectedI == i ? "* " : "") + element + (selectedI == i ? " *" : ""), "║"));
                    i++;
                }
                WriteLine("╚" + new string('═', Console.WindowWidth - 2) + "╝");
                ConsoleKey key = Console.ReadKey(true).Key;
                while (!new ConsoleKey[] { ConsoleKey.UpArrow, ConsoleKey.DownArrow, ConsoleKey.Enter }.Contains(key)) { key = Console.ReadKey(true).Key; }
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        if (--selectedI == 0) { selectedI = elements.Length; }
                        break;

                    case ConsoleKey.DownArrow:
                        if (++selectedI == elements.Length + 1) { selectedI = 1; }
                        break;

                    case ConsoleKey.Enter:
                        return selectedI;
                }
                Clear(curTop);
            }
        }
    }

    public class MessageBox
    {
        public MessageBoxOptions Options;
        public string Text;
        public string Title;
        public MessageBoxResult Result;

        public MessageBox(string text = "", string title = "MessageBox", MessageBoxOptions options = MessageBoxOptions.OkOnly)
        {
            Text = text;
            Title = title;
            Options = options;
        }

        public void Show() { Result = API.MessageBox(IntPtr.Zero, Text, Title, Options); }
    }

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
            WndClassEx wndClass = new WndClassEx();
            wndClass.cbSize = Marshal.SizeOf(typeof(WndClassEx));
            wndClass.style = (int)(ClassStyles.HorizontalRedraw | ClassStyles.VerticalRedraw);
            wndClass.lpfnWndProc = Marshal.GetFunctionPointerForDelegate((WndProc)((hWnd, message, wParam, lParam) => {
                try
                {
                    IntPtr result = HandleMessage((MessageType)message, hWnd, wParam, lParam);
                    return result == new IntPtr(-1) ? API.DefWindowProc(hWnd, (MessageType)message, wParam, lParam) : result;
                }
                catch { return API.DefWindowProc(hWnd, (MessageType)message, wParam, lParam); }
            }));
            wndClass.cbClsExtra = 0;
            wndClass.cbWndExtra = 0;
            wndClass.hInstance = hInstance;
            wndClass.hCursor = API.LoadCursor(IntPtr.Zero, (int)Win32IDCConstants.IDC_ARROW);
            wndClass.hbrBackground = API.GetStockObject(StockObjects.WHITE_BRUSH);
            wndClass.lpszMenuName = null;
            wndClass.lpszClassName = Title;
            UInt16 regRest = API.RegisterClassEx2(ref wndClass);
            hwnd = API.CreateWindowEx2(0, regRest, title, WindowStyles.WS_OVERLAPPEDWINDOW, -1, -1, -1, -1, IntPtr.Zero, IntPtr.Zero, hInstance, IntPtr.Zero);
        }

        public Window(Point startLoc, string title = "Untitled window") : this(title) { Location = startLoc; }

        public IntPtr HandleMessage(MessageType message, IntPtr hWnd, IntPtr wParam, IntPtr lParam)
        {
            switch (message)
            {
                case MessageType.PAINT:
                    IntPtr hdc = API.BeginPaint(hWnd, out PaintStruct ps);
                    Controls.ForEach((Control control) =>
                    {
                        if (control.IsEnabled) { control.Draw(hWnd, hdc); }
                    });
                    API.EndPaint(hWnd, ref ps);
                    return IntPtr.Zero;
                case MessageType.KEYUP:
                    Console.WriteLine("KEY PRESSED! " + (char)wParam);
                    return IntPtr.Zero;
                case MessageType.SYSKEYUP:
                    Console.WriteLine("KEY PRESSED! " + (char)wParam);
                    return IntPtr.Zero;
                case MessageType.LBUTTONUP:
                    Controls.ForEach((Control control) => {
                        if (new Rectangle(control.Location, control.Size).IntersectsWith(new Rectangle(API.GetPointFromInt(lParam), new Size(1, 1))) && control.Clicked != null) { control.Clicked.Invoke(MouseButton.Left); }
                    });
                    return IntPtr.Zero;
                case MessageType.RBUTTONUP:
                    Controls.ForEach((Control control) => {
                        if (new Rectangle(control.Location, control.Size).IntersectsWith(new Rectangle(API.GetPointFromInt(lParam), new Size(1, 1))) && control.Clicked != null) { control.Clicked.Invoke(MouseButton.Right); }
                    });
                    return IntPtr.Zero;
                case MessageType.MBUTTONUP:
                    Controls.ForEach((Control control) => {
                        if (new Rectangle(control.Location, control.Size).IntersectsWith(new Rectangle(API.GetPointFromInt(lParam), new Size(1, 1))) && control.Clicked != null) { control.Clicked.Invoke(MouseButton.Middle); }
                    });
                    return IntPtr.Zero;
                case MessageType.DESTROY:
                    API.PostQuitMessage(0);
                    return IntPtr.Zero;
            }
            return new IntPtr(-1);
        }

        public void Show()
        {
            Running = true;
            API.ShowWindow(hwnd, ShowWindowCommands.Normal);
            API.UpdateWindow(hwnd);
            API.UpdateWindow(hwnd);
            API.SetWindowPos(hwnd, new IntPtr(0), Location.X, Location.Y, Size.Width, Size.Height, 0);
            while (API.GetMessage(out Msg msg, IntPtr.Zero, 0, 0) != 0)
            {
                if (!Running) { API.PostQuitMessage(0); }
                API.SetWindowText(hwnd, Title);
                API.TranslateMessage(ref msg);
                API.DispatchMessage(ref msg);
            }
        }

        public void Hide() { API.ShowWindow(hwnd, ShowWindowCommands.Hide); }

        public void Close() { Running = false; }
    }

    public delegate void ClickEvent(MouseButton button);

    public abstract class Control
    {
        public ClickEvent Clicked;
        public string Name;
        public string Text;
        public Point Location = new Point(0, 0);
        public Size Size = new Size(50, 50);
        public bool IsEnabled = true;

        public abstract void Draw(IntPtr hwnd, IntPtr hdc);
    }

    public class TextControl : Control
    {
        public bool IsHCentered = false;
        public bool IsVCentered = false;
        public TextControl(string text = "TextControl") { Text = text; }
        public override void Draw(IntPtr hWnd, IntPtr hdc)
        {
            API.GetClientRect(hWnd, out Rectangle rect);
            uint drawArgs = Win32DTConstant.DT_SINGLELINE;
            if (IsHCentered) { drawArgs |= Win32DTConstant.DT_CENTER; }
            else { rect.X = Location.X; }
            if (IsVCentered) { drawArgs |= Win32DTConstant.DT_VCENTER; }
            else { rect.Y = Location.Y; }
            API.DrawText(hdc, Text, -1, ref rect, drawArgs);
        }
    }

    public static class ImageExtensions
    {
        public static Image ConvertBase64ToImage(this string base64string)
        {
            byte[] imageBytes = Convert.FromBase64String(base64string);
            using (MemoryStream ms = new(imageBytes, 0, imageBytes.Length))
            {
                ms.Write(imageBytes, 0, imageBytes.Length);
                return Image.FromStream(ms, true);
            }
        }

        public static string ConvertImageToBase64(this Image image)
        {
            using (MemoryStream memoryStream = new())
            {
                image.Save(memoryStream, image.RawFormat);
                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }
    }

    public delegate string PostRequestHandle(string request, string requestUrl);
    public class HttpServer
    {
        public PostRequestHandle PostRequestHandle;
        bool runServer;
        string websitePath;
        HttpListener listener;
        public HttpServer(int port, string websitePath)
        {
            runServer = false;
            this.websitePath = websitePath;
            listener = new();
            listener.Prefixes.Add("http://localhost:" + port + "/");
        }

        public void Start()
        {
            listener.Start();
            runServer = true;
            Task.Run(Run);
        }

        public void Stop()
        {
            runServer = false;
            listener.Stop();
        }

        async void Run()
        {
            string phpZipPath = Path.GetTempFileName();
            string phpExtractPath = Path.GetTempFileName();
            File.Delete(phpExtractPath);
            Directory.CreateDirectory(phpExtractPath);
            File.WriteAllBytes(phpZipPath, Properties.Resources.PHP);
            await Task.Run(() => Compression.ExtractZipFile(phpZipPath, phpExtractPath));
            while (runServer)
            {
                try
                {
                    HttpListenerContext ctx = await listener.GetContextAsync();
                    HttpListenerRequest req = ctx.Request;
                    HttpListenerResponse resp = ctx.Response;
                    string reqPath = req.Url.AbsolutePath == "/" ? "index" : req.Url.AbsolutePath.Substring(1);
                    byte[] pageData = Encoding.Default.GetBytes("<!DOCTYPE html><html lang='en'><head><link href='https://fonts.googleapis.com/css2?family=Nunito+Sans:wght@600;900&display=swap' rel='stylesheet'><script src='https://kit.fontawesome.com/4b9ba14b0f.js' crossorigin='anonymous'></script></head><body><div class='mainbox'><div class='err'>4</div><i class='far fa-question-circle fa-spin'></i><div class='err2'>4</div><div class='msg'>Maybe this page moved? Got deleted? Is hiding out in quarantine? Never existed in the first place?<p>Let's go <a href='/'>home</a> and try from there.</p></div></div></body><style>body{background-color:#95c2de}.mainbox{background-color:#95c2de;margin:auto;height:600px;width:600px;position:relative}.err{color:#ffffff;font-family:'Nunito Sans', sans-serif;font-size:11rem;position:absolute;left:20%;top:8%}.far{position:absolute;font-size:8.5rem;left:42%;top:15%;color:#ffffff}.err2{color:#ffffff;font-family:'Nunito Sans', sans-serif;font-size:11rem;position:absolute;left:68%;top:8%}.msg{text-align:center;font-family:'Nunito Sans', sans-serif;font-size:1.6rem;position:absolute;left:16%;top:45%;width:75%}a{text-decoration:none;color:white}a:hover{text-decoration:underline}</style></html>");
                    if (req.HttpMethod == "POST")
                    {
                        StreamReader reader = new(req.InputStream, req.ContentEncoding);
                        string postReq = PostRequestHandle.Invoke(reader.ReadToEnd(), reqPath);
                        if (postReq != "") { await resp.OutputStream.WriteAsync(Encoding.Default.GetBytes(postReq), 0, Encoding.Default.GetByteCount(postReq)); }
                        else { await resp.OutputStream.WriteAsync(pageData, 0, pageData.Length); }
                        resp.Close();
                        reader.Close();
                    }
                    else
                    {
                        string file = Path.Combine(websitePath, reqPath);
                        bool usePHPRenderer = true;
                        if (File.Exists(file + ".html") && string.IsNullOrEmpty(Path.GetExtension(file))) { file += ".html"; }
                        else if (File.Exists(file + ".htm") && string.IsNullOrEmpty(Path.GetExtension(file))) { file += ".htm"; }
                        else if (File.Exists(file + ".php") && string.IsNullOrEmpty(Path.GetExtension(file))) { file += ".php"; }
                        else if (string.IsNullOrEmpty(Path.GetExtension(file))) { usePHPRenderer = false; }
                        if (usePHPRenderer)
                        {
                            Process proc = new();
                            proc.StartInfo.FileName = Path.Combine(phpExtractPath, "php.exe");
                            proc.StartInfo.Arguments = "-d \"display_errors=1\" -d \"error_reporting=E_PARSE\" \"" + file + "\"";
                            proc.StartInfo.CreateNoWindow = true;
                            proc.StartInfo.UseShellExecute = false;
                            proc.StartInfo.RedirectStandardOutput = true;
                            proc.StartInfo.RedirectStandardError = true;
                            proc.Start();
                            pageData = Encoding.Default.GetBytes(proc.StandardOutput.ReadToEnd());
                            if (string.IsNullOrEmpty(Encoding.Default.GetString(pageData)))
                            {
                                pageData = Encoding.Default.GetBytes("Error" + pageData);
                                proc.StandardError.Close();
                            }
                            else { proc.StandardOutput.Close(); }
                            proc.Close();
                            resp.ContentLength64 = pageData.LongLength;
                            await resp.OutputStream.WriteAsync(pageData, 0, pageData.Length);
                            resp.Close();
                        }
                        else
                        {
                            if (File.Exists(file)) { pageData = File.ReadAllBytes(file); }
                            await resp.OutputStream.WriteAsync(pageData, 0, pageData.Length);
                            resp.Close();
                        }
                    }
                }
                catch { }
            }
        }
    }

    public class User
    {
        public string Username;
        public string Email;
        public string Password;
        public string Key;
        public string ProfilePicture = ImageExtensions.ConvertImageToBase64(Properties.Resources.Redly);

        public override string ToString() { return JsonConvert.SerializeObject(this); }
        public static User FromString(string str) { return JsonConvert.DeserializeObject<User>(str); }
        public static User FromFile(string path) { return User.FromString(File.ReadAllText(path)); }
    }

    public static class DbConnection
    {
        public static (User User, string Error) SignIn(string server, string username, string email, string password)
        {
            HttpClient client = new();
            string returnedString = client.PostAsync(server + "/SignIn", new StringContent(new User() { Username = username, Email = email, Password = password }.ToString(), Encoding.UTF8, "application/json")).Result.Content.ReadAsStringAsync().Result;
            string error = "";
            User user = new();
            try { user = User.FromString(returnedString); }
            catch { error = returnedString; }
            return (user, error);
        }

        public static (User User, string Error) LogIn(string server, string username, string password)
        {
            HttpClient client = new();
            string returnedString = client.PostAsync(server + "/LogIn", new StringContent(new User() { Username = username, Password = password }.ToString(), Encoding.UTF8, "application/json")).Result.Content.ReadAsStringAsync().Result;
            string error = "";
            User user = new();
            try { user = User.FromString(returnedString); }
            catch { error = returnedString; }
            return (user, error);
        }
    }

    public static class PowerSharp
    {
        public static void RunFile(string file) { RunString(File.ReadAllText(file)); }
        public static void RunString(string str) { CSharpScript.RunAsync(str, ScriptOptions.Default.AddReferences(MetadataReference.CreateFromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PowerExtensions.dll")))); }
    }
}