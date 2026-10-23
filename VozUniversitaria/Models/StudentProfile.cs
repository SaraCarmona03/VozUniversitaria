
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VozUniversitaria.Models;

public class StudentProfile
{
    [Key]
    public int StudentProfileId { get; set; }

    // Relación con la cuenta existente
    [Required]
    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public Users User { get; set; } = null!;

    // Información académica
    [Required]
    public required string Career { get; set; }

    [Range(1, 20)]
    public int CurrentSemester { get; set; }

    [Required]
    public required string Group { get; set; }

    [Required]
    public required string Shift { get; set; }

    [Required]
    public required string Campus { get; set; }

    [Required]
    public required string Modality { get; set; }

    [Required]
    public required string AdmissionPeriod { get; set; }

    // Información opcional
    public string? PhoneNumber { get; set; }

    public string? ProfilePhotoPath { get; set; }
}
