using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;


namespace HeroAdventureApp.Classes
{
    public class QuestManager
    {
        private User user;
        private Authenticator authenticator;
        private NotificationService notificationService;

        public QuestManager(User user, Authenticator authenticator, NotificationService notificationService)
        {
            this.user = user;
            this.authenticator = authenticator;
            this.notificationService = notificationService;
        }

        public void AddQuest(string title, string description, DateTime deadline, string priority)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Quest title cannot be empty.");
                return;
            }

            var quest = new Quest
            {
                Title = title,
                Description = string.IsNullOrWhiteSpace(description) ? "No description provided" : description,
                Deadline = deadline,
                Priority = priority,
                IsCompleted = false,
                ReminderSent = false // Initialisera påminnelse-flagga
            };

            user.Quests.Add(quest);
            authenticator.SaveUserData(user);
            Console.WriteLine($"Quest '{title}' added successfully!");
        }

        public void ShowAllQuests()
        {
            if (user.Quests.Count == 0)
            {
                Console.WriteLine("No quests found.");
                return;
            }

            Console.WriteLine("\n=== YOUR QUESTS ===");
            for (int i = 0; i < user.Quests.Count; i++)
            {
                var quest = user.Quests[i];
                string status = quest.IsCompleted ? "COMPLETED!" :
                                (quest.Deadline - DateTime.Now).TotalHours < 0 ? " :( EXPIRED!" :
                                (quest.Deadline - DateTime.Now).TotalHours < 24 ? "!! URGENT !!" : "Acitve";

                Console.WriteLine($"{i + 1}. {status}");
                Console.WriteLine($"   Title: {quest.Title}");
                Console.WriteLine($"   Description: {quest.Description}");
                Console.WriteLine($"   Deadline: {quest.Deadline:yyyy-MM-dd HH:mm}");
                Console.WriteLine($"   Priority: {quest.Priority}");
                Console.WriteLine();
            }
        }



        public string GenerateQuestReport()
        {
            int ongoingCount = user.Quests.Count(q => !q.IsCompleted);
            int urgentCount = user.Quests.Count(q => !q.IsCompleted &&
                                                     (q.Deadline - DateTime.Now).TotalHours < 24 &&
                                                     (q.Deadline - DateTime.Now).TotalHours >= 0);
            int underControlCount = user.Quests.Count(q => !q.IsCompleted &&
                                                           (q.Deadline - DateTime.Now).TotalHours >= 24);

            return $"You have {ongoingCount} ongoing quests, {urgentCount} must be completed today, and {underControlCount} are under control.";
        }


        public void CompleteQuest(string title)
        {
            var quest = user.Quests.FirstOrDefault(q => q.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
            if (quest != null)
            {
                quest.IsCompleted = true;
                quest.ReminderSent = false; // Återställ påminnelse om questen markeras som klar
                authenticator.SaveUserData(user);
                CheckForBadge();
                Console.WriteLine($"Quest '{title}' completed! Glory to the hero!");
            }
            else
            {
                Console.WriteLine("Quest not found.");
            }
        }

        public void UpdateQuest(string title, string newDescription = null, DateTime? newDeadline = null, string newPriority = null)
        {
            var quest = user.Quests.FirstOrDefault(q => q.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
            if (quest != null)
            {
                if (!string.IsNullOrEmpty(newDescription)) quest.Description = newDescription;
                if (newDeadline.HasValue)
                {
                    quest.Deadline = newDeadline.Value;
                    quest.ReminderSent = false; // Återställ påminnelse om deadline ändras
                    if (newDeadline > DateTime.Now)
                    {
                        quest.IsCompleted = false;
                    }

                }
                if (!string.IsNullOrEmpty(newPriority)) quest.Priority = newPriority;


                authenticator.SaveUserData(user);
                Console.WriteLine($"Quest '{title}' updated!");
            }
            else
            {
                Console.WriteLine("Quest not found.");
            }
        }


        public void DeleteQuest(int index)
        {
            if (index < 0 || index >= user.Quests.Count)
            {
                Console.WriteLine("Invalid index.");
                return;
            }

            var quest = user.Quests[index];
            user.Quests.RemoveAt(index);
            authenticator.SaveUserData(user);
            Console.WriteLine($"Quest '{quest.Title}' deleted.");
        }




        private void CheckForBadge()
        {
            int completedCount = user.Quests.Count(q => q.IsCompleted);
            if (completedCount % 5 == 0 && completedCount > 0)
            {
                string badge = completedCount switch
                {
                    5 => "Bronze Hero",
                    10 => "Silver Hero",
                    15 => "Gold Hero",
                    20 => "Legendary Hero",
                    _ => $"Hero Badge Level {completedCount / 5}"
                };

                if (!user.Badges.Contains(badge))
                {
                    user.Badges.Add(badge);
                    authenticator.SaveUserData(user);
                    Console.WriteLine($"\nCONGRATULATIONS! You earned: {badge}");
                }
            }
        }

        public void CheckNotifications()
        {
            var urgentQuests = user.Quests
                .Where(q => !q.IsCompleted && // Inte klar
                            !q.ReminderSent && // Ingen påminnelse skickad
                            (q.Deadline - DateTime.Now).TotalHours < 24 && // Inom 24h
                            q.Deadline > DateTime.Now) // Ej förfallen
                .ToList();

            foreach (var quest in urgentQuests)
            {
                notificationService.SendEmail(user.Email,
                    $"URGENT QUEST ALERT! '{quest.Title}' expires on {quest.Deadline:yyyy-MM-dd HH:mm}!");
                quest.ReminderSent = true; // Markera att påminnelse skickats
            }

            if (urgentQuests.Any())
            {
                authenticator.SaveUserData(user); // Spara ändringar i ReminderSent
            }
        }

        public List<string> GetBadges()
        {
            return user.Badges;
        }
    }
}
