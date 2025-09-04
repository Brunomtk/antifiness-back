// File: Infrastructure/Authenticate/JWTManager.cs

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Core.DTO.User;
using Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Authenticate
{
    public interface IJWTManager
    {
        Task<TokenJWT> AuthenticateAsync(User user);
    }

    public class JWTManager : IJWTManager
    {
        private readonly IConfiguration _configuration;

        public JWTManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<TokenJWT> AuthenticateAsync(User user)
        {
            // read configuration
            var keyBytes = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            // define claims
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,        user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(ClaimTypes.Name,                    user.Name),
                new Claim("userType",                         user.Type.ToString()),
                new Claim("userStatus",                       user.Status.ToString()),
            };

            // build token descriptor
            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(keyBytes),
                    SecurityAlgorithms.HmacSha256
                )
            };

            // create JWT
            var handler = new JwtSecurityTokenHandler();
            var securityToken = handler.CreateToken(descriptor);
            var token = handler.WriteToken(securityToken);

            // create a simple GUID refresh token
            var refreshToken = Guid.NewGuid().ToString();

            return Task.FromResult(new TokenJWT
            {
                Token = token,
                RefreshToken = refreshToken
            });
        }
    }
}
