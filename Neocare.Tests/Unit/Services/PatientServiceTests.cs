using Moq;
using Neocare.Application.DTOs;
using Neocare.Application.Services;
using Neocare.Domain.Entities;
using Neocare.Domain.Interfaces;
using Xunit;

namespace Neocare.Tests.Unit.Services;

public class PatientServiceTests
{
    private readonly Mock<IPatientRepository> _patientRepositoryMock = new();
    private readonly Mock<IAuditLogRepository> _auditRepositoryMock = new();
    private readonly PatientService _patientService;

    public PatientServiceTests()
    {
        _patientService = new PatientService(_patientRepositoryMock.Object, _auditRepositoryMock.Object);
    }

    [Fact]
    public async Task CreatePatient_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var createDto = new CreatePatientDto
        {
            Name = "João Silva",
            Email = "joao@test.com",
            CPF = "12345678901",
            PhoneNumber = "(11) 98765-4321",
            DateOfBirth = new DateTime(1990, 5, 15)
        };

        _patientRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Patient>())).Returns(Task.CompletedTask);
        _patientRepositoryMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);
        _auditRepositoryMock.Setup(a => a.LogAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<string>())).Returns(Task.CompletedTask);

        // Act
        var result = await _patientService.CreateAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("João Silva", result.Name);
        Assert.Equal("joao@test.com", result.Email);
        _patientRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Patient>()), Times.Once);
        _patientRepositoryMock.Verify(r => r.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task GetPatientById_WithValidId_ShouldReturnPatient()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var patient = new Patient
        {
            Id = patientId,
            Name = "João Silva",
            Email = "joao@test.com",
            CPF = "12345678901",
            PhoneNumber = "(11) 98765-4321",
            Status = "Active"
        };

        _patientRepositoryMock.Setup(r => r.GetByIdAsync(patientId)).ReturnsAsync(patient);

        // Act
        var result = await _patientService.GetByIdAsync(patientId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("João Silva", result.Name);
        _patientRepositoryMock.Verify(r => r.GetByIdAsync(patientId), Times.Once);
    }

    [Fact]
    public async Task GetPatientById_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        _patientRepositoryMock.Setup(r => r.GetByIdAsync(patientId)).ReturnsAsync((Patient?)null);

        // Act
        var result = await _patientService.GetByIdAsync(patientId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdatePatient_WithValidData_ShouldUpdateSuccessfully()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var patient = new Patient
        {
            Id = patientId,
            Name = "João Silva",
            Email = "joao@test.com",
            CPF = "12345678901",
            PhoneNumber = "(11) 98765-4321"
        };

        var updateDto = new CreatePatientDto
        {
            Name = "João Silva Atualizado",
            Email = "joao.new@test.com",
            CPF = "12345678901",
            PhoneNumber = "(11) 98765-4321",
            DateOfBirth = new DateTime(1990, 5, 15)
        };

        _patientRepositoryMock.Setup(r => r.GetByIdAsync(patientId)).ReturnsAsync(patient);
        _patientRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Patient>())).Returns(Task.CompletedTask);
        _patientRepositoryMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);
        _auditRepositoryMock.Setup(a => a.LogAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<string>())).Returns(Task.CompletedTask);

        // Act
        var result = await _patientService.UpdateAsync(patientId, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("João Silva Atualizado", result.Name);
        _patientRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Patient>()), Times.Once);
    }

    [Fact]
    public async Task DeletePatient_WithValidId_ShouldDeleteSuccessfully()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var patient = new Patient
        {
            Id = patientId,
            Name = "João Silva",
            Email = "joao@test.com",
            CPF = "12345678901",
            PhoneNumber = "(11) 98765-4321"
        };

        _patientRepositoryMock.Setup(r => r.GetByIdAsync(patientId)).ReturnsAsync(patient);
        _patientRepositoryMock.Setup(r => r.DeleteAsync(It.IsAny<Patient>())).Returns(Task.CompletedTask);
        _patientRepositoryMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);
        _auditRepositoryMock.Setup(a => a.LogAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<string>())).Returns(Task.CompletedTask);

        // Act
        await _patientService.DeleteAsync(patientId);

        // Assert
        _patientRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Patient>()), Times.Once);
    }
}
