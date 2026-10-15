using System;

namespace Bank_app
{
    // Here I keep a transfer, deposit or loan payment and when it should run.
    public class Transaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        // A source account of 0 means money comes from a deposit or a loan.
        public int FromAccount { get; set; }

        public int ToAccount { get; set; }
        public decimal Amount { get; set; }
        public decimal ReceivedAmount { get; set; }

        // Save this SEK value now so later rate changes do not change the transfer statistics.
        public decimal AmountInSek { get; set; }

        // Save when the payment was entered and the real time when it should run.
        public DateTime CreatedAt { get; set; }

        public DateTime ExecuteAt { get; set; }
        public TransactionType Type { get; set; }
        public TransactionStatus Status { get; set; }

        // The loan number connects a loan payment to the right loan.
        public int LoanId { get; set; }

        // Here I save the payment details and mark the payment as waiting.
        public Transaction(int id, int userId, int fromAccount, int toAccount, decimal amount, decimal receivedAmount, decimal amountInSek, TransactionType type, DateTime createdAt, DateTime executeAt)
        {
            Id = id;
            UserId = userId;
            FromAccount = fromAccount;
            ToAccount = toAccount;
            Amount = amount;
            ReceivedAmount = receivedAmount;
            AmountInSek = amountInSek;
            Type = type;
            CreatedAt = createdAt;
            ExecuteAt = executeAt;
            Status = TransactionStatus.Pending;
            LoanId = 0;
        }
    }
}