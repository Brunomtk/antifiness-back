// File: Infrastructure/Authenticate/JWTManager.cs

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Core.DTO.User;
using Core.Enums.User;
using Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Authenticate
{
    public interface IJWTManager
    {
        /// <summary>
        /// Gera um par de tokens (access + refresh) para o usuário.
        /// Se rememberMe = true, o access token expira em 30 dias; senão em 1 dia.
        /// </summary>
        Task<TokenJWT> AuthenticateAsync(User user, bool rememberMe = false);

        /// <summary>
        /// Renova o par de tokens a partir de um refresh token válido.
        /// </summary>
        Task<TokenJWT> RefreshTokenAsync(string refreshToken);
    }

    public sealed class JWTManager : IJWTManager
    {
        private readonly IConfiguration _configuration;

        public JWTManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<TokenJWT> AuthenticateAsync(User user, bool rememberMe = false)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var baseClaims = BuildUserClaims(
                userId: user.Id,
                username: user.Username,
                email: user.Email,
                name: user.Name,
                type: user.Type,
                status: user.Status,
                rememberMe: rememberMe,
                empresaId: user.EmpresaId,
                clientId: user.ClientId
            );

            var tokens = GenerateTokens(baseClaims, rememberMe);
            return Task.FromResult(tokens);
        }

        public Task<TokenJWT> RefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new SecurityTokenException("Refresh token é obrigatório.");

            var handler = new JwtSecurityTokenHandler();

            var key = _configuration["Jwt:Key"]
                      ?? throw new InvalidOperationException("Jwt:Key não configurado.");
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ValidateIssuer = !string.IsNullOrEmpty(issuer),
                ValidIssuer = issuer,
                ValidateAudience = !string.IsNullOrEmpty(audience),
                ValidAudience = audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(1)
            };

            try
            {
                var principal = handler.ValidateToken(refreshToken, validationParameters, out var validatedToken);

                if (validatedToken is not JwtSecurityToken jwt ||
                    !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new SecurityTokenException("Token inválido.");
                }

                var tokenType = principal.FindFirst("tokenType")?.Value;
                if (!string.Equals(tokenType, "refresh", StringComparison.OrdinalIgnoreCase))
                    throw new SecurityTokenException("Token informado não é um refresh token.");

                // RememberMe original
                var rememberMeClaim = principal.FindFirst("rememberMe")?.Value;
                var rememberMe = string.Equals(rememberMeClaim, "true", StringComparison.OrdinalIgnoreCase)
                                 || rememberMeClaim == "1";

                // Dados do usuário das claims
                var idStr = principal.FindFirst("userId")?.Value;
                var username = principal.FindFirst(JwtRegisteredClaimNames.UniqueName)?.Value;
                var email = principal.FindFirst(JwtRegisteredClaimNames.Email)?.Value
                            ?? principal.FindFirst(ClaimTypes.Email)?.Value;
                var name = principal.FindFirst(ClaimTypes.Name)?.Value;
                var typeStr = principal.FindFirst("userType")?.Value;
                var statusStr = principal.FindFirst("userStatus")?.Value;

                int userId = 0;
                int.TryParse(idStr, out userId);

                if (!Enum.TryParse<UserType>(typeStr, out var userType))
                    userType = UserType.Client;

                if (!Enum.TryParse<UserStatus>(statusStr, out var userStatus))
                    userStatus = UserStatus.Active;

                // Reconstrói as claims base do usuário
                var baseClaims = BuildUserClaims(
                    userId: userId,
                    username: username,
                    email: email,
                    name: name,
                    type: userType,
                    status: userStatus,
                    rememberMe: rememberMe,
                    empresaId: TryParseNullableInt(principal.FindFirst("empresaId")?.Value),
                    clientId: TryParseNullableInt(principal.FindFirst("clientId")?.Value)
                );

                // Gera novo par de tokens
                var tokens = GenerateTokens(baseClaims, rememberMe);
                return Task.FromResult(tokens);
            }
            catch (SecurityTokenException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new SecurityTokenException("Refresh token inválido ou expirado.");
            }
        }
        private static int? TryParseNullableInt(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            return int.TryParse(value, out var i) ? i : (int?)null;
        }

        

        

        

        

        



        
        /// <summary>
        /// Monta o conjunto base de claims do usuário (sem tokenType).
        /// </summary>
        private Claim[] BuildUserClaims(
            int userId,
            string? username,
            string? email,
            string? name,
            UserType type,
            UserStatus status,
            bool rememberMe,
            int? empresaId,
            int? clientId)
        {
            var role = RoleFromType(type);
            return new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,        userId.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, username ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Email,      email ?? string.Empty),
                new Claim(ClaimTypes.Name,                    name ?? string.Empty),
                new Claim(ClaimTypes.Role,                    role),
                new Claim("role",                             role),
                new Claim("userId",                           userId.ToString()),
                new Claim("userType",                         ((int)type).ToString()),
                new Claim("userTypeName",                     type.ToString()),
                new Claim("userStatus",                       ((int)status).ToString()),
                new Claim("userStatusName",                   status.ToString()),
                new Claim("empresaId",                       (empresaId ?? 0).ToString()),
                new Claim("clientId",                        (clientId ?? 0).ToString()),
                new Claim("rememberMe",                       rememberMe ? "true" : "false")
            };
        }

        private static string RoleFromType(UserType type)
        {
            return type switch
            {
                UserType.Admin => "ADMIN",
                UserType.Company => "COMPANY",
                UserType.Client => "CLIENTE",
                _ => "CLIENTE"
            };
        }

        /// <summary>
        /// Gera access token + refresh token a partir das claims base do usuário.
        /// </summary>
        private TokenJWT GenerateTokens(Claim[] baseClaims, bool rememberMe)
        {
            var key = _configuration["Jwt:Key"]
                      ?? throw new InvalidOperationException("Jwt:Key não configurado.");
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            var keyBytes = Encoding.UTF8.GetBytes(key);
            var handler = new JwtSecurityTokenHandler();

            // Access token: 1 dia ou 30 dias conforme RememberMe
            var accessExpires = rememberMe
                ? DateTime.UtcNow.AddDays(30)
                : DateTime.UtcNow.AddDays(1);

            var accessDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(baseClaims.Append(new Claim("tokenType", "access"))),
                Expires = accessExpires,
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(keyBytes),
                    SecurityAlgorithms.HmacSha256
                )
            };

            var accessToken = handler.WriteToken(handler.CreateToken(accessDescriptor));

            // Refresh token: sempre 30 dias
            var refreshDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(baseClaims.Append(new Claim("tokenType", "refresh"))),
                Expires = DateTime.UtcNow.AddDays(30),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(keyBytes),
                    SecurityAlgorithms.HmacSha256
                )
            };

            var refreshToken = handler.WriteToken(handler.CreateToken(refreshDescriptor));

            return new TokenJWT
            {
                Token = accessToken,
                RefreshToken = refreshToken
            };
        }
    }
}
