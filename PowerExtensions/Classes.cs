using System.Diagnostics;
using System.Drawing;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis;

namespace PowerExtensions
{
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
            await Task.Run(() => Extensions.ExtractZipFile(phpZipPath, phpExtractPath));
            while (runServer)
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
        }
    }

    public class ConsoleApplication : TextWriter
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        private TextWriter stdOutWriter;
        private TextWriter Logs { get; set; }
        private List<string> Inputs = new();
        public override Encoding Encoding { get { return Encoding.ASCII; } }

        public ConsoleApplication(string title = "Console Application")
        {
            Console.Title = title;
            this.stdOutWriter = Console.Out;
            Console.SetOut(this);
            Logs = new StringWriter();
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), 0xF030, 0x00000000);
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), 0xF000, 0x00000000);
        }

        public void Hide() { ShowWindow(GetConsoleWindow(), 0); }

        public void Show() { ShowWindow(GetConsoleWindow(), 5); }

        public void Write(string str) {
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
        public void Clear(int startIndex) {
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
                foreach (string element in elements) {
                    WriteLine(GetCenteredString((selectedI == i ? "* " : "") + element + (selectedI == i ? " *" : ""), "║"));
                    i ++;
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

    public delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
    public delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

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
    public struct WNDCLASS
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

    public struct POINT
    {
        public Int32 x;
        public Int32 Y;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct MSG
    {
        public IntPtr hwnd;
        public UInt32 message;
        public UIntPtr wParam;
        public UIntPtr lParam;
        public UInt32 time;
        public POINT pt;
    }

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
        WS_EX_NOREDIRECTIONBITMAP = 0x00200000,
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

    public class Window
    {
        [DllImport("user32.dll")]
        public static extern IntPtr DispatchMessage([In] ref MSG lpmsg);
        [DllImport("user32.dll")]
        public static extern bool TranslateMessage([In] ref MSG lpMsg);
        [DllImport("user32.dll")]
        public static extern sbyte GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);
        [DllImport("user32.dll", SetLastError = true, EntryPoint = "CreateWindowEx")]
        public static extern IntPtr CreateWindowEx(WindowStylesEx dwExStyle, string lpClassName, string lpWindowName, WindowStyles dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(IntPtr hWnd, ShowWindowCommands nCmdShow);
        public List<Control> Controls;
        public string Title;
        IntPtr handle;
        public Window(string title = "Untitled Window")
        {
            Title = title;
            WNDCLASS wndClass = new WNDCLASS();
            handle = CreateWindowEx(0, title, title, WindowStyles.WS_OVERLAPPEDWINDOW, -1, -1, -1, -1, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        }

        public void Show()
        {
            ShowWindow(handle, ShowWindowCommands.Normal);
        }

        public void Draw()
        {
            foreach (Control c in Controls) { c.Draw(this); }
        }
    }

    public abstract class Control
    {
        public string Name;
        public string Text;

        public abstract void Draw(Window window);
    }

    public class TextControl : Control
    {
        public TextControl(string text = "Text") { Name = text; Text = Name; }
        
        public override void Draw(Window window) {  }
    }

    public class ImageControl : Control
    {
        public Image Image;
        public bool IsRounded = false;
        public ImageControl(Image image) { Name = "Image"; Image = image; }

        public override void Draw(Window window) { }
    }

    public static class PowerSharp
    {
        public static void RunFile(string file) { RunString(File.ReadAllText(file)); }

        public static void RunString(string str) { CSharpScript.RunAsync(str, ScriptOptions.Default.AddReferences(MetadataReference.CreateFromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PowerExtensions.dll")))); }
    }

    public static class Extensions
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
                    using (Stream fsOutput = File.Create(fullZipToPath)) { StreamUtils.Copy(zipStream, fsOutput, buffer);}
                }
            }
        }

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

    public class User
    {
        public string Username;
        public string Email;
        public string Password;
        public string Key;
        public string ProfilePicture = Extensions.ConvertImageToBase64(Properties.Resources.Redly);

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
}