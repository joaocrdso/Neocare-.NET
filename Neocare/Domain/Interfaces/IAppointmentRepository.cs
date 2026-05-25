using Neocare.Domain.Entities;

namespace Neocare.Domain.Interfaces;

public interface IAppointmentRepository : IRepository<Appointment>
{
    Task<IEnumerable<Appointment>> GetByPatientIdAsync(Guid patientId);
    Task<IEnumerable<Appointment>> GetByHealthProfessionalIdAsync(Guid professionalId);
    Task<IEnumerable<Appointment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<bool> IsTimeSlotAvailableAsync(Guid healthProfessionalId, DateTime scheduledDate, int durationMinutes);
}
