using Newtonsoft.Json;
using System.Text;

namespace PowerExtensions
{
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
}
