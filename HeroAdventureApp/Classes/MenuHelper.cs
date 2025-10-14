using System;


namespace HeroAdventureApp.Classes
{
    public static class MenuHelper
    {
        public static void ShowMainMenu()
        {
            Console.Clear();
            Console.WriteLine("1) Registrera hjälte");
            Console.WriteLine("2) Logga in");
            Console.WriteLine("3) Avsluta");
        }

        public static void ShowLoggedInMenu()
        {
            Console.Clear();
            Console.WriteLine("1) Add new quest");
            Console.WriteLine("2) View all quests");
            Console.WriteLine("3) Update/Complete quest");
            Console.WriteLine("4) Request Guild Advisor help (AI)");
            Console.WriteLine("5) Show guild report"); // Visa quests, badges etc.
            Console.WriteLine("6) Logout");
        }
    }
}
