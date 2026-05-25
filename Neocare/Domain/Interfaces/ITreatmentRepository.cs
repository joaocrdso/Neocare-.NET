using Neocare.Domain.Entities;

namespace Neocare.Domain.Interfaces;

public interface ITreatmentRepository : IRepository<Treatment>
{
    Task<IEnumerable<Treatment>> GetByPatientIdAsync(Guid patientId);
    Task<IEnumerable<Treatment>> GetActiveAsync();
}
