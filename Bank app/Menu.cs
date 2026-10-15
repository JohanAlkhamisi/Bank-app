namespace Bank_app
{
    // Here I show the menus and ask the bank to do what the user chooses.
    public class Menu
    {
        private Bank bank = new Bank();
        private Input input;

        // Here I give Input the same bank that the menus use.
        public Menu()
        {
            input = new Input(bank);
        }

        // Here I show the start menu until the user chooses to exit.
        public void Start()
        {
            ShowLogo();

            while (input.EndOfInput == false)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("1. Logga in");
                Console.WriteLine("0. Avsluta");

                // Return to the normal console color.
                Console.ResetColor();

                string choice = input.ReadText("Välj: ");

                switch (choice)
                {
                    case "1":
                        Login();
                        break;

                    case "0":
                        return;

                    default:
                        Console.WriteLine("Välj ett alternativ i menyn.");
                        break;
                }
            }
        }

        // Here I print the bank logo in cyan and then reset the color.
        private void ShowLogo()
        {
            // ConsoleColor changes the text color of the logo.
            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.WriteLine(" ________________________ ");
            Console.WriteLine("|        CHAS BANK       |");
            Console.WriteLine("|________________________|");

            // Return to the normal console color.
            Console.ResetColor();
        }

        // Here I read the username and password and open the menu if login works.
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

        // Here I show the admin menu or the customer menu for the logged-in user.
        private void ShowUserMenu(User user)
        {
            while (input.EndOfInput == false)
            {
                Console.WriteLine();

                if (user.IsAdmin == true)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;

                    Console.WriteLine("1. Skapa användare");
                    Console.WriteLine("2. Lås upp användare");
                    Console.WriteLine("3. Uppdatera dagens valutakurser");
                    Console.WriteLine("4. Visa transaktionskön");
                    Console.WriteLine("5. Statistik");
                    Console.WriteLine("6. Ändra låneränta");
                    Console.WriteLine("7. Ändra kreditvärdighet");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;

                    Console.WriteLine("1. Mina konton");
                    Console.WriteLine("2. Öppna konto");
                    Console.WriteLine("3. Överför pengar");
                    Console.WriteLine("4. Öppna sparkonto");
                    Console.WriteLine("5. Sätt in pengar");
                    Console.WriteLine("6. Ansök om lån");
                    Console.WriteLine("7. Mina lån");
                    Console.WriteLine("8. Transaktionshistorik");
                    Console.WriteLine("9. Ändra kontaktuppgifter");
                    Console.WriteLine("10. Mina notifieringar");
                }

                // Return to the normal console color.
                Console.ResetColor();

                Console.WriteLine("0. Logga ut");
                Console.WriteLine($"Nästa behandling av transaktioner: {bank.GetNextProcessingTime():HH:mm}.");

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

                        case "3":
                            UpdateRates(user);
                            break;

                        case "4":
                            ShowQueue(user);
                            break;

                        case "5":
                            ShowStatistics(user);
                            break;

                        case "6":
                            SetLoanPercent(user);
                            break;

                        case "7":
                            SetCreditRating(user);
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

                        case "8":
                            ShowHistory(user);
                            break;

                        case "9":
                            UpdateProfile(user);
                            break;

                        case "10":
                            ShowNotifications(user);
                            break;

                        default:
                            Console.WriteLine("Ogiltigt menyval.");
                            break;
                    }
                }
            }
        }

        // Here I ask for the new username and password and show if the user was created.
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

        // Here I ask which user the admin wants to unlock.
        public void UnlockUser(User admin)
        {
            string username = input.ReadText("Användarnamn att låsa upp: ");

            if (input.EndOfInput == true)
            {
                return;
            }

            if (bank.UnlockUser(admin, username) == true)
            {
                Console.WriteLine("Användaren är upplåst.");
            }
            else
            {
                Console.WriteLine("Användaren finns inte eller du är inte administratör.");
            }
        }

        // Here I show the user's accounts, balances and money they can still use.
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

        // Here I ask for the new account's name and currency.
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

        // Here I read both account numbers and the amount to put in the transfer queue.
        private void Transfer(User user)
        {
            ShowAccounts(user);

            int from = input.ReadNumber("Från ditt kontonummer: ");
            int to = input.ReadNumber("Till kontonummer: ");
            decimal amount = input.ReadDecimal("Belopp i avsändarkontots valuta: ");

            if (input.EndOfInput == true)
            {
                return;
            }

            if (bank.Transfer(user, from, to, amount) == true)
            {
                Console.WriteLine($"Överföringen ligger i kö till {bank.GetNextProcessingTime():HH:mm}.");
            }
            else
            {
                Console.WriteLine("Kontrollera konton, ägare, belopp, tillgängligt saldo och valutakurser.");
            }
        }

        // Here I show how many payments are waiting and when they will run.
        private void ShowQueue(User admin)
        {
            Console.WriteLine($"Väntande transaktioner: {bank.GetPendingCount(admin)}.");
            Console.WriteLine($"Nästa behandling: {bank.GetNextProcessingTime():yyyy-MM-dd HH:mm}.");
            Console.WriteLine("Transaktionerna behandlas automatiskt medan programmet är öppet.");
        }

        // Here I ask for a name and currency for the new savings account.
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

        // Here I show one year's interest before asking if the user wants to deposit the money.
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
                    Console.WriteLine("Insättningen ligger i kö till nästa kvart.");
                }
            }
        }

        // Here I show the loan limit and interest before asking the user to confirm.
        private void RequestLoan(User user)
        {
            decimal limit = bank.GetLoanLimit(user);

            if (limit < 0)
            {
                Console.WriteLine("Admin måste uppdatera bankdagens valutakurser.");
                return;
            }

            Console.WriteLine($"Du kan låna ytterligare {limit} SEK.");

            ShowAccounts(user);

            int number = input.ReadNumber("Ditt konto i SEK: ");
            decimal amount = input.ReadDecimal("Lånebelopp i SEK: ");

            if (bank.AmountIsValid(amount) == false || input.EndOfInput == true)
            {
                Console.WriteLine("Ogiltigt belopp.");
                return;
            }

            decimal percent = bank.GetLoanPercent(user);
            decimal interest = bank.RoundMoney(amount * percent / 100);

            Console.WriteLine($"Årsränta: {percent} procent. Beräknad ränta första året: {interest} SEK.");

            string confirm = input.ReadText("Ansök om lånet? 1 ja, 0 nej: ");

            if (confirm == "1" && input.EndOfInput == false)
            {
                if (bank.RequestLoan(user, number, amount) == true)
                {
                    Console.WriteLine("Lånet registrerades. Pengarna ligger i kö till nästa kvart.");
                }
                else
                {
                    Console.WriteLine("Kontrollera konto, valuta och lånegräns.");
                }
            }
        }

        // Here I show the user's loans, interest rates and whether the money has been paid out.
        private void ShowLoans(User user)
        {
            List<Loan> loans = bank.GetLoans(user);

            if (loans.Count == 0)
            {
                Console.WriteLine("Du har inga lån.");
            }

            foreach (Loan loan in loans)
            {
                Console.WriteLine($"Lån {loan.Id}: {loan.Amount} SEK. Årsränta: {loan.InterestPercent} procent.");

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

        // Here I show each payment and whether it is waiting, finished or failed.
        private void ShowHistory(User user)
        {
            List<Transaction> history = bank.GetHistory(user);

            if (history.Count == 0)
            {
                Console.WriteLine("Inga transaktioner ännu.");
            }

            foreach (Transaction transaction in history)
            {
                Console.WriteLine($"Transaktion {transaction.Id}. Skapad {transaction.CreatedAt:yyyy-MM-dd HH:mm}. Behandlas {transaction.ExecuteAt:yyyy-MM-dd HH:mm}.");

                switch (transaction.Type)
                {
                    case TransactionType.Transfer:
                        Console.WriteLine("Överföring.");
                        break;

                    case TransactionType.Deposit:
                        Console.WriteLine("Insättning.");
                        break;

                    case TransactionType.Loan:
                        Console.WriteLine("Låneutbetalning.");
                        break;
                }

                int toIndex = bank.FindAccountIndex(transaction.ToAccount);

                if (toIndex != -1)
                {
                    BankAccount to = bank.GetAccount(toIndex);

                    if (transaction.FromAccount == 0)
                    {
                        Console.WriteLine($"Till konto {transaction.ToAccount}. Belopp: {transaction.ReceivedAmount} {to.Currency}.");
                    }
                    else
                    {
                        Console.WriteLine($"Från konto {transaction.FromAccount} till konto {transaction.ToAccount}. Mottagaren får {transaction.ReceivedAmount} {to.Currency}.");
                    }
                }
                else
                {
                    Console.WriteLine("Mottagarkontot finns inte längre.");
                }

                switch (transaction.Status)
                {
                    case TransactionStatus.Pending:
                        Console.WriteLine("Väntar.");
                        break;

                    case TransactionStatus.Completed:
                        Console.WriteLine("Genomförd.");
                        break;

                    case TransactionStatus.Failed:
                        Console.WriteLine("Misslyckad.");
                        break;
                }
            }
        }

        // Here I read the new email address and phone number.
        private void UpdateProfile(User user)
        {
            Console.WriteLine($"E-post: {user.Email}. Telefon: {user.Phone}.");

            string email = input.ReadText("Ny e-post, tomt tar bort: ");
            string phone = input.ReadText("Nytt telefonnummer, tomt tar bort: ");

            if (input.EndOfInput == true)
            {
                return;
            }

            if (bank.UpdateProfile(user, email, phone) == true)
            {
                Console.WriteLine("Uppgifterna uppdaterades.");
            }
            else
            {
                Console.WriteLine("Ange en e-postadress med @ och en punkt i domänen, till exempel namn@exempel.se.");
            }
        }

        // Here I show the user's pretend emails and SMS messages.
        private void ShowNotifications(User user)
        {
            Console.WriteLine("Här visas simulerade mejl och SMS. Inga riktiga meddelanden skickas.");
            Console.WriteLine($"Gränsen är över {bank.GetNotificationLimit()} i kontots valuta.");

            List<Notification> notifications = bank.GetNotifications(user);

            if (notifications.Count == 0)
            {
                Console.WriteLine("Du har inga notifieringar ännu.");
            }

            Console.WriteLine("Spara e-post eller telefonnummer under menyval 9 innan transaktionen genomförs.");

            foreach (Notification notification in notifications)
            {
                Console.WriteLine($"{notification.CreatedAt:yyyy-MM-dd HH:mm} | Typ: {notification.Channel} | Till: {notification.Address}");
                Console.WriteLine($"Konto: {notification.AccountNumber} | Transaktion: {notification.TransactionId}");
                Console.WriteLine(notification.Message);
                Console.WriteLine();
            }
        }

        // Here I turn 1, 2 or 3 into a low, medium or high credit category.
        private CreditRating ReadCreditRating()
        {
            CreditRating rating = CreditRating.Medium;
            bool valid = false;

            while (valid == false && input.EndOfInput == false)
            {
                int choice = input.ReadNumber("Kreditvärdighet: 1 låg, 2 medel, 3 hög: ");

                switch (choice)
                {
                    case 1:
                        rating = CreditRating.Low;
                        valid = true;
                        break;

                    case 2:
                        rating = CreditRating.Medium;
                        valid = true;
                        break;

                    case 3:
                        rating = CreditRating.High;
                        valid = true;
                        break;

                    default:
                        Console.WriteLine("Välj 1, 2 eller 3.");
                        break;
                }
            }

            return rating;
        }

        // Here I ask which credit category and interest rate the admin wants to change.
        private void SetLoanPercent(User admin)
        {
            CreditRating rating = ReadCreditRating();
            decimal percent = input.ReadDecimal("Årsränta i procent: ");

            if (input.EndOfInput == true)
            {
                return;
            }

            if (bank.SetLoanPercent(admin, rating, percent) == true)
            {
                Console.WriteLine("Räntan gäller för nya lån.");
            }
            else
            {
                Console.WriteLine("Räntan måste vara mellan 0 och 100 procent.");
            }
        }

        // Here I ask which user and credit category the admin wants to change.
        private void SetCreditRating(User admin)
        {
            string username = input.ReadText("Användarnamn: ");
            CreditRating rating = ReadCreditRating();

            if (input.EndOfInput == true)
            {
                return;
            }

            if (bank.SetCreditRating(admin, username, rating) == true)
            {
                Console.WriteLine("Kreditvärdigheten uppdaterades.");
            }
            else
            {
                Console.WriteLine("Användaren finns inte.");
            }
        }

        // Here I show the transfer count, total amount and most used account.
        private void ShowStatistics(User admin)
        {
            Console.WriteLine($"Genomförda överföringar: {bank.GetTransferCount(admin)}.");
            Console.WriteLine($"Totalt överfört: {bank.GetTransferTotal(admin)} SEK.");

            int number = bank.GetMostUsedAccount(admin);

            if (number == 0)
            {
                Console.WriteLine("Inget konto har en genomförd överföring ännu.");
            }
            else
            {
                Console.WriteLine($"Mest använt konto: {number}.");
            }
        }

        // Here I ask the admin to enter and confirm the exchange rates for today.
        private void UpdateRates(User admin)
        {
            if (bank.GetRatesDate() == DateTime.Today)
            {
                Console.WriteLine("Dagens kurser är redan sparade. Du kan ändra dem här.");
            }
            else
            {
                Console.WriteLine("Dagens valutakurser behöver uppdateras.");
            }

            Console.WriteLine("Ange hur många SEK en EUR respektive en USD är värd.");

            decimal euro = input.ReadDecimal("SEK för 1 EUR: ");
            decimal dollar = input.ReadDecimal("SEK för 1 USD: ");

            if (input.EndOfInput == true)
            {
                return;
            }

            if (bank.UpdateRates(admin, euro, dollar) == true)
            {
                Console.WriteLine("Dagens valutakurser sparades.");
            }
            else
            {
                Console.WriteLine("Kurserna måste vara mellan 0,0001 och 1000 SEK.");
            }
        }
    }
}