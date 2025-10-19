using HeroAdventureApp.Classes;
using Newtonsoft.Json;
using System;

public class User
{
    public string Username { get; set; }
    public string Password { get; set; } // Hasha i produktion!
    public string Email { get; set; }

    [JsonProperty("Quests")]
    public List<Quest> Quests { get; set; } = new List<Quest>();

    [JsonProperty("Badges")]
    public List<string> Badges { get; set; } = new List<string>();

    // För JSON serialisering av DateTime
    [JsonProperty(Order = -2)]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

