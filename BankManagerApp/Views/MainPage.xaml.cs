using BankManagerApp.Models;
using BankManagerApp.Services;
using Microsoft.Maui.Controls.Shapes;

namespace BankManagerApp.Views
{
    public partial class MainPage : ContentPage
    {
        private readonly DatabaseService _database;
        private User _currentUser;
        private List<BankAccountDb> _accounts;

        public MainPage()
        {
            InitializeComponent();
            _database = new DatabaseService();
            LoadUserData();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadAccounts();
        }

        private async void LoadUserData()
        {
            var userId = Preferences.Get("CurrentUserId", 0);
            if (userId > 0)
            {
                _currentUser = await _database.GetUserByIdAsync(userId);
                if (_currentUser != null)
                {
                    WelcomeLabel.Text = $"Hello, {_currentUser.FirstName}";
                }
            }
        }

        private async void LoadAccounts()
        {
            var userId = Preferences.Get("CurrentUserId", 0);
            if (userId > 0)
            {
                _accounts = await _database.GetAccountsAsync(userId);
                DisplayAccounts();
            }
        }

        private void DisplayAccounts()
        {
            AccountsList.Children.Clear();

            if (_accounts == null || _accounts.Count == 0)
            {
                var emptyLabel = new Label
                {
                    Text = "You haven't created any accounts yet\nClick the button below",
                    FontSize = 16,
                    TextColor = Colors.Gray,
                    HorizontalTextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 50, 0, 0)
                };
                AccountsList.Children.Add(emptyLabel);
                return;
            }

            foreach (var account in _accounts)
            {
                var frame = new Border
                {
                    Padding = 20,
                    StrokeShape = new RoundRectangle { CornerRadius = 15 },
                    StrokeThickness = 0,
                    BackgroundColor = Colors.White,
                    Margin = new Thickness(0, 0, 0, 15),
                    Shadow = new Shadow { Brush = Brush.Black, Offset = new Point(0, 2), Radius = 5, Opacity = 0.1f }
                };

                var tapGesture = new TapGestureRecognizer();
                tapGesture.Tapped += (s, e) => OnAccountTapped(account);
                frame.GestureRecognizers.Add(tapGesture);

                var grid = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                        new ColumnDefinition { Width = GridLength.Auto }
                    },
                    RowDefinitions =
                    {
                        new RowDefinition { Height = GridLength.Auto },
                        new RowDefinition { Height = GridLength.Auto }
                    }
                };

                var icon = new Label
                {
                    Text = "💳",
                    FontSize = 50,
                    VerticalOptions = LayoutOptions.Center
                };
                Grid.SetColumn(icon, 1);
                Grid.SetRowSpan(icon, 2);

                var nameLabel = new Label
                {
                    Text = account.Name,
                    FontSize = 20,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.FromArgb("#512BD4")
                };
                Grid.SetColumn(nameLabel, 0);
                Grid.SetRow(nameLabel, 0);

                var balanceLabel = new Label
                {
                    Text = $"{account.Balance:N0} Toman",
                    FontSize = 24,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.FromArgb("#2B0B98"),
                    Margin = new Thickness(0, 10, 0, 0)
                };
                Grid.SetColumn(balanceLabel, 0);
                Grid.SetRow(balanceLabel, 1);

                grid.Children.Add(icon);
                grid.Children.Add(nameLabel);
                grid.Children.Add(balanceLabel);

                frame.Content = grid;
                AccountsList.Children.Add(frame);
            }
        }

        private async void OnAccountTapped(BankAccountDb account)
        {
            await Shell.Current.GoToAsync($"AccountDetailPage?accountId={account.Id}");
        }

        private async void OnCreateAccountClicked(object sender, EventArgs e)
        {
            string accountName = await DisplayPromptAsync(
                "New Account",
                "Enter account name:",
                placeholder: "e.g., Checking Account");

            if (!string.IsNullOrWhiteSpace(accountName))
            {
                var userId = Preferences.Get("CurrentUserId", 0);
                var newAccount = new BankAccountDb
                {
                    UserId = userId,
                    Name = accountName,
                    Balance = 0,
                    CreatedAt = DateTime.Now
                };

                await _database.SaveAccountAsync(newAccount);
                LoadAccounts();

                await DisplayAlert("Success", $"Account '{accountName}' created successfully", "OK");
            }
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Logout", "Are you sure?", "Yes", "No");
            if (answer)
            {
                Preferences.Remove("CurrentUserId");
                Preferences.Set("IsLoggedIn", false);
                Application.Current.MainPage = new NavigationPage(new LoginPage());
            }
        }
    }
}
