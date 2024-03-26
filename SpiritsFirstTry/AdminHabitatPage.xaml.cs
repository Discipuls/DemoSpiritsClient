namespace SpiritsFirstTry;

public partial class AdminHabitatPage : ContentPage
{
	public AdminHabitatPage()
	{
		InitializeComponent();
	}
    protected override bool OnBackButtonPressed()
    {
        Shell.Current.GoToAsync("//MainPage");
        return true;
    }
}