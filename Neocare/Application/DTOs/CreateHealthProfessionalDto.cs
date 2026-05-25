namespace Neocare.Application.DTOs;

public class CreateHealthProfessionalDto
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
    public required string CPF { get; set; }
    public required string Specialty { get; set; }
    public required string CRM { get; set; }
}
