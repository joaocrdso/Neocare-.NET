using Microsoft.EntityFrameworkCore;
using Neocare.Domain.Entities;
using Neocare.Domain.Interfaces;
using Neocare.Infrastructure.Data;

namespace Neocare.Infrastructure.Repositories;

public class HealthProfessionalRepository : IHealthProfessionalRepository
{
    private readonly NeocareDbContext _context;

    public HealthProfessionalRepository(NeocareDbContext context) => _context = context;

    public async Task<HealthProfessional?> GetByIdAsync(Guid id) => await _context.HealthProfessionals.FindAsync(id);

    public async Task<IEnumerable<HealthProfessional>> GetAllAsync() => await _context.HealthProfessionals.ToListAsync();

    public async Task<HealthProfessional?> GetByEmailAsync(string email) =>
        await _context.HealthProfessionals.FirstOrDefaultAsync(h => h.Email == email);

    public async Task<HealthProfessional?> GetByCRMAsync(string crm) =>
        await _context.HealthProfessionals.FirstOrDefaultAsync(h => h.CRM == crm);

    public async Task AddAsync(HealthProfessional entity)
    {
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        await _context.HealthProfessionals.AddAsync(entity);
    }

    public async Task UpdateAsync(HealthProfessional entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _context.HealthProfessionals.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(HealthProfessional entity)
    {
        _context.HealthProfessionals.Remove(entity);
        await Task.CompletedTask;
    }

    public async Task SaveAsync() => await _context.SaveChangesAsync();
}
