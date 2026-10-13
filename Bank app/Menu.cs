namespace BankApp
{
    public class Menu
    {
        public void Start()
        {
            // Här visar jag första menyn tills användaren avslutar.
            while (true)
            {
                Console.WriteLine("------------------------------");
                Console.WriteLine("Välkommen till Chas Bank!");
                Console.WriteLine("0. Avsluta");
                Console.Write("Välj: ");

                string? choice = Console.ReadLine();

                // Här avslutar jag om det inte finns mer inmatning.
                if (choice == null)
                {
                    return;
                }

                switch (choice.Trim())
                {
                    case "0":
                        Console.WriteLine("Appen avslutas.");
                        return;

                    default:
                        Console.WriteLine(
                            "Ogiltigt val. Skriv 0 för att avsluta.");
                        break;
                }

                Console.WriteLine();
            }
        }
    }
}