using System.Diagnostics;
using Wice;
using Image = System.Drawing.Image;
using Path = System.IO.Path;
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

    public class Window
    {
        public string Title;
        Window window;
        public Window(string title = "Untitled Window")
        {
            Title = title;
            window = new Window();
            Task.Run(() => { window.Title = Title; });
        }

        public void Show() { window.Show(); }
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