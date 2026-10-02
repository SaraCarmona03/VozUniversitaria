using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VozUniversitaria.Models;
using VozUniversitaria.Services;
using VozUniversitaria.ViewModels;

namespace VozUniversitaria.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<Users> _userManager;
        private readonly SignInManager<Users> _signInManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly OtpService _otp;
        private readonly string[] _allowedDomains;

        public AccountController(
            UserManager<Users> userManager,
            SignInManager<Users> signInManager,
            RoleManager<IdentityRole<int>> roleManager,
            OtpService otp,
            IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _otp = otp;
            _allowedDomains = config.GetSection("AllowedEmailDomains").Get<string[]>()
                              ?? Array.Empty<string>();
        }

        // GET: /Account/SelectRole
        [HttpGet]
        public IActionResult SelectRole()
        {
            return View();
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register(UserRole role)
        {
            if (!IsPublicRegistrationRole(role))
            {
                TempData["Error"] = "That role cannot be selected during public registration.";
                return RedirectToAction(nameof(SelectRole));
            }

            ViewData["Role"] = role;
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            ViewData["Role"] = model.Role;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var email = model.Email.Trim().ToLowerInvariant();

            // Only institutional emails are accepted
            if (!IsAllowedDomain(email))
            {
                var domains = string.Join(", ", _allowedDomains.Select(d => "@" + d.TrimStart('@')));
                ModelState.AddModelError("Email", $"Use your institutional email ({domains}).");
                return View(model);
            }

            if (!IsPublicRegistrationRole(model.Role))
            {
                ModelState.AddModelError(nameof(model.Role), "Select a role available for public registration.");
                return View(model);
            }

            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                if (!existingUser.IsEmailVerified)
                {
                    TempData["Info"] = "This email is registered but not verified yet. Enter the code we sent you, or request a new one.";
                    return RedirectToAction("VerifyEmail", new { email });
                }

                ModelState.AddModelError("Email", "This email is already registered.");
                return View(model);
            }

            if (!await EnsureRoleExistsAsync(model.Role.ToString()))
            {
                ModelState.AddModelError(string.Empty, "The selected role could not be configured. Please try again.");
                return View(model);
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
                AddIdentityErrors(createResult);
                return View(model);
            }

            var roleResult = await _userManager.AddToRoleAsync(newUser, model.Role.ToString());
            if (!roleResult.Succeeded)
            {
                AddIdentityErrors(roleResult);
                return View(model);
            }

            var result = await _otp.SendAsync(newUser);
            if (result == OtpSendResult.Sent)
                TempData["Info"] = $"We sent a 6-digit code to {email}.";
            else
                TempData["Error"] = "We could not send the code. Use \"Resend code\" to try again.";

            return RedirectToAction("VerifyEmail", new { email });
        }

        // GET: /Account/VerifyEmail
        [HttpGet]
        public IActionResult VerifyEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return RedirectToAction("Login");
            }

            return View(new VerifyEmailViewModel { Email = email });
        }

        // POST: /Account/VerifyEmail
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

        // POST: /Account/ResendCode
        [HttpPost]
        public async Task<IActionResult> ResendCode(string email)
        {
            var user = await _userManager.FindByEmailAsync(email.Trim().ToLowerInvariant());

            if (user != null && !user.IsEmailVerified)
            {
                var result = await _otp.SendAsync(user);
                switch (result)
                {
                    case OtpSendResult.Sent:
                        TempData["Info"] = "We sent you a new code.";
                        break;
                    case OtpSendResult.TooSoon:
                        TempData["Error"] = "Please wait a minute before requesting another code.";
                        break;
                    default:
                        TempData["Error"] = "We could not send the code. Try again in a moment.";
                        break;
                }
            }

            return RedirectToAction("VerifyEmail", new { email });
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
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
                await _otp.SendAsync(user);
                TempData["Info"] = "Please verify your email first. Check your inbox for the code, or request a new one.";
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

        // POST: /Account/Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        private async Task<bool> EnsureRoleExistsAsync(string roleName)
        {
            if (await _roleManager.RoleExistsAsync(roleName))
            {
                return true;
            }

            var result = await _roleManager.CreateAsync(new IdentityRole<int>(roleName));
            return result.Succeeded || await _roleManager.RoleExistsAsync(roleName);
        }

        private void AddIdentityErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private static bool IsPublicRegistrationRole(UserRole role) =>
            role is UserRole.Student or UserRole.Teacher or UserRole.Staff;

        private bool IsAllowedDomain(string email)
        {
            var at = email.LastIndexOf('@');
            if (at < 0) return false;

            var domain = email[(at + 1)..];
            return _allowedDomains.Any(d =>
            {
                var allowed = d.TrimStart('@');
                return domain.Equals(allowed, StringComparison.OrdinalIgnoreCase)
                    || domain.EndsWith("." + allowed, StringComparison.OrdinalIgnoreCase);
            });
        }
    }
}