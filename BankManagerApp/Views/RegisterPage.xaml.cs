using BankManagerApp.Models;
using BankManagerApp.Services;

namespace BankManagerApp.Views
{
    public partial class RegisterPage : ContentPage
    {
        private readonly DatabaseService _database;

        public RegisterPage()
        {
            InitializeComponent();
            _database = new DatabaseService();
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            ErrorLabel.IsVisible = false;

            var firstName = FirstNameEntry.Text?.Trim();
            var lastName = LastNameEntry.Text?.Trim();
            var phoneNumber = PhoneEntry.Text?.Trim();

            // Validation
            if (string.IsNullOrWhiteSpace(firstName))
            {
                ShowError("Please enter your first name");
                return;
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                ShowError("Please enter your last name");
                return;
            }

            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                ShowError("Please enter your phone number");
                return;
            }

            if (phoneNumber.Length != 11 || !phoneNumber.StartsWith("09"))
            {
                ShowError("Phone number must be 11 digits and start with 09");
                return;
            }

            // Check if phone number already exists
            var existingUser = await _database.GetUserByPhoneAsync(phoneNumber);
            if (existingUser != null)
            {
                ShowError("This phone number is already registered");
                return;
            }

            // Create new user
            var newUser = new User
            {
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phoneNumber,
                CreatedAt = DateTime.Now
            };

            await _database.SaveUserAsync(newUser);

            // Get the created user to get the ID
            var user = await _database.GetUserByPhoneAsync(phoneNumber);

            // Save current user ID in preferences
            Preferences.Set("CurrentUserId", user.Id);
            Preferences.Set("IsLoggedIn", true);

            // Navigate to main page
            Application.Current.MainPage = new AppShell();
        }

        private async void OnLoginTapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private void ShowError(string message)
        {
            ErrorLabel.Text = message;
            ErrorLabel.IsVisible = true;
        }
    }
}
