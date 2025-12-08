using System.ComponentModel.DataAnnotations;

namespace Core.DTO.User
{
    /// <summary>
    /// Request para renovação de token usando um refresh token válido.
    /// </summary>
    public class RefreshTokenRequest
    {
        [Required(ErrorMessage = "Refresh token é obrigatório")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
