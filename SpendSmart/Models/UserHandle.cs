using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using SpendSmart.Models;


public class UserHandle
{
    private readonly UserDbContext _context;

    public UserHandle(UserDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Register(string username, string password, string role = "User")
    {
        if (await _context.Users.AnyAsync(u => u.Username == username))
            return false; // User deja există

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User { Username = username, PasswordHash = hashedPassword, Role = role };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<string> Login(string username, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return null; // Login eșuat

        return user.Role; // Returnează rolul utilizatorului
    }
}
