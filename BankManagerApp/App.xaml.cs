using Microsoft.Extensions.DependencyInjection;
using BankManagerApp.Converters;
using Microsoft.Extensions.DependencyInjection;
using BankManagerApp.Converters;
using BankManagerApp.Views;

namespace BankManagerApp;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		// Check if user is logged in
		bool isLoggedIn = Preferences.Get("IsLoggedIn", false);
		
		if (isLoggedIn)
		{
			return new Window(new AppShell());
		}
		else
		{
			return new Window(new NavigationPage(new LoginPage()));
		}
	}
}