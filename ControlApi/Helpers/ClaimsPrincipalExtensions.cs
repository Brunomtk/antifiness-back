using System.Security.Claims;

namespace ControlApi.Helpers
{
    public static class ClaimsPrincipalExtensions
    {
        public static string? GetRole(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Role)?.Value ?? user.FindFirst("role")?.Value;
        }

        public static int? GetUserId(this ClaimsPrincipal user)
        {
            return TryGetInt(user, "userId")
                   ?? TryGetInt(user, ClaimTypes.NameIdentifier)
                   ?? TryGetInt(user, "sub");
        }

        public static int? GetEmpresaId(this ClaimsPrincipal user)
        {
            return TryGetInt(user, "empresaId") ?? TryGetInt(user, "EmpresaId");
        }

        public static int? GetClientId(this ClaimsPrincipal user)
        {
            return TryGetInt(user, "clientId") ?? TryGetInt(user, "ClientId");
        }

        private static int? TryGetInt(ClaimsPrincipal user, string claimType)
        {
            var v = user.FindFirst(claimType)?.Value;
            if (int.TryParse(v, out var i)) return i;
            return null;
        }
    }
}
