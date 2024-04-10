using SpiritsFirstTry.ViewModels;

namespace SpiritsFirstTry;

public partial class LoginPage : ContentPage
{
	LoginViewModel viewModel;
	public LoginPage(LoginViewModel viewModel)
	{
		InitializeComponent();
		this.viewModel = viewModel;
		BindingContext = viewModel;
	}
}