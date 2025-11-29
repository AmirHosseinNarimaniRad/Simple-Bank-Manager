using BankManagerApp.Models;
using BankManagerApp.Services;
using Microsoft.Maui.Controls.Shapes;

namespace BankManagerApp.Views
{
    [QueryProperty(nameof(AccountId), "accountId")]
    public partial class AccountDetailPage : ContentPage
    {
        private readonly DatabaseService _database;
        private BankAccountDb _account;
        private List<TransactionDb> _transactions;

        public string AccountId { get; set; }

        public AccountDetailPage()
        {
            InitializeComponent();
            _database = new DatabaseService();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadAccountData();
        }

        private async Task LoadAccountData()
        {
            if (int.TryParse(AccountId, out int accountId))
            {
                _account = await _database.GetAccountByIdAsync(accountId);
                if (_account != null)
                {
                    AccountNameLabel.Text = _account.Name;
                    UpdateBalance();
                    await LoadTransactions();
                }
            }
        }

        private async Task LoadTransactions()
        {
            _transactions = await _database.GetTransactionsAsync(_account.Id);
            DisplayTransactions();
        }

        private void DisplayTransactions()
        {
            TransactionsList.Children.Clear();

            if (_transactions == null || _transactions.Count == 0)
            {
                var emptyLabel = new Label
                {
                    Text = "No transactions yet",
                    FontSize = 14,
                    TextColor = Colors.Gray,
                    HorizontalTextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 20, 0, 0)
                };
                TransactionsList.Children.Add(emptyLabel);
                return;
            }

            foreach (var transaction in _transactions.Take(10))
            {
                var frame = new Border
                {
                    Padding = 15,
                    Margin = new Thickness(0, 0, 0, 10),
                    StrokeShape = new RoundRectangle { CornerRadius = 10 },
                    StrokeThickness = 0,
                    BackgroundColor = Color.FromArgb("#F5F5F5"),
                };

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

                var typeLabel = new Label
                {
                    Text = transaction.Type == "Deposit" ? "✅ Deposit" : "❌ Withdraw",
                    FontSize = 16,
                    FontAttributes = FontAttributes.Bold
                };
                Grid.SetColumn(typeLabel, 0);
                Grid.SetRow(typeLabel, 0);

                var amountLabel = new Label
                {
                    Text = $"{(transaction.Type == "Deposit" ? "+" : "-")}{transaction.Amount:N0} Toman",
                    FontSize = 16,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = transaction.Type == "Deposit" ? Color.FromArgb("#4CAF50") : Color.FromArgb("#FF5722")
                };
                Grid.SetColumn(amountLabel, 1);
                Grid.SetRow(amountLabel, 0);

                var dateLabel = new Label
                {
                    Text = transaction.DateTime.ToString("yyyy/MM/dd HH:mm"),
                    FontSize = 12,
                    TextColor = Colors.Gray,
                    Margin = new Thickness(0, 5, 0, 0)
                };
                Grid.SetColumn(dateLabel, 0);
                Grid.SetRow(dateLabel, 1);
                Grid.SetColumnSpan(dateLabel, 2);

                grid.Children.Add(typeLabel);
                grid.Children.Add(amountLabel);
                grid.Children.Add(dateLabel);

                frame.Content = grid;
                TransactionsList.Children.Add(frame);
            }
        }

        private async void OnDepositClicked(object sender, EventArgs e)
        {
            if (_account == null) return;

            if (decimal.TryParse(AmountEntry.Text, out decimal amount) && amount > 0)
            {
                _account.Balance += amount;
                await _database.SaveAccountAsync(_account);

                var transaction = new TransactionDb
                {
                    AccountId = _account.Id,
                    Type = "Deposit",
                    Amount = amount,
                    Description = "Deposit",
                    DateTime = DateTime.Now
                };
                await _database.SaveTransactionAsync(transaction);

                UpdateUI($"Deposit of {amount:N0} Toman successful", Colors.Green);
                AmountEntry.Text = "";
                await LoadTransactions();
            }
            else
            {
                UpdateUI("Please enter a valid amount", Colors.Red);
            }
        }

        private async void OnWithdrawClicked(object sender, EventArgs e)
        {
            if (_account == null) return;

            if (decimal.TryParse(AmountEntry.Text, out decimal amount) && amount > 0)
            {
                if (_account.Balance >= amount)
                {
                    _account.Balance -= amount;
                    await _database.SaveAccountAsync(_account);

                    var transaction = new TransactionDb
                    {
                        AccountId = _account.Id,
                        Type = "Withdraw",
                        Amount = amount,
                        Description = "Withdraw",
                        DateTime = DateTime.Now
                    };
                    await _database.SaveTransactionAsync(transaction);

                    UpdateUI($"Withdrawal of {amount:N0} Toman successful", Colors.Green);
                    AmountEntry.Text = "";
                    await LoadTransactions();
                }
                else
                {
                    UpdateUI("Insufficient balance", Colors.Red);
                }
            }
            else
            {
                UpdateUI("Please enter a valid amount", Colors.Red);
            }
        }

        private void UpdateBalance()
        {
            BalanceLabel.Text = $"{_account.Balance:N0} Toman";
        }

        private void UpdateUI(string message, Color color)
        {
            UpdateBalance();
            StatusLabel.Text = message;
            StatusLabel.TextColor = color;

            Task.Delay(3000).ContinueWith(_ =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    StatusLabel.Text = "";
                });
            });
        }
    }
}
