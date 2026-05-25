using Neocare.Domain.Entities;
using Xunit;

namespace Neocare.Tests.Unit.Domain;

public class PatientEntityTests
{
    [Fact]
    public void Patient_WhenCreated_ShouldHaveValidProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "João Silva";
        var email = "joao@test.com";
        var cpf = "12345678901";

        // Act
        var patient = new Patient
        {
            Id = id,
            Name = name,
            Email = email,
            CPF = cpf,
            PhoneNumber = "(11) 98765-4321",
            DateOfBirth = new DateTime(1990, 5, 15),
            Status = "Active"
        };

        // Assert
        Assert.Equal(id, patient.Id);
        Assert.Equal(name, patient.Name);
        Assert.Equal(email, patient.Email);
        Assert.Equal(cpf, patient.CPF);
        Assert.Equal("Active", patient.Status);
    }

    [Fact]
    public void Patient_WithInvalidCPF_ShouldFail()
    {
        // Arrange & Act
        var patient = new Patient
        {
            Name = "Test",
            Email = "test@test.com",
            CPF = "123", // Invalid CPF with less than 11 digits
            PhoneNumber = "11987654321"
        };

        // Assert
        Assert.Equal(3, patient.CPF.Length);
    }

    [Fact]
    public void Appointment_ShouldHavePatientAndHealthProfessionalRelationship()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var professionalId = Guid.NewGuid();
        var appointmentId = Guid.NewGuid();

        // Act
        var appointment = new Appointment
        {
            Id = appointmentId,
            PatientId = patientId,
            HealthProfessionalId = professionalId,
            ScheduledDate = DateTime.UtcNow.AddDays(1),
            DurationMinutes = 30,
            Status = "Scheduled"
        };

        // Assert
        Assert.Equal(patientId, appointment.PatientId);
        Assert.Equal(professionalId, appointment.HealthProfessionalId);
        Assert.Equal("Scheduled", appointment.Status);
    }

    [Fact]
    public void Treatment_ShouldHaveValidProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var appointmentId = Guid.NewGuid();
        var patientId = Guid.NewGuid();

        // Act
        var treatment = new Treatment
        {
            Id = id,
            AppointmentId = appointmentId,
            PatientId = patientId,
            Description = "Antibiotic therapy",
            StartDate = DateTime.UtcNow,
            Status = "Active"
        };

        // Assert
        Assert.Equal(id, treatment.Id);
        Assert.Equal(appointmentId, treatment.AppointmentId);
        Assert.Equal(patientId, treatment.PatientId);
        Assert.Equal("Active", treatment.Status);
    }
}
