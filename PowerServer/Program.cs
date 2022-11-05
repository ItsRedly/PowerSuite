using PowerAPI;
using System.Net.Mail;
using System.Net;
using Cryptography;
using Newtonsoft.Json;

namespace PowerServer
{
    public static class Program
    {
        static HttpServer webServer; // A HTTP server
        static ConsoleTools tools; // The ConsoleTools class to make life easier
        [STAThread]
        public static void Main(string[] args) // Program entry point
        {
            tools = new("PowerServer"); // Initializing the ConsoleTools
            if (args.Length > 1 || args.Length == 1 && args[0] == "/help") // Check if it has help/incorrect params
            {
                tools.Help("PowerServer", new string[] { "Argument 1: Path to settings file (optional)", @"Example: PowerServer.exe C:\SettingsFile.json" }); // Show help menu
                tools.WaitForKeyPress(); // Wait for key press
                return; // Exit
            }
            tools.Header("PowerServer", "Version 1.0"); // Show app header
            ServerSettings settings = args.Length > 0 && File.Exists(args[0]) ? ServerSettings.FromFile(args[0]) : new(); // Create new instance of ServerSettings class or load existing one
            if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Users"))) { Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Users")); } // Create Users directory if dosent exist
            if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Website"))) { Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Website")); } // Create Website directory if dosent exist
            webServer = new(80, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Website")); // Initializing the HTTP server
            if (settings.EnableDB) { webServer.PostRequestHandle += DBPostRequestHandler; } // Enable DB case enabled on server settings
            if (settings.EnableWebServer) { webServer.Start(); } // Start HTTP server case enabled on server settings

            while (true) // App loop
            {
                switch (tools.Select(new string[] { (settings.EnableDB ? "Disable" : "Enable") + " PowerDB", (settings.EnableWebServer ? "Disable" : "Enable") + " PowerWeb", "Run PowerSend", "Save config file", "Exit" })) // Show selection prompt
                {
                    case 1: //Case to enable/disable DB
                        settings.EnableDB = !settings.EnableDB; // Change DB status to opposite of what it currently is
                        if (settings.EnableDB) { webServer.PostRequestHandle += DBPostRequestHandler; } // Enable DB case enabled on server settings
                        else { webServer.PostRequestHandle -= DBPostRequestHandler; } // Disable DB case not enabled on server settings
                        break; // Goto next iteration of loop

                    case 2: // Case to enable/disable HTTP server
                        settings.EnableWebServer = !settings.EnableWebServer; //Change HTTP server status to opposite of what it currently is
                        if (settings.EnableWebServer) { webServer.Start(); } // Start HTTP server case enabled on server settings
                        else { webServer.Stop(); } // Stop HTTP server case not enabled on server settings
                        break; // Goto next iteration of loop

                    case 3: // Case to run email sending wizard
                        string email = tools.Prompt("What is the email address that is going to be used?"); // Asks user "What is the email address that is going to be used?"
                        string displayName = tools.Prompt("What is the display name that is going to be used?"); // Asks user "What is the display name that is going to be used?"
                        string password = tools.Prompt("What is the application key for that email address?"); // Asks user "What is the application key for that email address
                        string header = tools.Prompt("What is the email header going to be?"); // Asks user "What is the email header going to be?"
                        string content = tools.Prompt(@"What is the email content going to be (use \n for newlines)?").Replace(@"\n", "\n"); // Asks user "What is the email content going to be (use \n for newlines)?"
                        SmtpClient smtp = new SmtpClient() { Host = "smtp.gmail.com", Port = 587, EnableSsl = true, DeliveryMethod = SmtpDeliveryMethod.Network, UseDefaultCredentials = false, Credentials = new NetworkCredential(email, password) }; //Creates an SMTP client with the data provided previously
                        Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Users")).ToList().ForEach((string file) => { // Get every user stored in the DB
                            using (MailMessage message = new(new(email, displayName), new MailAddress(User.FromFile(file).Email)) { Subject = header, Body = content }) { smtp.Send(message); } // Send message provided previously to user
                        });
                        break; // Goto next iteration of loop

                    case 4: // Case to export settings to file
                        bool enableDB, enableWebServer; // Creates two booleans for exported settings
                        string fileName = tools.Prompt("What filename do you want to export it to?"); // Asks user "What filename do you want to export it to?"
                        fileName = fileName == "" ? "settings" : fileName; // In case of null file name, set it to "settings"
                        string enableDBPrompt = tools.Prompt("Would you like to run PowerDB?"); // Asks user "Would you like to run PowerDB?"
                        while (!new string[] { "Yes", "Y", "yes", "y", "No", "N", "no", "n" }.Contains(enableDBPrompt)) // Checks if anwser is "Yes", "Y", "yes", "y", "No", "N", "no" or "n"
                        {
                            tools.WriteLine("Invalid anwser. Valid anwsers are: Yes, Y, yes, y, No, N, no, n", ConsoleColor.Red); // Writes "Invalid anwser. Valid anwsers are: Yes, Y, yes, y, No, N, no, n" with the color red
                            enableDBPrompt = tools.Prompt("Would you like to run PowerDB?"); // Asks user "Would you like to run PowerDB?"
                        }
                        enableDB = new string[] { "Yes", "Y", "yes", "y" }.Contains(enableDBPrompt); // Set Enable DB to "Yes", "Y", "yes" and "y" is contained on enable DB prompt anwser
                        string enableWebServerPrompt = tools.Prompt("Would you like to run PowerWeb?"); // Asks user "Would you like to run PowerWeb?"
                        while (!new string[] { "Yes", "Y", "yes", "y", "No", "N", "no", "n" }.Contains(enableWebServerPrompt)) // Checks if anwser is "Yes", "Y", "yes", "y", "No", "N", "no" or "n"
                        {
                            tools.WriteLine("Invalid anwser. Valid anwsers are: Yes, Y, yes, y, No, N, no, n", ConsoleColor.Red); // Writes "Invalid anwser. Valid anwsers are: Yes, Y, yes, y, No, N, no, n" with the color red
                            enableWebServerPrompt = tools.Prompt("Would you like to run PowerWeb?"); // Asks user "Would you like to run PowerWeb?"
                        }
                        enableWebServer = new string[] { "Yes", "Y", "yes", "y" }.Contains(enableWebServerPrompt); // Set Enable HTTP server to "Yes", "Y", "yes" and "y" is contained on enable HTTP prompt anwser
                        File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName + ".json"), new ServerSettings() { EnableDB = enableDB, EnableWebServer = enableWebServer }.ToString()); // Writes settings created previously to file with name also chosen previously
                        break; // Goto next iteration of loop
                    case 5: // Case exit app
                        return; // Exit app
                }
                tools.Clear(4); // Clear last selection
            }
        }

        static string DBPostRequestHandler(string request, string requestUrl) // Handle Post Request using DB
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