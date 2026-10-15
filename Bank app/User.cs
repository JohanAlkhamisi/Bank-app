using System;
using System.Collections.Generic;

namespace Bank_app
{
    // Here I keep a user and the details needed to log in.
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public bool IsAdmin { get; set; }

        // Three wrong passwords lock the user until the admin unlocks it.
        public int FailedAttempts { get; set; }

        public bool IsLocked { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public CreditRating CreditRating { get; set; }

        // The password is checked through CheckPassword.
        private string password;

        // Here I save the new user and start with 0 wrong login attempts.
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