using System.Security.Cryptography;
using System.Text;
using VozUniversitaria.Data;
using VozUniversitaria.Models;

namespace VozUniversitaria.Services;

public enum OtpSendResult { Sent, TooSoon, Failed }

public class OtpService
{
    private const int ExpiryMinutes = 10;
    private const int MaxAttempts = 5;
    private const int ResendCooldownSeconds = 60;

    private readonly AppDbContext _db;
    private readonly IEmailSender _email;
    private readonly ILogger<OtpService> _logger;

    public OtpService(AppDbContext db, IEmailSender email, ILogger<OtpService> logger)
    {
        _db = db;
        _email = email;
        _logger = logger;
    }

    /// <summary>Generates a new 6-digit code, stores its hash and emails it.</summary>
    public async Task<OtpSendResult> SendAsync(Users user)
    {
        var now = DateTime.UtcNow;

        if (user.OtpLastSentAt is { } last && (now - last).TotalSeconds < ResendCooldownSeconds)
            return OtpSendResult.TooSoon;

        var code = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
        user.OtpHash = Hash(user.Id, code);
        user.OtpExpiresAt = now.AddMinutes(ExpiryMinutes);
        user.OtpAttempts = 0;
        user.OtpLastSentAt = now;
        await _db.SaveChangesAsync();

        try
        {
            await _email.SendAsync(
                user.Email,
                "Your VozUniversitaria verification code",
                $"Your verification code is {code}. It expires in {ExpiryMinutes} minutes.\n\n" +
                "If you did not create this account, you can ignore this message.");
            return OtpSendResult.Sent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not send the OTP email to user {UserId}", user.Id);
            user.OtpLastSentAt = null; // allow an immediate retry
            await _db.SaveChangesAsync();
            return OtpSendResult.Failed;
        }
    }

    /// <summary>Checks the code. Marks the email as verified when it is correct.</summary>
    public async Task<bool> VerifyAsync(Users user, string code)
    {
        if (user.OtpHash is null
            || user.OtpExpiresAt is null
            || user.OtpExpiresAt < DateTime.UtcNow
            || user.OtpAttempts >= MaxAttempts)
            return false;

        user.OtpAttempts++;

        var ok = CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(Hash(user.Id, code)),
            Encoding.UTF8.GetBytes(user.OtpHash));

        if (ok)
        {
            user.IsEmailVerified = true;
            user.OtpHash = null;
            user.OtpExpiresAt = null;
            user.OtpAttempts = 0;
        }

        await _db.SaveChangesAsync();
        return ok;
    }

    private static string Hash(int userId, string code) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes($"{userId}:{code}")));
}