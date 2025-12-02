using BankManager.Data.Entities;
using BankManagerApp.Services;
using Microsoft.Extensions.DependencyInjection; // Added for GetRequiredService

namespace BankManagerApp.Views
{
    public partial class LoginPage : ContentPage
    {
        private readonly DatabaseService _database;
        private readonly IServiceProvider _serviceProvider;

        public LoginPage(DatabaseService database, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _database = database;
            _serviceProvider = serviceProvider;
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            ErrorLabel.IsVisible = false;

            var phoneNumber = PhoneEntry.Text?.Trim();

            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                ShowError("Please enter your phone number");
                return;
            }

            // Check if user exists
            var user = await _database.GetUserByPhoneAsync(phoneNumber);

            if (user == null)
            {
                ShowError("User not found with this phone number");
                return;
            }

            // Save current user ID in preferences
            Preferences.Set("CurrentUserId", user.Id);
            Preferences.Set("IsLoggedIn", true);

            // Navigate to main page
            Application.Current.MainPage = new AppShell();
        }

        private async void OnRegisterTapped(object sender, EventArgs e)
        {
            var registerPage = _serviceProvider.GetRequiredService<RegisterPage>();
            await Navigation.PushAsync(registerPage);
        }

        private void ShowError(string message)
        {
            ErrorLabel.Text = message;
            ErrorLabel.IsVisible = true;
        }
    }
}
