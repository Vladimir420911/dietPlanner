using testMaui;

public class UserProfile
{
    public int Id { get; set; }
    public string Name { get; set; } = "Пользователь";
    public int Age { get; set; } = 30;
    public string Gender { get; set; } = "Male";
    public double Weight { get; set; } = 70;
    public double Height { get; set; } = 170;

    public string Password { get; set; } = string.Empty;
    // Числовой коэффициент активности (хранится в БД)
    public double ActivityFactor { get; set; } = 1.55; // Moderate по умолчанию

    // Строковое представление для UI (синхронизируется с ActivityFactor)
    public string ActivityLevel
    {
        get => ActivityFactor switch
        {
            1.2 => "Low",
            1.55 => "Moderate",
            1.725 => "High",
            _ => "Moderate"
        };
        set
        {
            ActivityFactor = value switch
            {
                "Low" => 1.2,
                "Moderate" => 1.55,
                "High" => 1.725,
                _ => 1.55
            };
        }
    }

    public GoalType Goal { get; set; } = GoalType.Maintain;

    public double DailyCalorieNorm { get; set; }
    public double DailyProteinNorm { get; set; }
    public double DailyFatNorm { get; set; }
    public double DailyCarbNorm { get; set; }
}