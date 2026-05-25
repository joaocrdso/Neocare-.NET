using MongoDB.Driver;
using Microsoft.Extensions.Options;

namespace Neocare.Infrastructure.Persistence;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoCollection<AuditLog> AuditLogs => _database.GetCollection<AuditLog>("audit_logs");
}

public class AuditLog
{
    public MongoDB.Bson.ObjectId Id { get; set; }
    public required string Action { get; set; }
    public required string Entity { get; set; }
    public Guid EntityId { get; set; }
    public required string PerformedBy { get; set; }
    public DateTime Timestamp { get; set; }
}
