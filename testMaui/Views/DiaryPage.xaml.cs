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
        vm?.LoadAllMeals();   // загрузить приёмы пищи
        vm?.LoadProducts();   // обновить список продуктов
        vm?.LoadNorms();      // обновить нормы из профиля
    }
}