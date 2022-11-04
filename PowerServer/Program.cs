using PowerExtensions;
using System.Net.Mail;
using System.Net;
using Cryptography;
using Newtonsoft.Json;

namespace PowerServer
{
    public static class Program
    {
        static HttpServer webServer;
        static ConsoleTools tools;
        [STAThread]
        public static void Main(string[] args)
        {
            Window window = new("LOL");
            window.Controls.Add(new TextControl() { Text = "oi", IsHCentered = true });
            window.Show();
            tools = new("PowerServer");
            if (args.Length > 1 || args.Length == 1 && args[0] == "/help")
            {
                tools.Help("PowerServer", new string[] { "Argument 1: Path to settings file (optional)", @"Example: PowerServer.exe C:\SettingsFile.json" });
                tools.WaitForKeyPress();
                return;
            }
            //tools.Header("PowerServer", "Version 1.0");
            //RunRoutine(args.Length > 0 && File.Exists(args[0]) ? ServerSettings.FromFile(args[0]) : new ServerSettings());
        }

        static string DBPostRequestHandler(string request, string requestUrl)
        {
            User user = User.FromString(request);
            switch (requestUrl)
            {
                case "SignIn":
                    if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Users", user.Username)))
                    {
                        (string key, string token) encryption = Fernet.Encrypt(user.Password);
                        user.Password = encryption.token;
                        user.Key = encryption.key;
                        File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Users", user.Username), user.ToString());
                        User oldUsr = User.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Users", user.Username));
                        oldUsr.Key = null;
                        File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Users", "log.log"), "Created user with name " + user.Username + "!\n");
                        return oldUsr.ToString();
                    }
                    else
                    {
                        File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Users", "log.log"), "Declined creating user with name " + user.Username + " because a user with that name " + user.Username + " already exists...\n");
                        return "Already exists";
                    }

                case "LogIn":
                    if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Users", user.Username)))
                    {
                        File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Users", "log.log"), "Declined sending data for user " + user.Username + "because of user with that name not existing...\n");
                        return "User dosent exist";
                    }
                    else if (Fernet.Decrypt(User.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Users", user.Username)).Key, User.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Users", user.Username)).Password) != user.Password)
                    {
                        File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Users", "log.log"), "Declined sending data for user " + user.Username + "because of invalid password...\n");
                        return "Invalid password";
                    }
                    else
                    {
                        User oldUsr = User.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Users", user.Username));
                        oldUsr.Key = null;
                        File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Users", "log.log"), "Sent all data for user with name " + user.Username + "\n!");
                        return oldUsr.ToString();
                    }

                default:
                    return "";
            }
        }

        static void RunRoutine(ServerSettings settings)
        {
            if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Users"))) { Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Users")); }
            if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Website"))) { Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Website")); }
            webServer = new HttpServer(80, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Website"));
            if (settings.EnableDB) { webServer.PostRequestHandle += DBPostRequestHandler; }
            if (settings.EnableWebServer) { webServer.Start(); }

            while (true)
            {
                switch (tools.Select(new string[] { (settings.EnableDB ? "Disable" : "Enable") + " PowerDB", (settings.EnableWebServer ? "Disable" : "Enable") + " PowerWeb", "Run PowerSend", "Save config file", "Exit" })) {
                    case 1:
                        settings.EnableDB = !settings.EnableDB;
                        if (settings.EnableDB) { webServer.PostRequestHandle += DBPostRequestHandler; }
                        else { webServer.PostRequestHandle -= DBPostRequestHandler; }
                        break;

                    case 2:
                        settings.EnableWebServer = !settings.EnableWebServer;
                        if (settings.EnableWebServer) { webServer.Start(); }
                        else { webServer.Stop(); }
                        break;

                    case 3:
                        string email = tools.Prompt("What is the email address that is going to be used?");
                        string displayName = tools.Prompt("What is the email display name that is going to be used?");
                        string password = tools.Prompt("What is the application key for that email address?");
                        string header = tools.Prompt("What is the email header going to be?");
                        string content = tools.Prompt(@"What is the email content going to be (use \n for newlines)?").Replace(@"\n", "\n");
                        SmtpClient smtp = new SmtpClient() { Host = "smtp.gmail.com", Port = 587, EnableSsl = true, DeliveryMethod = SmtpDeliveryMethod.Network, UseDefaultCredentials = false, Credentials = new NetworkCredential(email, password) };
                        foreach (string file in Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Users")))
                        {
                            using (MailMessage message = new MailMessage(new MailAddress(email, displayName), new MailAddress(User.FromFile(file).Email)) { Subject = header, Body = content }) { smtp.Send(message); }
                        }
                        break;

                    case 4:
                        bool enableDB = false;
                        bool enableWebServer = false;
                        string fileName = tools.Prompt("What filename do you want to export it to?");
                        fileName = fileName == "" ? "settings" : fileName;
                        string enableDBPrompt = tools.Prompt("Would you like to run PowerDB?");
                        while (!new string[] { "Yes", "Y", "yes", "y", "No", "N", "no", "n" }.Contains(enableDBPrompt))
                        {
                            tools.WriteLine("Invalid anwser. Valid anwsers are: Yes, Y, yes, y, No, N, no, n", ConsoleColor.Red);
                            enableDBPrompt = tools.Prompt("Would you like to run PowerDB?");
                        }
                        enableDB = new string[] { "Yes", "Y", "yes", "y" }.Contains(enableDBPrompt);
                        string enableWebServerPrompt = tools.Prompt("Would you like to run PowerWeb?");
                        while (!new string[] { "Yes", "Y", "yes", "y", "No", "N", "no", "n" }.Contains(enableWebServerPrompt))
                        {
                            tools.WriteLine("Invalid anwser. Valid anwsers are: Yes, Y, yes, y, No, N, no, n", ConsoleColor.Red);
                            enableWebServerPrompt = tools.Prompt("Would you like to run PowerWeb?");
                        }
                        enableWebServer = new string[] { "Yes", "Y", "yes", "y" }.Contains(enableDBPrompt);
                        File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName + ".json"), new ServerSettings() { EnableDB = enableDB, EnableWebServer = enableWebServer}.ToString());
                        break;
                    case 5:
                        return;
                }
                tools.Clear(4);
            }
        }
    }

    public class ServerSettings
    {
        public bool EnableDB = true;
        public bool EnableWebServer = true;

        public override string ToString() { return JsonConvert.SerializeObject(this); }
        public static ServerSettings FromString(string str) { return JsonConvert.DeserializeObject<ServerSettings>(str); }
        public static ServerSettings FromFile(string path) { return ServerSettings.FromString(File.ReadAllText(path)); }
    }
}