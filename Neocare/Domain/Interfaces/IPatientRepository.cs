using Neocare.Domain.Entities;

namespace Neocare.Domain.Interfaces;

public interface IPatientRepository : IRepository<Patient>
{
    Task<Patient?> GetByEmailAsync(string email);
    Task<Patient?> GetByCPFAsync(string cpf);
    Task<IEnumerable<Patient>> GetByNameAsync(string name);
}
