using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using testMaui.Models;
using testMaui.Sevices;
using System;
using testMaui.Views;

namespace testMaui.ViewModels
{
    public partial class ProfileViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string userName;

        [ObservableProperty]
        private string age;

        [ObservableProperty]
        private string gender;

        [ObservableProperty]
        private string weight;

        [ObservableProperty]
        private string height;

        [ObservableProperty]
        private string activityLevel;

        [ObservableProperty]
        private string goal;

        [ObservableProperty]
        private double dailyCalorieNorm;

        [ObservableProperty]
        private double dailyProteinNorm;

        [ObservableProperty]
        private double dailyFatNorm;

        [ObservableProperty]
        private double dailyCarbNorm;

        public ProfileViewModel()
        {
            LoadUser();
        }

        private void LoadUser()
        {
            var user = AppState.CurrentUser;
            if (user == null) return;

            UserName = user.Name;
            Age = user.Age.ToString();
            Gender = user.Gender;
            Weight = user.Weight.ToString();
            Height = user.Height.ToString();
            ActivityLevel = user.ActivityLevel;
            Goal = user.Goal.ToString();

            DailyCalorieNorm = user.DailyCalorieNorm;
            DailyProteinNorm = user.DailyProteinNorm;
            DailyFatNorm = user.DailyFatNorm;
            DailyCarbNorm = user.DailyCarbNorm;
        }

        [RelayCommand]
        private void SaveProfile()
        {
            var user = AppState.CurrentUser;
            if (user == null) return;

            user.Name = UserName;
            if (int.TryParse(Age, out int a)) user.Age = a;
            user.Gender = Gender;
            if (double.TryParse(Weight, out double w)) user.Weight = w;
            if (double.TryParse(Height, out double h)) user.Height = h;
            user.ActivityLevel = ActivityLevel;
            if (Enum.TryParse<GoalType>(Goal, out var goalType))
                user.Goal = goalType;

            try
            {
                Database.SaveUserProfile(user);

                // Обновить отображаемые нормы (они пересчитались в SaveUserProfile)
                DailyCalorieNorm = user.DailyCalorieNorm;
                DailyProteinNorm = user.DailyProteinNorm;
                DailyFatNorm = user.DailyFatNorm;
                DailyCarbNorm = user.DailyCarbNorm;

                // Показать сообщение об успехе (опционально)
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка сохранения профиля: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task Logout()
        {
            Preferences.Remove("CurrentUserId");
            AppState.CurrentUserId = 0;
            AppState.CurrentUser = null;
            Application.Current.MainPage = new NavigationPage(new LoginPage());
        }
    }
}