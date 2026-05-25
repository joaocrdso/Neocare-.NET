namespace Neocare.Domain.Entities;

public class Treatment
{
    public Guid Id { get; set; }
    public Guid AppointmentId { get; set; }
    public Guid PatientId { get; set; }
    public required string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } = "Active";
    public string? Prescription { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Appointment? Appointment { get; set; }
    public Patient? Patient { get; set; }
}
