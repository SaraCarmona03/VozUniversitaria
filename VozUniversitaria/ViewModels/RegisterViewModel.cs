using System.ComponentModel.DataAnnotations;

namespace VozUniversitaria.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public required string Name { get; set; }

        [Required]
        public required string LastName { get; set; }

        [Required]
        public required string CaseNumber { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public required string ConfirmPassword { get; set; }
    }
}