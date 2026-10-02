using Microsoft.AspNetCore.Identity;
using VozUniversitaria.Models;

namespace VozUniversitaria.Services;

public sealed class LegacyBcryptPasswordHasher : IPasswordHasher<Users>
{
    private readonly PasswordHasher<Users> _identityHasher = new();

    public string HashPassword(Users user, string password)
    {
        return _identityHasher.HashPassword(user, password);
    }

    public PasswordVerificationResult VerifyHashedPassword(
        Users user,
        string hashedPassword,
        string providedPassword)
    {
        if (hashedPassword.StartsWith("$2", StringComparison.Ordinal)
            && BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword))
        {
            return PasswordVerificationResult.SuccessRehashNeeded;
        }

        return _identityHasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
    }
}
