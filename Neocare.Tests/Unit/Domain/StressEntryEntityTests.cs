using Xunit;
using Neocare.Domain.Entities;
using FluentAssertions;

namespace Neocare.Tests.Unit.Domain
{
    public class StressEntryEntityTests
    {
        [Fact]
        public void StressEntry_Creation_InitializesAllProperties()
        {
            // Arrange
            var id = Guid.NewGuid();
            var stressLevel = 8;
            var description = "Test stress entry";
            var symptoms = new List<string> { "Insônia", "Ansiedade" };
            var recordedAt = DateTime.UtcNow;
            var userId = "user123";

            // Act
            var stressEntry = new StressEntry
            {
                Id = id,
                StressLevel = stressLevel,
                Description = description,
                Symptoms = symptoms,
                RecordedAt = recordedAt,
                UserId = userId
            };

            // Assert
            stressEntry.Id.Should().Be(id);
            stressEntry.StressLevel.Should().Be(stressLevel);
            stressEntry.Description.Should().Be(description);
            stressEntry.Symptoms.Should().HaveCount(2);
            stressEntry.RecordedAt.Should().Be(recordedAt);
            stressEntry.UserId.Should().Be(userId);
        }

        [Fact]
        public void StressEntry_WithDefaultValues_InitializesCorrectly()
        {
            // Act
            var stressEntry = new StressEntry();

            // Assert
            stressEntry.Id.Should().Be(default(Guid));
            stressEntry.StressLevel.Should().Be(0);
            stressEntry.Description.Should().Be(string.Empty);
            stressEntry.Symptoms.Should().BeEmpty();
            stressEntry.UserId.Should().Be(string.Empty);
        }

        [Fact]
        public void StressEntry_SymptomsList_IsModifiable()
        {
            // Arrange
            var stressEntry = new StressEntry();

            // Act
            stressEntry.Symptoms.Add("Insônia");
            stressEntry.Symptoms.Add("Fadiga");

            // Assert
            stressEntry.Symptoms.Should().HaveCount(2);
            stressEntry.Symptoms.Should().Contain("Insônia");
            stressEntry.Symptoms.Should().Contain("Fadiga");
        }

        [Fact]
        public void StressEntry_WithValidStressLevel_IsCreated()
        {
            // Arrange & Act
            var stressEntry = new StressEntry { StressLevel = 10 };

            // Assert
            stressEntry.StressLevel.Should().Be(10);
        }
    }
}
