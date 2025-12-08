using BankManager.Data;
using BankManager.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankManagerApp.Services
{
    public class AuthService
    {
        private readonly BankDbContext _context;
        private const string CurrentUserIdKey = "CurrentUserId";

        public AuthService(BankDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Hash a password using BCrypt
        /// </summary>
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// <summary>
        /// Verify a password against a hash
        /// </summary>
        public bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        public async Task<(bool Success, string Message, int UserId)> RegisterAsync(string email, string username, string password)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    return (false, "تمام فیلدها الزامی هستند", 0);
                }

                if (password.Length < 6)
                {
                    return (false, "رمز عبور باید حداقل 6 کاراکتر باشد", 0);
                }

                // Check if email already exists
                var existingEmail = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (existingEmail != null)
                {
                    return (false, "این ایمیل قبلاً ثبت شده است", 0);
                }

                // Check if username already exists
                var existingUsername = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
                if (existingUsername != null)
                {
                    return (false, "این نام کاربری قبلاً ثبت شده است", 0);
                }

                // Create new user
                var user = new User
                {
                    Email = email.ToLower().Trim(),
                    Username = username.ToLower().Trim(),
                    PasswordHash = HashPassword(password),
                    MonthlyBudget = 0
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return (true, "ثبت‌نام با موفقیت انجام شد", user.Id);
            }
            catch (Exception ex)
            {
                return (false, $"خطا در ثبت‌نام: {ex.Message}", 0);
            }
        }

        /// <summary>
        /// Login with email/username and password
        /// </summary>
        public async Task<(bool Success, string Message, int UserId)> LoginAsync(string emailOrUsername, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(emailOrUsername) || string.IsNullOrWhiteSpace(password))
                {
                    return (false, "ایمیل/نام کاربری و رمز عبور الزامی هستند", 0);
                }

                var input = emailOrUsername.ToLower().Trim();

                // Find user by email or username
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == input || u.Username == input);

                if (user == null)
                {
                    return (false, "کاربری با این مشخصات یافت نشد", 0);
                }

                // Verify password
                if (!VerifyPassword(password, user.PasswordHash))
                {
                    return (false, "رمز عبور اشتباه است", 0);
                }

                return (true, "ورود موفقیت‌آمیز", user.Id);
            }
            catch (Exception ex)
            {
                return (false, $"خطا در ورود: {ex.Message}", 0);
            }
        }

        /// <summary>
        /// Generate a password reset token
        /// </summary>
        public async Task<(bool Success, string Message, string Token)> GeneratePasswordResetTokenAsync(string email)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email.ToLower().Trim());
                if (user == null)
                {
                    return (false, "کاربری با این ایمیل یافت نشد", string.Empty);
                }

                // Generate a random 6-digit token
                var token = new Random().Next(100000, 999999).ToString();
                user.PasswordResetToken = token;
                user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1); // Token valid for 1 hour

                await _context.SaveChangesAsync();

                return (true, "کد بازیابی ارسال شد", token);
            }
            catch (Exception ex)
            {
                return (false, $"خطا: {ex.Message}", string.Empty);
            }
        }

        /// <summary>
        /// Reset password using token
        /// </summary>
        public async Task<(bool Success, string Message)> ResetPasswordAsync(string email, string token, string newPassword)
        {
            try
            {
                if (newPassword.Length < 6)
                {
                    return (false, "رمز عبور باید حداقل 6 کاراکتر باشد");
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email.ToLower().Trim());
                if (user == null)
                {
                    return (false, "کاربری با این ایمیل یافت نشد");
                }

                if (user.PasswordResetToken != token)
                {
                    return (false, "کد بازیابی اشتباه است");
                }

                if (user.PasswordResetTokenExpiry == null || user.PasswordResetTokenExpiry < DateTime.UtcNow)
                {
                    return (false, "کد بازیابی منقضی شده است");
                }

                // Reset password
                user.PasswordHash = HashPassword(newPassword);
                user.PasswordResetToken = null;
                user.PasswordResetTokenExpiry = null;

                await _context.SaveChangesAsync();

                return (true, "رمز عبور با موفقیت تغییر کرد");
            }
            catch (Exception ex)
            {
                return (false, $"خطا: {ex.Message}");
            }
        }

        /// <summary>
        /// Save current user ID to preferences
        /// </summary>
        public void SaveCurrentUserId(int userId)
        {
            Preferences.Set(CurrentUserIdKey, userId);
        }

        /// <summary>
        /// Get current logged-in user ID
        /// </summary>
        public int GetCurrentUserId()
        {
            return Preferences.Get(CurrentUserIdKey, 0);
        }

        /// <summary>
        /// Check if user is logged in
        /// </summary>
        public bool IsLoggedIn()
        {
            return GetCurrentUserId() > 0;
        }

        /// <summary>
        /// Logout current user
        /// </summary>
        public void Logout()
        {
            Preferences.Remove(CurrentUserIdKey);
        }
    }
}
