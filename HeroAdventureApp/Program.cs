using HeroAdventureApp.Classes;
using System;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        var auth = new Authenticator();
        var ai = new GuildAdvisorAI();
        var notificationService = new NotificationService();
        User currentUser = null;

        while (true)
        {
            if (currentUser == null)
            {
                MenuHelper.ShowMainMenu();
                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    Console.Write("Username: "); string username = Console.ReadLine();
                    Console.Write("Password: "); string password = Console.ReadLine();
                    Console.Write("Email: "); string email = Console.ReadLine();
                    if (auth.Register(username, password, email))
                        Console.WriteLine("Registered!");
                    else
                        Console.WriteLine("Registration failed (weak password or duplicate username).");
                }
                else if (choice == "2")
                {
                    Console.Write("Username: "); string username = Console.ReadLine();
                    Console.Write("Password: "); string password = Console.ReadLine();
                    currentUser = auth.Login(username, password);
                    if (currentUser == null)
                        Console.WriteLine("Login failed.");
                }
                else if (choice == "3") break;
            }
            else
            {
                var questManager = new QuestManager(currentUser, notificationService);
                // Simulera notifikation-check i bakgrunden (i verklig app, använd timer)
                // För nu: Kallad manuellt

                MenuHelper.ShowLoggedInMenu();
                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    Console.Write("Title: "); string title = Console.ReadLine();
                    Console.Write("Description: "); string desc = Console.ReadLine();
                    Console.Write("Deadline (yyyy-mm-dd): "); DateTime deadline = DateTime.Parse(Console.ReadLine());
                    Console.Write("Priority (High/Medium/Low): "); string priority = Console.ReadLine();
                    var quest = new Quest { Title = title, Description = desc, Deadline = deadline, Priority = priority };
                    questManager.AddQuest(quest);
                }
                else if (choice == "2")
                {
                    questManager.ShowAllQuests();
                }
                else if (choice == "3")
                {
                    Console.Write("Quest title to update/complete: "); string title = Console.ReadLine();
                    Console.Write("New description (or empty): "); string newDesc = Console.ReadLine();
                    // Lägg till logik för deadline/priority om behövs
                    questManager.UpdateQuest(title, newDesc);
                    Console.Write("Complete? (y/n): "); if (Console.ReadLine() == "y") questManager.CompleteQuest(title);
                }
                else if (choice == "4")
                {
                    Console.Write("Basic quest description: "); string basic = Console.ReadLine();
                    string epic = ai.GenerateEpicDescription(basic);
                    Console.WriteLine($"Epic version: {epic}");
                }
                else if (choice == "5")
                {
                    questManager.ShowAllQuests();
                    Console.WriteLine("Badges: " + string.Join(", ", currentUser.Badges));
                    questManager.CheckNotifications(); // Manuell check
                }
                else if (choice == "6")
                {
                    currentUser = null;
                }
            }
            Thread.Sleep(3000); // Paus för läsbarhet
        }
    }
}
