using System.ComponentModel.DataAnnotations;

namespace Core.DTO.User
{
    public class UserAuthenticateRequest
    {
        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email deve ter um formato válido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Senha é obrigatória")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Quando true, o token de acesso será gerado com validade estendida (30 dias).
        /// Quando false (padrão), o token terá a validade padrão (1 dia).
        /// </summary>
        public bool RememberMe { get; set; } = false;
    }
}
