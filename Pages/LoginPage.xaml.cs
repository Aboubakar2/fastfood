using fastfood.ViewModels;
using fastfood.Data;

namespace fastfood.Pages;

public partial class LoginPage : ContentPage
{
	private readonly LoginViewModel _loginViewModel;
	public LoginPage(LoginViewModel  loginViewModel)
	{
		InitializeComponent();
		_loginViewModel = loginViewModel;
		BindingContext = _loginViewModel;

		Shell.SetNavBarIsVisible(this, false);
	}
}