using Xunit;
using Moq;
using Neocare.Application.DTOs;
using Neocare.Application.Services;
using Neocare.Domain.Entities;
using Neocare.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using FluentAssertions;

namespace Neocare.Tests.Unit.Services
{
    public class StressEntryServiceTests
    {
        private readonly Mock<IStressEntryRepository> _repositoryMock;
        private readonly IMemoryCache _cache;
        private readonly StressEntryService _service;

        public StressEntryServiceTests()
        {
            _repositoryMock = new Mock<IStressEntryRepository>();
            _cache = new MemoryCache(new MemoryCacheOptions());
            _service = new StressEntryService(_repositoryMock.Object, _cache);
        }

        // TESTES UNITÁRIOS - PADRÃO AAA

        #region SearchStressEntries
        [Fact]
        public async Task SearchStressEntries_WithValidParams_ReturnsResults()
        {
            // Arrange
            var searchParams = new SearchParams { Page = 1, PageSize = 10 };
            var stressEntries = new List<StressEntry>
            {
                new StressEntry { Id = Guid.NewGuid(), StressLevel = 5, Description = "Teste 1", RecordedAt = DateTime.UtcNow },
                new StressEntry { Id = Guid.NewGuid(), StressLevel = 8, Description = "Teste 2", RecordedAt = DateTime.UtcNow }
            };
            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(stressEntries);

            // Act
            var result = await _service.SearchStressEntries(searchParams);

            // Assert
            result.Items.Should().HaveCount(2);
            result.TotalItems.Should().Be(2);
            _repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task SearchStressEntries_WithMinStressLevel_FiltersCorrectly()
        {
            // Arrange
            var searchParams = new SearchParams { Page = 1, PageSize = 10, MinStressLevel = 7 };
            var stressEntries = new List<StressEntry>
            {
                new StressEntry { Id = Guid.NewGuid(), StressLevel = 5, Description = "Baixo", RecordedAt = DateTime.UtcNow },
                new StressEntry { Id = Guid.NewGuid(), StressLevel = 8, Description = "Alto", RecordedAt = DateTime.UtcNow }
            };
            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(stressEntries);

            // Act
            var result = await _service.SearchStressEntries(searchParams);

            // Assert
            result.Items.Should().HaveCount(1);
            result.Items.First().StressLevel.Should().Be(8);
        }

        [Fact]
        public async Task SearchStressEntries_WithSearchTerm_FiltersByDescription()
        {
            // Arrange
            var searchParams = new SearchParams { Page = 1, PageSize = 10, SearchTerm = "insônia" };
            var stressEntries = new List<StressEntry>
            {
                new StressEntry { Id = Guid.NewGuid(), StressLevel = 5, Description = "Insônia", RecordedAt = DateTime.UtcNow },
                new StressEntry { Id = Guid.NewGuid(), StressLevel = 8, Description = "Ansiedade", RecordedAt = DateTime.UtcNow }
            };
            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(stressEntries);

            // Act
            var result = await _service.SearchStressEntries(searchParams);

            // Assert
            result.Items.Should().HaveCount(1);
            result.Items.First().Description.Should().Contain("Insônia");
        }

        [Fact]
        public async Task SearchStressEntries_SecondCall_UsesCachedResult()
        {
            // Arrange
            var searchParams = new SearchParams { Page = 1, PageSize = 10 };
            var stressEntries = new List<StressEntry>
            {
                new StressEntry { Id = Guid.NewGuid(), StressLevel = 5, Description = "Teste", RecordedAt = DateTime.UtcNow }
            };
            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(stressEntries);

            // Act
            await _service.SearchStressEntries(searchParams);
            var cachedResult = await _service.SearchStressEntries(searchParams);

            // Assert
            cachedResult.Items.Should().HaveCount(1);
            _repositoryMock.Verify(r => r.GetAllAsync(), Times.Once); // Deve ser chamado apenas uma vez
        }
        #endregion

        #region GetByIdAsync
        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsStressEntry()
        {
            // Arrange
            var id = Guid.NewGuid();
            var stressEntry = new StressEntry 
            { 
                Id = id, 
                StressLevel = 7, 
                Description = "Teste", 
                RecordedAt = DateTime.UtcNow,
                UserId = "user123"
            };
            _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(stressEntry);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(id);
            result.StressLevel.Should().Be(7);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ThrowsKeyNotFoundException()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((StressEntry?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetByIdAsync(id));
        }
        #endregion

        #region CreateAsync
        [Fact]
        public async Task CreateAsync_WithValidData_CreatesStressEntry()
        {
            // Arrange
            var createDto = new CreateStressEntryDto
            {
                StressLevel = 8,
                Description = "Novo registro",
                Symptoms = new List<string> { "Insônia", "Ansiedade" },
                UserId = "user123"
            };
            var createdEntry = new StressEntry
            {
                Id = Guid.NewGuid(),
                StressLevel = createDto.StressLevel,
                Description = createDto.Description,
                Symptoms = createDto.Symptoms,
                RecordedAt = DateTime.UtcNow,
                UserId = createDto.UserId
            };
            _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<StressEntry>())).ReturnsAsync(createdEntry);

            // Act
            var result = await _service.CreateAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            result.StressLevel.Should().Be(8);
            result.Description.Should().Be("Novo registro");
            _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<StressEntry>()), Times.Once);
        }
        #endregion

        #region UpdateStressEntry
        [Fact]
        public async Task UpdateStressEntry_WithValidData_UpdatesSuccessfully()
        {
            // Arrange
            var id = Guid.NewGuid();
            var existingEntry = new StressEntry
            {
                Id = id,
                StressLevel = 5,
                Description = "Original",
                Symptoms = new List<string> { "Original" },
                RecordedAt = DateTime.UtcNow,
                UserId = "user123"
            };
            var updateDto = new StressEntryDto
            {
                Id = id,
                StressLevel = 8,
                Description = "Atualizado",
                Symptoms = new List<string> { "Atualizado" }
            };
            _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existingEntry);
            _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<StressEntry>())).ReturnsAsync(existingEntry);

            // Act
            var result = await _service.UpdateStressEntry(id, updateDto);

            // Assert
            result.Should().NotBeNull();
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<StressEntry>()), Times.Once);
        }

        [Fact]
        public async Task UpdateStressEntry_WithInvalidId_ReturnsNull()
        {
            // Arrange
            var id = Guid.NewGuid();
            var updateDto = new StressEntryDto { Id = id, StressLevel = 8 };
            _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((StressEntry?)null);

            // Act
            var result = await _service.UpdateStressEntry(id, updateDto);

            // Assert
            result.Should().BeNull();
        }
        #endregion

        #region DeleteStressEntry
        [Fact]
        public async Task DeleteStressEntry_WithValidId_DeletesSuccessfully()
        {
            // Arrange
            var id = Guid.NewGuid();
            var existingEntry = new StressEntry
            {
                Id = id,
                StressLevel = 7,
                Description = "Teste",
                RecordedAt = DateTime.UtcNow,
                UserId = "user123"
            };
            _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existingEntry);
            _repositoryMock.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);

            // Act
            var result = await _service.DeleteStressEntry(id);

            // Assert
            result.Should().BeTrue();
            _repositoryMock.Verify(r => r.DeleteAsync(id), Times.Once);
        }

        [Fact]
        public async Task DeleteStressEntry_WithInvalidId_ReturnsFalse()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((StressEntry?)null);

            // Act
            var result = await _service.DeleteStressEntry(id);

            // Assert
            result.Should().BeFalse();
        }
        #endregion
    }
}
