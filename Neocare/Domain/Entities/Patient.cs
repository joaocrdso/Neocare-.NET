namespace Neocare.Domain.Entities;

public class Patient
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
    public required string CPF { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? Address { get; set; }
    public string? MedicalHistory { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Status { get; set; } = "Active";

    public ICollection<Appointment> Appointments { get; set; } = [];
}
