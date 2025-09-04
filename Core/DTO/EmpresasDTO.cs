// File: Core/DTO/EmpresasDTO.cs
using System.ComponentModel.DataAnnotations;
using Core.Enums;

namespace Core.DTO
{
    public class EmpresasDTO
    {
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        public EmpresaStatus Status { get; set; }

        [RegularExpression(@"^\d{14}$", ErrorMessage = "CNPJ deve conter exatamente 14 dígitos.")]
        public string? CNPJ { get; set; }

        [Required, EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required int UserId { get; set; }
    }
}
