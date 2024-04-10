using SpiritsFirstTry.ViewModels;

namespace SpiritsFirstTry;

public partial class EditMarkerPage : ContentPage
{
	EditMarkerViewModel viewModel;
	public EditMarkerPage(EditMarkerViewModel viewModel)
	{
		InitializeComponent();
		this.viewModel = viewModel;
		BindingContext = viewModel;

		this.viewModel.SetupMap(this.MarkerMapView);
	}
}