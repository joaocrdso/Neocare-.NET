using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neocare.Application.DTOs;
using Neocare.Application.Interfaces;

namespace Neocare.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TreatmentsController : ControllerBase
{
    private readonly ITreatmentService _treatmentService;

    public TreatmentsController(ITreatmentService treatmentService) => 
        _treatmentService = treatmentService;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PaginationQueryDto pagination)
    {
        var (items, total, pages) = await _treatmentService.GetAllAsync(pagination);
        
        var response = new
        {
            data = items,
            _links = new
            {
                self = $"/api/treatments?pageNumber={pagination.PageNumber}&pageSize={pagination.PageSize}",
                first = $"/api/treatments?pageNumber=1&pageSize={pagination.PageSize}",
                previous = pagination.PageNumber > 1 ? $"/api/treatments?pageNumber={pagination.PageNumber - 1}&pageSize={pagination.PageSize}" : null,
                next = pagination.PageNumber < pages ? $"/api/treatments?pageNumber={pagination.PageNumber + 1}&pageSize={pagination.PageSize}" : null,
                last = $"/api/treatments?pageNumber={pages}&pageSize={pagination.PageSize}"
            },
            pagination = new { pageNumber = pagination.PageNumber, pageSize = pagination.PageSize, total, pages }
        };

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var treatment = await _treatmentService.GetByIdAsync(id);
        if (treatment == null) return NotFound();

        var response = new
        {
            data = treatment,
            _links = new
            {
                self = $"/api/treatments/{id}",
                update = new { href = $"/api/treatments/{id}", method = "PUT" },
                delete = new { href = $"/api/treatments/{id}", method = "DELETE" }
            }
        };

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTreatmentDto dto)
    {
        try
        {
            var treatment = await _treatmentService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = treatment.Id }, treatment);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateTreatmentDto dto)
    {
        try
        {
            var treatment = await _treatmentService.UpdateAsync(id, dto);
            return Ok(treatment);
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
            await _treatmentService.DeleteAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
