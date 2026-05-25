using Neocare.Application.DTOs;

namespace Neocare.Application.Interfaces;

public interface IAppointmentService
{
    Task<AppointmentDto> CreateAsync(CreateAppointmentDto dto);
    Task<AppointmentDto?> GetByIdAsync(Guid id);
    Task<(List<AppointmentDto> Items, int Total, int Pages)> GetAllAsync(PaginationQueryDto pagination);
    Task<AppointmentDto> UpdateAsync(Guid id, CreateAppointmentDto dto);
    Task DeleteAsync(Guid id);
}
