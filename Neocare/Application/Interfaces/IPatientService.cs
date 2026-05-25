using Neocare.Application.DTOs;

namespace Neocare.Application.Interfaces;

public interface IPatientService
{
    Task<PatientDto> CreateAsync(CreatePatientDto dto);
    Task<PatientDto?> GetByIdAsync(Guid id);
    Task<(List<PatientDto> Items, int Total, int Pages)> GetAllAsync(PaginationQueryDto pagination, string? name = null, string? status = null);
    Task<PatientDto> UpdateAsync(Guid id, CreatePatientDto dto);
    Task DeleteAsync(Guid id);
}
