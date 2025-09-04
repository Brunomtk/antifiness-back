using System.ComponentModel.DataAnnotations;
using Core.Enums;

namespace Core.DTO.Empresa
{
    public class CreateEmpresaRequest
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email deve ter um formato válido")]
        [StringLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
        public required string Email { get; set; }

        [StringLength(18, ErrorMessage = "CNPJ deve ter no máximo 18 caracteres")]
        public string? CNPJ { get; set; }

        [Required(ErrorMessage = "Status é obrigatório")]
        public EmpresaStatus Status { get; set; } = EmpresaStatus.Active;

        [Required(ErrorMessage = "UserId é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "UserId deve ser maior que 0")]
        public int UserId { get; set; }
    }
}
