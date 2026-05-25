using Neocare.Application.DTOs;
using Neocare.Application.Interfaces;
using Neocare.Domain.Entities;
using Neocare.Domain.Exceptions;
using Neocare.Domain.Interfaces;
using Neocare.Infrastructure.Persistence;

namespace Neocare.Application.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _repository;
    private readonly IAuditLogRepository _auditRepository;

    public AppointmentService(IAppointmentRepository repository, IAuditLogRepository auditRepository)
    {
        _repository = repository;
        _auditRepository = auditRepository;
    }

    public async Task<AppointmentDto> CreateAsync(CreateAppointmentDto dto)
    {
        var isAvailable = await _repository.IsTimeSlotAvailableAsync(
            dto.HealthProfessionalId,
            dto.ScheduledDate,
            dto.DurationMinutes
        );

        if (!isAvailable)
            throw new ValidationException("Horário não disponível para o profissional de saúde");

        var appointment = new Appointment
        {
            PatientId = dto.PatientId,
            HealthProfessionalId = dto.HealthProfessionalId,
            ScheduledDate = dto.ScheduledDate,
            DurationMinutes = dto.DurationMinutes,
            Reason = dto.Reason,
            Notes = dto.Notes
        };

        await _repository.AddAsync(appointment);
        await _repository.SaveAsync();
        await _auditRepository.LogAsync("CREATE", "Appointment", appointment.Id, "system");

        return MapToDto(appointment);
    }

    public async Task<AppointmentDto?> GetByIdAsync(Guid id)
    {
        var appointment = await _repository.GetByIdAsync(id);
        if (appointment == null) throw new NotFoundException("Appointment", id);
        return MapToDto(appointment);
    }

    public async Task<(List<AppointmentDto> Items, int Total, int Pages)> GetAllAsync(PaginationQueryDto pagination)
    {
        pagination.Validate();
        var appointments = await _repository.GetAllAsync();
        var total = appointments.Count();
        var pages = (int)Math.Ceiling(total / (double)pagination.PageSize);

        var items = appointments
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(MapToDto)
            .ToList();

        return (items, total, pages);
    }

    public async Task<AppointmentDto> UpdateAsync(Guid id, CreateAppointmentDto dto)
    {
        var appointment = await _repository.GetByIdAsync(id);
        if (appointment == null) throw new NotFoundException("Appointment", id);

        if (appointment.ScheduledDate != dto.ScheduledDate || appointment.HealthProfessionalId != dto.HealthProfessionalId)
        {
            var isAvailable = await _repository.IsTimeSlotAvailableAsync(
                dto.HealthProfessionalId,
                dto.ScheduledDate,
                dto.DurationMinutes
            );

            if (!isAvailable)
                throw new ValidationException("Horário não disponível para o profissional de saúde");
        }

        appointment.PatientId = dto.PatientId;
        appointment.HealthProfessionalId = dto.HealthProfessionalId;
        appointment.ScheduledDate = dto.ScheduledDate;
        appointment.DurationMinutes = dto.DurationMinutes;
        appointment.Reason = dto.Reason;
        appointment.Notes = dto.Notes;

        await _repository.UpdateAsync(appointment);
        await _repository.SaveAsync();
        await _auditRepository.LogAsync("UPDATE", "Appointment", appointment.Id, "system");

        return MapToDto(appointment);
    }

    public async Task DeleteAsync(Guid id)
    {
        var appointment = await _repository.GetByIdAsync(id);
        if (appointment == null) throw new NotFoundException("Appointment", id);

        await _repository.DeleteAsync(appointment);
        await _repository.SaveAsync();
        await _auditRepository.LogAsync("DELETE", "Appointment", appointment.Id, "system");
    }

    private static AppointmentDto MapToDto(Appointment appointment) => new()
    {
        Id = appointment.Id,
        PatientId = appointment.PatientId,
        HealthProfessionalId = appointment.HealthProfessionalId,
        ScheduledDate = appointment.ScheduledDate,
        DurationMinutes = appointment.DurationMinutes,
        Reason = appointment.Reason,
        Status = appointment.Status,
        Notes = appointment.Notes,
        CreatedAt = appointment.CreatedAt,
        UpdatedAt = appointment.UpdatedAt
    };
}
