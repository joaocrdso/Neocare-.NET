using Neocare.Application.DTOs;
using Neocare.Application.Interfaces;
using Neocare.Domain.Entities;
using Neocare.Domain.Exceptions;
using Neocare.Domain.Interfaces;
using Neocare.Infrastructure.Persistence;

namespace Neocare.Application.Services;

public class HealthProfessionalService : IHealthProfessionalService
{
    private readonly IHealthProfessionalRepository _repository;
    private readonly IAuditLogRepository _auditRepository;

    public HealthProfessionalService(IHealthProfessionalRepository repository, IAuditLogRepository auditRepository)
    {
        _repository = repository;
        _auditRepository = auditRepository;
    }

    public async Task<HealthProfessionalDto> CreateAsync(CreateHealthProfessionalDto dto)
    {
        var professional = new HealthProfessional
        {
            Name = dto.Name,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            CPF = dto.CPF,
            Specialty = dto.Specialty,
            CRM = dto.CRM
        };

        await _repository.AddAsync(professional);
        await _repository.SaveAsync();
        await _auditRepository.LogAsync("CREATE", "HealthProfessional", professional.Id, "system");

        return MapToDto(professional);
    }

    public async Task<HealthProfessionalDto?> GetByIdAsync(Guid id)
    {
        var professional = await _repository.GetByIdAsync(id);
        if (professional == null) throw new NotFoundException("HealthProfessional", id);
        return MapToDto(professional);
    }

    public async Task<(List<HealthProfessionalDto> Items, int Total, int Pages)> GetAllAsync(PaginationQueryDto pagination)
    {
        pagination.Validate();
        var professionals = await _repository.GetAllAsync();
        var total = professionals.Count();
        var pages = (int)Math.Ceiling(total / (double)pagination.PageSize);

        var items = professionals
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(MapToDto)
            .ToList();

        return (items, total, pages);
    }

    public async Task<HealthProfessionalDto> UpdateAsync(Guid id, CreateHealthProfessionalDto dto)
    {
        var professional = await _repository.GetByIdAsync(id);
        if (professional == null) throw new NotFoundException("HealthProfessional", id);

        professional.Name = dto.Name;
        professional.Email = dto.Email;
        professional.PhoneNumber = dto.PhoneNumber;
        professional.CPF = dto.CPF;
        professional.Specialty = dto.Specialty;
        professional.CRM = dto.CRM;

        await _repository.UpdateAsync(professional);
        await _repository.SaveAsync();
        await _auditRepository.LogAsync("UPDATE", "HealthProfessional", professional.Id, "system");

        return MapToDto(professional);
    }

    public async Task DeleteAsync(Guid id)
    {
        var professional = await _repository.GetByIdAsync(id);
        if (professional == null) throw new NotFoundException("HealthProfessional", id);

        await _repository.DeleteAsync(professional);
        await _repository.SaveAsync();
        await _auditRepository.LogAsync("DELETE", "HealthProfessional", professional.Id, "system");
    }

    private static HealthProfessionalDto MapToDto(HealthProfessional professional) => new()
    {
        Id = professional.Id,
        Name = professional.Name,
        Email = professional.Email,
        PhoneNumber = professional.PhoneNumber,
        CPF = professional.CPF,
        Specialty = professional.Specialty,
        CRM = professional.CRM,
        CreatedAt = professional.CreatedAt,
        UpdatedAt = professional.UpdatedAt,
        Status = professional.Status
    };
}
