namespace Neocare.Domain.Interfaces;

public interface IAuditLogRepository
{
    Task LogAsync(string action, string entity, Guid entityId, string performedBy);
}
