using System.Runtime.InteropServices;
using System.Text;

namespace PowerExtensions
{
    public class ConsoleTools : TextWriter
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

        public ConsoleTools(string appTitle = "Console Application")
        {
            Console.Title = appTitle;
            this.stdOutWriter = Console.Out;
            Console.SetOut(this);
            Logs = new StringWriter();
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), 0xF030, 0x00000000);
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), 0xF000, 0x00000000);
        }

        public void Hide() { ShowWindow(GetConsoleWindow(), 0); }

        public void Show() { ShowWindow(GetConsoleWindow(), 5); }

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
}