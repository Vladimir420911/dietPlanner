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
        var vm = BindingContext as DiaryViewModel;
        vm?.LoadAllMeals();   // загружает приёмы пищи за день
        vm?.LoadProducts();   // обновляет список продуктов
    }
}