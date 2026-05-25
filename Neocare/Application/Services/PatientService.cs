using Neocare.Application.DTOs;
using Neocare.Application.Interfaces;
using Neocare.Domain.Entities;
using Neocare.Domain.Exceptions;
using Neocare.Domain.Interfaces;
using Neocare.Infrastructure.Persistence;

namespace Neocare.Application.Services;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _repository;
    private readonly IAuditLogRepository _auditRepository;

    public PatientService(IPatientRepository repository, IAuditLogRepository auditRepository)
    {
        _repository = repository;
        _auditRepository = auditRepository;
    }

    public async Task<PatientDto> CreateAsync(CreatePatientDto dto)
    {
        var patient = new Patient
        {
            Name = dto.Name,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            CPF = dto.CPF,
            DateOfBirth = dto.DateOfBirth,
            Address = dto.Address,
            MedicalHistory = dto.MedicalHistory
        };

        await _repository.AddAsync(patient);
        await _repository.SaveAsync();
        await _auditRepository.LogAsync("CREATE", "Patient", patient.Id, "system");

        return MapToDto(patient);
    }

    public async Task<PatientDto?> GetByIdAsync(Guid id)
    {
        var patient = await _repository.GetByIdAsync(id);
        if (patient == null) throw new NotFoundException("Patient", id);
        return MapToDto(patient);
    }

    public async Task<(List<PatientDto> Items, int Total, int Pages)> GetAllAsync(PaginationQueryDto pagination, string? name = null, string? status = null)
    {
        pagination.Validate();
        var patients = await _repository.GetAllAsync();
        var filtered = patients.AsEnumerable();

        if (!string.IsNullOrEmpty(name))
            filtered = filtered.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrEmpty(status))
            filtered = filtered.Where(p => p.Status == status);

        var total = filtered.Count();
        var pages = (int)Math.Ceiling(total / (double)pagination.PageSize);

        var items = filtered
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(MapToDto)
            .ToList();

        return (items, total, pages);
    }

    public async Task<PatientDto> UpdateAsync(Guid id, CreatePatientDto dto)
    {
        var patient = await _repository.GetByIdAsync(id);
        if (patient == null) throw new NotFoundException("Patient", id);

        patient.Name = dto.Name;
        patient.Email = dto.Email;
        patient.PhoneNumber = dto.PhoneNumber;
        patient.CPF = dto.CPF;
        patient.DateOfBirth = dto.DateOfBirth;
        patient.Address = dto.Address;
        patient.MedicalHistory = dto.MedicalHistory;

        await _repository.UpdateAsync(patient);
        await _repository.SaveAsync();
        await _auditRepository.LogAsync("UPDATE", "Patient", patient.Id, "system");

        return MapToDto(patient);
    }

    public async Task DeleteAsync(Guid id)
    {
        var patient = await _repository.GetByIdAsync(id);
        if (patient == null) throw new NotFoundException("Patient", id);

        await _repository.DeleteAsync(patient);
        await _repository.SaveAsync();
        await _auditRepository.LogAsync("DELETE", "Patient", patient.Id, "system");
    }

    private static PatientDto MapToDto(Patient patient) => new()
    {
        Id = patient.Id,
        Name = patient.Name,
        Email = patient.Email,
        PhoneNumber = patient.PhoneNumber,
        CPF = patient.CPF,
        DateOfBirth = patient.DateOfBirth,
        Address = patient.Address,
        MedicalHistory = patient.MedicalHistory,
        CreatedAt = patient.CreatedAt,
        UpdatedAt = patient.UpdatedAt,
        Status = patient.Status
    };
}
