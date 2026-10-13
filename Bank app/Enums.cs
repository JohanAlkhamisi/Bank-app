namespace BankApp
{
    // Här samlar jag de val som ska användas i banken.
    public enum Currency
    {
        SEK,
        EUR,
        USD
    }

    public enum CreditRating
    {
        Low = 1,
        Medium,
        High
    }

    public enum TransactionType
    {
        Transfer,
        Deposit,
        Withdrawal,
        LoanPayout,
        LoanPayment
    }

    public enum TransactionStatus
    {
        Pending,
        Completed,
        Failed
    }

    public enum LoanStatus
    {
        Pending,
        Active,
        Repaid,
        Cancelled
    }

    public enum NotificationChannel
    {
        None,
        Email,
        Sms,
        Both
    }
}