using Bank_app;
using System.Runtime.CompilerServices;

namespace BankApp
{
    public class Menu
    {

        private Bank bank = new Bank();
        private Input input = new Input();


        // here i show the start menu until the user exits.
        public void Start()
        {
            ShowLogo();

            while (input.EndOfInput == false)
            {
                Console.WriteLine("1. Logga in");
                Console.WriteLine("0. Avsluta");

                string choice = input.ReadText("Välj: ");

                switch (choice)
                {
                    case "1":
                        Login();
                        break;

                    case "0":
                        return;

                    default:
                        Console.WriteLine(
                            "Välj ett alternativ i menyn.");
                        break;

                }
            }

        }

        // the logo
        private void ShowLogo()
        {
            Console.WriteLine(" ________________________ ");
            Console.WriteLine("|        CHAS BANK       |");
            Console.WriteLine("|________________________|");

        }


        // here i use the returned list position to get the user



        private void Login()
        {
            string username = input.ReadText("Användarnamn: ");
            string password = input.ReadText("Lösenord: ");

            if (input.EndOfInput == true)
            {
                return;
            }

            int userIndex = bank.Login(username, password);

            if (userIndex == -1)
            {
                Console.WriteLine("Fel uppgifter eller låst användare.");
                return;
            }
            User user = bank.GetUser(userIndex);

            ShowLogo();

            Console.WriteLine($"Välkommen {user.Username}!");

            ShowUserMenu(user);

        }

        // here i show the choices for logged in user
        private void ShowUserMenu()
        {
            while (input.EndOfInput == false)
            {
                Console.WriteLine();
                if (user.IsAdmin == true)
                {
                    Console.WriteLine("1. Skapa användare");
                    Console.WriteLine("2. Lås upp användare");
                }
                else
                { 
                    // need to add customer: menu text here 
                }

                Console.WriteLine("0. Logga ut");

                string choice = input.ReadText("Välj: ");

                if (choice == "0")
                {
                    return;
                }

                if (user.IsAdmin == true)
                {
                    switch (choice)
                    {
                        case "1":
                            CreateUser(user);
                            break;
                        case "2":
                            UnlockUser(user);
                            break;
                        default:
                            Console.WriteLine("Ogiltigt menyval.");
                            break;
                    }
                }
                else
                {
                    switch (choice)
                    {
                        // NEED to add customer: cases here 

                        default:
                            Console.WriteLine("Ogiltigt menyval.");
                            break;

                    }
                }

            }

        }

        // here i read the credentials and show the result returned by bank
        private void CreateUser(User admin)
        {
            string username = input.ReadText("Nytt användarnamn: ");
            string password = input.ReadText("Nytt lösenord: ");

            if (input.EndOfInput == true)
            {
                return;
            }

            bool created = bank.CreateUser(admin, username, password);


            if (created == true)
            {
                Console.WriteLine("Användaren skapades.");

            }
            else
            {
                Console.WriteLine("Användaren kunde inte skapas. Användarnamnet måste vara unikt och innehålla minst 3 tecken. Lösenordet måste innehålla minst 6 tecken.");
            }


        }


        public void UnlockUser(User admin)
        {
            string username = input.ReadText("Användarnamn att låsa upp: ");

            if (input.EndOfInput == true)
            {
                return;
            }

            if (bank.UnlockUser(admin, username) == true)
            {
                Console.WriteLine("Använde är upplåst.");
            }
            else
            {
                Console.WriteLine("Anändaren finns inte eller du är inte adminstratör.");
            }

        }



    }
}