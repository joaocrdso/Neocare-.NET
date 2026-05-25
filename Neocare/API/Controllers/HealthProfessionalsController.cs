using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neocare.Application.DTOs;
using Neocare.Application.Interfaces;

namespace Neocare.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HealthProfessionalsController : ControllerBase
{
    private readonly IHealthProfessionalService _healthProfessionalService;

    public HealthProfessionalsController(IHealthProfessionalService healthProfessionalService) => 
        _healthProfessionalService = healthProfessionalService;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PaginationQueryDto pagination)
    {
        var (items, total, pages) = await _healthProfessionalService.GetAllAsync(pagination);
        
        var response = new
        {
            data = items,
            _links = new
            {
                self = $"/api/health-professionals?pageNumber={pagination.PageNumber}&pageSize={pagination.PageSize}",
                first = $"/api/health-professionals?pageNumber=1&pageSize={pagination.PageSize}",
                previous = pagination.PageNumber > 1 ? $"/api/health-professionals?pageNumber={pagination.PageNumber - 1}&pageSize={pagination.PageSize}" : null,
                next = pagination.PageNumber < pages ? $"/api/health-professionals?pageNumber={pagination.PageNumber + 1}&pageSize={pagination.PageSize}" : null,
                last = $"/api/health-professionals?pageNumber={pages}&pageSize={pagination.PageSize}"
            },
            pagination = new { pageNumber = pagination.PageNumber, pageSize = pagination.PageSize, total, pages }
        };

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var professional = await _healthProfessionalService.GetByIdAsync(id);
        if (professional == null) return NotFound();

        var response = new
        {
            data = professional,
            _links = new
            {
                self = $"/api/health-professionals/{id}",
                update = new { href = $"/api/health-professionals/{id}", method = "PUT" },
                delete = new { href = $"/api/health-professionals/{id}", method = "DELETE" }
            }
        };

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateHealthProfessionalDto dto)
    {
        var professional = await _healthProfessionalService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = professional.Id }, professional);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateHealthProfessionalDto dto)
    {
        try
        {
            var professional = await _healthProfessionalService.UpdateAsync(id, dto);
            return Ok(professional);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _healthProfessionalService.DeleteAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
