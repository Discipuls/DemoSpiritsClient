using SpiritsFirstTry.ViewModels;

namespace SpiritsFirstTry;

public partial class SpiritUpdatePage : ContentPage
{
	SpiritUpdateViewModel _viewModel;
	public SpiritUpdatePage(SpiritUpdateViewModel viewModel)
	{
		_viewModel = viewModel;
		BindingContext = _viewModel;
		InitializeComponent();
        _ = viewModel.SetupMap(MainMapView);

    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        SetupTypeListView();
        SetupHabitatsListView();
    }

    private void SetupHabitatsListView()
    {
        this.HabitatsListView.ItemTemplate = new DataTemplate(() =>
        {
            Picker picker = new Picker();
            picker.SetBinding(Picker.ItemsSourceProperty, new Binding(".", source: _viewModel.Habitats));
            picker.SetBinding(Picker.SelectedIndexProperty, "index");
            picker.ItemDisplayBinding = new Binding("Name"); 
            Grid views = new Grid();
            views.Children.Add(picker);

            return new ViewCell
            {
                View = views
            };
        });
    }

    private void SetupTypeListView()
    {
        this.TypeListView.ItemTemplate = new DataTemplate(() =>
        {
            Picker picker = new Picker();
            picker.ItemsSource = _viewModel.Types;
            picker.SetBinding(Picker.SelectedIndexProperty, ".");
            Grid views = new Grid();
            views.Children.Add(picker);

            return new ViewCell
            {
                View = views
            };
        });
    }
}