using fastfood.Data;
using fastfood.ViewModels;


namespace fastfood.Pages;

public partial class ExemptionPage : ContentPage
{
	private readonly UserViewModel _userViewModel;
	public ExemptionPage(UserViewModel userViewModel)
	{
		InitializeComponent();
		_userViewModel = userViewModel;
        BindingContext = _userViewModel;

		// Get the event
		_userViewModel.ScrollToFormRequested += OnScrollToFormRequested;
		// get the deleting event
		_userViewModel.DeleteUserRequested += OnDeleteUseRequested;
		InitializeViewModelAsync();
	}
	private async void InitializeViewModelAsync() =>
		await _userViewModel.InitializeAsync();

	//Method for events starting
	private async void OnScrollToFormRequested(object sender, EventArgs e)
	{
		//defile la page vers le formulaire
		await MainScrollView.ScrollToAsync(FormStackLayout, ScrollToPosition.Start, true);
	}
	private async void OnDeleteUseRequested(object sender, User user)
	{
		bool confirm = await DisplayAlert(
			"Confirmation",
			$"Êtes-vous sûr de vouloir supprimer l'utilisateur {user.UserName} ?",
			"Oui",
			"Non");
		if (confirm)
		{
			await _userViewModel.PerformDeleteUserAsync(user);
		}
	}
	// Comin
	public async void OnGoBackClicked(object sender, EventArgs e)
	{
		await Navigation.PopAsync();
	}
}