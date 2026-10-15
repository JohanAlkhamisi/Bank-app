using System;
using System.Collections.Generic;
using System.Text;

namespace Bank_app
{
    // here i keep an account and protect its balance with private field
    public class BankAccount
    {
        public int Number { get; set; }
        public int OwnerId { get; set; }
        public string Name { get; set; }
        public Currency Currency { get; set; }

        private decimal balance;

        // the getter let other classes read the balance
        public decimal Balance
        {
            get 
            {
                return balance;
            }
        }


        // here i give the account it starting values 
        public BankAccount(int number, int ownerId, string name, Currency currency, decimal startingBalance)
        {
            Number = number;
            OwnerId = ownerId;
            Name = name;
            Currency = currency;
            balance = startingBalance;
        }


        // here i check the amount before adding money 
        public bool Deposit(decimal amount)
        {
            if (amount <= 0)
            {
                return false;

            }

            balance = balance + amount;

            return true;
        
        }


        // here i check the amount and balance before taking money out 
        public bool Withdraw(decimal amount)
        {
            if (amount <= 0 || amount > balance)
            {
                return false;

            }

            balance = balance - amount;

            return true;
        }
       
        // A normal account has no savings interest.
        public virtual decimal CalculateYearlyInterest(decimal amount)
        {
            return 0;
        }


    }
}
