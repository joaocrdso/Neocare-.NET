using Neocare.Domain.Entities;

namespace Neocare.Domain.Interfaces;

public interface IHealthProfessionalRepository : IRepository<HealthProfessional>
{
    Task<HealthProfessional?> GetByEmailAsync(string email);
    Task<HealthProfessional?> GetByCRMAsync(string crm);
}
