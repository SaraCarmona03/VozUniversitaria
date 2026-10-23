using VozUniversitaria.Models;

namespace VozUniversitaria.Repositories;

/// <summary>Define la persistencia de los cambios asociados al OTP de un usuario.</summary>
public interface IOtpRepository
{
    /// <summary>Guarda el estado del usuario y permite cancelar la operación.</summary>
    Task SaveAsync(Users user, CancellationToken cancellationToken = default);
}
