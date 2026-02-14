using System;
using System.Threading.Tasks;
using ControlApi.Helpers;
using Core.Enums.User;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ControlApi.Controllers
{
    [ApiController]
    [Route("api/admin/sessions")]
    [Authorize]
    public class AdminSessionsController : ControllerBase
    {
        private readonly DbContextClass _db;

        public AdminSessionsController(DbContextClass db)
        {
            _db = db;
        }

        /// <summary>
        /// Revoga (desloga) todos os usuários que NÃO são ADMIN.
        /// Implementação via TokenVersion: incrementa a versão e invalida JWTs anteriores.
        /// </summary>
        [HttpPost("logout-non-admin")]
        public async Task<IActionResult> LogoutNonAdmin()
        {
            var role = User.GetRole();
            if (!string.Equals(role, "ADMIN", StringComparison.OrdinalIgnoreCase))
                return Forbid();

            // EF Core 7+ (ExecuteUpdateAsync) - incrementa no banco sem carregar tudo em memória
            var affected = await _db.Users
                .Where(u => u.Type != UserType.Admin)
                .ExecuteUpdateAsync(s => s.SetProperty(u => u.TokenVersion, u => u.TokenVersion + 1));

            return Ok(new { revokedUsers = affected });
        }
    }
}
