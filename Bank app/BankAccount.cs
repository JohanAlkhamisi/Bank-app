using System;
using System.Collections.Generic;
using System.Text;

namespace Bank_app
{
    // Here I keep the account details and its balance.
    public class BankAccount
    {
        public int Number { get; set; }
        public int OwnerId { get; set; }
        public string Name { get; set; }
        public Currency Currency { get; set; }

        private decimal balance;

        // Here I let other classes read the balance.
        public decimal Balance
        {
            get 
            {
                return balance;
            }
        }


        // Here I save the account number, owner, name, currency and starting balance. 
        public BankAccount(int number, int ownerId, string name, Currency currency, decimal startingBalance)
        {
            Number = number;
            OwnerId = ownerId;
            Name = name;
            Currency = currency;
            balance = startingBalance;
        }


        // Here I check that the amount is above 0 before adding it to the balance. 
        public bool Deposit(decimal amount)
        {
            if (amount <= 0)
            {
                return false;

            }

            balance = balance + amount;

            return true;
        
        }


        // Here I check the amount and that there is enough money before taking it out. 
        public bool Withdraw(decimal amount)
        {
            if (amount <= 0 || amount > balance)
            {
                return false;

            }

            balance = balance - amount;

            return true;
        }

        // Here I return 0 because a normal account does not earn interest.
        public virtual decimal CalculateYearlyInterest(decimal amount)
        {
            return 0;
        }


    }
}
