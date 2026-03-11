using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using testMaui.Models;
using testMaui.Sevices;
using System.Collections.ObjectModel;

namespace testMaui.ViewModels
{
    public partial class DiaryViewModel : BaseViewModel
    {
        private readonly int _userId = AppState.CurrentUserId;

        [ObservableProperty]
        private DateTime selectedDate = DateTime.Today;

        [ObservableProperty]
        private ObservableCollection<Meal> meals;

        [ObservableProperty]
        private double totalCalories;

        [ObservableProperty]
        private double totalProteins;

        [ObservableProperty]
        private double totalFats;

        [ObservableProperty]
        private double totalCarbs;

        [ObservableProperty]
        private double dailyNormCalories;

        [ObservableProperty]
        private double dailyNormProteins;

        [ObservableProperty]
        private double dailyNormFats;

        [ObservableProperty]
        private double dailyNormCarbs;

        [ObservableProperty]
        private string selectedMealType = "Завтрак";

        [ObservableProperty]
        private Product selectedProductForAdd;

        [ObservableProperty]
        private string quantityGrams;

        public ObservableCollection<Product> Products { get; }

        public DiaryViewModel()
        {
            Products = new ObservableCollection<Product>(Database.GetAllProducts());
            LoadMeals();
            LoadNorms();
        }

        partial void OnSelectedDateChanged(DateTime value)
        {
            LoadMeals();
        }

        private void LoadMeals()
        {
            var mealsList = Database.GetMealsForDate(SelectedDate, _userId);
            Meals = new ObservableCollection<Meal>(mealsList);
            CalculateTotals();
        }

        private void CalculateTotals()
        {
            TotalCalories = Meals.Sum(m => m.TotalCalories);
            TotalProteins = Meals.Sum(m => m.TotalProteins);
            TotalFats = Meals.Sum(m => m.TotalFats);
            TotalCarbs = Meals.Sum(m => m.TotalCarbs);
        }

        private void LoadNorms()
        {
            var user = AppState.CurrentUser;
            if (user == null) return;
            DailyNormCalories = user.DailyCalorieNorm;
            DailyNormProteins = user.DailyProteinNorm;
            DailyNormFats = user.DailyFatNorm;
            DailyNormCarbs = user.DailyCarbNorm;
        }

        [RelayCommand]
        private void AddMealItem()
        {
            if (SelectedProductForAdd == null) return;
            if (!double.TryParse(QuantityGrams, out double grams) || grams <= 0) return;

            var meal = Meals.FirstOrDefault(m => m.Type == SelectedMealType);
            if (meal == null)
            {
                meal = new Meal
                {
                    DateTime = SelectedDate,
                    Type = SelectedMealType
                };
                int newMealId = Database.AddMeal(meal, _userId);
                meal.Id = newMealId;
                meal.Items = new List<MealItem>();
                Meals.Add(meal);
            }

            var item = new MealItem
            {
                Product = SelectedProductForAdd,
                QuantityGrams = grams
            };
            int newItemId = Database.AddMealItem(item, meal.Id);
            item.Id = newItemId;
            meal.Items.Add(item);
            CalculateTotals();
            QuantityGrams = string.Empty;
        }

        [RelayCommand]
        private void DeleteMeal(Meal meal)
        {
            if (meal == null) return;
            Database.DeleteMeal(meal.Id);
            Meals.Remove(meal);
            CalculateTotals();
        }
    }
}