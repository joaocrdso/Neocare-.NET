using Neocare.Application.DTOs;
using Neocare.Application.Interfaces;
using Neocare.Domain.Entities;
using Neocare.Domain.Exceptions;
using Neocare.Domain.Interfaces;
using Neocare.Infrastructure.Persistence;

namespace Neocare.Application.Services;

public class TreatmentService : ITreatmentService
{
    private readonly ITreatmentRepository _repository;
    private readonly IAuditLogRepository _auditRepository;

    public TreatmentService(ITreatmentRepository repository, IAuditLogRepository auditRepository)
    {
        _repository = repository;
        _auditRepository = auditRepository;
    }

    public async Task<TreatmentDto> CreateAsync(CreateTreatmentDto dto)
    {
        var treatment = new Treatment
        {
            AppointmentId = dto.AppointmentId,
            PatientId = dto.PatientId,
            Description = dto.Description,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Prescription = dto.Prescription
        };

        await _repository.AddAsync(treatment);
        await _repository.SaveAsync();
        await _auditRepository.LogAsync("CREATE", "Treatment", treatment.Id, "system");

        return MapToDto(treatment);
    }

    public async Task<TreatmentDto?> GetByIdAsync(Guid id)
    {
        var treatment = await _repository.GetByIdAsync(id);
        if (treatment == null) throw new NotFoundException("Treatment", id);
        return MapToDto(treatment);
    }

    public async Task<(List<TreatmentDto> Items, int Total, int Pages)> GetAllAsync(PaginationQueryDto pagination)
    {
        pagination.Validate();
        var treatments = await _repository.GetAllAsync();
        var total = treatments.Count();
        var pages = (int)Math.Ceiling(total / (double)pagination.PageSize);

        var items = treatments
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(MapToDto)
            .ToList();

        return (items, total, pages);
    }

    public async Task<TreatmentDto> UpdateAsync(Guid id, CreateTreatmentDto dto)
    {
        var treatment = await _repository.GetByIdAsync(id);
        if (treatment == null) throw new NotFoundException("Treatment", id);

        treatment.AppointmentId = dto.AppointmentId;
        treatment.PatientId = dto.PatientId;
        treatment.Description = dto.Description;
        treatment.StartDate = dto.StartDate;
        treatment.EndDate = dto.EndDate;
        treatment.Prescription = dto.Prescription;

        await _repository.UpdateAsync(treatment);
        await _repository.SaveAsync();
        await _auditRepository.LogAsync("UPDATE", "Treatment", treatment.Id, "system");

        return MapToDto(treatment);
    }

    public async Task DeleteAsync(Guid id)
    {
        var treatment = await _repository.GetByIdAsync(id);
        if (treatment == null) throw new NotFoundException("Treatment", id);

        await _repository.DeleteAsync(treatment);
        await _repository.SaveAsync();
        await _auditRepository.LogAsync("DELETE", "Treatment", treatment.Id, "system");
    }

    private static TreatmentDto MapToDto(Treatment treatment) => new()
    {
        Id = treatment.Id,
        AppointmentId = treatment.AppointmentId,
        PatientId = treatment.PatientId,
        Description = treatment.Description,
        StartDate = treatment.StartDate,
        EndDate = treatment.EndDate,
        Status = treatment.Status,
        Prescription = treatment.Prescription,
        CreatedAt = treatment.CreatedAt,
        UpdatedAt = treatment.UpdatedAt
    };
}
