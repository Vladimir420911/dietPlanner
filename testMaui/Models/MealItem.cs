

namespace testMaui.Models
{
    public class MealItem
    {
        public int Id { get; set; }
        public Product Product { get; set; } = null!;
        public double QuantityGrams { get; set; }
        public double Calories => Product.Calories * QuantityGrams / 100.0;
        public double Proteins => Product.Proteins * QuantityGrams / 100.0;
        public double Fats => Product.Fats * QuantityGrams / 100.0;
        public double Carbs => Product.Carbs * QuantityGrams / 100.0;
    }
}
