namespace Core.DTO.Nutrition
{
    /// <summary>
    /// Resumo com os principais micronutrientes agregados da dieta (totais do dia).
    /// As unidades seguem as definidas nos tipos cadastrados (mg, µg, IU etc.).
    /// </summary>
    public class DietMicronutrientsDTO
    {
        public decimal? VitaminA { get; set; }        // µg RAE
        public decimal? VitaminC { get; set; }        // mg
        public decimal? VitaminD { get; set; }        // µg ou IU
        public decimal? VitaminE { get; set; }        // mg
        public decimal? VitaminK { get; set; }        // µg
        public decimal? VitaminB1 { get; set; }       // Tiamina
        public decimal? VitaminB2 { get; set; }       // Riboflavina
        public decimal? VitaminB3 { get; set; }       // Niacina
        public decimal? VitaminB6 { get; set; }
        public decimal? Folate { get; set; }          // B9 (µg)
        public decimal? VitaminB12 { get; set; }      // µg
        public decimal? Calcium { get; set; }         // mg
        public decimal? Iron { get; set; }            // mg
        public decimal? Magnesium { get; set; }       // mg
        public decimal? Potassium { get; set; }       // mg
        public decimal? Zinc { get; set; }            // mg
        public decimal? Sodium { get; set; }          // mg
        public decimal? Selenium { get; set; }        // µg
    }
}
