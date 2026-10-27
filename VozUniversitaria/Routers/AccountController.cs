using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VozUniversitaria.Models;
using VozUniversitaria.Services;
using VozUniversitaria.ViewModels;

namespace VozUniversitaria.Routers
{
    /// <summary>Controla el registro, la verificación de correo
    /// y el acceso de usuarios.</summary>
    public class AccountController : Controller
    {
        private readonly UserManager<Users> _userManager;
        private readonly SignInManager<Users> _signInManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly OtpService _otp;
        private readonly AccountRegistrationService _registration;

        /// <summary>Recibe los servicios de Identity, OTP y los dominios
        /// de correo permitidos.</summary>
        public AccountController(
            UserManager<Users> userManager,
            SignInManager<Users> signInManager,
            RoleManager<IdentityRole<int>> roleManager,
            OtpService otp,
            AccountRegistrationService registration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _otp = otp;
            _registration = registration;
        }

        /// <summary>Muestra los roles que pueden elegir los usuarios
        ///  al registrarse.</summary>
        [HttpGet]
        public IActionResult SelectRole()
        {
            return View();
        }

        /// <summary>Muestra el formulario de registro para un rol público válido.</summary>
        [HttpGet]
        public IActionResult Register(UserRole role)
        {
            return Redirect("/app/index.html#registro");
        }

        /// <summary>Valida los datos, crea la cuenta y solicita el envío
        /// del código OTP.</summary>
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            ViewData["Role"] = model.Role;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _registration.RegisterAsync(model);
            if (result.Status == AccountRegistrationStatus.EmailAlreadyRegisteredUnverified)
            {
                TempData["Info"] = "This email is registered but not verified yet. Enter the code we sent you, or request a new one.";
                return RedirectToAction("VerifyEmail", new { email = result.Email });
            }

            if (result.Status == AccountRegistrationStatus.Created)
            {
                SetOtpStatusMessage(result.OtpSendResult
                    ?? throw new InvalidOperationException("The registration result did not include the OTP status."),
                    result.Email);
                return RedirectToAction("VerifyEmail", new { email = result.Email });
            }

            if (result.Status == AccountRegistrationStatus.EmailAlreadyRegistered)
            {
                ModelState.AddModelError("Email", "This email is already registered.");
            }
            else if (result.Errors is not null)
            {
                foreach (var (field, errors) in result.Errors)
                {
                    foreach (var error in errors)
                    {
                        ModelState.AddModelError(field, error);
                    }
                }
            }

            return View(model);
        }

        /// <summary>Muestra el formulario de verificación para el correo
        /// indicado.</summary>
        [HttpGet]
        public IActionResult VerifyEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return RedirectToAction("Login");
            }

            return View(new VerifyEmailViewModel { Email = email });
        }

        /// <summary>Valida el código y confirma el correo cuando es correcto.</summary>
        [HttpPost]
        public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var email = model.Email.Trim().ToLowerInvariant();
            var user = await _userManager.FindByEmailAsync(email);

            if (user is not null && user.IsEmailVerified)
            {
                return RedirectToAction("Login");
            }

            if (user == null || !await _otp.VerifyAsync(user, model.Code))
            {
                ModelState.AddModelError(nameof(model.Code), "Invalid or expired code. You can request a new one.");
                return View(model);
            }

            TempData["Success"] = "Email verified. You can log in now.";
            return RedirectToAction("Login");
        }

        /// <summary>Solicita otro código respetando el límite de
        /// reenvío definido por el servicio.</summary>
        [HttpPost]
        public async Task<IActionResult> ResendCode(string email)
        {
            var user = await _userManager.FindByEmailAsync(email.Trim().ToLowerInvariant());

            if (user != null && !user.IsEmailVerified)
            {
                SetOtpStatusMessage(await _otp.SendAsync(user));
            }

            return RedirectToAction("VerifyEmail", new { email });
        }

        /// <summary>Muestra el formulario de inicio de sesión.</summary>
        [HttpGet]
        public IActionResult Login()
        {
            return Redirect("/app/index.html");
        }

        /// <summary>Valida las credenciales y crea la sesión si la cuenta
        /// cumple los requisitos.</summary>
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var email = model.Email.Trim().ToLowerInvariant();
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View(model);
            }

            if (!user.IsActive)
            {
                ModelState.AddModelError(string.Empty, "This account is inactive. Please contact support.");
                return View(model);
            }

            if (!user.IsEmailVerified)
            {
                SetOtpStatusMessage(await _otp.SendAsync(user));
                return RedirectToAction("VerifyEmail", new { email = user.Email });
            }

            if (!user.EmailConfirmed)
            {
                ModelState.AddModelError(string.Empty, "Please verify your email before signing in.");
                return View(model);
            }

            if (!Enum.IsDefined(user.Role))
            {
                ModelState.AddModelError(string.Empty, "The account has an invalid role. Please contact support.");
                return View(model);
            }

            var passwordResult = await _signInManager.CheckPasswordSignInAsync(
                user,
                model.Password,
                lockoutOnFailure: true);

            if (passwordResult.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "This account is temporarily locked. Please try again later.");
                return View(model);
            }

            if (!passwordResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View(model);
            }

            if (!await EnsureRoleExistsAsync(user.Role.ToString()))
            {
                ModelState.AddModelError(string.Empty, "The account role could not be verified. Please contact support.");
                return View(model);
            }

            if (!await _userManager.IsInRoleAsync(user, user.Role.ToString()))
            {
                var roleResult = await _userManager.AddToRoleAsync(user, user.Role.ToString());
                if (!roleResult.Succeeded)
                {
                    AddIdentityErrors(roleResult);
                    return View(model);
                }
            }

            await _signInManager.SignInAsync(user, model.RememberMe);

            return RedirectToAction("Index", "Home");
        }

        /// <summary>Cierra la sesión actual y vuelve a la página de inicio.</summary>
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // Crea roles faltantes para conservar la compatibilidad con cuentas existentes.
        private async Task<bool> EnsureRoleExistsAsync(string roleName)
        {
            if (await _roleManager.RoleExistsAsync(roleName))
            {
                return true;
            }

            var result = await _roleManager.CreateAsync(new IdentityRole<int>(roleName));
            return result.Succeeded || await _roleManager.RoleExistsAsync(roleName);
        }

        // Muestra los errores devueltos por Identity en el formulario actual.
        private void AddIdentityErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        // Mantiene consistentes los mensajes de OTP en registro,
        // inicio de sesión y reenvío.
        private void SetOtpStatusMessage(OtpSendResult result, string? email = null)
        {
            var (key, message) = result switch
            {
                OtpSendResult.Sent => ("Info", email is null
                    ? "We sent you a new code."
                    : $"We sent a 6-digit code to {email}."),
                OtpSendResult.TooSoon => ("Error", "Please wait a minute before requesting another code."),
                OtpSendResult.Failed => ("Error", "We could not send the code. Please wait a minute and try again."),
                _ => throw new ArgumentOutOfRangeException(nameof(result), result, "Unknown OTP send result.")
            };

            TempData[key] = message;
        }

    }
}