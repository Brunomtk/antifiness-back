using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Core.DTO.User;
using Services;
using System.ComponentModel.DataAnnotations;
using Core.Enums.User;

namespace ControlApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Autentica um usuário com email e senha
        /// </summary>
        /// <param name="request">Dados de autenticação</param>
        /// <returns>Token JWT e refresh token</returns>
        [HttpPost("authenticate")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate([FromBody] UserAuthenticateRequest request)
        {
            try
            {
                var token = await _userService.AuthenticateAsync(request);
                return Ok(token);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        
        /// <summary>
        /// Renova o token de acesso usando um refresh token válido.
        /// </summary>
        /// <param name="request">Refresh token</param>
        /// <returns>Novo par de tokens (access + refresh)</returns>
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var token = await _userService.RefreshTokenAsync(request);
                return Ok(token);
            }
            catch (SecurityTokenException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

/// <summary>
        /// Registra um novo usuário (público)
        /// </summary>
        /// <param name="request">Dados do usuário</param>
        /// <returns>Sucesso ou falha na criação</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequest request)
        {
            try
            {
                var createRequest = new CreateUserRequest
                {
                    Name = request.Name,
                    Username = request.Username,
                    Email = request.Email,
                    Password = request.Password,
                    Type = UserType.Client, // Usuários registrados são sempre clientes
                    Status = UserStatus.Active,
                    Phone = request.Phone,
                    Avatar = request.Avatar
                };

                var result = await _userService.CreateUserAsync(createRequest);
                return Ok(new { success = result, message = "Usuário registrado com sucesso" });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Cria um novo usuário (admin apenas)
        /// </summary>
        /// <param name="request">Dados do usuário</param>
        /// <returns>Sucesso ou falha na criação</returns>
        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            try
            {
                var result = await _userService.CreateUserAsync(request);
                return Ok(new { success = result, message = "Usuário criado com sucesso" });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Lista todos os usuários com filtros opcionais
        /// </summary>
        /// <param name="role">Filtro por role (admin/client)</param>
        /// <param name="status">Filtro por status (active/inactive/pending)</param>
        /// <param name="search">Busca por nome ou email</param>
        /// <returns>Lista de usuários</returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUsers(
            [FromQuery] string? role = null,
            [FromQuery] string? status = null,
            [FromQuery] string? search = null)
        {
            try
            {
                var users = await _userService.GetAllUsersAsync(role, status, search);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Busca um usuário por ID
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <returns>Dados do usuário</returns>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUserById([FromRoute] int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                    return NotFound(new { message = "Usuário não encontrado" });

                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza um usuário existente
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <param name="request">Dados para atualização</param>
        /// <returns>Sucesso ou falha na atualização</returns>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromRoute] int id, [FromBody] UpdateUserRequest request)
        {
            try
            {
                var result = await _userService.UpdateUserAsync(id, request);
                if (!result)
                    return NotFound(new { message = "Usuário não encontrado" });

                return Ok(new { success = result, message = "Usuário atualizado com sucesso" });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Remove um usuário
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <returns>Sucesso ou falha na remoção</returns>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            try
            {
                var result = await _userService.DeleteUserAsync(id);
                if (!result)
                    return NotFound(new { message = "Usuário não encontrado" });

                return Ok(new { success = result, message = "Usuário removido com sucesso" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Retorna estatísticas dos usuários
        /// </summary>
        /// <returns>Dados estatísticos</returns>
        [HttpGet("stats")]
        [Authorize]
        public async Task<IActionResult> GetUserStats()
        {
            try
            {
                var stats = await _userService.GetUserStatsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
