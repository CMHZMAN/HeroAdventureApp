using System;
using System.Collections.Generic;
using System.Linq;


namespace HeroAdventureApp.Classes
{
   

    public class GuildAdvisorAI
    {
     
        private static readonly HttpClient httpClient = new HttpClient();

        string apiKey = Environment.GetEnvironmentVariable("");


        public string GenerateEpicDescription(string title)
        {
            // Simulerad AI: Generera en episk beskrivning på ca 50 ord
            string baseStory = $"In the shadowed realms of ancient lore, a hero arises to {title.ToLower()}. Amidst roaring tempests and forgotten ruins, thou shalt confront vile foes and unravel mysteries of yore. With sword in hand and heart aflame, forge thy destiny through trials of fire and shadow, claiming glory eternal for thy noble cause. The fate of realms hangs in the balance—will thou prevail?";

            // Begränsa till ca 50 ord (räkna och trimma om behövs)
            string[] words = baseStory.Split(' ');
            return string.Join(" ", words.Take(50)) + "..."; // Ta första 50 orden

            // För riktig AI (valfritt, kräver NuGet: OpenAI):
            // var openAi = new OpenAIAPI("din-api-nyckel");
            // var response = await openAi.Completions.CreateCompletionAsync(new CompletionRequest 
            // { Prompt = $"Generate an epic quest description in about 50 words based on the title: '{title}'", MaxTokens = 100 });
            // return response.Completions[0].Text.Trim();
        }

        public string SuggestPrioritizedQuests(List<Quest> quests)
        {
            if (quests.Count == 0) return "No quests to prioritize yet, hero. Embark on new adventures!";

            // Sortera quests: Först prioritet (High=3, Medium=2, Low=1), sedan deadline (tidigare först), sedan innehåll (t.ex. nyckelord som "rädda" ger bonus)
            var prioritized = quests
                .Where(q => !q.IsCompleted)
                .Select(q => new
                {
                    Quest = q,
                    PriorityScore = GetPriorityScore(q.Priority) + GetContentBonus(q.Title + " " + q.Description) - (q.Deadline - DateTime.Now).TotalDays // Lägre dagar = högre prioritet
                })
                .OrderByDescending(p => p.PriorityScore)
                .Take(3) // Top 3
                .ToList();

            string suggestions = "Guild Advisor suggests prioritizing these quests:\n";
            for (int i = 0; i < prioritized.Count; i++)
            {
                var q = prioritized[i].Quest;
                suggestions += $"{i + 1}. {q.Title} (Priority: {q.Priority}, Deadline: {q.Deadline:yyyy-MM-dd}) - Epic reason: In dire times, this quest demands thy valor forthwith!\n";
            }

            return suggestions;
        }

        private int GetPriorityScore(string priority)
        {
            return priority.ToLower() switch
            {
                "high" => 3,
                "medium" => 2,
                "low" => 1,
                _ => 0
            };
        }

        private double GetContentBonus(string content)
        {
            // Simulerad "AI-analys" av innehåll: Bonus för nyckelord som indikerar brådska
            string[] urgentKeywords = { "rädda", "save", "destroy", "urgent", "dragon", "evil" };
            return urgentKeywords.Count(k => content.ToLower().Contains(k)) * 0.5; // Bonus per match
        }
    }
}
