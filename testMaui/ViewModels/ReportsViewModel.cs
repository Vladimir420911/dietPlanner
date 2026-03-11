using CommunityToolkit.Mvvm.ComponentModel;
using testMaui.Sevices;
using System.Linq;

namespace testMaui.ViewModels
{
    public partial class ReportsViewModel : BaseViewModel
    {
        private readonly int _userId = AppState.CurrentUserId;

        [ObservableProperty]
        private DateTime startDate = DateTime.Today.AddDays(-7);

        [ObservableProperty]
        private DateTime endDate = DateTime.Today;

        [ObservableProperty]
        private double averageCalories;

        [ObservableProperty]
        private double totalCalories;

        public void GenerateReport()
        {
            var meals = Database.GetMealsInRange(_userId, StartDate, EndDate);
            TotalCalories = meals.Sum(m => m.TotalCalories);
            AverageCalories = meals.Count > 0 ? meals.Average(m => m.TotalCalories) : 0;
        }
    }
}