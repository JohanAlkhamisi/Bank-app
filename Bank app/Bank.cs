using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Bank_app
{
    public class Bank
    {

        // Here I keep the users, accounts, transfers and loans while the app is running
        private List<User> users = new List<User>();
        private List<BankAccount> accounts = new List<BankAccount>();
        private Queue<Transaction> waiting = new Queue<Transaction>();
        private List<Transaction> transactions = new List<Transaction>();
        private List<Loan> loans = new List<Loan>();
        private int bankMinute = 0;

        private Dictionary<CreditRating, decimal> loanPercents = new Dictionary<CreditRating, decimal>();
        private Dictionary<Currency, decimal> exchangeRates = new Dictionary<Currency, decimal>();

        // Here I add the test users and their first accounts.
        public Bank()
        {
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

        // Here I make a list of this user's accounts.
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

        // Here I get the account from its place in the list. The place must be checked first.
        public BankAccount GetAccount(int accountIndex)
        {
            return accounts[accountIndex];
        }

        // Here I create an account with a starting balance of 0 for the logged-in user.
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

        // Here I check the login details. I return the user's place in the list, or -1 if login fails.
        public int Login(string username, string password)
        {
            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].Username == username)
                {
                    if (users[i].Username == username)
                    {
                        return -1;
                    }
                    if (users[i].CheckPassword(password) == true)
                    {
                        users[i].FailedAttempts = 0;
                        return i;
                    }
                    if (users[i].FailedAttempts >= 3)
                    {
                        users[i].IsLocked = true;
                    }

                    return -1;
                }
            }
            return -1;
        }

        //here i let admin reset the lock and attemt counter 
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

        // Here I get the user from the place in the list found during login. 
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

        // Here I get the time in minutes from the bank's simulated clock.
        public int GetBankMinute()
        {
            return bankMinute;
        }

        public int GetBankDay()
        {
            return bankMinute / 1440 + 1;
        }

        // Here I check that the amount is above 0, no more than 100000 and has at most two decimal places 
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

        // Here I start with the balance and check transfers that are still waiting. 
        public decimal GetAvailableBalance(BankAccount account)
        {
            decimal available = account.Balance;

            foreach (Transaction transaction in waiting)
            {
                if (transaction.FromAccount == account.Number)
                {
                    available = available + transaction.Amount;
                }
            }
            return available;
        }

        public decimal ConvertMoney(decimal amount, Currency from, Currency to)
        {
            if (from == to)
            {
                return amount;
            }

            return -1;
        }

        // Here I check the accounts and amount before putting the transfer in the queue.
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

            if (from.OwnerId != user.Id
                || amount > GetAvailableBalance(from))
            {
                return false;
            }

            decimal received = ConvertMoney(amount, from.Currency, to.Currency);

            decimal amountInSek = ConvertMoney(amount, from.Currency, Currency.SEK);

            if (received <= 0 || amountInSek <= 0)
            {
                return false;
            }

            Transaction transaction = new Transaction(
                transactions.Count + 1,
                user.Id,
                fromNumber,
                toNumber,
                amount,
                received,
                amountInSek,
                TransactionType.Transfer,
                bankMinute,
                bankMinute + 15);

            waiting.Enqueue(transaction);
            transactions.Add(transaction);

            return true;
        }

        // Here I let the admin move the bank's simulated clock forward by 15 minutes
        public int ProcessNextQuarter(User admin)
        {
            if (admin.IsAdmin == false)
            {
                return -1;
            }

            bankMinute = bankMinute + 15;

            int completed = 0;

            while (waiting.Count > 0)
            {
                Transaction transaction = waiting.Peek();

                if (transaction.ExecuteMinute > bankMinute)
                {
                    break;
                }

                transaction = waiting.Dequeue();

                int toIndex = FindAccountIndex(transaction.ToAccount);

                if (toIndex == -1)
                {
                    transaction.Status = TransactionStatus.Failed;
                    continue;
                }

                BankAccount to = accounts[toIndex];

                if (transaction.FromAccount != 0)
                {
                    int fromIndex = FindAccountIndex(
                        transaction.FromAccount);

                    if (fromIndex == -1)
                    {
                        transaction.Status = TransactionStatus.Failed;
                        continue;
                    }

                    BankAccount from = accounts[fromIndex];

                    if (from.Withdraw(transaction.Amount) == false)
                    {
                        transaction.Status = TransactionStatus.Failed;
                        continue;
                    }
                }

                to.Deposit(transaction.ReceivedAmount);

                // Here I find the loan for this payment and mark it as paid out.
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
                completed++;
            }

            return completed;
        }

        // Here I add the new savings account to the list of accounts.
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

        // Here I put the deposit in the queue. The money is added when the admin runs it.
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


            Transaction transaction = new Transaction(transactions.Count + 1, user.Id, 0, accountNumber, amount, amount, 0, TransactionType.Deposit, bankMinute, bankMinute + 15);

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


        // Here I return the loan interest rate, which is 5 percent per year.
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
                // Here I count all the user's loans, including loans waiting to be paid out.
                totalLoans = totalLoans + loan.Amount;

                if (loan.IsPaidOut == true)
                {
                    // Here I take away borrowed money so it does not count as the user's own money.
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
                        ownMoney = ownMoney - transaction.AmountInSek;
                    }
                }
            }

            if (ownMoney < 0)
            {
                ownMoney = 0;
            }

            decimal remaining = ownMoney * 5 - totalLoans;

            if (remaining < 0)
            {
                remaining = 0;
            }

            return remaining;

        }


        // Here I check the account and loan limit before putting the loan payment in the queue.
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


            Transaction transaction = new Transaction(transactions.Count + 1, user.Id, 0, accountNumber, amount, amount, amount, TransactionType.Loan, bankMinute, bankMinute + 15);

            transaction.LoanId = loan.Id;

            waiting.Enqueue(transaction);
            transactions.Add(transaction);
            return true;
        }

        // Here I make a list of transfers, deposits and loan payments for this user's accounts.
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
                        ownsAccount = true;
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


        public bool UpdateProfile(User user, string email, string phone)
        {
            bool hasAt = false;
            bool hasDot = false;

            for (int i = 0; i < email.Length; i++)
            {
                if (email[i] == '@')
                {
                    hasAt = true;
                }

                if (email[i] == '.')
                {
                    hasDot = true;
                }
            }



            if (email != "" && (hasAt == false || hasDot == false))
            {
                return false;
            }

            user.Email = email;
            user.Phone = phone;

            return true;



        }



        // Here I save the percentage for future loans in a credit category.

        public bool SetLoanPercent(User admin, CreditRating rating, decimal percent)
        {
            if (admin.IsAdmin == false || percent < 0 || percent > 100)
            {
                return false;
            }

            loanPercents[rating] = percent;
            return true;
        }


        // Here I find the user with a loop and change the credit category.
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

        // Here I count completed transfers once each.
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
                if (transaction.Type == TransactionType.Transfer
                    && transaction.Status == TransactionStatus.Completed)
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

        // Here I count each account's uses with two normal loops.
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
                    if (transaction.Type == TransactionType.Transfer
                        && transaction.Status == TransactionStatus.Completed)
                    {
                        if (transaction.FromAccount == account.Number
                            || transaction.ToAccount == account.Number)
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

    }
}
