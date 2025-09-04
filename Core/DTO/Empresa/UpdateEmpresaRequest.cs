using System.ComponentModel.DataAnnotations;
using Core.Enums;

namespace Core.DTO.Empresa
{
    public class UpdateEmpresaRequest
    {
        [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        public string? Name { get; set; }

        [EmailAddress(ErrorMessage = "Email deve ter um formato válido")]
        [StringLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
        public string? Email { get; set; }

        [StringLength(18, ErrorMessage = "CNPJ deve ter no máximo 18 caracteres")]
        public string? CNPJ { get; set; }

        public EmpresaStatus? Status { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "UserId deve ser maior que 0")]
        public int? UserId { get; set; }
    }
}
