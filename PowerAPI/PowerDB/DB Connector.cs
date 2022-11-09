using System.Text;

namespace PowerAPI.PowerDB
{
    public static class DbConnector
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