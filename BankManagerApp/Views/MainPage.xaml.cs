using BankManager.Data.Entities;
using BankManagerApp.Services;
using Microsoft.Maui.Controls.Shapes;

namespace BankManagerApp.Views
{
    public partial class MainPage : ContentPage
    {
        private readonly DatabaseService _database;
        private readonly IServiceProvider _serviceProvider;
        private List<Wallet>? _accounts;

        public MainPage(DatabaseService database, IServiceProvider serviceProvider)
        {
            try
            {
                InitializeComponent();
                _database = database;
                _serviceProvider = serviceProvider;
                Console.WriteLine("MainPage: Constructor completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"MainPage Constructor CRASH: {ex}");
                throw;
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            Console.WriteLine("MainPage: OnAppearing started");
            
            try
            {
                // Set simple date
                if (DateLabel != null)
                {
                    DateLabel.Text = "📅 امروز";
                }
                
                // Set welcome message
                if (WelcomeLabel != null)
                {
                    WelcomeLabel.Text = "سلام، کاربر عزیز";
                }
                if (BudgetLabel != null)
                {
                    BudgetLabel.Text = "0 تومان";
                }
                
                // Load accounts for default user
                int userId = 1;
                _accounts = await _database.GetWalletsAsync(userId);
                
                // Update summary
                if (_accounts != null && TotalBalanceLabel != null)
                {
                    decimal totalBalance = _accounts.Sum(a => a.Balance);
                    TotalBalanceLabel.Text = $"{totalBalance:N0} تومان";
                }
                
                // Display wallets manually (no CollectionView)
                DisplayWallets();
                
                Console.WriteLine("MainPage: OnAppearing completed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"MainPage OnAppearing CRASH: {ex}");
                try
                {
                    await DisplayAlert("خطا", $"خطا: {ex.Message}", "باشه");
                }
                catch { }
            }
        }

        private void DisplayWallets()
        {
            try
            {
                if (WalletsContainer == null) return;
                
                WalletsContainer.Clear();
                
                if (_accounts == null || _accounts.Count == 0)
                {
                    WalletsContainer.Add(new Label 
                    { 
                        Text = "هنوز کیف پولی ساخته نشده است",
                        TextColor = Colors.Gray,
                        HorizontalOptions = LayoutOptions.Center,
                        Margin = new Thickness(0, 20)
                    });
                }
                else
                {
                    foreach (var wallet in _accounts)
                    {
                        var border = new Border
                        {
                            StrokeShape = new RoundRectangle { CornerRadius = 10 },
                            BackgroundColor = Colors.White,
                            StrokeThickness = 0,
                            Padding = 15,
                            Margin = new Thickness(0, 0, 0, 10)
                        };

                        var grid = new Grid
                        {
                            ColumnDefinitions =
                            {
                                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                                new ColumnDefinition { Width = GridLength.Auto },
                                new ColumnDefinition { Width = GridLength.Auto }
                            },
                            ColumnSpacing = 10
                        };

                        var nameLabel = new Label
                        {
                            Text = wallet.Name,
                            FontSize = 16,
                            FontAttributes = FontAttributes.Bold,
                            TextColor = Color.FromArgb("#333"),
                            VerticalOptions = LayoutOptions.Center
                        };
                        Grid.SetColumn(nameLabel, 0);

                        var balanceLabel = new Label
                        {
                            Text = $"{wallet.Balance:N0} تومان",
                            FontSize = 14,
                            TextColor = Color.FromArgb("#512BD4"),
                            VerticalOptions = LayoutOptions.Center
                        };
                        Grid.SetColumn(balanceLabel, 1);

                        var deleteButton = new Button
                        {
                            Text = "🗑",
                            BackgroundColor = Colors.Transparent,
                            TextColor = Colors.Red,
                            FontSize = 16,
                            Padding = 0,
                            WidthRequest = 40,
                            HeightRequest = 40,
                            VerticalOptions = LayoutOptions.Center
                        };
                        deleteButton.Clicked += async (s, e) => 
                        {
                            bool confirm = await DisplayAlert("حذف", $"آیا کیف پول '{wallet.Name}' حذف شود؟", "بله", "خیر");
                            if (confirm)
                            {
                                await _database.DeleteWalletAsync(wallet);
                                OnAppearing(); // Refresh list
                            }
                        };
                        Grid.SetColumn(deleteButton, 2);

                        grid.Add(nameLabel);
                        grid.Add(balanceLabel);
                        grid.Add(deleteButton);
                        border.Content = grid;

                        // Add tap gesture to the border (excluding the button area effectively)
                        // Note: Button click handles itself, tap on border handles navigation
                        var tapGesture = new TapGestureRecognizer();
                        tapGesture.Tapped += async (s, e) =>
                        {
                            var detailPage = _serviceProvider.GetRequiredService<AccountDetailPage>();
                            detailPage.AccountId = wallet.Id;
                            await Navigation.PushAsync(detailPage);
                        };
                        border.GestureRecognizers.Add(tapGesture);

                        WalletsContainer.Add(border);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DisplayWallets CRASH: {ex}");
            }
        }

        private async void OnCreateAccountClicked(object sender, EventArgs e)
        {
            try
            {
                Console.WriteLine("OnCreateAccountClicked: Started");
                
                string? accountName = await DisplayPromptAsync(
                    "کیف پول جدید",
                    "نام کیف پول را وارد کنید:",
                    placeholder: "مثال: پول نقد",
                    maxLength: 20,
                    accept: "ساختن",
                    cancel: "لغو");

                if (!string.IsNullOrWhiteSpace(accountName))
                {
                    int userId = 1;
                    
                    var newAccount = new Wallet
                    {
                        UserId = userId,
                        Name = accountName,
                        Balance = 0
                        // CreatedAt and UpdatedAt will be set automatically by BankDbContext
                    };

                    await _database.SaveWalletAsync(newAccount);
                    
                    // Reload accounts
                    _accounts = await _database.GetWalletsAsync(userId);
                    
                    // Update summary
                    if (_accounts != null && TotalBalanceLabel != null)
                    {
                        decimal totalBalance = _accounts.Sum(a => a.Balance);
                        TotalBalanceLabel.Text = $"{totalBalance:N0} تومان";
                    }

                    // Refresh display
                    DisplayWallets();

                    await DisplayAlert("موفق", $"کیف پول '{accountName}' ساخته شد", "باشه");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"OnCreateAccountClicked CRASH: {ex}");
                var message = ex.InnerException != null ? $"{ex.Message}\n\nInner: {ex.InnerException.Message}" : ex.Message;
                await DisplayAlert("خطا", $"مشکل: {message}", "باشه");
            }
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            try
            {
                bool answer = await DisplayAlert("خروج", "آیا می‌خواهید از برنامه خارج شوید؟", "بله", "خیر");
                if (answer)
                {
                    Application.Current?.Quit();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"OnLogoutClicked CRASH: {ex}");
            }
        }
    }
}
