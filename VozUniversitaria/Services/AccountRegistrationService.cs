using Microsoft.AspNetCore.Identity;
using VozUniversitaria.Models;
using VozUniversitaria.ViewModels;

namespace VozUniversitaria.Services;

public enum AccountRegistrationStatus
{
    Created,
    InvalidEmailDomain,
    InvalidRole,
    EmailAlreadyRegistered,
    EmailAlreadyRegisteredUnverified,
    RoleSetupFailed,
    UserCreationFailed,
    RoleAssignmentFailed
}

public sealed record AccountRegistrationResult(
    AccountRegistrationStatus Status,
    string Email,
    OtpSendResult? OtpSendResult = null,
    IReadOnlyDictionary<string, string[]>? Errors = null);

public sealed class AccountRegistrationService
{
    private readonly UserManager<Users> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly OtpService _otp;
    private readonly string[] _allowedDomains;

    public AccountRegistrationService(
        UserManager<Users> userManager,
        RoleManager<IdentityRole<int>> roleManager,
        OtpService otp,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _otp = otp;
        _allowedDomains = configuration.GetSection("AllowedEmailDomains").Get<string[]>()
            ?? Array.Empty<string>();
    }

    public async Task<AccountRegistrationResult> RegisterAsync(RegisterViewModel model)
    {
        var email = model.Email.Trim().ToLowerInvariant();
        if (!IsAllowedDomain(email))
        {
            var domains = string.Join(", ", _allowedDomains.Select(domain => "@" + domain.TrimStart('@')));
            return Failure(AccountRegistrationStatus.InvalidEmailDomain, email,
                "Email", $"Use your institutional email ({domains}).");
        }

        if (!IsPublicRegistrationRole(model.Role))
        {
            return Failure(AccountRegistrationStatus.InvalidRole, email,
                nameof(model.Role), "Select a role available for public registration.");
        }

        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser is not null)
        {
            return new AccountRegistrationResult(
                existingUser.IsEmailVerified
                    ? AccountRegistrationStatus.EmailAlreadyRegistered
                    : AccountRegistrationStatus.EmailAlreadyRegisteredUnverified,
                email);
        }

        var roleName = model.Role.ToString();
        if (!await EnsureRoleExistsAsync(roleName))
        {
            return Failure(AccountRegistrationStatus.RoleSetupFailed, email,
                string.Empty, "The selected role could not be configured. Please try again.");
        }

        var newUser = new Users
        {
            Name = model.Name,
            LastName = model.LastName,
            CaseNumber = model.CaseNumber,
            UserName = email,
            Email = email,
            LockoutEnabled = true,
            Role = model.Role
        };

        var createResult = await _userManager.CreateAsync(newUser, model.Password);
        if (!createResult.Succeeded)
        {
            return new AccountRegistrationResult(
                AccountRegistrationStatus.UserCreationFailed,
                email,
                Errors: MapIdentityErrors(createResult));
        }

        var roleResult = await _userManager.AddToRoleAsync(newUser, roleName);
        if (!roleResult.Succeeded)
        {
            return new AccountRegistrationResult(
                AccountRegistrationStatus.RoleAssignmentFailed,
                email,
                Errors: MapIdentityErrors(roleResult));
        }

        var otpResult = await _otp.SendAsync(newUser);
        return new AccountRegistrationResult(AccountRegistrationStatus.Created, email, otpResult);
    }

    private async Task<bool> EnsureRoleExistsAsync(string roleName)
    {
        if (await _roleManager.RoleExistsAsync(roleName))
        {
            return true;
        }

        var result = await _roleManager.CreateAsync(
            new IdentityRole<int>(roleName));
        return result.Succeeded || await _roleManager.RoleExistsAsync(roleName);
    }

    private bool IsAllowedDomain(string email)
    {
        var at = email.LastIndexOf('@');
        if (at < 0)
        {
            return false;
        }

        var domain = email[(at + 1)..];
        return _allowedDomains.Any(configuredDomain =>
        {
            var allowedDomain = configuredDomain.TrimStart('@');
            return domain.Equals(allowedDomain, StringComparison.OrdinalIgnoreCase)
                || domain.EndsWith("." + allowedDomain, StringComparison.OrdinalIgnoreCase);
        });
    }

    private static bool IsPublicRegistrationRole(UserRole role) =>
        role is UserRole.Student or UserRole.Teacher or UserRole.Staff;

    private static AccountRegistrationResult Failure(
        AccountRegistrationStatus status,
        string email,
        string field,
        string message) =>
        new(status, email, Errors: new Dictionary<string, string[]>
        {
            [field] = [message]
        });

    private static IReadOnlyDictionary<string, string[]> MapIdentityErrors(IdentityResult result)
    {
        var errors = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        foreach (var error in result.Errors)
        {
            var field = error.Code switch
            {
                "DuplicateEmail" or "DuplicateUserName" => "Email",
                "PasswordTooShort" or "PasswordRequiresDigit" or "PasswordRequiresLower"
                    or "PasswordRequiresUpper" or "PasswordRequiresNonAlphanumeric"
                    or "PasswordRequiresUniqueChars" => "Password",
                _ => string.Empty
            };

            if (!errors.TryGetValue(field, out var messages))
            {
                messages = [];
                errors[field] = messages;
            }

            messages.Add(error.Description);
        }

        return errors.ToDictionary(pair => pair.Key, pair => pair.Value.ToArray());
    }
}
