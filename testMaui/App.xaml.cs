using testMaui.Sevices;
using testMaui.Views;
namespace testMaui
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            // Инициализация пользователя (с id = 1)
            int savedUserId = Preferences.Get("CurrentUserId", -1);
            if (savedUserId != -1)
            {
                try
                {
                    var user = Database.GetUserProfile(savedUserId);
                    if (user != null)
                    {
                        AppState.CurrentUserId = user.Id;
                        AppState.CurrentUser = user;
                        MainPage = new AppShell();
                        return;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Ошибка загрузки пользователя: {ex.Message}");
                }
            }
            
            MainPage = new NavigationPage(new LoginPage());
            
        }
    }
}