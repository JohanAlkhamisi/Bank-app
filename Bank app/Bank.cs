using System;
using System.Collections.Generic;

namespace Bank_app
{
    // Here I keep the users, accounts and the methods that handle money.
    public class Bank
    {
        // The lists keep the data while the app is open.
        private List<User> users = new List<User>();
        private List<BankAccount> accounts = new List<BankAccount>();

        // The oldest waiting payment is checked first.
        private Queue<Transaction> waiting = new Queue<Transaction>();

        private List<Transaction> transactions = new List<Transaction>();
        private List<Loan> loans = new List<Loan>();

        // These messages stay in the app. No real emails or SMS messages are sent.
        private List<Notification> notifications = new List<Notification>();

        // A message is created for amounts above 10000 in the account's currency.
        private decimal notificationLimit = 10000;

        // DateTime keeps a real date and time instead of a simulated minute counter.
        private DateTime nextProcessingTime;

        // The empty starting date means the admin has not confirmed any rates yet.
        private DateTime ratesUpdatedOn = DateTime.MinValue;

        // Each credit category has its own interest percentage.
        private Dictionary<CreditRating, decimal> loanPercents = new Dictionary<CreditRating, decimal>();

        // Each rate says how many SEK one unit of the currency is worth.
        private Dictionary<Currency, decimal> exchangeRates = new Dictionary<Currency, decimal>();

        // Here I add the test users, accounts and starting interest rates.
        public Bank()
        {
            nextProcessingTime = NextQuarter(DateTime.Now);

            users.Add(new User(1, "admin", "Admin123", true));
            users.Add(new User(2, "johan", "johan123", false));
            users.Add(new User(3, "sara", "Sara123", false));

            loanPercents.Add(CreditRating.Low, 8);
            loanPercents.Add(CreditRating.Medium, 5);
            loanPercents.Add(CreditRating.High, 3);

            exchangeRates.Add(Currency.SEK, 1);
            exchangeRates.Add(Currency.EUR, 11);
            exchangeRates.Add(Currency.USD, 10);

            accounts.Add(new BankAccount(1001, 2, "Lönekonto", Currency.SEK, 10000));
            accounts.Add(new BankAccount(1002, 3, "Lönekonto", Currency.SEK, 5000));
            accounts.Add(new BankAccount(1003, 2, "Extra", Currency.SEK, 0));
        }

        // Here I make a list of only this user's accounts.
        public List<BankAccount> GetAccounts(User user)
        {
            List<BankAccount> result = new List<BankAccount>();

            foreach (BankAccount account in accounts)
            {
                if (account.OwnerId == user.Id)
                {
                    result.Add(account);
                }
            }

            return result;
        }

        // Here I find the account's place in the list. I return -1 if it is missing.
        public int FindAccountIndex(int accountNumber)
        {
            for (int i = 0; i < accounts.Count; i++)
            {
                if (accounts[i].Number == accountNumber)
                {
                    return i;
                }
            }

            return -1;
        }

        // Here I get an account using a list position that has already been checked.
        public BankAccount GetAccount(int accountIndex)
        {
            return accounts[accountIndex];
        }

        // Here I create an account with a new number and a balance of 0.
        public bool OpenAccount(User user, string name, Currency currency)
        {
            if (HasText(name) == false)
            {
                return false;
            }

            BankAccount account = new BankAccount(1001 + accounts.Count, user.Id, name, currency, 0);

            accounts.Add(account);

            return true;
        }

        // Here I check the login details and return the user's place in the list, or -1 if login fails.
        public int Login(string username, string password)
        {
            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].Username == username)
                {
                    if (users[i].IsLocked == true)
                    {
                        return -1;
                    }

                    if (users[i].CheckPassword(password) == true)
                    {
                        users[i].FailedAttempts = 0;
                        return i;
                    }

                    // A wrong password adds one failed attempt.
                    users[i].FailedAttempts = users[i].FailedAttempts + 1;

                    if (users[i].FailedAttempts >= 3)
                    {
                        users[i].IsLocked = true;
                    }

                    return -1;
                }
            }

            return -1;
        }

        // Here I let the admin unlock a user and reset the number of wrong login attempts.
        public bool UnlockUser(User admin, string username)
        {
            if (admin.IsAdmin == false)
            {
                return false;
            }

            foreach (User user in users)
            {
                if (user.Username == username)
                {
                    user.IsLocked = false;
                    user.FailedAttempts = 0;

                    return true;
                }
            }

            return false;
        }

        // Here I get the user from the list position found during login.
        public User GetUser(int userIndex)
        {
            return users[userIndex];
        }

        // Here I check that the text is not empty or made up of only spaces.
        public bool HasText(string text)
        {
            if (text == null || text == "")
            {
                return false;
            }

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] != ' ')
                {
                    return true;
                }
            }

            return false;
        }

        // Here I check the admin, username and password before adding a new user.
        public bool CreateUser(User admin, string username, string password)
        {
            if (admin.IsAdmin == false)
            {
                return false;
            }

            if (HasText(username) == false || HasText(password) == false)
            {
                return false;
            }

            if (username.Length < 3 || password.Length < 6)
            {
                return false;
            }

            foreach (User user in users)
            {
                if (user.Username == username)
                {
                    return false;
                }
            }

            users.Add(new User(users.Count + 1, username, password, false));

            return true;
        }

        // Here I get the real time when the next batch of payments will run.
        public DateTime GetNextProcessingTime()
        {
            return nextProcessingTime;
        }

        // Here I find the next time ending in :00, :15, :30 or :45.
        private DateTime NextQuarter(DateTime time)
        {
            // Start at the current whole minute, without seconds.
            DateTime minute = new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, 0);

            // The remainder tells me how many minutes are left until the next quarter-hour.
            int minutesToAdd = 15 - time.Minute % 15;

            return minute.AddMinutes(minutesToAdd);
        }

        // Here I check that the amount is above 0, at most 100000 and has at most two decimal places.
        public bool AmountIsValid(decimal amount)
        {
            if (amount <= 0 || amount > 100000)
            {
                return false;
            }

            if ((amount * 100) % 1 != 0)
            {
                return false;
            }

            return true;
        }

        // Here I round the amount to two decimal places using normal maths.
        public decimal RoundMoney(decimal amount)
        {
            decimal cents = amount * 100;
            decimal fraction = cents % 1;

            cents = cents - fraction;

            if (fraction >= 0.5m)
            {
                cents = cents + 1;
            }

            return cents / 100;
        }

        // Here I subtract waiting transfers so the same money cannot be spent twice.
        public decimal GetAvailableBalance(BankAccount account)
        {
            decimal available = account.Balance;

            foreach (Transaction transaction in waiting)
            {
                if (transaction.FromAccount == account.Number)
                {
                    available = available - transaction.Amount;
                }
            }

            return available;
        }

        // Here I change the amount to the other currency using the rates saved for today.
        public decimal ConvertMoney(decimal amount, Currency from, Currency to)
        {
            if (from == to)
            {
                return amount;
            }

            // Foreign currency needs rates that the admin has confirmed today.
            if (ratesUpdatedOn != DateTime.Today)
            {
                return -1;
            }

            // First change the amount to SEK, then change SEK to the receiving currency.
            decimal amountInSek = amount * exchangeRates[from];
            decimal received = amountInSek / exchangeRates[to];

            return RoundMoney(received);
        }

        // Here I check both accounts and put the transfer in the queue for the next quarter-hour.
        public bool Transfer(User user, int fromNumber, int toNumber, decimal amount)
        {
            if (AmountIsValid(amount) == false || fromNumber == toNumber)
            {
                return false;
            }

            int fromIndex = FindAccountIndex(fromNumber);
            int toIndex = FindAccountIndex(toNumber);

            if (fromIndex == -1 || toIndex == -1)
            {
                return false;
            }

            BankAccount from = accounts[fromIndex];
            BankAccount to = accounts[toIndex];

            if (from.OwnerId != user.Id || amount > GetAvailableBalance(from))
            {
                return false;
            }

            decimal received = ConvertMoney(amount, from.Currency, to.Currency);
            decimal amountInSek = ConvertMoney(amount, from.Currency, Currency.SEK);

            if (received <= 0 || amountInSek <= 0)
            {
                return false;
            }

            // Use the real submission time and finish any older batch before adding this request.
            DateTime createdAt = DateTime.Now;

            ProcessTransactions(createdAt);

            DateTime executeAt = NextQuarter(createdAt);

            Transaction transaction = new Transaction(transactions.Count + 1, user.Id, fromNumber, toNumber, amount, received, amountInSek, TransactionType.Transfer, createdAt, executeAt);

            // The same transaction is saved in the queue and the history list.
            waiting.Enqueue(transaction);
            transactions.Add(transaction);

            return true;
        }

        // Here I run the waiting payments when their real processing time has arrived.
        public int ProcessTransactions(DateTime currentTime)
        {
            int completed = 0;

            // The queue is only processed when the real clock reaches the next quarter-hour.
            if (currentTime >= nextProcessingTime)
            {
                while (waiting.Count > 0)
                {
                    // Look at the first payment without removing it yet.
                    Transaction transaction = waiting.Peek();

                    if (transaction.ExecuteAt > currentTime)
                    {
                        break;
                    }

                    transaction = waiting.Dequeue();

                    // A failed check changes this to false.
                    bool valid = true;

                    int toIndex = FindAccountIndex(transaction.ToAccount);

                    if (toIndex == -1)
                    {
                        valid = false;
                    }

                    if (valid == true && transaction.FromAccount != 0)
                    {
                        int fromIndex = FindAccountIndex(transaction.FromAccount);

                        if (fromIndex == -1)
                        {
                            valid = false;
                        }
                        else
                        {
                            BankAccount from = accounts[fromIndex];

                            if (from.Withdraw(transaction.Amount) == false)
                            {
                                valid = false;
                            }
                        }
                    }

                    if (valid == true)
                    {
                        BankAccount to = accounts[toIndex];

                        to.Deposit(transaction.ReceivedAmount);

                        // Find the loan for this payment and mark it as paid out.
                        if (transaction.Type == TransactionType.Loan)
                        {
                            foreach (Loan loan in loans)
                            {
                                if (loan.Id == transaction.LoanId)
                                {
                                    loan.IsPaidOut = true;
                                }
                            }
                        }

                        transaction.Status = TransactionStatus.Completed;

                        // Only a completed payment can create messages.
                        CreateNotifications(transaction);

                        completed = completed + 1;
                    }
                    else
                    {
                        transaction.Status = TransactionStatus.Failed;
                    }
                }

                // After this batch, wait for the next real quarter-hour.
                nextProcessingTime = NextQuarter(currentTime);
            }

            return completed;
        }

        // Here I add the new savings account to the same list as normal accounts.
        public bool OpenSavingsAccount(User user, string name, Currency currency)
        {
            if (HasText(name) == false)
            {
                return false;
            }

            SavingsAccount account = new SavingsAccount(1001 + accounts.Count, user.Id, name, currency);

            accounts.Add(account);

            return true;
        }

        // Here I put the deposit in the queue. The money is added at the next quarter-hour.
        public bool Deposit(User user, int accountNumber, decimal amount)
        {
            if (AmountIsValid(amount) == false)
            {
                return false;
            }

            int accountIndex = FindAccountIndex(accountNumber);

            if (accountIndex == -1 || accounts[accountIndex].OwnerId != user.Id)
            {
                return false;
            }

            // Use the real submission time and finish any older batch before adding this request.
            DateTime createdAt = DateTime.Now;

            ProcessTransactions(createdAt);

            DateTime executeAt = NextQuarter(createdAt);

            Transaction transaction = new Transaction(transactions.Count + 1, user.Id, 0, accountNumber, amount, amount, 0, TransactionType.Deposit, createdAt, executeAt);

            // The same transaction is saved in the queue and the history list.
            waiting.Enqueue(transaction);
            transactions.Add(transaction);

            return true;
        }

        // Here I make a list of this user's loans.
        public List<Loan> GetLoans(User user)
        {
            List<Loan> result = new List<Loan>();

            foreach (Loan loan in loans)
            {
                if (loan.UserId == user.Id)
                {
                    result.Add(loan);
                }
            }

            return result;
        }

        // Here I get the interest rate for the user's credit category.
        public decimal GetLoanPercent(User user)
        {
            return loanPercents[user.CreditRating];
        }

        // Here I work out how much more money the user can borrow.
        public decimal GetLoanLimit(User user)
        {
            decimal ownMoney = 0;
            decimal totalLoans = 0;

            foreach (BankAccount account in GetAccounts(user))
            {
                if (account.Balance > 0)
                {
                    decimal value = ConvertMoney(account.Balance, account.Currency, Currency.SEK);

                    if (value < 0)
                    {
                        return -1;
                    }

                    ownMoney = ownMoney + value;
                }
            }

            foreach (Loan loan in GetLoans(user))
            {
                // Waiting loans and paid-out loans both reduce how much more the user can borrow.
                totalLoans = totalLoans + loan.Amount;

                if (loan.IsPaidOut == true)
                {
                    // Borrowed money must not count as the user's own money.
                    ownMoney = ownMoney - loan.Amount;
                }
            }

            foreach (Transaction transaction in waiting)
            {
                if (transaction.Type == TransactionType.Transfer && transaction.UserId == user.Id)
                {
                    int targetIndex = FindAccountIndex(transaction.ToAccount);

                    if (targetIndex != -1 && accounts[targetIndex].OwnerId != user.Id)
                    {
                        int fromIndex = FindAccountIndex(transaction.FromAccount);

                        if (fromIndex == -1)
                        {
                            return -1;
                        }

                        // Use today's SEK value of money already promised to another customer.
                        decimal reserved = ConvertMoney(transaction.Amount, accounts[fromIndex].Currency, Currency.SEK);

                        if (reserved < 0)
                        {
                            return -1;
                        }

                        ownMoney = ownMoney - reserved;
                    }
                }
            }

            if (ownMoney < 0)
            {
                ownMoney = 0;
            }

            // The total loan limit is five times the user's own money, minus loans already taken.
            decimal remaining = ownMoney * 5 - totalLoans;

            if (remaining < 0)
            {
                remaining = 0;
            }

            return remaining;
        }

        // Here I check the account and loan limit before queuing the loan payment.
        public bool RequestLoan(User user, int accountNumber, decimal amount)
        {
            if (AmountIsValid(amount) == false)
            {
                return false;
            }

            int accountIndex = FindAccountIndex(accountNumber);

            if (accountIndex == -1)
            {
                return false;
            }

            BankAccount account = accounts[accountIndex];

            if (account.OwnerId != user.Id || account.Currency != Currency.SEK)
            {
                return false;
            }

            decimal limit = GetLoanLimit(user);

            if (limit < 0 || amount > limit)
            {
                return false;
            }

            Loan loan = new Loan(loans.Count + 1, user.Id, amount, GetLoanPercent(user));

            loans.Add(loan);

            // Use the real submission time and finish any older batch before adding this request.
            DateTime createdAt = DateTime.Now;

            ProcessTransactions(createdAt);

            DateTime executeAt = NextQuarter(createdAt);

            Transaction transaction = new Transaction(transactions.Count + 1, user.Id, 0, accountNumber, amount, amount, amount, TransactionType.Loan, createdAt, executeAt);

            transaction.LoanId = loan.Id;

            // The same transaction is saved in the queue and the history list.
            waiting.Enqueue(transaction);
            transactions.Add(transaction);

            return true;
        }

        // Here I collect the transfers, deposits and loan payments for this user's accounts.
        public List<Transaction> GetHistory(User user)
        {
            List<Transaction> result = new List<Transaction>();

            foreach (Transaction transaction in transactions)
            {
                bool ownsAccount = false;

                int fromIndex = FindAccountIndex(transaction.FromAccount);
                int toIndex = FindAccountIndex(transaction.ToAccount);

                if (fromIndex != -1)
                {
                    if (accounts[fromIndex].OwnerId == user.Id)
                    {
                        ownsAccount = true;
                    }
                }

                if (toIndex != -1)
                {
                    if (accounts[toIndex].OwnerId == user.Id)
                    {
                        ownsAccount = true;
                    }
                }

                if (ownsAccount == true)
                {
                    result.Add(transaction);
                }
            }

            return result;
        }

        // Here I check the email address and save the new contact details.
        public bool UpdateProfile(User user, string email, string phone)
        {
            // These flags keep track of the @ sign and a dot after it.
            bool hasAt = false;
            bool hasDot = false;

            // A failed check changes this to false.
            bool valid = true;

            for (int i = 0; i < email.Length; i++)
            {
                if (email[i] == ' ')
                {
                    valid = false;
                }

                if (email[i] == '@')
                {
                    if (hasAt == true || i == 0 || i == email.Length - 1)
                    {
                        valid = false;
                    }

                    hasAt = true;
                }

                if (email[i] == '.' && hasAt == true && i > 0 && i < email.Length - 1)
                {
                    if (email[i - 1] != '@')
                    {
                        hasDot = true;
                    }
                }
            }

            // An empty email removes the saved address. Otherwise its basic format must be valid.
            if (email != "")
            {
                if (hasAt == false || hasDot == false || valid == false || email[email.Length - 1] == '.')
                {
                    return false;
                }
            }

            user.Email = email;
            user.Phone = phone;

            return true;
        }

        // Here I let the admin change the interest rate for new loans in a credit category.
        public bool SetLoanPercent(User admin, CreditRating rating, decimal percent)
        {
            if (admin.IsAdmin == false || percent < 0 || percent > 100)
            {
                return false;
            }

            loanPercents[rating] = percent;

            return true;
        }

        // Here I let the admin change the user's credit category.
        public bool SetCreditRating(User admin, string username, CreditRating rating)
        {
            if (admin.IsAdmin == false)
            {
                return false;
            }

            foreach (User user in users)
            {
                if (user.Username == username)
                {
                    user.CreditRating = rating;

                    return true;
                }
            }

            return false;
        }

        // Here I count the transfers that have been completed.
        public int GetTransferCount(User admin)
        {
            if (admin.IsAdmin == false)
            {
                return 0;
            }

            int count = 0;

            foreach (Transaction transaction in transactions)
            {
                if (transaction.Type == TransactionType.Transfer && transaction.Status == TransactionStatus.Completed)
                {
                    count = count + 1;
                }
            }

            return count;
        }

        // Here I add the saved SEK amounts for completed transfers.
        public decimal GetTransferTotal(User admin)
        {
            if (admin.IsAdmin == false)
            {
                return 0;
            }

            decimal total = 0;

            foreach (Transaction transaction in transactions)
            {
                if (transaction.Type == TransactionType.Transfer && transaction.Status == TransactionStatus.Completed)
                {
                    total = total + transaction.AmountInSek;
                }
            }

            return total;
        }

        // Here I find which account appears in the most completed transfers.
        public int GetMostUsedAccount(User admin)
        {
            if (admin.IsAdmin == false)
            {
                return 0;
            }

            int mostUsedNumber = 0;
            int highestCount = 0;

            foreach (BankAccount account in accounts)
            {
                int count = 0;

                foreach (Transaction transaction in transactions)
                {
                    if (transaction.Type == TransactionType.Transfer && transaction.Status == TransactionStatus.Completed)
                    {
                        if (transaction.FromAccount == account.Number || transaction.ToAccount == account.Number)
                        {
                            count++;
                        }
                    }
                }

                if (count > highestCount)
                {
                    highestCount = count;
                    mostUsedNumber = account.Number;
                }
            }

            return mostUsedNumber;
        }

        // Here I let the admin save how many SEK one EUR and one USD are worth today.
        public bool UpdateRates(User admin, decimal euro, decimal dollar)
        {
            if (admin.IsAdmin == false)
            {
                return false;
            }

            if (euro < 0.0001m || euro > 1000 || dollar < 0.0001m || dollar > 1000)
            {
                return false;
            }

            exchangeRates[Currency.EUR] = euro;
            exchangeRates[Currency.USD] = dollar;

            // These rates are valid for the current calendar day.
            ratesUpdatedOn = DateTime.Today;

            return true;
        }

        // Here I get the date when the admin last confirmed the exchange rates.
        public DateTime GetRatesDate()
        {
            return ratesUpdatedOn;
        }

        // Here I get the amount that a payment must be above to create a message.
        public decimal GetNotificationLimit()
        {
            return notificationLimit;
        }

        // Here I check the account that paid and the account that received money.
        private void CreateNotifications(Transaction transaction)
        {
            if (transaction.FromAccount != 0)
            {
                CreateAccountNotifications(transaction, transaction.FromAccount, transaction.Amount, false);
            }

            // A deposit or loan only has a receiving account.
            CreateAccountNotifications(transaction, transaction.ToAccount, transaction.ReceivedAmount, true);
        }

        // Here I create messages if this account had a payment above the limit.
        private void CreateAccountNotifications(Transaction transaction, int accountNumber, decimal amount, bool incoming)
        {
            if (amount <= notificationLimit)
            {
                return;
            }

            int accountIndex = FindAccountIndex(accountNumber);

            if (accountIndex == -1)
            {
                return;
            }

            BankAccount account = accounts[accountIndex];
            string message;

            if (incoming == true)
            {
                message = "Du har fått " + amount + " " + account.Currency + " till konto " + account.Number + ".";
            }
            else
            {
                message = amount + " " + account.Currency + " har förts över från konto " + account.Number + ".";
            }

            // Find the owner of this account before choosing the saved contact details.
            foreach (User user in users)
            {
                if (user.Id == account.OwnerId)
                {
                    // Save one pretend email if the owner has an email address.
                    if (HasText(user.Email) == true)
                    {
                        Notification email = new Notification(user.Id, account.Number, transaction.Id, "Email", user.Email, message);
                        notifications.Add(email);
                    }

                    // Save one pretend SMS if the owner has a phone number.
                    if (HasText(user.Phone) == true)
                    {
                        Notification sms = new Notification(user.Id, account.Number, transaction.Id, "SMS", user.Phone, message);
                        notifications.Add(sms);
                    }

                    break;
                }
            }
        }

        // Here I make a list of only this user's messages.
        public List<Notification> GetNotifications(User user)
        {
            List<Notification> result = new List<Notification>();

            foreach (Notification notification in notifications)
            {
                if (notification.UserId == user.Id)
                {
                    result.Add(notification);
                }
            }

            return result;
        }

        // Here I show the admin how many payments are still waiting.
        public int GetPendingCount(User admin)
        {
            if (admin.IsAdmin == false)
            {
                return 0;
            }

            return waiting.Count;
        }
    }
}