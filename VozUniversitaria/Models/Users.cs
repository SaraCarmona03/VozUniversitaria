using Microsoft.AspNetCore.Identity;

namespace VozUniversitaria.Models;

public class Users : IdentityUser<int>
{
    public required string Name { get; set; } // First name
    public required string LastName { get; set; } // Last name
    public required string CaseNumber { get; set; } // Student ID or enrollment number
    public UserRole Role { get; set; } // User permissions role
    public bool IsActive { get; set; } = true; // Account status (active/suspended)
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Account creation date

    // Email verification (OTP)
    public bool IsEmailVerified { get; set; } // True once the OTP code was confirmed
    public string? OtpHash { get; set; } // Hash of the current code (never the code itself)
    public DateTime? OtpExpiresAt { get; set; } // When the current code expires
    public int OtpAttempts { get; set; } // Wrong attempts for the current code
    public DateTime? OtpLastSentAt { get; set; } // Used for the resend cooldown
}

public enum UserRole
{
    Student,
    Teacher,
    Admin,
    Staff
}