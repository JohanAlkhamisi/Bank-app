using System;
using System.Collections.Generic;
using System.Text;

namespace Bank_app
{
    public class User
    {
        // Here I keep the user's number, username and information about login attempts.
        public int Id { get; set; }
        public string Username { get; set; }
        public bool IsAdmin { get; set; }
        public int FailedAttempts { get; set; }
        public bool IsLocked { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public CreditRating CreditRating { get; set; }
        private string password;

        // Here I save the new user's details and start with 0 wrong login attempts.
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

        // Here I check if the entered password matches the saved password.
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
