using System.Collections.Generic;

namespace Core.DTO.Nutrition
{
    public class CalcMicrosInputDTO
    {
        public List<Item> Items { get; set; } = new();
        public class Item
        {
            public int FoodId { get; set; }
            public decimal PortionGrams { get; set; }
        }
    }
}
