using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using FluentAssertions;
using System.Net.Http.Json;
using Neocare.Application.DTOs;

namespace Neocare.Tests.Integration.API
{
    /// <summary>
    /// Testes de integração para validar endpoints da API seguindo padrão AAA
    /// </summary>
    public class StressEntriesApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public StressEntriesApiTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        // TESTES DE INTEGRAÇÃO - PADRÃO AAA

        #region Health Checks - Validam saúde da aplicação
        [Fact]
        public async Task HealthCheck_Get_ReturnsHealthy()
        {
            // Arrange - dados não necessários para health check
            // Act - fazer requisição GET ao endpoint de health
            var response = await _client.GetAsync("/health");

            // Assert - validar que a aplicação está saudável
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task HealthCheckReady_Get_ReturnsHealthy()
        {
            // Arrange - sem preparação necessária
            // Act - fazer requisição ao endpoint de prontidão
            var response = await _client.GetAsync("/health/ready");

            // Assert - validar readiness probe
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }
        #endregion

        #region Error Handling - Validam tratamento de erros
        [Fact]
        public async Task InvalidEndpoint_Returns404()
        {
            // Arrange - definir endpoint inválido
            var invalidEndpoint = "/api/nonexistent";

            // Act - fazer requisição a endpoint não existente
            var response = await _client.GetAsync(invalidEndpoint);

            // Assert - validar retorno 404
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateNonExistentEntry_ReturnsNotFound()
        {
            // Arrange - preparar ID que não existe
            var nonExistentId = Guid.NewGuid();
            var updateDto = new StressEntryDto
            {
                Id = nonExistentId,
                StressLevel = 8,
                Description = "Não existe",
                Symptoms = new List<string> { }
            };

            // Act - tentar atualizar registro inexistente
            var response = await _client.PutAsJsonAsync($"/api/stress/{nonExistentId}", updateDto);

            // Assert - validar que não foi encontrado
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteNonExistentEntry_ReturnsNotFound()
        {
            // Arrange - preparar ID de registro que não existe
            var nonExistentId = Guid.NewGuid();

            // Act - tentar deletar registro inexistente
            var response = await _client.DeleteAsync($"/api/stress/{nonExistentId}");

            // Assert - validar retorno 404
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }
        #endregion

        #region API Response Format - Validam estrutura da resposta
        [Fact]
        public async Task ApiResponse_HealthCheckWorks()
        {
            // Arrange - não precisa de preparação para health check
            // Act - fazer requisição ao health check
            var response = await _client.GetAsync("/health");

            // Assert - validar que está saudável
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }
        #endregion
    }
}
