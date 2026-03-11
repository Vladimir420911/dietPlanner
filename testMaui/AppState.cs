using testMaui.Models;
namespace testMaui
{
    public static class AppState
    {
        public static int CurrentUserId { get; set; }
        public static UserProfile? CurrentUser { get; set; }
    }
}
