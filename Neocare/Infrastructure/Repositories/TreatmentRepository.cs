using Microsoft.EntityFrameworkCore;
using Neocare.Domain.Entities;
using Neocare.Domain.Interfaces;
using Neocare.Infrastructure.Data;

namespace Neocare.Infrastructure.Repositories;

public class TreatmentRepository : ITreatmentRepository
{
    private readonly NeocareDbContext _context;

    public TreatmentRepository(NeocareDbContext context) => _context = context;

    public async Task<Treatment?> GetByIdAsync(Guid id) =>
        await _context.Treatments.Include(t => t.Appointment).FirstOrDefaultAsync(t => t.Id == id);

    public async Task<IEnumerable<Treatment>> GetAllAsync() =>
        await _context.Treatments.Include(t => t.Appointment).ToListAsync();

    public async Task<IEnumerable<Treatment>> GetByPatientIdAsync(Guid patientId) =>
        await _context.Treatments.Where(t => t.PatientId == patientId).ToListAsync();

    public async Task<IEnumerable<Treatment>> GetActiveAsync() =>
        await _context.Treatments.Where(t => t.Status == "Active").ToListAsync();

    public async Task AddAsync(Treatment entity)
    {
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        await _context.Treatments.AddAsync(entity);
    }

    public async Task UpdateAsync(Treatment entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Treatments.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Treatment entity)
    {
        _context.Treatments.Remove(entity);
        await Task.CompletedTask;
    }

    public async Task SaveAsync() => await _context.SaveChangesAsync();
}
