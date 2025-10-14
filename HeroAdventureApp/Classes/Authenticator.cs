using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace HeroAdventureApp.Classes
{
    public class Authenticator
    {
        private List<User> users = new List<User>();
        private string dataFile = "users.json";

        public Authenticator()
        {
            if (File.Exists(dataFile))
            {
                string json = File.ReadAllText(dataFile);
                users = JsonConvert.DeserializeObject<List<User>>(json);
            }
        }

        private void SaveData()
        {
            string json = JsonConvert.SerializeObject(users);
            File.WriteAllText(dataFile, json);
        }

        public bool Register(string username, string password, string email)
        {
            if (users.Exists(u => u.Username == username)) return false;

            if (!IsPasswordStrong(password)) return false;

            var user = new User { Username = username, Password = password, Email = email }; // Hasha password i verklig app!
            users.Add(user);
            SaveData();
            return true;
        }

        private bool IsPasswordStrong(string password)
        {
            if (password.Length < 6) return false;
            if (!Regex.IsMatch(password, @"\d")) return false; // Minst 1 siffra
            if (!Regex.IsMatch(password, @"[A-Z]")) return false; // Stor bokstav
            if (!Regex.IsMatch(password, @"[!@#$%^&*()]")) return false; // Specialtecken
            return true;
        }

        public User Login(string username, string password)
        {
            return users.FirstOrDefault(u => u.Username == username && u.Password == password);
        }
    }
}
