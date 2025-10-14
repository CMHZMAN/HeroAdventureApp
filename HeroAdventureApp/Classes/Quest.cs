using System;


namespace HeroAdventureApp.Classes
{
    public class Quest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
        public string Priority { get; set; } // "High", "Medium", "Low"
        public bool IsCompleted { get; set; }
    }
}
