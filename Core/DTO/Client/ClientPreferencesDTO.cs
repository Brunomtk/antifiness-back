// File: Core/DTO/ClientPreferencesDTO.cs
using System.Collections.Generic;

namespace Core.DTO.Client
{
    public class ClientPreferencesDTO
    {
        public IList<string> DietaryRestrictions { get; set; } = new List<string>();
        public IList<string> FavoriteFood { get; set; } = new List<string>();
        public IList<string> DislikedFood { get; set; } = new List<string>();
        public MealTimesDTO MealTimes { get; set; } = new MealTimesDTO();
        public WorkoutPreferencesDTO WorkoutPreferences { get; set; } = new WorkoutPreferencesDTO();
    }

    public class MealTimesDTO
    {
        public string Breakfast { get; set; } = string.Empty;
        public string Lunch { get; set; } = string.Empty;
        public string Dinner { get; set; } = string.Empty;
        public IList<string> Snacks { get; set; } = new List<string>();
    }

    public class WorkoutPreferencesDTO
    {
        public IList<string> Types { get; set; } = new List<string>();
        public int Duration { get; set; }
        public int Frequency { get; set; }
        public string TimeOfDay { get; set; } = string.Empty;
    }
}
