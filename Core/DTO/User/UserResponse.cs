using Core.Enums.User;

namespace Core.DTO.User
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Avatar { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public UserType Type { get; set; }
        public UserStatus StatusEnum { get; set; }
        
        // Novos campos adicionados
        public int? ClientId { get; set; }
        public int? EmpresaId { get; set; }
    }
}
