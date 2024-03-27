using CommunityToolkit.Mvvm.Input;
using SpiritsClassLibrary.Models;
using SpiritsFirstTry.ViewModels;

namespace SpiritsFirstTry;
public partial class AdminSpiritPage : ContentPage
{
    AdminSpiritsViewModel viewModel;
    public AdminSpiritPage(AdminSpiritsViewModel viewModel)
    {
        this.viewModel = viewModel;
        BindingContext = viewModel;

        InitializeComponent();

    }

    protected override bool OnBackButtonPressed()
    {
        Shell.Current.GoToAsync("//MainPage");
        return true;
    }
    [RelayCommand]
    public void Delete(MapSpirit spirit)
    {
        viewModel.Delete(spirit);
    }

    private void TapCreate(object sender, TappedEventArgs args)
    {
        _ = viewModel.Create();
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        //  this.SpiritsListView.ItemsSource = viewModel.Spirits; 

        base.OnNavigatedTo(args);
      //  this.SpiritsListView.ItemsSource = viewModel.Spirits;

    }
    protected override void OnAppearing()
    {
        //this.SpiritsListView.ItemsSource = viewModel.Spirits;

        base.OnAppearing();
     //   this.SpiritsListView.ItemsSource = viewModel.Spirits;

    }
}