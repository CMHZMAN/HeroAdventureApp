using System;
using Newtonsoft.Json;

namespace HeroAdventureApp.Classes
{
    public class Quest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
        public string Priority { get; set; } // "High", "Medium", "Low"
        public bool IsCompleted { get; set; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool ReminderSent { get; set; } // Ny flagga för att spåra om påminnelse skickats
    }
}
