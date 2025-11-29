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
		// Use NavigationPage instead of Shell to avoid XAML issues
		return new Window(new NavigationPage(new Views.MainPage())
		{
			BarBackgroundColor = Color.FromArgb("#512BD4"),
			BarTextColor = Colors.White
		});
	}
}