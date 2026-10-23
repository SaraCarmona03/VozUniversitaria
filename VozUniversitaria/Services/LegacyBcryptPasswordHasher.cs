using Microsoft.AspNetCore.Identity;
using VozUniversitaria.Models;

namespace VozUniversitaria.Services;

/// <summary>
/// Mantiene la compatibilidad con contraseñas BCrypt antiguas y usa Identity
/// para las contraseñas nuevas.
/// </summary>
public sealed class LegacyBcryptPasswordHasher : IPasswordHasher<Users>
{
    private readonly PasswordHasher<Users> _identityHasher = new();

    /// <summary>Genera un hash nuevo con el algoritmo de ASP.NET Core Identity.</summary>
    public string HashPassword(Users user, string password)
    {
        return _identityHasher.HashPassword(user, password);
    }

    /// <summary>
    /// Verifica hashes BCrypt heredados o hashes Identity y solicita migrar
    /// los BCrypt válidos después del inicio de sesión.
    /// </summary>
    public PasswordVerificationResult VerifyHashedPassword(
        Users user,
        string hashedPassword,
        string providedPassword)
    {
        // Un BCrypt válido se acepta una vez y se vuelve a guardar con el hasher actual.
        if (hashedPassword.StartsWith("$2", StringComparison.Ordinal)
            && BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword))
        {
            return PasswordVerificationResult.SuccessRehashNeeded;
        }

        return _identityHasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
    }
}
