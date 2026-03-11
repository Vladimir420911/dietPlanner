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
        private string password; // новое поле

        [ObservableProperty]
        private double dailyCalorieNorm;

        [ObservableProperty]
        private double dailyProteinNorm;

        [ObservableProperty]
        private double dailyFatNorm;

        [ObservableProperty]
        private double dailyCarbNorm;

        // Флаг: true, если создаётся новый пользователь
        private bool IsNewUser => AppState.CurrentUser == null;

        public ProfileViewModel()
        {
            LoadUser();
        }

        private void LoadUser()
        {
            var user = AppState.CurrentUser;
            if (user == null)
            {
                // Значения по умолчанию для нового пользователя
                UserName = "Новый пользователь";
                Age = "30";
                Gender = "Male";
                Weight = "70";
                Height = "170";
                ActivityLevel = "Moderate";
                Goal = "Maintain";
                Password = string.Empty;
                // Нормы пока не отображаем
                return;
            }

            // Заполняем данными существующего пользователя
            UserName = user.Name;
            Age = user.Age.ToString();
            Gender = user.Gender;
            Weight = user.Weight.ToString();
            Height = user.Height.ToString();
            ActivityLevel = user.ActivityLevel;
            Goal = user.Goal.ToString();
            Password = user.Password; // может быть пустым, если раньше не хранили

            DailyCalorieNorm = user.DailyCalorieNorm;
            DailyProteinNorm = user.DailyProteinNorm;
            DailyFatNorm = user.DailyFatNorm;
            DailyCarbNorm = user.DailyCarbNorm;
        }

        [RelayCommand]
        private async Task SaveProfile()
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
            if (IsNewUser && string.IsNullOrWhiteSpace(Password))
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "При регистрации необходимо ввести пароль", "OK");
                return;
            }
            if (!Enum.TryParse<GoalType>(Goal, out var goalType))
                goalType = GoalType.Maintain;

            // Создаём или обновляем объект пользователя
            var user = IsNewUser ? new UserProfile() : AppState.CurrentUser!;

            user.Name = UserName;
            user.Age = age;
            user.Gender = Gender;
            user.Weight = weight;
            user.Height = height;
            user.ActivityLevel = ActivityLevel;
            user.Goal = goalType;
            if (!string.IsNullOrWhiteSpace(Password)) // если пароль введён, обновляем
                user.Password = Password;

            try
            {
                if (IsNewUser)
                {
                    // Создание нового пользователя
                    int newId = Database.CreateUser(user, Password);
                    user.Id = newId;
                    Preferences.Set("CurrentUserId", newId);
                    AppState.CurrentUserId = newId;
                    AppState.CurrentUser = user;
                    // Переходим на главную
                    Application.Current.MainPage = new AppShell();
                }
                else
                {
                    // Обновление существующего
                    Database.SaveUserProfile(user); // предполагается, что он обновляет все поля, кроме пароля?
                                                    // Если SaveUserProfile не обновляет пароль, нужно написать отдельный метод или добавить параметр.
                                                    // Для простоты предлагаю создать метод UpdateUserWithPassword.
                                                    // Либо расширить SaveUserProfile.
                                                    // Ниже пример метода, который обновляет и пароль.
                    Database.UpdateUserWithPassword(user);
                    // Обновляем нормы на экране
                    DailyCalorieNorm = user.DailyCalorieNorm;
                    DailyProteinNorm = user.DailyProteinNorm;
                    DailyFatNorm = user.DailyFatNorm;
                    DailyCarbNorm = user.DailyCarbNorm;
                    await Application.Current.MainPage.DisplayAlert("Успех", "Профиль сохранён", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", $"Не удалось сохранить: {ex.Message}", "OK");
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