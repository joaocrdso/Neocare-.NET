namespace Neocare.Application.DTOs;

public class CreatePatientDto
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
    public required string CPF { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? Address { get; set; }
    public string? MedicalHistory { get; set; }
}
