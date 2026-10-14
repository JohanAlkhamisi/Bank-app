using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace Bank_app
{
    // here i add the same transaction to the queue and the history 
    internal class Transaction
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


        // here i use minutes to keep track of time in the bank sim
        public Transaction(int id, int userId, int fromAccount,int toAccount,decimal amount, decimal receivedAmount, decimal amountInSek, TransactionType type, int createdMinute, int executeMinute)
        {
            Id = id;
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
