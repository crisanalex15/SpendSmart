using Microsoft.AspNetCore.Identity;
using System;
using Microsoft.EntityFrameworkCore;

namespace SpendSmart.Models
{
    public class UserHandle
    {
        private readonly UserDbContext _context1;
        private readonly PasswordHasher<User> _hasher = new();

        //public AuthService(UserDbContext context)
        //{
        //    _context1 = context;
        //}

        public async Task<bool> Register(string username, string password)
        {
            if (_context1.Users.Any(u => u.Username == username))
                return false; // User deja existent

            var user = new User
            {
                Username = username,
                PasswordHash = _hasher.HashPassword(null, password)
            };

            _context1.Users.Add(user);
            await _context1.SaveChangesAsync();
            return true;
        }

        public bool ValidateUser(string username, string password)
        {
            var user = _context1.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return false;

            return _hasher.VerifyHashedPassword(null, user.PasswordHash, password) == PasswordVerificationResult.Success;
        }
    }
}
