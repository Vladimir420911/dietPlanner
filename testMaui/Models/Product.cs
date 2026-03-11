namespace testMaui.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Calories { get; set; }      // ккал на 100 г
        public double Proteins { get; set; }      // белки на 100 г
        public double Fats { get; set; }           // жиры на 100 г
        public double Carbs { get; set; }          // углеводы на 100 г
    }
}
