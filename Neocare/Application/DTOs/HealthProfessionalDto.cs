namespace Neocare.Application.DTOs;

public class HealthProfessionalDto
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
    public string Status { get; set; } = string.Empty;
}
