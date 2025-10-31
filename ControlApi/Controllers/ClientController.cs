using Core.DTO.Client;
using Core.DTO.Diet;
using Core.DTO.Workout;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.Threading.Tasks;

namespace ControlApi.Controllers
{
    /// <summary>
    /// Controller para gerenciamento de clientes
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        /// <summary>
        /// Lista clientes com filtros opcionais
        /// </summary>
        /// <param name="status">Status do cliente (active, inactive, paused)</param>
        /// <param name="kanbanStage">Estágio no kanban do CRM (lead, prospect, qualified, etc.)</param>
        /// <param name="goalType">Tipo de objetivo</param>
        /// <param name="activityLevel">Nível de atividade (sedentary, light, moderate, active, very_active)</param>
        /// <param name="planId">ID do plano associado</param>
        /// <param name="empresaId">ID da empresa</param>
        /// <param name="search">Busca por nome ou email</param>
        /// <param name="page">Página (padrão: 1)</param>
        /// <param name="pageSize">Itens por página (padrão: 20)</param>
        /// <param name="orderBy">Campo para ordenação (padrão: name)</param>
        /// <param name="orderDirection">Direção da ordenação (asc/desc, padrão: asc)</param>
        /// <returns>Lista paginada de clientes</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ClientsPagedDTO), 200)]
        public async Task<ActionResult<ClientsPagedDTO>> GetClients(
            [FromQuery] string? status = null,
            [FromQuery] string? kanbanStage = null,
            [FromQuery] string? goalType = null,
            [FromQuery] string? activityLevel = null,
            [FromQuery] string? planId = null,
            [FromQuery] int? empresaId = null,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? orderBy = "name",
            [FromQuery] string? orderDirection = "asc")
        {
            var filters = new ClientFiltersDTO
            {
                Status = status,
                KanbanStage = kanbanStage,
                GoalType = goalType,
                ActivityLevel = activityLevel,
                PlanId = planId,
                EmpresaId = empresaId,
                Search = search,
                Page = page,
                PageSize = pageSize,
                OrderBy = orderBy,
                OrderDirection = orderDirection
            };

            var result = await _clientService.GetClientsPagedAsync(filters);
            return Ok(result);
        }

        /// <summary>
        /// Busca cliente por ID
        /// </summary>
        /// <param name="id">ID do cliente</param>
        /// <returns>Dados do cliente</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ClientResponse), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ClientResponse>> GetById(int id)
        {
            var client = await _clientService.GetClientByIdAsync(id);
            if (client == null)
                return NotFound(new { message = "Cliente não encontrado" });

            return Ok(client);
        }

        /// <summary>
        /// Cria um novo cliente
        /// </summary>
        /// <param name="request">Dados do cliente</param>
        /// <returns>Cliente criado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ClientResponse), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        public async Task<ActionResult<ClientResponse>> Create([FromBody] CreateClientRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var client = await _clientService.CreateClientAsync(request);
            if (client == null)
                return Conflict(new { message = "Email já está em uso por outro cliente" });

            return CreatedAtAction(nameof(GetById), new { id = client.Id }, client);
        }

        /// <summary>
        /// Atualiza um cliente existente
        /// </summary>
        /// <param name="id">ID do cliente</param>
        /// <param name="request">Dados para atualização</param>
        /// <returns>Resultado da operação</returns>
        [HttpPut("{id:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<ActionResult<bool>> Update(int id, [FromBody] UpdateClientRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _clientService.UpdateClientAsync(id, request);
            if (!result)
                return NotFound(new { message = "Cliente não encontrado ou email já está em uso" });

            return Ok(true);
        }

        /// <summary>
        /// Remove um cliente
        /// </summary>
        /// <param name="id">ID do cliente</param>
        /// <returns>Resultado da operação</returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var result = await _clientService.DeleteClientAsync(id);
            if (!result)
                return NotFound(new { message = "Cliente não encontrado" });

            return Ok(true);
        }

        /// <summary>
        /// Adiciona progresso de peso para um cliente
        /// </summary>
        /// <param name="clientId">ID do cliente</param>
        /// <param name="request">Dados do progresso de peso</param>
        /// <returns>Registro de progresso criado</returns>
        [HttpPost("{clientId:int}/progress/weight")]
        [ProducesResponseType(typeof(ClientMeasurementDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ClientMeasurementDTO>> AddWeightProgress(int clientId, [FromBody] AddWeightProgressRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _clientService.AddWeightProgressAsync(clientId, request);
            if (result == null)
                return NotFound(new { message = "Cliente não encontrado" });

            return CreatedAtAction(nameof(GetById), new { id = clientId }, result);
        }

        /// <summary>
        /// Adiciona medidas corporais para um cliente
        /// </summary>
        /// <param name="clientId">ID do cliente</param>
        /// <param name="request">Dados das medidas corporais</param>
        /// <returns>Registro de medidas criado</returns>
        [HttpPost("{clientId:int}/progress/measurements")]
        [ProducesResponseType(typeof(ClientMeasurementDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ClientMeasurementDTO>> AddMeasurementsProgress(int clientId, [FromBody] AddMeasurementsProgressRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _clientService.AddMeasurementsProgressAsync(clientId, request);
            if (result == null)
                return NotFound(new { message = "Cliente não encontrado" });

            return CreatedAtAction(nameof(GetById), new { id = clientId }, result);
        }

        /// <summary>
        /// Adiciona fotos de progresso para um cliente
        /// </summary>
        /// <param name="clientId">ID do cliente</param>
        /// <param name="request">Dados da foto de progresso</param>
        /// <returns>Registro de foto criado</returns>
        [HttpPost("{clientId:int}/progress/photos")]
        [ProducesResponseType(typeof(AchievementDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<AchievementDTO>> AddPhotoProgress(int clientId, [FromBody] AddPhotoProgressRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _clientService.AddPhotoProgressAsync(clientId, request);
            if (result == null)
                return NotFound(new { message = "Cliente não encontrado" });

            return CreatedAtAction(nameof(GetById), new { id = clientId }, result);
        }

        /// <summary>
        /// Adiciona uma conquista para um cliente
        /// </summary>
        /// <param name="clientId">ID do cliente</param>
        /// <param name="request">Dados da conquista</param>
        /// <returns>Conquista criada</returns>
        [HttpPost("{clientId:int}/achievements")]
        [ProducesResponseType(typeof(AchievementDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<AchievementDTO>> AddAchievement(int clientId, [FromBody] AddAchievementRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _clientService.AddAchievementAsync(clientId, request);
            if (result == null)
                return NotFound(new { message = "Cliente não encontrado" });

            return CreatedAtAction(nameof(GetById), new { id = clientId }, result);
        }

        /// <summary>
        /// Obtém estatísticas dos clientes
        /// </summary>
        /// <returns>Estatísticas agregadas</returns>
        [HttpGet("stats")]
        [ProducesResponseType(typeof(ClientStatsDTO), 200)]
        public async Task<ActionResult<ClientStatsDTO>> GetStats()
        {
            var stats = await _clientService.GetClientStatsAsync();
            return Ok(stats);
        }
        /// <summary>
        /// Obtém a dieta atual do cliente
        /// </summary>
        [HttpGet("{id:int}/diet/current")]
        [ProducesResponseType(typeof(DietSummaryDTO), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<DietSummaryDTO>> GetCurrentDiet([FromRoute] int id)
        {
            var diet = await _clientService.GetCurrentDietAsync(id);
            return Ok(diet);
        }


        /// <summary>
        /// Obtém o treino atual do cliente
        /// </summary>
        [HttpGet("{id:int}/workout/current")]
        [ProducesResponseType(typeof(WorkoutSummaryDTO), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<WorkoutSummaryDTO>> GetCurrentWorkout([FromRoute] int id)
        {
            var workout = await _clientService.GetCurrentWorkoutAsync(id);
            return Ok(workout);
        }


        /// <summary>
        /// Obtém informações básicas do cliente (sem coleções pesadas), separado da dieta
        /// </summary>
        [HttpGet("{id:int}/basic")]
        [ProducesResponseType(typeof(ClientBasicDTO), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ClientBasicDTO>> GetBasic([FromRoute] int id)
        {
            var c = await _clientService.GetClientBasicByIdAsync(id);
            if (c == null) return NotFound();
            return Ok(c);
        }


        /// <summary>
        /// Histórico de dietas do cliente (mais recentes primeiro)
        /// </summary>
        [HttpGet("{id:int}/diet/history")]
        [ProducesResponseType(typeof(IEnumerable<DietSummaryDTO>), 200)]
        public async Task<ActionResult<IEnumerable<DietSummaryDTO>>> GetDietHistory([FromRoute] int id)
        {
            var items = await _clientService.GetDietHistoryAsync(id);
            return Ok(items);
        }


        /// <summary>
        /// Histórico de treinos do cliente (mais recentes primeiro)
        /// </summary>
        [HttpGet("{id:int}/workout/history")]
        [ProducesResponseType(typeof(IEnumerable<WorkoutSummaryDTO>), 200)]
        public async Task<ActionResult<IEnumerable<WorkoutSummaryDTO>>> GetWorkoutHistory([FromRoute] int id)
        {
            var items = await _clientService.GetWorkoutHistoryAsync(id);
            return Ok(items);
        }


        /// <summary>
        /// Lista as conquistas do cliente (paginado)
        /// </summary>
        /// <param name="clientId">ID do cliente</param>
        /// <param name="page">Página (>=1)</param>
        /// <param name="pageSize">Tamanho da página (>=1)</param>
        /// <returns>Lista de achievements</returns>
        [HttpGet("{clientId:int}/achievements")]
        [ProducesResponseType(typeof(IEnumerable<AchievementDTO>), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<IEnumerable<AchievementDTO>>> GetAchievements(
            int clientId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest(new { message = "page e pageSize devem ser >= 1" });

            var list = await _clientService.GetAchievementsAsync(clientId, page, pageSize);
            return Ok(list);
        }

}
}
