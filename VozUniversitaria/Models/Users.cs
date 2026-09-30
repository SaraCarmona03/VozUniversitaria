namespace VozUniversitaria.Models;

public class Users
{
    public int Id { get; set; } // Unique identifier
    public required string Name { get; set; } // First name
    public required string LastName { get; set; } // Last name
    public required string CaseNumber { get; set; } // Student ID or enrollment number
    public required string Email { get; set; } // User email
    public required string PasswordHash { get; set; } // Encrypted password
    public UserRole Role { get; set; } // User permissions role
    public bool IsActive { get; set; } = true; // Account status (active/suspended)
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Account creation date

    // Email verification (OTP)
    public bool IsEmailVerified { get; set; } // True once the OTP code was confirmed
    public string? OtpHash { get; set; } // Hash of the current code (never the code itself)
    public DateTime? OtpExpiresAt { get; set; } // When the current code expires
    public int OtpAttempts { get; set; } // Wrong attempts for the current code
    public DateTime? OtpLastSentAt { get; set; } // Used for the resend cooldown
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

public class Users : IdentityUser
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string CaseNumber { get; set; } = string.Empty;

    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum UserRole
{
    Student,
    Teacher,
    Admin,
    Staff
}