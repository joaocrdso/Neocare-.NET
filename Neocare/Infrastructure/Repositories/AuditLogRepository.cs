using Neocare.Domain.Interfaces;
using Neocare.Infrastructure.Persistence;

namespace Neocare.Infrastructure.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly MongoDbContext _context;

    public AuditLogRepository(MongoDbContext context) => _context = context;

    public async Task LogAsync(string action, string entity, Guid entityId, string performedBy)
    {
        var log = new AuditLog
        {
            Action = action,
            Entity = entity,
            EntityId = entityId,
            PerformedBy = performedBy,
            Timestamp = DateTime.UtcNow
        };

        await _context.AuditLogs.InsertOneAsync(log);
    }
}
