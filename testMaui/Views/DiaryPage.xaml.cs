using testMaui.ViewModels;

namespace testMaui.Views;

public partial class DiaryPage : ContentPage
{
	public DiaryPage()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        (BindingContext as DiaryViewModel)?.LoadAllMeals();
    }
}