using Microsoft.AspNetCore.Identity;
using VozUniversitaria.Models;
using VozUniversitaria.Services;
using Xunit;

public class UserIdentityTests
{
    [Fact]
    public void Users_should_inherit_from_IdentityUser()
    {
        Assert.True(typeof(IdentityUser<int>).IsAssignableFrom(typeof(Users)));
    }

    [Fact]
    public void Users_should_keep_email_verification_and_application_role_data()
    {
        Assert.NotNull(typeof(Users).GetProperty(nameof(Users.IsEmailVerified)));
        Assert.NotNull(typeof(Users).GetProperty(nameof(Users.OtpHash)));
        Assert.NotNull(typeof(Users).GetProperty(nameof(Users.OtpExpiresAt)));
        Assert.NotNull(typeof(Users).GetProperty(nameof(Users.OtpAttempts)));
        Assert.NotNull(typeof(Users).GetProperty(nameof(Users.OtpLastSentAt)));
        Assert.NotNull(typeof(Users).GetProperty(nameof(Users.Role)));
    }

    [Fact]
    public void Legacy_bcrypt_passwords_should_be_accepted_for_identity_rehashing()
    {
        var hasher = new LegacyBcryptPasswordHasher();
        var user = new Users
        {
            Name = "Test",
            LastName = "User",
            CaseNumber = "123",
            Email = "test@ues.mx"
        };
        var legacyHash = BCrypt.Net.BCrypt.HashPassword("Password123");

        var result = hasher.VerifyHashedPassword(user, legacyHash, "Password123");

        Assert.Equal(PasswordVerificationResult.SuccessRehashNeeded, result);
    }

    [Fact]
    public void Identity_password_hashes_should_continue_to_verify()
    {
        var hasher = new LegacyBcryptPasswordHasher();
        var user = new Users
        {
            Name = "Test",
            LastName = "User",
            CaseNumber = "123",
            Email = "test@ues.mx"
        };
        var hash = hasher.HashPassword(user, "Password123");

        var result = hasher.VerifyHashedPassword(user, hash, "Password123");

        Assert.Equal(PasswordVerificationResult.Success, result);
    }
}
