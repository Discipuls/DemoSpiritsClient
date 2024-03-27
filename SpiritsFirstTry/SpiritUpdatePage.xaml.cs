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
            picker.Loaded += OnHabitatPickerLoaded;
            picker.ItemDisplayBinding = new Binding("Name"); 
            Grid views = new Grid();
            views.Children.Add(picker);

            return new ViewCell
            {
                View = views
            };
        });
    }

    void OnHabitatPickerLoaded(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;
        this._viewModel.HabitatPicker.Add(picker);
        Console.WriteLine("ID: " + picker.Id.ToString());
    }

    private void SetupTypeListView()
    {
        this.TypeListView.ItemTemplate = new DataTemplate(() =>
        {
            Picker picker = new Picker();
            picker.ItemsSource = _viewModel.Types;
            picker.SetBinding(Picker.SelectedIndexProperty, ".");
            picker.Loaded += OnTypePickerLoaded;
            Grid views = new Grid();
            views.Children.Add(picker);

            return new ViewCell
            {
                View = views
            };
        });
    }


    void OnTypePickerLoaded(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;
        this._viewModel.ClassificationPicker.Add(picker);
    }

    private void TapEditMarkerImage(object sender, TappedEventArgs args)
    {
        _ = _viewModel.EditMarkerImage(this.markerImage);
        // TODO move to SAVE

    }
    private void TapEditCardImage(object sender, TappedEventArgs args)
    {
        _ = _viewModel.EditCardImage(this.CardImage);
        // TODO move to SAVE

    }

    private void TapEditMarker(object sender, TappedEventArgs args)
    {
        _ = _viewModel.EditMarker();
        // TODO move to SAVE

    }
}