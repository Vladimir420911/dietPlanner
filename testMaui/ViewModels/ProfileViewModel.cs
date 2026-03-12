using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using testMaui.Sevices;
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

        public double CaloriesProgress => DailyCalorieNorm > 0 ? DailyCalorieNorm / (AppState.CurrentUser?.DailyCalorieNorm ?? 1) : 0;

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
                Gender = "Мужской";
                Weight = "70";
                Height = "170";
                ActivityLevel = "Средняя";
                Goal = "Поддержание";
                Password = string.Empty;
                // Нормы пока не отображаем
                return;
            }

            // Заполнение существующего пользователя (преобразование в русские)
            UserName = user.Name;
            Age = user.Age.ToString();
            Gender = user.Gender == "Male" ? "Мужской" : "Женский";
            Weight = user.Weight.ToString();
            Height = user.Height.ToString();
            // ActivityLevel — вычисляем по ActivityFactor
            ActivityLevel = user.ActivityFactor switch
            {
                1.2 => "Низкая",
                1.55 => "Средняя",
                1.725 => "Высокая",
                _ => "Средняя"
            };
            Goal = user.Goal switch
            {
                GoalType.Lose => "Похудение",
                GoalType.Maintain => "Поддержание",
                GoalType.Gain => "Набор массы",
                _ => "Поддержание"
            };
            Password = user.Password;

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

            // Преобразование русских значений в английские для БД
            string gender = Gender == "Мужской" ? "Male" : "Female";
            double activityFactor = ActivityLevel switch
            {
                "Низкая" => 1.2,
                "Средняя" => 1.55,
                "Высокая" => 1.725,
                _ => 1.55
            };
            GoalType goal = Goal switch
            {
                "Похудение" => GoalType.Lose,
                "Поддержание" => GoalType.Maintain,
                "Набор массы" => GoalType.Gain,
                _ => GoalType.Maintain
            };

            // Создаём или обновляем пользователя
            var user = AppState.CurrentUser ?? new UserProfile();

            user.Name = UserName;
            user.Age = age;
            user.Gender = gender;
            user.Weight = weight;
            user.Height = height;
            user.ActivityFactor = activityFactor; // напрямую задаём коэффициент
            user.Goal = goal;
            if (!string.IsNullOrWhiteSpace(Password))
                user.Password = Password;

            try
            {
                if (AppState.CurrentUser != null)
                {
                    Database.UpdateUserWithPassword(user);
                    // Обновляем нормы
                    DailyCalorieNorm = user.DailyCalorieNorm;
                    DailyProteinNorm = user.DailyProteinNorm;
                    DailyFatNorm = user.DailyFatNorm;
                    DailyCarbNorm = user.DailyCarbNorm;
                    await Application.Current.MainPage.DisplayAlert("Успех", "Профиль обновлён", "OK");
                }
                else
                {
                    int newId = Database.CreateUser(user, Password);
                    user.Id = newId;
                    Preferences.Set("CurrentUserId", newId);
                    AppState.CurrentUserId = newId;
                    AppState.CurrentUser = user;
                    Application.Current.MainPage = new AppShell();
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