using System;
using System.Collections.Generic;

namespace Bank_app
{
    // Here I keep a loan, its owner and the interest rate agreed for it.
    public class Loan
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public decimal InterestPercent { get; set; }
        public bool IsPaidOut { get; set; }

        // Here I save the new loan and mark that its money has not been paid out yet.
        public Loan(int id, int userId, decimal amount, decimal interestPercent)
        {
            Id = id;
            UserId = userId;
            Amount = amount;
            InterestPercent = interestPercent;
            IsPaidOut = false;
        }

        // Here I work out the interest on the loan for one year.
        public decimal CalculateYearlyInterest()
        {
            decimal interest = Amount * InterestPercent / 100;

            return interest;
        }
    }
}