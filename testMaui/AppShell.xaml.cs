using testMaui.Views;

namespace testMaui
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            // Регистрируем маршрут для страницы входа, чтобы можно было перейти на неё через GoToAsync
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        }

    }
}
