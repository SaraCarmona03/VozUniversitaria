using System.ComponentModel.DataAnnotations;

namespace VozUniversitaria.ViewModels;

public class VerifyEmailViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "Enter the 6-digit code.")]
    [Display(Name = "Verification code")]
    public string Code { get; set; } = string.Empty;
}