using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.DTO.Feedback
{
    public class MandatoryFeedbackQuestionDTO
    {
        public string Key { get; set; } = string.Empty; // ex: overallRating
        public string Label { get; set; } = string.Empty; // texto para UI
        public string Type { get; set; } = "rating"; // rating | text | yesno
        public bool Required { get; set; } = true;
        public int? Min { get; set; }
        public int? Max { get; set; }
    }

    public class MandatoryFeedbackPendingResponse
    {
        public bool HasPending { get; set; }
        public int? DaysUntilNextRequired { get; set; } // null quando pendente
        public string Title { get; set; } = "Como foi sua experiência?";
        public string Description { get; set; } = "Responda rapidinho para melhorarmos o app 🙂";
        public List<MandatoryFeedbackQuestionDTO> Questions { get; set; } = new();
    }

    public class SubmitMandatoryFeedbackRequest
    {
        [Required]
        public int ClientId { get; set; }

        /// <summary>
        /// Se você quiser identificar o treinador (opcional)
        /// </summary>
        public int? TrainerId { get; set; }

        // Perguntas (fixas) – o front envia os campos que existem na UI
        [Range(1,5)]
        public int OverallRating { get; set; }

        [Range(1,5)]
        public int AppUsabilityRating { get; set; }

        [Range(1,5)]
        public int DietRating { get; set; }

        [Range(1,5)]
        public int WorkoutRating { get; set; }

        [MaxLength(2000)]
        public string? Comment { get; set; }

        public bool IsAnonymous { get; set; } = false;
    }
}
