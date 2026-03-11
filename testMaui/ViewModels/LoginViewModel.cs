using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using testMaui.Models;
using testMaui.Sevices;
using System.Diagnostics;

namespace testMaui.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string userName = "Новый пользователь";

        [ObservableProperty]
        private string age = "30";

        [ObservableProperty]
        private string gender = "Male";

        [ObservableProperty]
        private string weight = "70";

        [ObservableProperty]
        private string height = "170";

        [ObservableProperty]
        private string activityLevel = "Moderate";

        [ObservableProperty]
        private string goal = "Maintain";

        [RelayCommand]
        private async Task CreateProfile()
        {
            // Валидация
            if (string.IsNullOrWhiteSpace(UserName))
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Введите имя", "OK");
                return;
            }
            if (!int.TryParse(Age, out int age) || age <= 0)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Некорректный возраст", "OK");
                return;
            }
            if (!double.TryParse(Weight, out double weight) || weight <= 0)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Некорректный вес", "OK");
                return;
            }
            if (!double.TryParse(Height, out double height) || height <= 0)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Некорректный рост", "OK");
                return;
            }
            if (!Enum.TryParse<GoalType>(Goal, out var goalType))
                goalType = GoalType.Maintain;

            var user = new UserProfile
            {
                Name = UserName,
                Age = age,
                Gender = Gender,
                Weight = weight,
                Height = height,
                ActivityLevel = ActivityLevel,
                Goal = goalType
            };

            try
            {
                int newId = Database.CreateUser(user);
                Preferences.Set("CurrentUserId", newId);
                AppState.CurrentUserId = newId;
                AppState.CurrentUser = user;

                Application.Current.MainPage = new AppShell();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка создания пользователя: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Не удалось создать профиль", "OK");
            }
        }
    }
}