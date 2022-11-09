using System.IO;
using Newtonsoft.Json;
using PowerAPI.Extensions;
using PowerAPI.Properties;

namespace PowerAPI.PowerDB
{
    public class User
    {
        public string Username;
        public string Email;
        public string Password;
        public string Key;
        public string ProfilePicture = BitmapExtensions.ConvertBitmapToBase64(Resources.Redly);

        public User(User user) : this(user.Username, user.Email, user.Password, user.Key, user.ProfilePicture) { }
        public User(string username = "", string email = "", string password = "", string key = "", string profilePicture = "") {
            Username = username;
            Email = email;
            Password = password;
            Key = key;
            ProfilePicture = profilePicture;
        }

        public override string ToString() { return JsonConvert.SerializeObject(this); }
        public static User FromString(string str) { return JsonConvert.DeserializeObject<User>(str); }
        public static User FromFile(string path) { return User.FromString(File.ReadAllText(path)); }
    }
}