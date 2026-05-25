using Neocare.Application.DTOs;
using Neocare.Domain.Entities;
using Neocare.Domain.Exceptions;
using Neocare.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Linq.Expressions;

namespace Neocare.Application.Services
{
    public class StressEntryService
    {
        private readonly IStressEntryRepository _repository;
        private readonly IMemoryCache _cache;
        private const string CacheKeyPrefix = "StressEntries_";

        public StressEntryService(IStressEntryRepository repository, IMemoryCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<SearchResult<StressEntryDto>> SearchStressEntries(SearchParams searchParams)
        {
            var cacheKey = $"{CacheKeyPrefix}{searchParams.GetHashCode()}";
            
            if (_cache.TryGetValue(cacheKey, out SearchResult<StressEntryDto> cachedResult))
            {
                return cachedResult;
            }

            var entries = await _repository.GetAllAsync();
            
            // Aplicar filtros
            var query = entries.AsQueryable();

            if (searchParams.MinStressLevel.HasValue)
                query = query.Where(e => e.StressLevel >= searchParams.MinStressLevel.Value);

            if (searchParams.MaxStressLevel.HasValue)
                query = query.Where(e => e.StressLevel <= searchParams.MaxStressLevel.Value);

            if (searchParams.FromDate.HasValue)
                query = query.Where(e => e.RecordedAt >= searchParams.FromDate.Value);

            if (searchParams.ToDate.HasValue)
                query = query.Where(e => e.RecordedAt <= searchParams.ToDate.Value);

            if (!string.IsNullOrWhiteSpace(searchParams.SearchTerm))
                query = query.Where(e => 
                    e.Description.Contains(searchParams.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    e.Symptoms.Any(s => s.Contains(searchParams.SearchTerm, StringComparison.OrdinalIgnoreCase)));

            // Ordenação
            query = searchParams.SortBy.ToLower() switch
            {
                "date" => searchParams.SortDirection == "desc" 
                            ? query.OrderByDescending(e => e.RecordedAt)
                            : query.OrderBy(e => e.RecordedAt),
                "level" => searchParams.SortDirection == "desc"
                            ? query.OrderByDescending(e => e.StressLevel)
                            : query.OrderBy(e => e.StressLevel),
                _ => query.OrderByDescending(e => e.RecordedAt)
            };

            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)searchParams.PageSize);

            var items = query
                .Skip((searchParams.Page - 1) * searchParams.PageSize)
                .Take(searchParams.PageSize)
                .Select(MapToDto)
                .ToList();

            var result = new SearchResult<StressEntryDto>
            {
                Items = items,
                TotalCount = totalItems,
                TotalPages = totalPages,
                CurrentPage = searchParams.Page
            };

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5));
            
            _cache.Set(cacheKey, result, cacheOptions);

            return result;
        }

        public async Task<StressEntryDto> GetByIdAsync(Guid id)
        {
            var entry = await _repository.GetByIdAsync(id);
            if (entry == null)
                throw new NotFoundException("StressEntry", id);

            return MapToDto(entry);
        }
        

        public async Task<IEnumerable<StressEntryDto>> GetByUserIdAsync(string userId)
        {
            var entries = await _repository.GetByUserIdAsync(userId);
            return entries.Select(MapToDto);
        }

        public async Task<StressEntryDto> CreateAsync(CreateStressEntryDto createDto)
        {
            var entry = new StressEntry
            {
                Id = Guid.NewGuid(),
                StressLevel = createDto.StressLevel,
                Description = createDto.Description,
                Symptoms = createDto.Symptoms,
                RecordedAt = DateTime.UtcNow,
                UserId = createDto.UserId
            };

            var created = await _repository.CreateAsync(entry);
            _cache.Remove(CacheKeyPrefix);
            return MapToDto(created);
        }

        public async Task<StressEntryDto> UpdateStressEntry(Guid id, StressEntryDto entryDto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                return null;

            existing.StressLevel = entryDto.StressLevel;
            existing.Description = entryDto.Description;
            existing.Symptoms = entryDto.Symptoms;

            var updated = await _repository.UpdateAsync(existing);
            _cache.Remove(CacheKeyPrefix);
            return MapToDto(updated);
        }

        public async Task<bool> DeleteStressEntry(Guid id)
        {
            var entry = await _repository.GetByIdAsync(id);
            if (entry == null)
                return false;

            await _repository.DeleteAsync(id);
            _cache.Remove(CacheKeyPrefix);
            return true;
        }

        private static StressEntryDto MapToDto(StressEntry entry)
        {
            return new StressEntryDto
            {
                Id = entry.Id,
                StressLevel = entry.StressLevel,
                Description = entry.Description,
                Symptoms = entry.Symptoms,
                RecordedAt = entry.RecordedAt,
                UserId = entry.UserId
            };
        }
    }
}