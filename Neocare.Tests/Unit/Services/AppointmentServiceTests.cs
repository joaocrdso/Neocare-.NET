using Moq;
using Neocare.Application.DTOs;
using Neocare.Application.Services;
using Neocare.Domain.Entities;
using Neocare.Domain.Interfaces;
using Xunit;

namespace Neocare.Tests.Unit.Services;

public class AppointmentServiceTests
{
    private readonly Mock<IAppointmentRepository> _appointmentRepositoryMock = new();
    private readonly Mock<IAuditLogRepository> _auditRepositoryMock = new();
    private readonly AppointmentService _appointmentService;

    public AppointmentServiceTests()
    {
        _appointmentService = new AppointmentService(_appointmentRepositoryMock.Object, _auditRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateAppointment_WithAvailableTimeSlot_ShouldCreateSuccessfully()
    {
        // Arrange
        var createDto = new CreateAppointmentDto
        {
            PatientId = Guid.NewGuid(),
            HealthProfessionalId = Guid.NewGuid(),
            ScheduledDate = DateTime.UtcNow.AddDays(1),
            DurationMinutes = 30,
            Reason = "Regular checkup"
        };

        _appointmentRepositoryMock
            .Setup(r => r.IsTimeSlotAvailableAsync(createDto.HealthProfessionalId, createDto.ScheduledDate, createDto.DurationMinutes))
            .ReturnsAsync(true);

        _appointmentRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Appointment>())).Returns(Task.CompletedTask);
        _appointmentRepositoryMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);
        _auditRepositoryMock.Setup(a => a.LogAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<string>())).Returns(Task.CompletedTask);

        // Act
        var result = await _appointmentService.CreateAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Regular checkup", result.Reason);
        _appointmentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Appointment>()), Times.Once);
    }

    [Fact]
    public async Task CreateAppointment_WithUnavailableTimeSlot_ShouldThrowException()
    {
        // Arrange
        var createDto = new CreateAppointmentDto
        {
            PatientId = Guid.NewGuid(),
            HealthProfessionalId = Guid.NewGuid(),
            ScheduledDate = DateTime.UtcNow.AddDays(1),
            DurationMinutes = 30,
            Reason = "Regular checkup"
        };

        _appointmentRepositoryMock
            .Setup(r => r.IsTimeSlotAvailableAsync(createDto.HealthProfessionalId, createDto.ScheduledDate, createDto.DurationMinutes))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _appointmentService.CreateAsync(createDto));
        _appointmentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Appointment>()), Times.Never);
    }

    [Fact]
    public async Task GetAppointmentById_WithValidId_ShouldReturnAppointment()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var appointment = new Appointment
        {
            Id = appointmentId,
            PatientId = Guid.NewGuid(),
            HealthProfessionalId = Guid.NewGuid(),
            ScheduledDate = DateTime.UtcNow.AddDays(1),
            DurationMinutes = 30,
            Status = "Scheduled"
        };

        _appointmentRepositoryMock.Setup(r => r.GetByIdAsync(appointmentId)).ReturnsAsync(appointment);

        // Act
        var result = await _appointmentService.GetByIdAsync(appointmentId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Scheduled", result.Status);
    }
}
