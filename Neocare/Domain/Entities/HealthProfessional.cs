namespace Neocare.Domain.Entities;

public class HealthProfessional
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
    public required string CPF { get; set; }
    public required string Specialty { get; set; }
    public required string CRM { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Status { get; set; } = "Active";

    public ICollection<Appointment> Appointments { get; set; } = [];
}
