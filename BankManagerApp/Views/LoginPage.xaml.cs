using BankManagerApp.Services;

namespace BankManagerApp.Views
{
    public partial class LoginPage : ContentPage
    {
        private readonly AuthService _authService;
        private readonly DatabaseService _database;

        public LoginPage(AuthService authService, DatabaseService database)
        {
            InitializeComponent();
            _authService = authService;
            _database = database;
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            try
            {
                LoginButton.IsEnabled = false;
                LoginButton.Text = "در حال ورود...";

                var emailOrUsername = EmailUsernameEntry.Text?.Trim();
                var password = PasswordEntry.Text;

                if (string.IsNullOrWhiteSpace(emailOrUsername) || string.IsNullOrWhiteSpace(password))
                {
                    await DisplayAlert("خطا", "لطفاً تمام فیلدها را پر کنید", "باشه");
                    return;
                }

                var (success, message, userId) = await _authService.LoginAsync(emailOrUsername, password);

                if (success)
                {
                    _authService.SaveCurrentUserId(userId);
                    Application.Current!.MainPage = new NavigationPage(new MainPage(_database, _authService));
                }
                else
                {
                    await DisplayAlert("خطا", message, "باشه");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("خطا", $"خطای غیرمنتظره: {ex.Message}", "باشه");
            }
            finally
            {
                LoginButton.IsEnabled = true;
                LoginButton.Text = "ورود";
            }
        }

        private async void OnRegisterTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegisterPage(_authService, _database));
        }

        private async void OnForgotPasswordTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PasswordResetPage(_authService));
        }
    }
}
