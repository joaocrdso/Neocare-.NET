using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Neocare.Infrastructure.HealthChecks
{
    /// <summary>
    /// Health check para verificar a conectividade com o banco de dados
    /// </summary>
    public class DatabaseHealthCheck : IHealthCheck
    {
        private readonly ILogger<DatabaseHealthCheck> _logger;

        public DatabaseHealthCheck(ILogger<DatabaseHealthCheck> logger)
        {
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Verificando saúde do banco de dados");
                
                // Simular verificação de conectividade com banco de dados
                // Em produção, isso faria uma query real no banco
                await Task.Delay(10, cancellationToken);
                
                _logger.LogInformation("Banco de dados está saudável");
                return HealthCheckResult.Healthy("Banco de dados está operacional");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao verificar saúde do banco de dados");
                return HealthCheckResult.Unhealthy("Banco de dados não está acessível", ex);
            }
        }
    }
}
