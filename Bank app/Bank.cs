using System;
using System.Collections.Generic;
using System.Text;

namespace Bank_app
{
    public class Bank
    {

        // here i create a list that will have users
        private List<User> users = new List<User>();
        private List<BankAccount> accounts = new List<BankAccount>();
        private Queue<Transaction> waiting = new Queue<Transaction>();
        private List<Transaction> transactions = new List<Transaction>();
        private int bankMinute = 0;

        // here i create the demo users and their starting values
        public Bank()
        {
            users.Add(new User(1, "admin", "Admin123", true));
            users.Add(new User(2, "johan", "johan123", false));
            users.Add(new User(3, "sara", "Sara123", false));

            accounts.Add(new BankAccount(1001, 2, "Lönekonto", Currency.SEK, 10000));
            accounts.Add(new BankAccount(1002, 3, "Lönekonto", Currency.SEK, 5000));
            accounts.Add(new BankAccount(1003, 2, "Extra", Currency.SEK, 0));


        }

        //here i collect only the user account
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

        // here i search by account number and return its list position 
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

        // Call this after checking that the index is not minus one
        public BankAccount GetAccount(int accountIndex)
        {
            return accounts[accountIndex];
        }

        //here i create a empty account owned by the logged in user
        public bool OpenAccount(User user, string name, Currency currency)
        {
            if (HasText(name) == false)
            {
                return false;
            }
            BankAccount account = new BankAccount(1001 + accounts.count, user.Id, name, currency,0)

                accounts.Add(account);
            return true;

        }

        //here i return the matching users position in the list and minus one means that the login failed.
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

        // Login has 
        public User GetUser(int userIndex)
        {
            return users[userIndex];
        }

        // here i look for at least one character that is not a space
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


        // here i check the role and search the list before creating a user
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

        // The bank's clock moves forward 15 minutes when the admin runs the transfers.
        public int GetBankMinute()
        {
            return bankMinute;
        }

        public int GetBankDay()
        {
            return bankMinute / 1440 + 1;
        }

        // here i validate money using comparison 
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

        // waiting outgoing transfer reserv money that cannot be spent again 
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

        public decimal ConvertMoney(
    decimal amount,
    Currency from,
    Currency to)
        {
            if (from == to)
            {
                return amount;
            }

            return -1;
        }

        // Here I check both accounts before putting a transfer in the queue.
        public bool Transfer(
            User user,
            int fromNumber,
            int toNumber,
            decimal amount)
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

            decimal received = ConvertMoney(
                amount,
                from.Currency,
                to.Currency);

            decimal amountInSek = ConvertMoney(
                amount,
                from.Currency,
                Currency.SEK);

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

        // Admin advances simulated time and processes the requests that are due.
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

                transaction.Status = TransactionStatus.Completed;
                completed++;
            }

            return completed;
        }



    }
}
