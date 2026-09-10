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
}

public enum UserRole
{
    Student,
    Teacher,
    Admin,
    Staff
}