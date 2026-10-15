using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace Bank_app
{
    // Here I keep the details of a transfer, deposit or loan payment.
    public class Transaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int FromAccount { get; set; }
        public int ToAccount { get; set; }
        public decimal Amount { get; set; }
        public decimal ReceivedAmount { get; set; }
        public decimal AmountInSek { get; set; }
        public int CreatedMinute { get; set; }
        public int ExecuteMinute { get; set; }
        public TransactionType Type { get; set; }
        public TransactionStatus Status { get; set; }

        // Here I save the loan's number so I can find it when the money is paid out.
        public int LoanId { get; set; }

        // Here I save the payment details and its time in the bank's simulated minutes.
        public Transaction(int id, int userId, int fromAccount,int toAccount,decimal amount, decimal receivedAmount, decimal amountInSek, TransactionType type, int createdMinute, int executeMinute)
        {
            Id = id;
            LoanId = 0;
            UserId = userId;
            FromAccount = fromAccount;
            ToAccount = toAccount;
            Amount = amount;
            ReceivedAmount = receivedAmount;
            AmountInSek = amountInSek;
            Type = type;
            CreatedMinute = createdMinute;
            ExecuteMinute = executeMinute;
            Status = TransactionStatus.Pending;
        }
    
    }
}
