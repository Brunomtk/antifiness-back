using System;
using System.ComponentModel.DataAnnotations;

namespace Core.DTO.Client
{
    public class AddPhotoProgressRequest
    {
        [Required(ErrorMessage = "Data é obrigatória")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Imagem é obrigatória")]
        public string Image { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Observações devem ter no máximo 1000 caracteres")]
        public string? Notes { get; set; }
    }
}
