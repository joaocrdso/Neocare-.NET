using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neocare.Application.DTOs;
using Neocare.Application.Interfaces;

namespace Neocare.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentsController(IAppointmentService appointmentService) => 
        _appointmentService = appointmentService;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PaginationQueryDto pagination)
    {
        var (items, total, pages) = await _appointmentService.GetAllAsync(pagination);
        
        var response = new
        {
            data = items,
            _links = new
            {
                self = $"/api/appointments?pageNumber={pagination.PageNumber}&pageSize={pagination.PageSize}",
                first = $"/api/appointments?pageNumber=1&pageSize={pagination.PageSize}",
                previous = pagination.PageNumber > 1 ? $"/api/appointments?pageNumber={pagination.PageNumber - 1}&pageSize={pagination.PageSize}" : null,
                next = pagination.PageNumber < pages ? $"/api/appointments?pageNumber={pagination.PageNumber + 1}&pageSize={pagination.PageSize}" : null,
                last = $"/api/appointments?pageNumber={pages}&pageSize={pagination.PageSize}"
            },
            pagination = new { pageNumber = pagination.PageNumber, pageSize = pagination.PageSize, total, pages }
        };

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var appointment = await _appointmentService.GetByIdAsync(id);
        if (appointment == null) return NotFound();

        var response = new
        {
            data = appointment,
            _links = new
            {
                self = $"/api/appointments/{id}",
                update = new { href = $"/api/appointments/{id}", method = "PUT" },
                delete = new { href = $"/api/appointments/{id}", method = "DELETE" }
            }
        };

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAppointmentDto dto)
    {
        try
        {
            var appointment = await _appointmentService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, appointment);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateAppointmentDto dto)
    {
        try
        {
            var appointment = await _appointmentService.UpdateAsync(id, dto);
            return Ok(appointment);
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
            await _appointmentService.DeleteAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
