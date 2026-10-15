using System;
using System.Collections.Generic;
using System.Text;

namespace Bank_app
{
    public class User
    {
        // here i keep the details used to identify a user
        public int Id { get; set; }
        public string Username { get; set; }
        public bool IsAdmin { get; set; }
        public int FailedAttempts { get; set; }
        public bool IsLocked { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public CreditRating CreditRating { get; set; }
        private string password;

        // here i give the new user its starting values 
        public User(int id, string username, string password, bool isAdmin)
        {
            Id = id;
            Username = username;
            this.password = password;
            IsAdmin = isAdmin;
            FailedAttempts = 0;
            IsLocked = false;

            Email = "";

            Phone = "";

            CreditRating = CreditRating.Medium;

        }

        //here i compare the password
        public bool CheckPassword(string enteredPassword)
        {
            if (password == enteredPassword)
            {
                return true;
            }

            return false;
                


        }
    }
}
