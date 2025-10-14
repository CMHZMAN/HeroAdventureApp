using System;


namespace HeroAdventureApp.Classes
{
    public class GuildAdvisorAI
    {
        public string GenerateEpicDescription(string basicDesc)
        {
            // Simulerad AI: Gör det episkt
            return $"In the ancient realms of valor, thou shalt {basicDesc.ToLower()} amidst dragons and shadows, forging legends eternal!";

            // För riktig AI (valfritt):
            // Använd OpenAI API här, t.ex.:
            // var openAi = new OpenAIAPI("din-api-nyckel");
            // var response = openAi.Completions.CreateCompletionAsync(new CompletionRequest { Prompt = $"Make this quest epic: {basicDesc}", MaxTokens = 100 });
            // return response.Result.Completions[0].Text;
        }
    }
}
