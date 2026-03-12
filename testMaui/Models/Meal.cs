

namespace testMaui.Models
{
    public class Meal
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public string Type { get; set; } = string.Empty;
        public List<MealItem> Items { get; set; } = new();

        public double TotalCalories => Items.Sum(i => i.Calories);
        public double TotalProteins => Items.Sum(i => i.Proteins);
        public double TotalFats => Items.Sum(i => i.Fats);
        public double TotalCarbs => Items.Sum(i => i.Carbs);
    }
}
