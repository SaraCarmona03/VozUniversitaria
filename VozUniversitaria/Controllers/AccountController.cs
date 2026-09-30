using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VozUniversitaria.Data;
using VozUniversitaria.Models;
using VozUniversitaria.Services;
using VozUniversitaria.ViewModels;

namespace VozUniversitaria.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly OtpService _otp;
        private readonly string[] _allowedDomains;

        public AccountController(AppDbContext context, OtpService otp, IConfiguration config)
        {
            _context = context;
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
            ViewData["Role"] = role;
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // Keep the selected role available when the form is shown again
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

            // Check if the email is already registered
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
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

            var newUser = new Users
            {
                Name = model.Name,
                LastName = model.LastName,
                CaseNumber = model.CaseNumber,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Role = model.Role
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

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

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user != null && user.IsEmailVerified)
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
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

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
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View(model);
            }

            // Credentials are correct, but the email still has to be verified
            if (!user.IsEmailVerified)
            {
                await _otp.SendAsync(user); // does nothing if a code was sent less than a minute ago
                TempData["Info"] = "Please verify your email first. Check your inbox for the code, or request a new one.";
                return RedirectToAction("VerifyEmail", new { email = user.Email });
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Home");
        }

        // POST: /Account/Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

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