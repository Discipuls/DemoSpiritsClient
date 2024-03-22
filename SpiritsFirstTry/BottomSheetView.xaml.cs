using SpiritsFirstTry.ViewModels;
using The49.Maui.BottomSheet;

namespace SpiritsFirstTry;

public partial class BottomSheetView : BottomSheet
{
	BottomSheetViewModel ViewModel;
	public BottomSheetView(BottomSheetViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
		ViewModel = vm;
		vm.CurrentCardImage = this.CurrentCardImage;
	}

    private void search_TextChanged(object sender, TextChangedEventArgs e)
    {
		ViewModel.performSearch(this.searchbar.Text);
    }
}