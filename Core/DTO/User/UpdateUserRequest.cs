using System.ComponentModel.DataAnnotations;
using Core.Enums.User;

namespace Core.DTO.User
{
    public class UpdateUserRequest
    {
        [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        public string? Name { get; set; }

        [StringLength(50, ErrorMessage = "Username deve ter no máximo 50 caracteres")]
        public string? Username { get; set; }

        [EmailAddress(ErrorMessage = "Email deve ter um formato válido")]
        [StringLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
        public string? Email { get; set; }

        [StringLength(100, MinimumLength = 6, ErrorMessage = "Senha deve ter entre 6 e 100 caracteres")]
        public string? Password { get; set; }

        public UserType? Type { get; set; }

        public UserStatus? Status { get; set; }

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
