using Microsoft.EntityFrameworkCore;
using Neocare.Domain.Entities;
using Neocare.Domain.Interfaces;
using Neocare.Infrastructure.Data;

namespace Neocare.Infrastructure.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly NeocareDbContext _context;

    public PatientRepository(NeocareDbContext context) => _context = context;

    public async Task<Patient?> GetByIdAsync(Guid id) => await _context.Patients.FindAsync(id);

    public async Task<IEnumerable<Patient>> GetAllAsync() => await _context.Patients.ToListAsync();

    public async Task<Patient?> GetByEmailAsync(string email) =>
        await _context.Patients.FirstOrDefaultAsync(p => p.Email == email);

    public async Task<Patient?> GetByCPFAsync(string cpf) =>
        await _context.Patients.FirstOrDefaultAsync(p => p.CPF == cpf);

    public async Task<IEnumerable<Patient>> GetByNameAsync(string name) =>
        await _context.Patients.Where(p => p.Name.Contains(name)).ToListAsync();

    public async Task AddAsync(Patient entity)
    {
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        await _context.Patients.AddAsync(entity);
    }

    public async Task UpdateAsync(Patient entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Patients.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Patient entity)
    {
        _context.Patients.Remove(entity);
        await Task.CompletedTask;
    }

    public async Task SaveAsync() => await _context.SaveChangesAsync();
}
