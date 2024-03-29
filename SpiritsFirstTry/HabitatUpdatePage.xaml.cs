using SpiritsFirstTry.ViewModels;

namespace SpiritsFirstTry;

public partial class HabitatUpdatePage : ContentPage
{
	private HabitatUpdateViewModel viewModel;
	public HabitatUpdatePage(HabitatUpdateViewModel viewModel)
	{
		this.viewModel = viewModel;
		BindingContext = viewModel;
		InitializeComponent();
        this.viewModel.SetupMap(this.HabitatMapView);

    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }

    protected override bool OnBackButtonPressed()
    {
        HabitatMapView.GeometryEditor.Stop();
        return base.OnBackButtonPressed();
    }
}