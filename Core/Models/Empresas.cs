// File: Core/Models/Empresas.cs
using Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    [Table("Empresas")]
    public class Empresas : BaseModel
    {
        [Required]
        public required string Name { get; set; }

        [Required, EmailAddress]
        public required string Email { get; set; }

        public EmpresaStatus Status { get; set; }

        public string? CNPJ { get; set; }

        [Required]
        public required int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public required User User { get; set; }
    }
}
