using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using testMaui.Models;
using testMaui.Sevices;
using System.Diagnostics;
using testMaui.Views;

namespace testMaui.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string userName;

        [ObservableProperty]
        private string password;

        [RelayCommand]
        private async Task Login()
        {
            if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Password))
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Введите имя и пароль", "OK");
                return;
            }

            try
            {
                var user = Database.Authenticate(UserName, Password);
                if (user == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка", "Неверное имя или пароль", "OK");
                    return;
                }

                Preferences.Set("CurrentUserId", user.Id);
                AppState.CurrentUserId = user.Id;
                AppState.CurrentUser = user;
                Application.Current.MainPage = new AppShell();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", $"Ошибка входа: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task GoToRegister()
        {
            // Переходим на ProfilePage для регистрации нового пользователя
            await Application.Current.MainPage.Navigation.PushAsync(new ProfilePage());
        }

    }
}