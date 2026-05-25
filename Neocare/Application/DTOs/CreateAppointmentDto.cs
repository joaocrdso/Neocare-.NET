namespace Neocare.Application.DTOs;

public class CreateAppointmentDto
{
    public Guid PatientId { get; set; }
    public Guid HealthProfessionalId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public int DurationMinutes { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
