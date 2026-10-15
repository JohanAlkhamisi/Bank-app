using System;

namespace Bank_app
{
    // Here I keep a pretend email or SMS that the user can read in the app.
    public class Notification
    {
        public int UserId { get; set; }
        public int AccountNumber { get; set; }
        public int TransactionId { get; set; }
        public string Channel { get; set; }
        public string Address { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }

        // Here I save who the message is for, what happened and when it was created.
        public Notification(int userId, int accountNumber, int transactionId, string channel, string address, string message)
        {
            UserId = userId;
            AccountNumber = accountNumber;
            TransactionId = transactionId;
            Channel = channel;
            Address = address;
            Message = message;
            CreatedAt = DateTime.Now;
        }
    }
}