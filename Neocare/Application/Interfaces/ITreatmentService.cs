using Neocare.Application.DTOs;

namespace Neocare.Application.Interfaces;

public interface ITreatmentService
{
    Task<TreatmentDto> CreateAsync(CreateTreatmentDto dto);
    Task<TreatmentDto?> GetByIdAsync(Guid id);
    Task<(List<TreatmentDto> Items, int Total, int Pages)> GetAllAsync(PaginationQueryDto pagination);
    Task<TreatmentDto> UpdateAsync(Guid id, CreateTreatmentDto dto);
    Task DeleteAsync(Guid id);
}
