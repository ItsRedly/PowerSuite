using System.Text;
using PowerAPI.Extensions;

namespace PowerAPI.PowerConsole
{
    public class Console : TextWriter
    {
        TextWriter stdOutWriter;
        TextWriter Logs { get; set; }
        List<string> Inputs = new();
        public override Encoding Encoding { get { return Encoding.ASCII; } }

        public Console(string applicationTitle = "System.Console Application")
        {
            System.Console.Title = applicationTitle;
            this.stdOutWriter = System.Console.Out;
            System.Console.SetOut(this);
            Logs = new StringWriter();
            PInvokes.DeleteMenu(PInvokes.GetSystemMenu(PInvokes.GetConsoleWindow(), false), 0xF030, 0x00000000);
            PInvokes.DeleteMenu(PInvokes.GetSystemMenu(PInvokes.GetConsoleWindow(), false), 0xF000, 0x00000000);
        }

        public void Hide() { PInvokes.ShowWindow(PInvokes.GetConsoleWindow(), ShowWindowType.Hide); }

        public void Show() { PInvokes.ShowWindow(PInvokes.GetConsoleWindow(), ShowWindowType.Normal); }

        public void SetTitle(string title) { System.Console.Title = title; }

        public void Write(string str)
        {
            Logs.Write(str);
            stdOutWriter.Write(str);
        }

        public void Write(string str, ConsoleColor foregroundColor)
        {
            ConsoleColor foregroundBackup = System.Console.ForegroundColor;
            System.Console.ForegroundColor = foregroundColor;
            Write(str);
            System.Console.ForegroundColor = foregroundBackup;
        }

        public void Write(string str, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            ConsoleColor foregroundBackup = System.Console.ForegroundColor;
            ConsoleColor backgroundBackup = System.Console.BackgroundColor;
            System.Console.ForegroundColor = foregroundColor;
            System.Console.BackgroundColor = backgroundColor;
            Write(str);
            System.Console.ForegroundColor = foregroundBackup;
            System.Console.BackgroundColor = backgroundBackup;
        }

        public void WriteAsync(string str) {
            Logs.WriteAsync(str);
            stdOutWriter.WriteAsync(str);
        }

        public void WriteAsync(string str, ConsoleColor foregroundColor)
        {
            ConsoleColor foregroundBackup = System.Console.ForegroundColor;
            System.Console.ForegroundColor = foregroundColor;
            WriteAsync(str);
            System.Console.ForegroundColor = foregroundBackup;
        }

        public void WriteAsync(string str, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            ConsoleColor backgroundBackup = System.Console.ForegroundColor;
            System.Console.BackgroundColor = backgroundColor;
            WriteAsync(str, foregroundColor);
            System.Console.BackgroundColor = backgroundBackup;
        }

        public void WriteLine(string str)
        {
            Logs.WriteLine(str);
            stdOutWriter.WriteLine(str);
        }

        public void WriteLine(string str, ConsoleColor foregroundColor)
        {
            ConsoleColor foregroundBackup = System.Console.ForegroundColor;
            System.Console.ForegroundColor = foregroundColor;
            WriteLine(str);
            System.Console.ForegroundColor = foregroundBackup;
        }

        public void WriteLine(string str, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            ConsoleColor foregroundBackup = System.Console.ForegroundColor;
            ConsoleColor backgroundBackup = System.Console.BackgroundColor;
            System.Console.ForegroundColor = foregroundColor;
            System.Console.BackgroundColor = backgroundColor;
            WriteLine(str);
            System.Console.ForegroundColor = foregroundBackup;
            System.Console.BackgroundColor = backgroundBackup;
        }

        public void WriteLineAsync(string str) {
            Logs.WriteLineAsync(str);
            stdOutWriter.WriteLineAsync(str);
        }

        public void WriteLineAsync(string str, ConsoleColor foregroundColor)
        {
            ConsoleColor foregroundBackup = System.Console.ForegroundColor;
            System.Console.ForegroundColor = foregroundColor;
            WriteLineAsync(str);
            System.Console.ForegroundColor = foregroundBackup;
        }

        public void WriteLineAsync(string str, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            ConsoleColor backgroundBackup = System.Console.ForegroundColor;
            System.Console.BackgroundColor = backgroundColor;
            WriteLineAsync(str, foregroundColor);
            System.Console.BackgroundColor = backgroundBackup;
        }

        public string GetInput()
        {
            string input = "";
            int startWriteI = System.Console.CursorTop - 1;
            int currentInputI = 0;
            while (true)
            {
                ConsoleKeyInfo key = System.Console.ReadKey(true);
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

        public void Clear() { System.Console.Clear(); }
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
            int windowWidth = System.Console.WindowWidth - (2 * decorationString.Length);
            string returnString = "";
            foreach (string text in content.Split("\n")) { returnString += string.Format(decorationString + "{0," + ((windowWidth / 2) + (text.Length / 2)) + "}{1," + (windowWidth - (windowWidth / 2) - (text.Length / 2) + decorationString.Length) + "}", text, decorationString) + "\n"; }
            return returnString.Substring(0, returnString.Length - 1);
        }

        public void WaitForKeyPress()
        {
            Write("Press any key to continue...");
            System.Console.ReadKey(true);
        }

        public void Header(string title, string subtitle = "")
        {
            WriteLine("╔" + new string('═', System.Console.WindowWidth - 2) + "╗");
            WriteLine(GetCenteredString(title, "║"));
            if (!string.IsNullOrEmpty(subtitle)) { WriteLine(GetCenteredString(subtitle, "║")); }
            WriteLine("╚" + new string('═', System.Console.WindowWidth - 2) + "╝");
        }

        public void Help(string title, string[] helpInstructions)
        {
            WriteLine("╔" + new string('═', System.Console.WindowWidth - 2) + "╗");
            WriteLine(GetCenteredString(title, "║"));
            WriteLine(GetCenteredString("Help instructions", "║"));
            foreach (string helpInstruction in helpInstructions) { WriteLine(GetCenteredString(helpInstruction, "║")); }
            WriteLine("╚" + new string('═', System.Console.WindowWidth - 2) + "╝");
        }

        public int Select(string[] elements)
        {
            int selectedI = 1;
            while (true)
            {
                int curTop = System.Console.CursorTop;
                int i = 1;
                WriteLine("╔" + new string('═', System.Console.WindowWidth - 2) + "╗");
                foreach (string element in elements)
                {
                    WriteLine(GetCenteredString((selectedI == i ? "* " : "") + element + (selectedI == i ? " *" : ""), "║"));
                    i++;
                }
                WriteLine("╚" + new string('═', System.Console.WindowWidth - 2) + "╝");
                ConsoleKey key = System.Console.ReadKey(true).Key;
                while (!new ConsoleKey[] { ConsoleKey.UpArrow, ConsoleKey.DownArrow, ConsoleKey.Enter }.Contains(key)) { key = System.Console.ReadKey(true).Key; }
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