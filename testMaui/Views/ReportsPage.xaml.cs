using testMaui.ViewModels;

namespace testMaui.Views;

public partial class ReportsPage : ContentPage
{
	public ReportsPage()
	{
		InitializeComponent();
	}

    private void OnGenerateReport(object sender, EventArgs e)
    {
        (BindingContext as ReportsViewModel)?.GenerateReport();
    }
}