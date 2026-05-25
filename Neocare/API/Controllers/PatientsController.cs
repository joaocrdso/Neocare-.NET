using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neocare.Application.DTOs;
using Neocare.Application.Interfaces;

namespace Neocare.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _patientService;

    public PatientsController(IPatientService patientService) => _patientService = patientService;

    /// <summary>Lista todos os pacientes com paginação e filtros.</summary>
    /// <param name="pagination">Parâmetros de paginação e ordenação.</param>
    /// <param name="name">Filtro opcional por nome do paciente.</param>
    /// <param name="status">Filtro opcional por status do paciente.</param>
    /// <returns>Lista paginada de pacientes com links HATEOAS.</returns>
    /// <response code="200">Retorna a lista paginada de pacientes.</response>
    /// <response code="401">Não autenticado.</response>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PaginationQueryDto pagination, [FromQuery] string? name, [FromQuery] string? status)
    {
        var (items, total, pages) = await _patientService.GetAllAsync(pagination, name, status);
        
        var response = new
        {
            data = items,
            _links = new
            {
                self = $"/api/patients?pageNumber={pagination.PageNumber}&pageSize={pagination.PageSize}&orderBy={pagination.OrderBy}&orderDirection={pagination.OrderDirection}",
                first = $"/api/patients?pageNumber=1&pageSize={pagination.PageSize}",
                previous = pagination.PageNumber > 1 ? $"/api/patients?pageNumber={pagination.PageNumber - 1}&pageSize={pagination.PageSize}" : null,
                next = pagination.PageNumber < pages ? $"/api/patients?pageNumber={pagination.PageNumber + 1}&pageSize={pagination.PageSize}" : null,
                last = $"/api/patients?pageNumber={pages}&pageSize={pagination.PageSize}"
            },
            pagination = new { pageNumber = pagination.PageNumber, pageSize = pagination.PageSize, total, pages }
        };

        return Ok(response);
    }

    /// <summary>Obtém um paciente por ID.</summary>
    /// <param name="id">ID do paciente.</param>
    /// <returns>Dados do paciente com links HATEOAS.</returns>
    /// <response code="200">Retorna os dados do paciente.</response>
    /// <response code="401">Não autenticado.</response>
    /// <response code="404">Paciente não encontrado.</response>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var patient = await _patientService.GetByIdAsync(id);
        if (patient == null) return NotFound();

        var response = new
        {
            data = patient,
            _links = new
            {
                self = $"/api/patients/{id}",
                update = new { href = $"/api/patients/{id}", method = "PUT" },
                delete = new { href = $"/api/patients/{id}", method = "DELETE" }
            }
        };

        return Ok(response);
    }

    /// <summary>Cria um novo paciente.</summary>
    /// <param name="dto">Dados do paciente a ser criado.</param>
    /// <returns>Dados do paciente criado.</returns>
    /// <response code="201">Paciente criado com sucesso.</response>
    /// <response code="400">Requisição inválida.</response>
    /// <response code="401">Não autenticado.</response>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePatientDto dto)
    {
        var patient = await _patientService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = patient.Id }, patient);
    }

    /// <summary>Atualiza um paciente existente.</summary>
    /// <param name="id">ID do paciente a ser atualizado.</param>
    /// <param name="dto">Dados atualizados do paciente.</param>
    /// <returns>Dados do paciente atualizado.</returns>
    /// <response code="200">Paciente atualizado com sucesso.</response>
    /// <response code="400">Requisição inválida.</response>
    /// <response code="401">Não autenticado.</response>
    /// <response code="404">Paciente não encontrado.</response>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreatePatientDto dto)
    {
        try
        {
            var patient = await _patientService.UpdateAsync(id, dto);
            return Ok(patient);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>Deleta um paciente.</summary>
    /// <param name="id">ID do paciente a ser deletado.</param>
    /// <returns>Sem conteúdo.</returns>
    /// <response code="204">Paciente deletado com sucesso.</response>
    /// <response code="401">Não autenticado.</response>
    /// <response code="404">Paciente não encontrado.</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _patientService.DeleteAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
