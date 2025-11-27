using Microsoft.Extensions.DependencyInjection;
using BankManagerApp.Converters;

namespace BankManagerApp;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}
}