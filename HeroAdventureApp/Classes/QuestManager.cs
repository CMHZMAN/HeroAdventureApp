using System;
using System.Collections.Generic;
using System.Linq;


namespace HeroAdventureApp.Classes
{
    public class QuestManager
    {
        private List<Quest> quests;
        private User user;
        private NotificationService notificationService;

        public QuestManager(User user, NotificationService notificationService)
        {
            this.user = user;
            this.quests = user.Quests;
            this.notificationService = notificationService;
        }

        public void AddQuest(Quest quest)
        {
            quests.Add(quest);
            Console.WriteLine("Quest added!");
        }

        public void ShowAllQuests()
        {
            foreach (var quest in quests)
            {
                Console.WriteLine($"Title: {quest.Title}, Desc: {quest.Description}, Deadline: {quest.Deadline}, Priority: {quest.Priority}, Completed: {quest.IsCompleted}");
            }
        }

        public void CompleteQuest(string title)
        {
            var quest = quests.FirstOrDefault(q => q.Title == title);
            if (quest != null)
            {
                quest.IsCompleted = true;
                CheckForBadge();
                Console.WriteLine("Quest completed!");
            }
        }

        public void UpdateQuest(string title, string newDesc, DateTime? newDeadline = null, string newPriority = null)
        {
            var quest = quests.FirstOrDefault(q => q.Title == title);
            if (quest != null)
            {
                quest.Description = newDesc ?? quest.Description;
                quest.Deadline = newDeadline ?? quest.Deadline;
                quest.Priority = newPriority ?? quest.Priority;
                Console.WriteLine("Quest updated!");
            }
        }

        private void CheckForBadge()
        {
            int completedCount = quests.Count(q => q.IsCompleted);
            if (completedCount % 5 == 0)
            {
                string badge = $"Hero Badge Level {completedCount / 5}";
                user.Badges.Add(badge);
                Console.WriteLine($"Earned badge: {badge}");
            }
        }

        // Kallad periodiskt för att kolla deadlines
        public void CheckNotifications()
        {
            foreach (var quest in quests.Where(q => !q.IsCompleted))
            {
                if ((quest.Deadline - DateTime.Now).TotalHours < 24)
                {
                    notificationService.SendEmail(user.Email, $"Reminder: {quest.Title} is due soon!");
                }
            }
        }
    }
}
