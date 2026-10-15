using Bank_app;

namespace BankApp
{
    // A savings account inherits the normal account's data and methods.
    public class SavingsAccount : BankAccount
    {
        public decimal InterestPercent { get; set; }

        // Here I send the shared values to the base constructor.
        public SavingsAccount(int number, int ownerId,string name,Currency currency) : base(number, ownerId, name, currency, 0)
        {
            InterestPercent = 2;
        }

        // Here I calculate one year's simple interest.
        public override decimal CalculateYearlyInterest(decimal amount)
        {
            decimal interest = amount * InterestPercent / 100;

            return interest;
        }
    }
}