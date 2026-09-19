using System.ComponentModel.DataAnnotations;

public class Users
{
    [Key]
    public int Id { get; set; } // Unique identifier

    [Required]
    [StringLength(100)]
    public required string Name { get; set; } // First name

    [Required]
    [StringLength(100)]
    public required string LastName { get; set; } // Last name

    [Required]
    [StringLength(50)]
    public required string CaseNumber { get; set; } // Student ID or enrollment number

    [Required]
    [EmailAddress]
    [StringLength(255)]
    public required string Email { get; set; } // User email

    [Required]
    [StringLength(255)]
    public required string PasswordHash { get; set; } // Encrypted password
    public UserRole Role { get; set; } // User permissions role
    public bool IsActive { get; set; } = true; // Account status (active/suspended)
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Account creation date
}

public enum UserRole
{
    Student,
    Teacher,
    Admin,
    Staff
}