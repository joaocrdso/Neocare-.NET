namespace Neocare.Infrastructure.Persistence;

public class JwtSettings
{
    public required string SecretKey { get; set; }
    public int ExpirationInHours { get; set; } = 24;
}

public class MongoDbSettings
{
    public required string ConnectionString { get; set; }
    public required string DatabaseName { get; set; }
}
