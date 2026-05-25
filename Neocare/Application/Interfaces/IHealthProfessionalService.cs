using Neocare.Application.DTOs;

namespace Neocare.Application.Interfaces;

public interface IHealthProfessionalService
{
    Task<HealthProfessionalDto> CreateAsync(CreateHealthProfessionalDto dto);
    Task<HealthProfessionalDto?> GetByIdAsync(Guid id);
    Task<(List<HealthProfessionalDto> Items, int Total, int Pages)> GetAllAsync(PaginationQueryDto pagination);
    Task<HealthProfessionalDto> UpdateAsync(Guid id, CreateHealthProfessionalDto dto);
    Task DeleteAsync(Guid id);
}
