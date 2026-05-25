using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Neocare.Application.DTOs;
using Xunit;

namespace Neocare.Tests.Integration.API;

public class PatientsApiTests : IAsyncLifetime
{
    private WebApplicationFactory<Program>? _factory;
    private HttpClient? _client;
    private string? _token;

    public async Task InitializeAsync()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();

        // Register and login to get token
        var registerDto = new RegisterDto
        {
            Email = $"test-{Guid.NewGuid()}@example.com",
            Password = "Password123!"
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerDto);
        var authResponse = await registerResponse.Content.ReadAsAsync<AuthResponseDto>();
        _token = authResponse?.Token;

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
        
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _client?.Dispose();
        _factory?.Dispose();
        await Task.CompletedTask;
    }

    [Fact]
    public async Task CreatePatient_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var createDto = new CreatePatientDto
        {
            Name = "João Silva",
            Email = $"joao-{Guid.NewGuid()}@example.com",
            CPF = "12345678901",
            PhoneNumber = "(11) 98765-4321",
            DateOfBirth = new DateTime(1990, 5, 15)
        };

        // Act
        var response = await _client!.PostAsJsonAsync("/api/patients", createDto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var content = await response.Content.ReadAsAsync<PatientDto>();
        Assert.NotNull(content);
        Assert.Equal("João Silva", content.Name);
    }

    [Fact]
    public async Task GetPatients_ShouldReturnOkWithPaginatedData()
    {
        // Arrange - Create a patient first
        var createDto = new CreatePatientDto
        {
            Name = "Test Patient",
            Email = $"test-{Guid.NewGuid()}@example.com",
            CPF = "98765432101",
            PhoneNumber = "(11) 98765-4321",
            DateOfBirth = new DateTime(1990, 5, 15)
        };

        await _client!.PostAsJsonAsync("/api/patients", createDto);

        // Act
        var response = await _client.GetAsync("/api/patients?pageNumber=1&pageSize=10");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
