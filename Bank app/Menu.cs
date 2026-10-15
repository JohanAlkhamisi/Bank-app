using Bank_app;
using System.Runtime.CompilerServices;

namespace Bank_app
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
        private void ShowUserMenu(User user)
        {
            while (input.EndOfInput == false)
            {
                Console.WriteLine();
                if (user.IsAdmin == true)
                {
                    Console.WriteLine("1. Skapa användare");
                    Console.WriteLine("2. Lås upp användare");
                    Console.WriteLine("4. Behandla nästa simulerade kvart");
                }
                else
                {
                    Console.WriteLine("1. Mina konton");
                    Console.WriteLine("2. Öppna konto");
                    Console.WriteLine("3. Överför pengar");
                    Console.WriteLine("4. Öppna sparkonto");
                    Console.WriteLine("5. Sätt in pengar");
                    Console.WriteLine("6. Ansök om lån");
                    Console.WriteLine("7. Mina lån");
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
                        case "4":
                            ProcessNextQuarter(user);
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
                        case "1":
                            ShowAccounts(user);
                            break;

                        case "2":
                            OpenAccount(user);
                            break;
                        case "3":
                            Transfer(user);
                            break;
                        case "4":
                            OpenSavingsAccount(user);
                            break;

                        case "5":
                            Deposit(user);
                            break;
                        case "6":
                            RequestLoan(user);
                            break;

                        case "7":
                            ShowLoans(user);
                            break;
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

        //here i print the accounts returned for the logged in user
        private void ShowAccounts(User user)
        {
            List<BankAccount> accounts = bank.GetAccounts(user);

            if (accounts.Count == 0)
            {
                Console.WriteLine("Du har inga konton.");
            }

            foreach (BankAccount account in accounts)
            {
                Console.WriteLine($"{account.Number} | {account.Name} | Saldo {account.Balance} {account.Currency} | Tillgängligt {bank.GetAvailableBalance(account)}");
            }

        }

        // here i read the account details and ask bank to create the acoutn 
        private void OpenAccount(User user)
        {
            string name = input.ReadText("Kontonamn: ");
            Currency currency = input.ReadCurrency();

            if (input.EndOfInput == true)
            {
                return;
            }
            if (bank.OpenAccount(user, name, currency) == true)
            {
                Console.WriteLine("Kontot skapades. Du hittar det under Mina konton");
            }
            else
            {
                Console.WriteLine("Kontot behöver ett namn.");
            }

        }

        private void Transfer(User user)
        {
            ShowAccounts(user);

            int from = input.ReadNumber("Från ditt kontonummer: ");
            int to = input.ReadNumber("Till kontonummer: ");

            decimal amount = input.ReadDecimal(
                "Belopp i avsändarkontots valuta: ");

            if (input.EndOfInput == true)
            {
                return;
            }

            if (bank.Transfer(user, from, to, amount) == true)
            {
                Console.WriteLine(
                    $"Överföringen väntar till bankminut {bank.GetBankMinute() + 15}.");
            }
            else
            {
                Console.WriteLine(
                    "Kontrollera konton, ägare, belopp, tillgängligt saldo och valutakurser.");
            }
        }

        // This advances simulated time. It does not wait fifteen real minutes.
        private void ProcessNextQuarter(User admin)
        {
            int completed = bank.ProcessNextQuarter(admin);

            Console.WriteLine(
                $"Bankens simulerade tid: minut {bank.GetBankMinute()}, dag {bank.GetBankDay()}.");

            Console.WriteLine(
                $"Genomförda transaktioner: {completed}.");
        }


        // Here I read the details for a savings account.
        private void OpenSavingsAccount(User user)
        {
            string name = input.ReadText("Sparkontots namn: ");
            Currency currency = input.ReadCurrency();

            if (input.EndOfInput == true)
            {
                return;
            }

            if (bank.OpenSavingsAccount(user, name, currency) == true)
            {
                Console.WriteLine("Sparkontot skapades. Årsräntan är 2 procent.");
            }
            else
            {
                Console.WriteLine("Kontot behöver ett namn.");
            }
        }

        // Here I calculate interest before asking for confirmation.
        private void Deposit(User user)
        {
            ShowAccounts(user);

            int number = input.ReadNumber("Ditt kontonummer: ");

            decimal amount = input.ReadDecimal("Belopp i kontots valuta: ");

            int index = bank.FindAccountIndex(number);

            if (index == -1 || bank.AmountIsValid(amount) == false)
            {
                Console.WriteLine("Ogiltigt konto eller belopp.");
                return;
            }

            BankAccount account = bank.GetAccount(index);

            if (account.OwnerId != user.Id)
            {
                Console.WriteLine("Du får bara använda dina egna konton.");
                return;
            }

            decimal interest = account.CalculateYearlyInterest(amount);
            interest = bank.RoundMoney(interest);

            Console.WriteLine($"Beräknad ränta på insättningen under ett år: {interest} {account.Currency}.");

            string confirm = input.ReadText("Sätt in pengarna? 1 ja, 0 nej: ");

            if (confirm == "1" && input.EndOfInput == false)
            {
                if (bank.Deposit(user, number, amount) == true)
                {
                    Console.WriteLine("Insättningen väntar till nästa simulerade kvart.");
                }
            }
        }
        // Here I show the allowance and interest before confirmation.
        private void RequestLoan(User user)
        {
            decimal limit = bank.GetLoanLimit(user);

            if (limit < 0)
            {
                Console.WriteLine(
                    "Admin måste uppdatera bankdagens valutakurser.");
                return;
            }

            Console.WriteLine(
                $"Du kan låna ytterligare {limit} SEK.");

            ShowAccounts(user);

            int number = input.ReadNumber("Ditt konto i SEK: ");
            decimal amount = input.ReadDecimal("Lånebelopp i SEK: ");

            if (bank.AmountIsValid(amount) == false
                || input.EndOfInput == true)
            {
                Console.WriteLine("Ogiltigt belopp.");
                return;
            }

            decimal percent = bank.GetLoanPercent(user);
            decimal interest = bank.RoundMoney(amount * percent / 100);

            Console.WriteLine(
                $"Årsränta: {percent} procent. Beräknad ränta första året: {interest} SEK.");

            string confirm = input.ReadText(
                "Ansök om lånet? 1 ja, 0 nej: ");

            if (confirm == "1" && input.EndOfInput == false)
            {
                if (bank.RequestLoan(user, number, amount) == true)
                {
                    Console.WriteLine(
                        "Lånet registrerades. Pengarna väntar till nästa simulerade kvart.");
                }
                else
                {
                    Console.WriteLine(
                        "Kontrollera konto, valuta och lånegräns.");
                }
            }
        }

        // Here I show each loan using a loop and if-else.
        private void ShowLoans(User user)
        {
            List<Loan> loans = bank.GetLoans(user);

            if (loans.Count == 0)
            {
                Console.WriteLine("Du har inga lån.");
            }

            foreach (Loan loan in loans)
            {
                Console.WriteLine(
                    $"Lån {loan.Id}: {loan.Amount} SEK. Årsränta: {loan.InterestPercent} procent.");

                if (loan.IsPaidOut == true)
                {
                    Console.WriteLine("Utbetalt.");
                }
                else
                {
                    Console.WriteLine("Väntar på utbetalning.");
                }
            }
        }


    }
}
