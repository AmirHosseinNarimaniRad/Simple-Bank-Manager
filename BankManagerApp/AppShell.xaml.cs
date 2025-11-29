using BankManagerApp.Views;

namespace BankManagerApp;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		
		Routing.RegisterRoute(nameof(AccountDetailPage), typeof(AccountDetailPage));
		Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
		Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
	}
}
