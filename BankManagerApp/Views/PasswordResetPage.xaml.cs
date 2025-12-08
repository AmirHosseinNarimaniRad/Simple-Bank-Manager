using BankManagerApp.Services;

namespace BankManagerApp.Views
{
    public partial class PasswordResetPage : ContentPage
    {
        private readonly AuthService _authService;
        private string? _currentEmail;

        public PasswordResetPage(AuthService authService)
        {
            InitializeComponent();
            _authService = authService;
        }

        private async void OnSendTokenClicked(object sender, EventArgs e)
        {
            try
            {
                SendTokenButton.IsEnabled = false;
                SendTokenButton.Text = "در حال ارسال...";

                var email = EmailEntry.Text?.Trim();

                if (string.IsNullOrWhiteSpace(email))
                {
                    await DisplayAlert("خطا", "لطفاً ایمیل خود را وارد کنید", "باشه");
                    return;
                }

                var (success, message, token) = await _authService.GeneratePasswordResetTokenAsync(email);

                if (success)
                {
                    _currentEmail = email;
                    
                    // Show token (in production, this would be sent via email)
                    TokenDisplayLabel.Text = $"کد بازیابی شما: {token}";
                    TokenDisplayLabel.IsVisible = true;
                    
                    // Show step 2
                    Step2Container.IsVisible = true;
                    
                    await DisplayAlert("موفق", "کد بازیابی ارسال شد. در برنامه واقعی، این کد به ایمیل شما ارسال می‌شود.", "باشه");
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
                SendTokenButton.IsEnabled = true;
                SendTokenButton.Text = "ارسال کد بازیابی";
            }
        }

        private async void OnResetPasswordClicked(object sender, EventArgs e)
        {
            try
            {
                ResetPasswordButton.IsEnabled = false;
                ResetPasswordButton.Text = "در حال تغییر...";

                var token = TokenEntry.Text?.Trim();
                var newPassword = NewPasswordEntry.Text;

                if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(newPassword))
                {
                    await DisplayAlert("خطا", "لطفاً کد و رمز عبور جدید را وارد کنید", "باشه");
                    return;
                }

                if (string.IsNullOrWhiteSpace(_currentEmail))
                {
                    await DisplayAlert("خطا", "لطفاً ابتدا کد بازیابی را دریافت کنید", "باشه");
                    return;
                }

                var (success, message) = await _authService.ResetPasswordAsync(_currentEmail, token, newPassword);

                if (success)
                {
                    await DisplayAlert("موفق", "رمز عبور با موفقیت تغییر کرد. اکنون می‌توانید وارد شوید.", "باشه");
                    await Navigation.PopToRootAsync();
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
                ResetPasswordButton.IsEnabled = true;
                ResetPasswordButton.Text = "تغییر رمز عبور";
            }
        }

        private async void OnBackToLoginTapped(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }
    }
}
