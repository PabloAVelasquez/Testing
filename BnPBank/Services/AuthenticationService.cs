using BnPBank.Data;
using BnPBank.Models;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace BnPBank.Services
{
    public class AuthenticationService
    {
        private readonly SessionTokenManager _sessionTokenManager;

        public AuthenticationService(SessionTokenManager sessionTokenManager)
        {
            _sessionTokenManager = sessionTokenManager;
        }

        public async Task<User> AuthenticateUserAsync(string username, string password)
        {
            using (var dbContext = new BankingDbContext())
            {
                // Get the user with the given username
                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);

                // If no such user exists, return null
                if (user == null)
                {
                    return null;
                }

                // Check if the entered password matches the stored hashed password
                if (!BCrypt.Net.BCrypt.Verify(password, user.HashedPassword))
                {
                    return null;
                }

                // Generate a session token and store it
                var sessionToken = Guid.NewGuid().ToString();
                _sessionTokenManager.StoreToken(sessionToken, user.Username);

                // Return the authenticated user
                return user;
            }
        }

        public void LogoutUser()
        {
            // Clear the session token
            _sessionTokenManager.ClearToken();
        }

        public async Task<bool> SetUserPassword(string username, string newPassword)
        {
            using (var dbContext = new BankingDbContext())
            {
                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
                if (user != null)
                {
                    user.HashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
                    dbContext.Users.Update(user);
                    await dbContext.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}