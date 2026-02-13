// File: Core/Models/User.cs

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Enums.User;

namespace Core.Models
{
    [Table("Users")]
    public class User : BaseModel
    {
        [Required]
        public required string Name { get; set; }

        [Required]
        public required string Username { get; set; }

        [Required, EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string Password { get; set; }

        [Required]
        public required UserType Type { get; set; }

        [Required]
        public required UserStatus Status { get; set; }

        [Phone]
        public string? Phone { get; set; }

        public string? Avatar { get; set; }

        // Novos campos adicionados
        public int? ClientId { get; set; }
        public int? EmpresaId { get; set; }

        // Timestamps herdados de BaseModel: CreatedAt, UpdatedAt

        // Navigation properties
        public ICollection<Empresas>? Empresas { get; set; }

        // Helper properties for API responses
        [NotMapped]
        public string Role => Type switch
        {
            UserType.Admin => "ADMIN",
            UserType.Company => "COMPANY",
            UserType.Client => "CLIENTE",
            _ => "CLIENTE"
        };

        [NotMapped]
        public string StatusString => Status.ToString().ToLower();
    }
}
