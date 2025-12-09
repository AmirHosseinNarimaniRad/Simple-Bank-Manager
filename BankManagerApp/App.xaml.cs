using BankManagerApp.Services;
using BankManagerApp.Views;

namespace BankManagerApp
{
    public partial class App : Application
    {
        public App(AuthService authService, DatabaseService database)
        {
            InitializeComponent();

            // Initialize database
            _ = database.Init();

            // Check if user is logged in
            if (authService.IsLoggedIn())
            {
                // User is logged in, go to MainPage
                MainPage = new NavigationPage(new MainPage(database, authService));
            }
            else
            {
                // User is not logged in, go to LoginPage
                MainPage = new NavigationPage(new LoginPage(authService, database));
            }
        }
    }
}