using Core.Enums;

namespace Core.DTO.Empresa
{
    public class EmpresaResponse
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public string? CNPJ { get; set; }
        public EmpresaStatus Status { get; set; }
        public string StatusText => Status.ToString();
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
