namespace BankManagerApp;

using BankManagerApp.Views;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		
		Routing.RegisterRoute(nameof(AccountDetailPage), typeof(AccountDetailPage));
	}
}
