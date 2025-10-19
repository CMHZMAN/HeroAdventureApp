using HeroAdventureApp.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public class Authenticator
{
    private List<User> users;
    private string usersFilePath;
    private readonly object fileLock = new object(); // För thread safety

    public Authenticator(string dataDirectory = "HeroData")
    {
        usersFilePath = Path.Combine(dataDirectory, "users.json");
        EnsureDataDirectory(dataDirectory);
        LoadUsers();
    }

    private void EnsureDataDirectory(string directory)
    {
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
            Console.WriteLine($"Created data directory: {directory}");
        }
    }

    private void LoadUsers()
    {
        lock (fileLock)
        {
            try
            {
                if (File.Exists(usersFilePath))
                {
                    string json = File.ReadAllText(usersFilePath);
                    users = JsonConvert.DeserializeObject<List<User>>(json) ?? new List<User>();

                    // Ladda quests från individuella quest-filer för varje user
                    foreach (var user in users)
                    {
                        LoadUserQuests(user);
                        LoadUserBadges(user);
                    }
                }
                else
                {
                    users = new List<User>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading users: {ex.Message}");
                users = new List<User>();
            }
        }
    }

    private void LoadUserQuests(User user)
    {
        string questsFile = GetUserQuestsFile(user.Username);
        if (File.Exists(questsFile))
        {
            try
            {
                string json = File.ReadAllText(questsFile);
                user.Quests = JsonConvert.DeserializeObject<List<Quest>>(json) ?? new List<Quest>();
            }
            catch
            {
                user.Quests = new List<Quest>();
            }
        }
    }

    private void LoadUserBadges(User user)
    {
        string badgesFile = GetUserBadgesFile(user.Username);
        if (File.Exists(badgesFile))
        {
            try
            {
                string json = File.ReadAllText(badgesFile);
                user.Badges = JsonConvert.DeserializeObject<List<string>>(json) ?? new List<string>();
            }
            catch
            {
                user.Badges = new List<string>();
            }
        }
    }

    public bool Register(string username, string password, string email)
    {
        lock (fileLock)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(email))
            {
                Console.WriteLine("❌ Username, password, and email cannot be empty.");
                return false;
            }

            if (users.Any(u => u.Username.ToLower() == username.ToLower()))
            {
                Console.WriteLine("❌ Username already exists.");
                return false;
            }

            if (!IsPasswordStrong(password))
            {
                Console.WriteLine("❌ Password does not meet requirements.");
                return false;
            }

            var user = new User
            {
                Username = username,
                Password = password, // Hasha i produktion!
                Email = email,
                Quests = new List<Quest>(),
                Badges = new List<string>()
            };

            users.Add(user);
            SaveUsers();
            SaveUserQuests(user);
            SaveUserBadges(user);

            Console.WriteLine($"Hero {username} registered successfully!");
            return true;
        }
    }

    public User Login(string username, string password)
    {
        lock (fileLock)
        {
            var user = users.FirstOrDefault(u =>
                u.Username.ToLower() == username.ToLower() &&
                u.Password == password);

            if (user != null)
            {
                // Ladda färsk data
                LoadUserQuests(user);
                LoadUserBadges(user);
            }

            return user;
        }
    }

    public bool IsPasswordStrong(string password)
    {
        if (password.Length < 6) return false;
        if (!Regex.IsMatch(password, @"\d")) return false;
        if (!Regex.IsMatch(password, @"[A-Z]")) return false;
        if (!Regex.IsMatch(password, @"[!@#$%^&*(),.?]")) return false;
        return true;
    }

    private void SaveUsers()
    {
        try
        {
            string json = JsonConvert.SerializeObject(users, Formatting.Indented);
            File.WriteAllText(usersFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving users: {ex.Message}");
        }
    }

    public void SaveUserData(User user)
    {
        lock (fileLock)
        {
            SaveUserQuests(user);
            SaveUserBadges(user);
            SaveUsers(); // Uppdatera master-listan
        }
    }

    private string GetUserQuestsFile(string username)
    {
        return Path.Combine("HeroData", $"{username}_quests.json");
    }

    private string GetUserBadgesFile(string username)
    {
        return Path.Combine("HeroData", $"{username}_badges.json");
    }

    private void SaveUserQuests(User user)
    {
        try
        {
            string questsFile = GetUserQuestsFile(user.Username);
            string json = JsonConvert.SerializeObject(user.Quests, Formatting.Indented);
            File.WriteAllText(questsFile, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving quests for {user.Username}: {ex.Message}");
        }
    }

    private void SaveUserBadges(User user)
    {
        try
        {
            string badgesFile = GetUserBadgesFile(user.Username);
            string json = JsonConvert.SerializeObject(user.Badges, Formatting.Indented);
            File.WriteAllText(badgesFile, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving badges for {user.Username}: {ex.Message}");
        }
    }
}