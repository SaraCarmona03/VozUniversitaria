using System.ComponentModel.DataAnnotations;

namespace VozUniversitaria.Routers.Api;

public sealed class RegisterRequest
{
    public string? Name { get; init; }
    public string? LastName { get; init; }
    public string? CaseNumber { get; init; }
    public string? Email { get; init; }
    public string? Password { get; init; }
    public string? ConfirmPassword { get; init; }
}

public sealed class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    public string Password { get; init; } = string.Empty;

    public bool RememberMe { get; init; }
}

public sealed class ResendVerificationRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;
}

public sealed class VerifyEmailRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    [RegularExpression(@"^\d{6}$")]
    public string Code { get; init; } = string.Empty;
}
