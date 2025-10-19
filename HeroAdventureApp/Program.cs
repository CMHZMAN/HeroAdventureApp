using HeroAdventureApp.Classes;
using System;
using System.Timers;

class Program
{
    private static System.Timers.Timer notificationTimer;

    static void Main(string[] args)
    {
        var authenticator = new Authenticator();
        var notificationService = new NotificationService();
        var ai = new GuildAdvisorAI();
        User currentUser = null;
        QuestManager questManager = null;

        SetupNotificationTimer();

        while (true)
        {
            if (currentUser == null)
            {
                HandleMainMenu(authenticator, ai, out currentUser); // Lägg till ai för login-suggestion
                if (currentUser != null)
                {
                    questManager = new QuestManager(currentUser, authenticator, notificationService);
                    notificationTimer.Elapsed += (sender, e) => questManager?.CheckNotifications();
                    notificationTimer.Start();
                }
            }
            else
            {
                HandleLoggedInMenu(questManager, ai, currentUser, out currentUser);
                if (currentUser == null)
                {
                    notificationTimer.Stop();
                    questManager = null;
                }
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }

    private static void SetupNotificationTimer()
    {
        notificationTimer = new System.Timers.Timer(10 * 60 * 1000); // 10 minuter
        notificationTimer.AutoReset = true;
    }

    static void HandleMainMenu(Authenticator auth, GuildAdvisorAI ai, out User currentUser)
    {
        Console.Clear();
        MenuHelper.ShowMainMenu();

        string choice = GetValidInput("Choose an option (1-3): ", allowEmpty: false);
        currentUser = null;

        if (string.IsNullOrEmpty(choice)) return;

        switch (choice)
        {
            case "1":
                // Registrering 
                Console.Clear();
                string username = GetValidInput("Enter hero username: ", allowEmpty: false);
                if (string.IsNullOrEmpty(username)) break;

                string password = GetValidInput("Enter password (min 6 chars, 1 number, 1 uppercase, 1 special): ", allowEmpty: false);
                if (string.IsNullOrEmpty(password)) break;

                string email = GetValidInput("Enter email: ", allowEmpty: false);
                if (string.IsNullOrEmpty(email)) break;

                if (auth.Register(username, password, email))
                {
                    Console.WriteLine("Hero registered! You can now log in.");
                }
                else
                {
                    Console.WriteLine("Registration failed. Check password strength or username availability.");
                }
                break;

            case "2":
                // Login med AI-suggestion
                Console.Clear();
                string loginUser = GetValidInput("Username: ", allowEmpty: false);
                if (string.IsNullOrEmpty(loginUser)) break;

                string loginPass = GetValidInput("Password: ", allowEmpty: false);
                if (string.IsNullOrEmpty(loginPass)) break;

                currentUser = auth.Login(loginUser, loginPass);
                if (currentUser != null)
                {
                    Console.WriteLine($"Welcome back, {currentUser.Username}!");
                    // AI föreslår prioriteringar
                    string suggestions = ai.SuggestPrioritizedQuests(currentUser.Quests);
                    Console.WriteLine(suggestions);
                }
                else
                {
                    Console.WriteLine("Invalid credentials.");
                }
                break;

            case "3":
                Environment.Exit(0);
                break;

            default:
                Console.WriteLine("Invalid option. Please choose 1, 2, or 3.");
                break;
        }
    }

    static void HandleLoggedInMenu(QuestManager qm, GuildAdvisorAI ai, User user, out User currentUser)
    {
        Console.Clear();
        MenuHelper.ShowLoggedInMenu();

        string choice = GetValidInput("Choose an option (1-6): ", allowEmpty: false);
        currentUser = user;

        if (string.IsNullOrEmpty(choice)) return;

        switch (choice)
        {
            case "1": // Add new quest med AI-beskrivning
                Console.Clear();
                string title = GetValidInput("Title: ", allowEmpty: false);
                if (string.IsNullOrEmpty(title)) break;

                string deadlineInput = GetValidInput("Deadline (yyyy-MM-dd, press ENTER for 7 days from now): ", allowEmpty: true);
                DateTime deadline;
                if (string.IsNullOrEmpty(deadlineInput))
                {
                    deadline = DateTime.Now.AddDays(7);
                }
                else if (!DateTime.TryParse(deadlineInput + "T23:59:59", out deadline))
                {
                    Console.WriteLine("Invalid date format. Quest not added.");
                    break;
                }

                string priority = GetValidInput("Priority (High/Medium/Low, press ENTER for Medium): ", allowEmpty: true);
                priority = string.IsNullOrEmpty(priority) ? "Medium" : priority;

                if (!new[] { "High", "Medium", "Low" }.Contains(priority, StringComparer.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Invalid priority. Quest not added.");
                    break;
                }

                // AI genererar beskrivning
                string desc = ai.GenerateEpicDescription(title);
                Console.WriteLine($"AI-generated description: {desc}");

                qm.AddQuest(title, desc, deadline, priority);
                break;

            case "2": // View quests
                qm.ShowAllQuests();
                break;

            case "3": // Update/Complete/Delete med index
                Console.Clear();
                qm.ShowAllQuests(); // Visa listan med index
                string indexInput = GetValidInput("Enter quest number to modify: ", allowEmpty: false);
                if (!int.TryParse(indexInput, out int index) || index < 1)
                {
                    Console.WriteLine("Invalid index.");
                    break;
                }
                index--; // Konvertera till 0-based

                if (index < 0 || index >= user.Quests.Count)
                {
                    Console.WriteLine("Invalid choise.");
                    break;
                }

                string action = GetValidInput("Action: (u)pdate, (c)omplete, (d)elete, press ENTER to cancel: ", allowEmpty: true).ToLower();
                if (string.IsNullOrEmpty(action)) break;

                switch (action)
                {
                    case "c":
                        qm.CompleteQuest(user.Quests[index].Title);
                        break;
                    case "d":
                        qm.DeleteQuest(index);
                        break;
                    case "u":
                        string newDesc = GetValidInput("New description (press ENTER to keep current): ", allowEmpty: true);
                        string newDeadlineInput = GetValidInput("New deadline (yyyy-MM-dd, press ENTER to keep current): ", allowEmpty: true);
                        DateTime? newDeadline = null;
                        if (!string.IsNullOrEmpty(newDeadlineInput) && DateTime.TryParse(newDeadlineInput + "T23:59:59", out DateTime parsedDeadline))
                        {
                            newDeadline = parsedDeadline;
                        }
                        else if (!string.IsNullOrEmpty(newDeadlineInput))
                        {
                            Console.WriteLine("Invalid date format.");
                            break;
                        }

                        string newPriority = GetValidInput("New priority (High/Medium/Low, press ENTER to keep current): ", allowEmpty: true);
                        if (!string.IsNullOrEmpty(newPriority) && !new[] { "High", "Medium", "Low" }.Contains(newPriority, StringComparer.OrdinalIgnoreCase))
                        {
                            Console.WriteLine("Invalid priority.");
                            break;
                        }

                        qm.UpdateQuest(user.Quests[index].Title, newDesc, newDeadline, newPriority);
                        break;
                    default:
                        Console.WriteLine("Invalid action.");
                        break;
                }
                break;

            case "4": // AI help, för manuell generering om det behövs
                string idea = GetValidInput("Enter basic quest idea (press ENTER for a generic idea): ", allowEmpty: true);
                idea = string.IsNullOrEmpty(idea) ? "Embark on a heroic journey" : idea;
                string epicDesc = ai.GenerateEpicDescription(idea);
                Console.WriteLine($"\n✨ Guild Advisor says: {epicDesc}");
                break;

            case "5": // Show report
                Console.Clear();
                Console.WriteLine("\n--=== * GUILD REPORT * ===--");
                Console.WriteLine(qm.GenerateQuestReport());
                Console.WriteLine($"\nBadges earned: {string.Join(", ", qm.GetBadges())}");
                Console.WriteLine("\n=== DETAILED QUEST LIST ===");
                qm.ShowAllQuests();
                break;

            case "6": // Logout
                currentUser = null;
                Console.WriteLine("Logged out. Safe travels, hero!");
                break;

            default:
                Console.WriteLine("Invalid option. Please choose 1-6.");
                break;
        }
    }

    static string GetValidInput(string prompt, bool allowEmpty)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                if (allowEmpty)
                {
                    return string.Empty;
                }
                else
                {
                    Console.WriteLine("Input cannot be empty. Please try again.");
                    continue;
                }
            }

            return input.Trim();
        }
    }
}