namespace Neocare.Application.DTOs;

public class CreateTreatmentDto
{
    public Guid AppointmentId { get; set; }
    public Guid PatientId { get; set; }
    public required string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Prescription { get; set; }
}
