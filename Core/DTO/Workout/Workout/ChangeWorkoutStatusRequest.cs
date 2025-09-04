using System.ComponentModel.DataAnnotations;
using Core.Enums;

namespace Core.DTO.Workout.Workout
{
    public class ChangeWorkoutStatusRequest
    {
        [Required(ErrorMessage = "Status é obrigatório")]
        public WorkoutStatus Status { get; set; }
    }
}
