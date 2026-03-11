using CommunityToolkit.Mvvm.ComponentModel;

namespace testMaui.ViewModels
{
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        private string title = string.Empty;

        [ObservableProperty]
        private bool isBusy;
    }
}
