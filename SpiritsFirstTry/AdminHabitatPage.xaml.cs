using SpiritsFirstTry.ViewModels;

namespace SpiritsFirstTry;

public partial class AdminHabitatPage : ContentPage
{
    private AdminHabitatViewModel viewModel;
	public AdminHabitatPage(AdminHabitatViewModel viewModel)
	{
		InitializeComponent();
        this.viewModel = viewModel;
        BindingContext = viewModel;
	}
    protected override bool OnBackButtonPressed()
    {
        Shell.Current.GoToAsync("//MainPage");
        return true;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }

    private void TapCreate(object sender, TappedEventArgs args)
    {
       // _ = viewModel.Create();
    }

}