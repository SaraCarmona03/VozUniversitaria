using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using VozUniversitaria.Models;
using VozUniversitaria.Repositories;

namespace VozUniversitaria.Services;

/// <summary>Resultado de intentar enviar o reenviar un código OTP.</summary>
public enum OtpSendResult
{
    Sent,
    TooSoon,
    Failed
}

/// <summary>Genera, envía y valida códigos de un solo uso para verificar correos.</summary>
public class OtpService
{
    private const int ExpiryMinutes = 10;
    private const int MaxAttempts = 5;
    private const int ResendCooldownSeconds = 60;

    // Las dependencias separan la persistencia del usuario del transporte de correo.
    private readonly IOtpRepository _otpRepository;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<OtpService> _logger;

    /// <summary>Construye el servicio con persistencia, envío de correo y registro de errores.</summary>
    public OtpService(IOtpRepository otpRepository, IEmailSender email, ILogger<OtpService> logger)
    {
        _otpRepository = otpRepository;
        _emailSender = email;
        _logger = logger;
    }

    /// <summary>Genera un código, guarda su hash y solicita el envío por correo.</summary>
    public async Task<OtpSendResult> SendAsync(Users user)
    {
        ArgumentNullException.ThrowIfNull(user);
        var email = user.Email
            ?? throw new InvalidOperationException("Cannot send an OTP to a user without an email address.");

        var now = DateTime.UtcNow;

        // Evita que una cuenta solicite códigos repetidamente.
        if (user.OtpLastSentAt is { } last && (now - last).TotalSeconds < ResendCooldownSeconds)
            return OtpSendResult.TooSoon;

        // Solo se persiste el hash del código; el código sin procesar se usa para el correo.
        var code = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
        user.OtpHash = Hash(user.Id, code);
        user.OtpExpiresAt = now.AddMinutes(ExpiryMinutes);
        user.OtpAttempts = 0;
        user.OtpLastSentAt = now;
        await _otpRepository.SaveAsync(user);

        try
        {
            await _emailSender.SendAsync(
                email,
                "Your VozUniversitaria verification code",
                $"Your verification code is {code}. It expires in {ExpiryMinutes} minutes.\n\n" +
                "If you did not create this account, you can ignore this message.");
            return OtpSendResult.Sent;
        }
        catch (Exception ex) when (ex is SmtpCommandException
            or SmtpProtocolException
            or ServiceNotConnectedException
            or ServiceNotAuthenticatedException
            or SslHandshakeException
            or System.Security.Authentication.AuthenticationException
            or IOException
            or SocketException
            or TimeoutException)
        {
            _logger.LogError(ex, "Could not send an OTP email to user {UserId}.", user.Id);
            return OtpSendResult.Failed;
        }
    }

    /// <summary>Valida el código y confirma el correo cuando coincide y no ha expirado.</summary>
    public async Task<bool> VerifyAsync(Users user, string code)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(code);

        // No se permite verificar con un código inexistente, vencido o agotado.
        if (user.OtpHash is null
            || user.OtpExpiresAt is null
            || user.OtpExpiresAt < DateTime.UtcNow
            || user.OtpAttempts >= MaxAttempts)
            return false;

        user.OtpAttempts++;

        // La comparación en tiempo constante evita filtrar diferencias del hash.
        var ok = CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(Hash(user.Id, code)),
            Encoding.UTF8.GetBytes(user.OtpHash));

        if (ok)
        {
            // Al confirmar, se marca el correo como verificado y se invalida el código.
            user.IsEmailVerified = true;
            user.EmailConfirmed = true;
            user.OtpHash = null;
            user.OtpExpiresAt = null;
            user.OtpAttempts = 0;
        }

        await _otpRepository.SaveAsync(user);
        return ok;
    }

    // Vincular el código al identificador impide reutilizar el hash entre usuarios.
    private static string Hash(int userId, string code) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes($"{userId}:{code}")));
}