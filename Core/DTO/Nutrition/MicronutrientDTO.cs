namespace Core.DTO.Nutrition
{
    public class MicronutrientDTO
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
    }
}
