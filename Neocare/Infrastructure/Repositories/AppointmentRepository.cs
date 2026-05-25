using Microsoft.EntityFrameworkCore;
using Neocare.Domain.Entities;
using Neocare.Domain.Interfaces;
using Neocare.Infrastructure.Data;

namespace Neocare.Infrastructure.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly NeocareDbContext _context;

    public AppointmentRepository(NeocareDbContext context) => _context = context;

    public async Task<Appointment?> GetByIdAsync(Guid id) => 
        await _context.Appointments.Include(a => a.Patient).Include(a => a.HealthProfessional).FirstOrDefaultAsync(a => a.Id == id);

    public async Task<IEnumerable<Appointment>> GetAllAsync() =>
        await _context.Appointments.Include(a => a.Patient).Include(a => a.HealthProfessional).ToListAsync();

    public async Task<IEnumerable<Appointment>> GetByPatientIdAsync(Guid patientId) =>
        await _context.Appointments.Where(a => a.PatientId == patientId).Include(a => a.HealthProfessional).ToListAsync();

    public async Task<IEnumerable<Appointment>> GetByHealthProfessionalIdAsync(Guid professionalId) =>
        await _context.Appointments.Where(a => a.HealthProfessionalId == professionalId).Include(a => a.Patient).ToListAsync();

    public async Task<IEnumerable<Appointment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate) =>
        await _context.Appointments.Where(a => a.ScheduledDate >= startDate && a.ScheduledDate <= endDate).ToListAsync();

    public async Task<bool> IsTimeSlotAvailableAsync(Guid healthProfessionalId, DateTime scheduledDate, int durationMinutes)
    {
        var endTime = scheduledDate.AddMinutes(durationMinutes);
        var conflicting = await _context.Appointments.AnyAsync(a =>
            a.HealthProfessionalId == healthProfessionalId &&
            a.ScheduledDate < endTime &&
            a.ScheduledDate.AddMinutes(a.DurationMinutes) > scheduledDate
        );
        return !conflicting;
    }

    public async Task AddAsync(Appointment entity)
    {
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        await _context.Appointments.AddAsync(entity);
    }

    public async Task UpdateAsync(Appointment entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Appointments.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Appointment entity)
    {
        _context.Appointments.Remove(entity);
        await Task.CompletedTask;
    }

    public async Task SaveAsync() => await _context.SaveChangesAsync();
}
