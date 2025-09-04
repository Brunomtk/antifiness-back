using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Enums;

namespace Core.Models.Workout
{
    public class Exercise : BaseModel
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [StringLength(2000)]
        public string? Instructions { get; set; }

        [Required]
        public string MuscleGroups { get; set; } = string.Empty; // JSON array

        [Required]
        public string Equipment { get; set; } = string.Empty; // JSON array

        [Required]
        public ExerciseDifficulty Difficulty { get; set; }

        [Required]
        public ExerciseCategory Category { get; set; }

        [StringLength(1000)]
        public string? Tips { get; set; }

        [StringLength(1000)]
        public string? Variations { get; set; }

        public string? MediaUrls { get; set; } // JSON array

        public bool IsActive { get; set; } = true;

        public int EmpresaId { get; set; }

        // Navigation properties
        public virtual ICollection<WorkoutExercise> WorkoutExercises { get; set; } = new List<WorkoutExercise>();
    }
}
