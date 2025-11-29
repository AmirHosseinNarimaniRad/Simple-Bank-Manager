using BankManagerApp.Models;
using BankManagerApp.Services;
using Microsoft.Maui.Controls.Shapes;

namespace BankManagerApp.Views
{
    [QueryProperty(nameof(AccountId), "accountId")]
    public partial class AccountDetailPage : ContentPage
    {
        private readonly DatabaseService _database;
        private Wallet _account;
        private List<TransactionDb> _transactions;
        private string _currentTransactionType = "Income"; // Default to Income (واریز)

        public int AccountId
        {
            set
            {
                LoadAccount(value);
            }
        }

        public AccountDetailPage()
        {
            InitializeComponent();
            _database = new DatabaseService();
            UpdateTransactionTypeUI();
        }

        private async void LoadAccount(int accountId)
        {
            _account = await _database.GetWalletAsync(accountId);
            if (_account != null)
            {
                AccountNameLabel.Text = _account.Name;
                UpdateBalance();
                await LoadTransactions();
            }
        }

        private void UpdateBalance()
        {
            if (_account != null)
            {
                BalanceLabel.Text = $"{_account.Balance:N0} تومان";
            }
        }

        private async Task LoadTransactions()
        {
            if (_account != null)
            {
                _transactions = await _database.GetTransactionsAsync(_account.Id);
                
                // Sort by date descending (newest first)
                _transactions = _transactions.OrderByDescending(t => t.DateTime).ToList();
                
                BindableLayout.SetItemsSource(TransactionsContainer, _transactions);
                
                if (EmptyStateLabel != null)
                {
                    EmptyStateLabel.IsVisible = _transactions.Count == 0;
                }
            }
        }

        private void OnTransactionTypeChanged(object sender, EventArgs e)
        {
            if (sender == IncomeTab)
            {
                _currentTransactionType = "Income";
            }
            else if (sender == ExpenseTab)
            {
                _currentTransactionType = "Expense";
            }
            UpdateTransactionTypeUI();
        }

        private void UpdateTransactionTypeUI()
        {
            if (_currentTransactionType == "Income")
            {
                IncomeTab.BackgroundColor = Color.FromArgb("#E8F5E9");
                IncomeTab.TextColor = Color.FromArgb("#2E7D32");
                ExpenseTab.BackgroundColor = Color.FromArgb("#F5F5F5");
                ExpenseTab.TextColor = Color.FromArgb("#666");
                
                CategoryPicker.ItemsSource = new string[] { "حقوق", "واریز ریالی", "دنگ", "نقدی", "سایر" };
            }
            else
            {
                IncomeTab.BackgroundColor = Color.FromArgb("#F5F5F5");
                IncomeTab.TextColor = Color.FromArgb("#666");
                ExpenseTab.BackgroundColor = Color.FromArgb("#FFEBEE");
                ExpenseTab.TextColor = Color.FromArgb("#C62828");

                CategoryPicker.ItemsSource = new string[] { "خوراکی", "حمل‌ونقل", "خرید", "قبض", "تفریح", "سایر" };
            }
            CategoryPicker.SelectedIndex = 0;
        }

        private async void OnSubmitTransactionClicked(object sender, EventArgs e)
        {
            if (_account == null) return;

            if (decimal.TryParse(AmountEntry.Text, out decimal amount) && amount > 0)
            {
                // REMOVED: Balance check - allow negative balance
                // This allows users to track overspending

                // Update Balance
                if (_currentTransactionType == "Income")
                    _account.Balance += amount;
                else
                    _account.Balance -= amount;

                await _database.SaveWalletAsync(_account);

                // Save Transaction
                var transaction = new TransactionDb
                {
                    AccountId = _account.Id,
                    Type = _currentTransactionType == "Income" ? "Deposit" : "Withdraw",
                    Category = CategoryPicker.SelectedItem?.ToString() ?? "سایر",
                    IncomeType = _currentTransactionType == "Income" ? CategoryPicker.SelectedItem?.ToString() : null,
                    Amount = amount,
                    Description = DescriptionEntry.Text,
                    DateTime = DateTime.Now
                };
                await _database.SaveTransactionAsync(transaction);

                // UI Feedback
                UpdateBalance();
                AmountEntry.Text = "";
                DescriptionEntry.Text = "";
                await LoadTransactions();
                
                await DisplayAlert("موفق", "تراکنش با موفقیت ثبت شد", "باشه");
            }
            else
            {
                await DisplayAlert("خطا", "لطفاً مبلغ معتبر وارد کنید", "باشه");
            }
        }

        private async void OnDeleteWalletClicked(object sender, EventArgs e)
        {
            if (_account == null) return;

            bool confirm = await DisplayAlert("حذف کیف پول", 
                $"آیا مطمئن هستید که می‌خواهید کیف پول '{_account.Name}' را حذف کنید؟\nاین عملیات غیرقابل بازگشت است و تمام تراکنش‌های آن پاک خواهد شد.", 
                "بله، حذف کن", 
                "خیر");

            if (confirm)
            {
                await _database.DeleteWalletAsync(_account);
                await Navigation.PopAsync();
            }
        }
    }
}
