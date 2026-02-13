using System;
using System.Threading.Tasks;
using ControlApi.Helpers;
using Core.DTO.Empresa;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace ControlApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EmpresasController : ControllerBase
    {
        private readonly IEmpresasService _empresasService;

        public EmpresasController(IEmpresasService empresasService)
        {
            _empresasService = empresasService;
        }

        private int? GetScopedEmpresaIdForCompany()
        {
            var role = User.GetRole();
            if (string.Equals(role, "COMPANY", StringComparison.OrdinalIgnoreCase))
                return User.GetEmpresaId();
            return null;
        }

        /// <summary>
        /// Criar uma nova empresa
        /// </summary>
        [HttpPost("create")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<EmpresaResponse>> CreateEmpresa([FromBody] CreateEmpresaRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _empresasService.CreateEmpresaAsync(request);
            if (result == null)
                return Conflict("Email ou CNPJ já existe, ou usuário não encontrado");

            return CreatedAtAction(nameof(GetEmpresaById), new { id = result.Id }, result);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<Core.DTO.Empresa.EmpresasPagedDTO>> GetEmpresas(
            [FromQuery] string? name = null,
            [FromQuery] string? status = null,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = "Name",
            [FromQuery] string? sortDirection = "asc")
        {
            var filters = new EmpresaFiltersDTO
            {
                Name = name,
                Search = search,
                Page = page,
                PageSize = pageSize,
                SortBy = sortBy,
                SortDirection = sortDirection
            };

            // Converter string status para enum se fornecido
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<Core.Enums.EmpresaStatus>(status, true, out var statusEnum))
            {
                filters.Status = statusEnum;
            }

            var result = await _empresasService.GetEmpresasPagedAsync(filters);
            return Ok(result);
        }

        /// <summary>
        /// Obter estatísticas das empresas
        /// </summary>
        /// <returns>Estatísticas das empresas</returns>
        [HttpGet("stats")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<EmpresaStats>> GetEmpresaStats()
        {
            var stats = await _empresasService.GetEmpresaStatsAsync();
            return Ok(stats);
        }

        /// <summary>
        /// Buscar empresa por ID
        /// </summary>
        /// <param name="id">ID da empresa</param>
        /// <returns>Dados da empresa</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<EmpresaResponse>> GetEmpresaById(int id)
        {
            var scopedEmpresaId = GetScopedEmpresaIdForCompany();
            if (scopedEmpresaId.HasValue && scopedEmpresaId.Value != id)
                return Forbid();

            var empresa = await _empresasService.GetEmpresaByIdAsync(id);
            if (empresa == null)
                return NotFound("Empresa não encontrada");

            return Ok(empresa);
        }

        /// <summary>
        /// Atualizar uma empresa
        /// </summary>
        /// <param name="id">ID da empresa</param>
        /// <param name="request">Dados para atualização</param>
        /// <returns>Empresa atualizada</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<EmpresaResponse>> UpdateEmpresa(int id, [FromBody] UpdateEmpresaRequest request)
        {
            var scopedEmpresaId = GetScopedEmpresaIdForCompany();
            if (scopedEmpresaId.HasValue && scopedEmpresaId.Value != id)
                return Forbid();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _empresasService.UpdateEmpresaAsync(id, request);
            if (result == null)
                return NotFound("Empresa não encontrada ou email/CNPJ já existe");

            return Ok(result);
        }

        /// <summary>
        /// Deletar uma empresa
        /// </summary>
        /// <param name="id">ID da empresa</param>
        /// <returns>Resultado da operação</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<bool>> DeleteEmpresa(int id)
        {
            var result = await _empresasService.DeleteEmpresaAsync(id);
            if (!result)
                return NotFound("Empresa não encontrada");

            return Ok(true);
        }

        /// <summary>
        /// Alternar status da empresa (Ativo/Inativo)
        /// </summary>
        /// <param name="id">ID da empresa</param>
        /// <returns>Empresa com status atualizado</returns>
        [HttpPatch("{id}/toggle-status")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<EmpresaResponse>> ToggleEmpresaStatus(int id)
        {
            var result = await _empresasService.ToggleEmpresaStatusAsync(id);
            if (result == null)
                return NotFound("Empresa não encontrada");

            return Ok(result);
        }
    }
}
