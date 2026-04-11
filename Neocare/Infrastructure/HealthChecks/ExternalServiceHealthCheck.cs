using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Neocare.Infrastructure.HealthChecks
{
    /// <summary>
    /// Health check para verificar a disponibilidade de serviços externos
    /// </summary>
    public class ExternalServiceHealthCheck : IHealthCheck
    {
        private readonly ILogger<ExternalServiceHealthCheck> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public ExternalServiceHealthCheck(ILogger<ExternalServiceHealthCheck> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Verificando saúde de serviços externos");
                
                // Simular verificação de serviço externo
                // Em produção, isso faria uma chamada HTTP real
                var httpClient = _httpClientFactory.CreateClient();
                var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter(TimeSpan.FromSeconds(5));
                
                // Placeholder: seria uma chamada real a um serviço externo
                await Task.Delay(10, cts.Token);
                
                _logger.LogInformation("Serviços externos estão disponíveis");
                return HealthCheckResult.Healthy("Serviços externos estão acessíveis");
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Timeout ao verificar serviços externos");
                return HealthCheckResult.Degraded("Serviços externos respondendo lentamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao verificar saúde de serviços externos");
                return HealthCheckResult.Unhealthy("Serviços externos não estão disponíveis", ex);
            }
        }
    }
}
