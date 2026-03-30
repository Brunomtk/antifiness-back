using System;
using System.Linq;
using System.Security.Claims;

namespace ControlApi.Helpers
{
    public static class ClaimsPrincipalExtensions
    {
        public static string? GetRole(this ClaimsPrincipal user)
        {
            return FirstNonEmpty(user,
                ClaimTypes.Role,
                "role",
                "userTypeName",
                "userType");
        }

        public static int? GetUserId(this ClaimsPrincipal user)
        {
            return TryGetPositiveInt(user,
                "userId",
                ClaimTypes.NameIdentifier,
                "sub",
                ClaimTypes.Sid,
                "nameid");
        }

        public static int? GetEmpresaId(this ClaimsPrincipal user)
        {
            return TryGetPositiveInt(user,
                "empresaId",
                "EmpresaId",
                "companyId",
                "CompanyId",
                "company_id");
        }

        public static int? GetClientId(this ClaimsPrincipal user)
        {
            return TryGetPositiveInt(user,
                "clientId",
                "ClientId",
                "clienteId",
                "ClienteId");
        }

        private static int? TryGetPositiveInt(ClaimsPrincipal user, params string[] claimTypes)
        {
            foreach (var claimType in claimTypes)
            {
                var raw = user.FindFirst(claimType)?.Value;
                if (string.IsNullOrWhiteSpace(raw))
                    continue;

                if (int.TryParse(raw, out var value) && value > 0)
                    return value;
            }

            return null;
        }

        private static string? FirstNonEmpty(ClaimsPrincipal user, params string[] claimTypes)
        {
            foreach (var claimType in claimTypes)
            {
                var value = user.FindFirst(claimType)?.Value;
                if (!string.IsNullOrWhiteSpace(value))
                    return value;
            }

            return null;
        }
    }
}
