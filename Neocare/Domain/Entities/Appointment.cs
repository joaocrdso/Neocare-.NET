namespace Neocare.Domain.Entities;

public class Appointment
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid HealthProfessionalId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public int DurationMinutes { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = "Scheduled";
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Patient? Patient { get; set; }
    public HealthProfessional? HealthProfessional { get; set; }
    public Treatment? Treatment { get; set; }
}
