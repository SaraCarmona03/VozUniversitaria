using System.Security.Claims;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VozUniversitaria.Models;
using VozUniversitaria.Services;
using VozUniversitaria.ViewModels;

namespace VozUniversitaria.Routers.Api;

[ApiController]
[Route("api")]
[AutoValidateAntiforgeryToken]
public sealed class AuthApiController : ControllerBase
{
    private readonly UserManager<Users> _userManager;
    private readonly SignInManager<Users> _signInManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly OtpService _otp;
    private readonly IAntiforgery _antiforgery;
    private readonly AccountRegistrationService _registration;

    public AuthApiController(
        UserManager<Users> userManager,
        SignInManager<Users> signInManager,
        RoleManager<IdentityRole<int>> roleManager,
        OtpService otp,
        IAntiforgery antiforgery,
        AccountRegistrationService registration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _otp = otp;
        _antiforgery = antiforgery;
        _registration = registration;
    }

    [HttpGet("security/csrf")]
    [AllowAnonymous]
    public IActionResult GetCsrfToken()
    {
        var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
        return Ok(new { requestToken = tokens.RequestToken });
    }

    [HttpGet("session")]
    [AllowAnonymous]
    public IActionResult GetSession()
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return Ok(new { authenticated = false });
        }

        return Ok(new
        {
            authenticated = true,
            userId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            name = User.Identity.Name,
            roles = User.FindAll(ClaimTypes.Role).Select(claim => claim.Value).ToArray()
        });
    }

    [HttpPost("auth/register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var model = new RegisterViewModel
        {
            Role = UserRole.Student,
            Name = request.Name ?? string.Empty,
            LastName = request.LastName ?? string.Empty,
            CaseNumber = request.CaseNumber ?? string.Empty,
            Email = request.Email ?? string.Empty,
            Password = request.Password ?? string.Empty,
            ConfirmPassword = request.ConfirmPassword ?? string.Empty
        };

        if (!TryValidateModel(model))
        {
            return RegistrationValidationError(MapModelErrors());
        }

        var result = await _registration.RegisterAsync(model);
        switch (result.Status)
        {
            case AccountRegistrationStatus.Created:
                return StatusCode(StatusCodes.Status201Created, new
                {
                    requiresEmailVerification = true,
                    email = result.Email,
                    message = result.OtpSendResult == OtpSendResult.Failed
                        ? "La cuenta se creó, pero no se pudo enviar el código. Solicita un reenvío."
                        : "La cuenta se creó. Revisa tu correo para obtener el código de verificación."
                });
            case AccountRegistrationStatus.EmailAlreadyRegistered:
                return RegistrationValidationError(new Dictionary<string, string[]>
                {
                    ["email"] = ["Este correo ya está registrado."]
                });
            case AccountRegistrationStatus.EmailAlreadyRegisteredUnverified:
                return RegistrationValidationError(new Dictionary<string, string[]>
                {
                    ["email"] = ["Este correo ya está registrado y pendiente de verificación. Inicia sesión o solicita otro código."]
                });
            case AccountRegistrationStatus.InvalidEmailDomain:
            case AccountRegistrationStatus.InvalidRole:
            case AccountRegistrationStatus.UserCreationFailed:
            case AccountRegistrationStatus.RoleAssignmentFailed:
                return RegistrationValidationError(ToApiErrors(result.Errors));
            case AccountRegistrationStatus.RoleSetupFailed:
                return RegistrationValidationError(ToApiErrors(result.Errors));
            default:
                throw new ArgumentOutOfRangeException(nameof(result.Status), result.Status,
                    "Unknown account registration result.");
        }
    }

    [HttpPost("auth/login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return Unauthorized(new { error = "Correo o contraseña no válidos." });
        }

        if (!user.IsActive)
        {
            return StatusCode(StatusCodes.Status403Forbidden,
                new { error = "La cuenta está inactiva. Contacta con soporte." });
        }

        if (!user.IsEmailVerified)
        {
            var sendResult = await _otp.SendAsync(user);
            return StatusCode(StatusCodes.Status403Forbidden, new
            {
                error = sendResult switch
                {
                    OtpSendResult.Sent => "Debes verificar tu correo. Enviamos un código.",
                    OtpSendResult.TooSoon => "Debes verificar tu correo. Espera antes de solicitar otro código.",
                    OtpSendResult.Failed => "Debes verificar tu correo, pero no pudimos enviar el código.",
                    _ => throw new ArgumentOutOfRangeException(nameof(sendResult))
                },
                requiresEmailVerification = true,
                email
            });
        }

        if (!user.EmailConfirmed)
        {
            return StatusCode(StatusCodes.Status403Forbidden,
                new { error = "Verifica tu correo antes de iniciar sesión." });
        }

        if (!Enum.IsDefined(user.Role))
        {
            return StatusCode(StatusCodes.Status403Forbidden,
                new { error = "La cuenta tiene un rol no válido." });
        }

        var passwordResult = await _signInManager.CheckPasswordSignInAsync(
            user,
            request.Password,
            lockoutOnFailure: true);

        if (passwordResult.IsLockedOut)
        {
            return StatusCode(423, new { error = "La cuenta está bloqueada temporalmente." });
        }

        if (!passwordResult.Succeeded)
        {
            return Unauthorized(new { error = "Correo o contraseña no válidos." });
        }

        var roleName = user.Role.ToString();
        if (!await EnsureRoleExistsAsync(roleName))
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "No se pudo comprobar el rol de la cuenta.");
        }

        if (!await _userManager.IsInRoleAsync(user, roleName))
        {
            var addRoleResult = await _userManager.AddToRoleAsync(user, roleName);
            if (!addRoleResult.Succeeded)
            {
                return Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "No se pudo asignar el rol de la cuenta.",
                    detail: string.Join("; ", addRoleResult.Errors.Select(error => error.Description)));
            }
        }

        await _signInManager.SignInAsync(user, request.RememberMe);
        return NoContent();
    }

    [HttpPost("auth/logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return NoContent();
    }

    [HttpPost("auth/verification/resend")]
    [AllowAnonymous]
    public async Task<IActionResult> ResendVerification(ResendVerificationRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email.Trim().ToLowerInvariant());
        if (user is not null && !user.IsEmailVerified)
        {
            await _otp.SendAsync(user);
        }

        return Accepted(new
        {
            message = "Si la cuenta requiere verificación, se enviará un código cuando sea posible."
        });
    }

    [HttpPost("auth/verification/verify")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyEmail(VerifyEmailRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email.Trim().ToLowerInvariant());
        if (user is null || user.IsEmailVerified || !await _otp.VerifyAsync(user, request.Code))
        {
            return BadRequest(new { error = "El código no es válido o expiró." });
        }

        return NoContent();
    }

    [HttpGet("admin/access")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public IActionResult AdminAccess() => Ok(new { authorized = true });

    private async Task<bool> EnsureRoleExistsAsync(string roleName)
    {
        if (await _roleManager.RoleExistsAsync(roleName))
        {
            return true;
        }

        var result = await _roleManager.CreateAsync(new IdentityRole<int>(roleName));
        return result.Succeeded || await _roleManager.RoleExistsAsync(roleName);
    }

    private IActionResult RegistrationValidationError(
        IReadOnlyDictionary<string, string[]> errors) =>
        BadRequest(new
        {
            error = "Revisa los datos del formulario.",
            errors
        });

    private Dictionary<string, string[]> MapModelErrors() =>
        ModelState
            .Where(entry => entry.Value?.Errors.Count > 0)
            .ToDictionary(
                entry => ToApiFieldName(entry.Key),
                entry => entry.Value!.Errors
                    .Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage)
                        ? "El valor enviado no es válido."
                        : error.ErrorMessage)
                    .ToArray());

    private static IReadOnlyDictionary<string, string[]> ToApiErrors(
        IReadOnlyDictionary<string, string[]>? errors)
    {
        if (errors is null || errors.Count == 0)
        {
            return new Dictionary<string, string[]>
            {
                [string.Empty] = ["No se pudo crear la cuenta. Inténtalo de nuevo."]
            };
        }

        return errors.ToDictionary(
            entry => ToApiFieldName(entry.Key),
            entry => entry.Value);
    }

    private static string ToApiFieldName(string field) =>
        field.Length == 0 ? field : char.ToLowerInvariant(field[0]) + field[1..];
}
