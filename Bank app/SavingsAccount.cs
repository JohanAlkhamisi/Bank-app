namespace Bank_app
{
    // Here I reuse BankAccount for a savings account that earns interest.
    public class SavingsAccount : BankAccount
    {
        public decimal InterestPercent { get; set; }

        // Here I send the shared details to BankAccount and set the interest rate to 2 percent.
        public SavingsAccount(int number, int ownerId, string name, Currency currency) : base(number, ownerId, name, currency, 0)
        {
            InterestPercent = 2;
        }

        // Here I work out one year's interest on the amount.
        public override decimal CalculateYearlyInterest(decimal amount)
        {
            decimal interest = amount * InterestPercent / 100;

            return interest;
        }
    }
}