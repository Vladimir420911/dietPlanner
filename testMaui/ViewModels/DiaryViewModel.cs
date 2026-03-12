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
        private List<Meal> _allMeals = new(); // все приёмы за день

        [ObservableProperty]
        private DateTime selectedDate = DateTime.Today;

        [ObservableProperty]
        private string selectedMealType = "Завтрак";

        [ObservableProperty]
        private ObservableCollection<MealItem> currentItems; // для отображения

        // Общие итоги за день
        [ObservableProperty]
        private double totalCalories, totalProteins, totalFats, totalCarbs;

        // Нормы
        [ObservableProperty]
        private double dailyNormCalories, dailyNormProteins, dailyNormFats, dailyNormCarbs;

        // Вычисляемые остатки
        public double RemainingCalories => DailyNormCalories - TotalCalories;
        public double RemainingProteins => DailyNormProteins - TotalProteins;
        public double RemainingFats => DailyNormFats - TotalFats;
        public double RemainingCarbs => DailyNormCarbs - TotalCarbs;

        // Прогресс-бары
        public double CaloriesProgress => DailyNormCalories > 0 ? TotalCalories / DailyNormCalories : 0;
        public double ProteinsProgress => DailyNormProteins > 0 ? TotalProteins / DailyNormProteins : 0;
        public double FatsProgress => DailyNormFats > 0 ? TotalFats / DailyNormFats : 0;
        public double CarbsProgress => DailyNormCarbs > 0 ? TotalCarbs / DailyNormCarbs : 0;

        public ObservableCollection<Product> Products { get; } = new();

        [ObservableProperty]
        private Product selectedProductForAdd;

        [ObservableProperty]
        private string quantityGrams;

        public DiaryViewModel()
        {
            LoadProducts();
            LoadNorms();
            LoadAllMeals();
            FilterCurrentItems();
        }

        partial void OnSelectedDateChanged(DateTime value)
        {
            LoadAllMeals();
            FilterCurrentItems();
        }

        partial void OnSelectedMealTypeChanged(string value)
        {
            FilterCurrentItems();
        }

        public void LoadProducts()
        {
            var prods = Database.GetAllProducts();
            Products.Clear();
            foreach (var p in prods) Products.Add(p);
        }

        public void LoadNorms()
        {
            var user = AppState.CurrentUser;
            if (user == null) return;
            DailyNormCalories = user.DailyCalorieNorm;
            DailyNormProteins = user.DailyProteinNorm;
            DailyNormFats = user.DailyFatNorm;
            DailyNormCarbs = user.DailyCarbNorm;
            // Уведомляем об изменениях вычисляемых свойств
            OnPropertyChanged(nameof(RemainingCalories));
            OnPropertyChanged(nameof(RemainingProteins));
            OnPropertyChanged(nameof(RemainingFats));
            OnPropertyChanged(nameof(RemainingCarbs));
            OnPropertyChanged(nameof(CaloriesProgress));
            OnPropertyChanged(nameof(ProteinsProgress));
            OnPropertyChanged(nameof(FatsProgress));
            OnPropertyChanged(nameof(CarbsProgress));
        }

        public void LoadAllMeals()
        {
            _allMeals = Database.GetMealsForDate(SelectedDate, _userId);
            CalculateTotals();
            FilterCurrentItems();
        }

        private void CalculateTotals()
        {
            TotalCalories = _allMeals.Sum(m => m.TotalCalories);
            TotalProteins = _allMeals.Sum(m => m.TotalProteins);
            TotalFats = _allMeals.Sum(m => m.TotalFats);
            TotalCarbs = _allMeals.Sum(m => m.TotalCarbs);
            // Уведомляем об изменениях вычисляемых свойств
            OnPropertyChanged(nameof(RemainingCalories));
            OnPropertyChanged(nameof(RemainingProteins));
            OnPropertyChanged(nameof(RemainingFats));
            OnPropertyChanged(nameof(RemainingCarbs));
            OnPropertyChanged(nameof(CaloriesProgress));
            OnPropertyChanged(nameof(ProteinsProgress));
            OnPropertyChanged(nameof(FatsProgress));
            OnPropertyChanged(nameof(CarbsProgress));
        }

        private void FilterCurrentItems()
        {
            var items = _allMeals.Where(m => m.Type == SelectedMealType)
                                 .SelectMany(m => m.Items)
                                 .ToList();
            CurrentItems = new ObservableCollection<MealItem>(items);
        }

        [RelayCommand]
        private void AddMealItem()
        {
            if (SelectedProductForAdd == null) return;
            if (!double.TryParse(QuantityGrams, out double grams) || grams <= 0) return;

            var meal = _allMeals.FirstOrDefault(m => m.Type == SelectedMealType);
            if (meal == null)
            {
                meal = new Meal { DateTime = SelectedDate, Type = SelectedMealType };
                int newMealId = Database.AddMeal(meal, _userId);
                meal.Id = newMealId;
                meal.Items = new List<MealItem>();
                _allMeals.Add(meal);
            }

            var item = new MealItem
            {
                Product = SelectedProductForAdd,
                QuantityGrams = grams
            };
            int newItemId = Database.AddMealItem(item, meal.Id);
            item.Id = newItemId;
            meal.Items.Add(item);

            FilterCurrentItems();
            CalculateTotals();
            QuantityGrams = string.Empty;
        }

        [RelayCommand]
        private void DeleteMealItem(MealItem item)
        {
            if (item == null) return;
            Database.DeleteMealItem(item.Id);

            var meal = _allMeals.FirstOrDefault(m => m.Items.Contains(item));
            meal?.Items.Remove(item);
            if (meal != null && !meal.Items.Any())
            {
                Database.DeleteMeal(meal.Id);
                _allMeals.Remove(meal);
            }

            FilterCurrentItems();
            CalculateTotals();
        }

        [RelayCommand]
        private void AddMealOfType()
        {
            if (_allMeals.Any(m => m.Type == SelectedMealType)) return;
            var meal = new Meal { DateTime = SelectedDate, Type = SelectedMealType };
            int newMealId = Database.AddMeal(meal, _userId);
            meal.Id = newMealId;
            meal.Items = new List<MealItem>();
            _allMeals.Add(meal);
            FilterCurrentItems();
        }

        [RelayCommand]
        private void SelectMealType(string type)
        {
            SelectedMealType = type;
        }
    }
}