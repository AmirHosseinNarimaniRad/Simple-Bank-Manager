using BankManagerApp.Services;
using System.Text.RegularExpressions;

namespace BankManagerApp.Views
{
    public partial class RegisterPage : ContentPage
    {
        private readonly AuthService _authService;
        private readonly DatabaseService _database;

        public RegisterPage(AuthService authService, DatabaseService database)
        {
            InitializeComponent();
            _authService = authService;
            _database = database;
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            try
            {
                RegisterButton.IsEnabled = false;
                RegisterButton.Text = "در حال ثبت‌نام...";

                var email = EmailEntry.Text?.Trim();
                var username = UsernameEntry.Text?.Trim();
                var password = PasswordEntry.Text;
                var confirmPassword = ConfirmPasswordEntry.Text;

                // Validate inputs
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(username) || 
                    string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
                {
                    await DisplayAlert("خطا", "لطفاً تمام فیلدها را پر کنید", "باشه");
                    return;
                }

                // Validate email format
                if (!IsValidEmail(email))
                {
                    await DisplayAlert("خطا", "فرمت ایمیل صحیح نیست", "باشه");
                    return;
                }

                // Validate username (alphanumeric only)
                if (!Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$"))
                {
                    await DisplayAlert("خطا", "نام کاربری فقط می‌تواند شامل حروف انگلیسی، اعداد و _ باشد", "باشه");
                    return;
                }

                if (username.Length < 3)
                {
                    await DisplayAlert("خطا", "نام کاربری باید حداقل 3 کاراکتر باشد", "باشه");
                    return;
                }

                // Validate password
                if (password.Length < 6)
                {
                    await DisplayAlert("خطا", "رمز عبور باید حداقل 6 کاراکتر باشد", "باشه");
                    return;
                }

                if (password != confirmPassword)
                {
                    await DisplayAlert("خطا", "رمز عبور و تکرار آن یکسان نیستند", "باشه");
                    return;
                }

                // Register user
                var (success, message, userId) = await _authService.RegisterAsync(email, username, password);

                if (success)
                {
                    // Auto-login after registration
                    _authService.SaveCurrentUserId(userId);
                    await DisplayAlert("موفق", "ثبت‌نام با موفقیت انجام شد", "باشه");
                    Application.Current!.MainPage = new NavigationPage(new MainPage(_database));
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
                RegisterButton.IsEnabled = true;
                RegisterButton.Text = "ثبت‌نام";
            }
        }

        private async void OnLoginTapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
