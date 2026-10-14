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

    }
}
