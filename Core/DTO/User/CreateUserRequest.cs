using System.ComponentModel.DataAnnotations;
using Core.Enums.User;

namespace Core.DTO.User
{
    public class CreateUserRequest
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Username é obrigatório")]
        [StringLength(50, ErrorMessage = "Username deve ter no máximo 50 caracteres")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email deve ter um formato válido")]
        [StringLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Senha é obrigatória")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Senha deve ter entre 6 e 100 caracteres")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tipo de usuário é obrigatório")]
        public UserType Type { get; set; }

        [Required(ErrorMessage = "Status é obrigatório")]
        public UserStatus Status { get; set; } = UserStatus.Active;

        [Phone(ErrorMessage = "Telefone deve ter um formato válido")]
        [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
        public string? Phone { get; set; }

        [StringLength(500, ErrorMessage = "Avatar deve ter no máximo 500 caracteres")]
        public string? Avatar { get; set; }

        // Novos campos adicionados
        public int? ClientId { get; set; }
        public int? EmpresaId { get; set; }
    }
}
