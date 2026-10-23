using Microsoft.EntityFrameworkCore;
using VozUniversitaria.Data;
using VozUniversitaria.Models;

namespace VozUniversitaria.Repositories;

/// <summary>Persiste los cambios de verificación y OTP usando Entity Framework Core.</summary>
public class OtpRepository : IOtpRepository
{
    // Contexto de EF Core compartido con el resto de operaciones del usuario en el request.
    private readonly AppDbContext _db;

    /// <summary>Recibe el contexto de base de datos asociado al request actual.</summary>
    public OtpRepository(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Persiste únicamente las propiedades de verificación y OTP cuando el usuario
    /// ya existe, evitando sobrescribir otros datos del perfil.
    /// </summary>
    public async Task SaveAsync(Users user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);

        var entry = _db.Entry(user);
        if (entry.State == EntityState.Detached)
        {
            _db.Users.Attach(user);
        }

        // Para entidades nuevas, EF Core ya conoce todas las propiedades que debe insertar.
        if (entry.State != EntityState.Added)
        {
            entry.Property(current => current.IsEmailVerified).IsModified = true;
            entry.Property(current => current.EmailConfirmed).IsModified = true;
            entry.Property(current => current.OtpHash).IsModified = true;
            entry.Property(current => current.OtpExpiresAt).IsModified = true;
            entry.Property(current => current.OtpAttempts).IsModified = true;
            entry.Property(current => current.OtpLastSentAt).IsModified = true;
        }

        await _db.SaveChangesAsync(cancellationToken);
    }
}
