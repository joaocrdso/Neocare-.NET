using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Neocare.Application.DTOs;
using Xunit;

namespace Neocare.Tests.Integration.API;

public class AuthApiTests : IAsyncLifetime
{
    private WebApplicationFactory<Program>? _factory;
    private HttpClient? _client;

    public async Task InitializeAsync()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _client?.Dispose();
        _factory?.Dispose();
        await Task.CompletedTask;
    }

    [Fact]
    public async Task Register_WithValidData_ShouldReturnOkAndToken()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Email = $"test-{Guid.NewGuid()}@example.com",
            Password = "Password123!"
        };

        // Act
        var response = await _client!.PostAsJsonAsync("/api/auth/register", registerDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsAsync<AuthResponseDto>();
        Assert.NotNull(content);
        Assert.NotEmpty(content.Token);
        Assert.Equal(registerDto.Email, content.Email);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnOkAndToken()
    {
        // Arrange
        var email = $"test-{Guid.NewGuid()}@example.com";
        var password = "Password123!";

        var registerDto = new RegisterDto { Email = email, Password = password };
        await _client!.PostAsJsonAsync("/api/auth/register", registerDto);

        var loginDto = new LoginDto { Email = email, Password = password };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsAsync<AuthResponseDto>();
        Assert.NotNull(content);
        Assert.NotEmpty(content.Token);
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ShouldReturnBadRequest()
    {
        // Arrange
        var email = $"test-{Guid.NewGuid()}@example.com";
        var password = "Password123!";

        var registerDto = new RegisterDto { Email = email, Password = password };
        await _client!.PostAsJsonAsync("/api/auth/register", registerDto);

        var loginDto = new LoginDto { Email = email, Password = "WrongPassword!" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
