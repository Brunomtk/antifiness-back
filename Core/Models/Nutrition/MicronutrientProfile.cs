using Microsoft.EntityFrameworkCore;

namespace Core.Models.Nutrition
{
    /// <summary>
    /// Perfil de micronutrientes. Mapeado como tipo OWNED (colunas na própria tabela do agregado).
    /// Unidades sugeridas: mg, µg, conforme comentário por campo.
    /// </summary>
    [Owned]
    public class MicronutrientProfile
    {
        // Vitaminas
        public decimal? VitaminA { get; set; }      // µg RAE
        public decimal? VitaminC { get; set; }      // mg
        public decimal? VitaminD { get; set; }      // µg
        public decimal? VitaminE { get; set; }      // mg
        public decimal? VitaminK { get; set; }      // µg
        public decimal? VitaminB1 { get; set; }     // Tiamina (mg)
        public decimal? VitaminB2 { get; set; }     // Riboflavina (mg)
        public decimal? VitaminB3 { get; set; }     // Niacina (mg)
        public decimal? VitaminB6 { get; set; }     // mg
        public decimal? Folate { get; set; }        // B9 (µg)
        public decimal? VitaminB12 { get; set; }    // µg

        // Minerais
        public decimal? Calcium { get; set; }       // mg
        public decimal? Iron { get; set; }          // mg
        public decimal? Magnesium { get; set; }     // mg
        public decimal? Potassium { get; set; }     // mg
        public decimal? Zinc { get; set; }          // mg
        public decimal? Sodium { get; set; }        // mg
        public decimal? Selenium { get; set; }      // µg
        public decimal? Phosphorus { get; set; }    // mg
        public decimal? Copper { get; set; }        // mg
        public decimal? Manganese { get; set; }     // mg
        public decimal? Iodine { get; set; }        // µg
    }
}
