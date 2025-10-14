using System;
using System.Collections.Generic;


namespace HeroAdventureApp.Classes
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; } // Hasha i produktion!
        public string Email { get; set; }
        public List<Quest> Quests { get; set; } = new List<Quest>();
        public List<string> Badges { get; set; } = new List<string>(); // T.ex. "Bronze Hero", "Silver Hero"
    }
}
