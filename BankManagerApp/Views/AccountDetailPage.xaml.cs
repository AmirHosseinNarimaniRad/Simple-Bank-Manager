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
        private string _currentTransactionType = "Income"; // "Income" or "Expense"
        
        // Calendar Fields
        private System.Globalization.PersianCalendar _persianCalendar;
        private int _currentYear;
        private int _currentMonth;
        private DateTime? _selectedDate;
        private bool _isYearView = false;

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
            
            // Initialize Calendar
            _persianCalendar = new System.Globalization.PersianCalendar();
            DateTime now = DateTime.Now;
            _currentYear = _persianCalendar.GetYear(now);
            _currentMonth = _persianCalendar.GetMonth(now);
            _selectedDate = null;
            _isYearView = false;

            UpdateTransactionTypeUI();
            RenderCalendar();
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
                var allTransactions = await _database.GetTransactionsAsync(_account.Id);
                
                // Filter by Date
                if (_selectedDate.HasValue)
                {
                    // Filter by specific day
                    _transactions = allTransactions.Where(t => t.DateTime.Date == _selectedDate.Value.Date).ToList();
                }
                else if (_isYearView)
                {
                    // Filter by current year
                    _transactions = allTransactions.Where(t => _persianCalendar.GetYear(t.DateTime) == _currentYear).ToList();
                }
                else
                {
                    // Filter by current month
                    _transactions = allTransactions.Where(t => 
                    {
                        var pYear = _persianCalendar.GetYear(t.DateTime);
                        var pMonth = _persianCalendar.GetMonth(t.DateTime);
                        return pYear == _currentYear && pMonth == _currentMonth;
                    }).ToList();
                }
                
                // Sort by date descending (newest first)
                _transactions = _transactions.OrderByDescending(t => t.DateTime).ToList();
                
                BindableLayout.SetItemsSource(TransactionsContainer, _transactions);
                
                if (EmptyStateLabel != null)
                {
                    EmptyStateLabel.IsVisible = _transactions.Count == 0;
                }
                
                UpdateStats();
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

                CategoryPicker.ItemsSource = new string[] { "خوراکی", "حمل‌ونقل", "خرید", "قبض", "تفریح", "سایر","بیف لاین" };
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
        // Calendar Methods
        private void RenderCalendar()
        {
            CalendarGrid.Children.Clear();
            
            // Update Labels
            string[] monthNames = { "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند" };
            CurrentMonthLabel.Text = monthNames[_currentMonth - 1];
            CurrentYearLabel.Text = _currentYear.ToString();

            // Highlight Year Label if in Year View
            if (_isYearView)
            {
                CurrentYearLabel.TextColor = Color.FromArgb("#512BD4");
                CurrentYearLabel.BackgroundColor = Color.FromArgb("#E8EAF6");
            }
            else
            {
                CurrentYearLabel.TextColor = Color.FromArgb("#333");
                CurrentYearLabel.BackgroundColor = Colors.Transparent;
            }

            // Get first day of the month
            DateTime firstDayOfMonth = _persianCalendar.ToDateTime(_currentYear, _currentMonth, 1, 0, 0, 0, 0);
            DayOfWeek startDayOfWeek = _persianCalendar.GetDayOfWeek(firstDayOfMonth);
            
            // Calculate offset (Saturday = 0, Sunday = 1, ...)
            int startCol = (int)startDayOfWeek + 1;
            if (startCol == 7) startCol = 0;

            int daysInMonth = _persianCalendar.GetDaysInMonth(_currentYear, _currentMonth);

            for (int day = 1; day <= daysInMonth; day++)
            {
                int row = (day + startCol - 1) / 7;
                int col = (day + startCol - 1) % 7;

                var dayButton = new Button
                {
                    Text = day.ToString(),
                    BackgroundColor = Colors.Transparent,
                    TextColor = Color.FromArgb("#333"),
                    CornerRadius = 20,
                    WidthRequest = 40,
                    HeightRequest = 40,
                    Padding = 0,
                    FontSize = 14
                };

                // Highlight selected day
                DateTime thisDate = _persianCalendar.ToDateTime(_currentYear, _currentMonth, day, 0, 0, 0, 0);
                
                // Check if it is Today
                bool isToday = thisDate.Date == DateTime.Now.Date;

                if (_selectedDate.HasValue && _selectedDate.Value.Date == thisDate.Date)
                {
                    // Selected Day Style
                    dayButton.BackgroundColor = Color.FromArgb("#512BD4");
                    dayButton.TextColor = Colors.White;
                }
                else if (isToday)
                {
                    // Today Style (Distinct Highlight)
                    dayButton.BackgroundColor = Color.FromArgb("#E3F2FD"); // Light Blue
                    dayButton.TextColor = Color.FromArgb("#1565C0"); // Dark Blue
                    dayButton.BorderColor = Color.FromArgb("#1565C0");
                    dayButton.BorderWidth = 1;
                    dayButton.FontAttributes = FontAttributes.Bold;
                }

                dayButton.Clicked += (s, e) => OnDayClicked(thisDate);

                Grid.SetRow(dayButton, row);
                Grid.SetColumn(dayButton, col);
                CalendarGrid.Children.Add(dayButton);
            }
            
            UpdateStats();
        }

        private void OnNextMonthClicked(object sender, EventArgs e)
        {
            // In RTL, "Next" (Left Arrow) should go to Next Month
            _currentMonth++;
            if (_currentMonth > 12)
            {
                _currentMonth = 1;
                _currentYear++;
            }
            _selectedDate = null; 
            _isYearView = false;
            RenderCalendar();
            _ = LoadTransactions();
        }

        private void OnPrevMonthClicked(object sender, EventArgs e)
        {
            // In RTL, "Prev" (Right Arrow) should go to Previous Month
            _currentMonth--;
            if (_currentMonth < 1)
            {
                _currentMonth = 12;
                _currentYear--;
            }
            _selectedDate = null;
            _isYearView = false;
            RenderCalendar();
            _ = LoadTransactions();
        }

        private void OnNextYearClicked(object sender, EventArgs e)
        {
            _currentYear++;
            _selectedDate = null;
            // Keep current view mode (Year or Month)
            RenderCalendar();
            _ = LoadTransactions();
        }

        private void OnPrevYearClicked(object sender, EventArgs e)
        {
            _currentYear--;
            _selectedDate = null;
            RenderCalendar();
            _ = LoadTransactions();
        }

        private void OnViewYearStatsClicked(object sender, EventArgs e)
        {
            _isYearView = !_isYearView; // Toggle
            _selectedDate = null; // Clear day selection
            RenderCalendar();
            _ = LoadTransactions();
        }

        private void OnDayClicked(DateTime date)
        {
            if (_selectedDate.HasValue && _selectedDate.Value.Date == date.Date)
            {
                _selectedDate = null; // Deselect
            }
            else
            {
                _selectedDate = date;
                _isYearView = false; // Switch to day view
            }
            RenderCalendar();
            _ = LoadTransactions();
        }

        private void OnToggleCalendarClicked(object sender, EventArgs e)
        {
            CalendarContent.IsVisible = !CalendarContent.IsVisible;
            ToggleCalendarButton.Text = CalendarContent.IsVisible ? "▲" : "▼";
        }

        private void UpdateStats()
        {
            if (_transactions == null) return;

            decimal totalIncome = _transactions
                .Where(t => !string.IsNullOrEmpty(t.Type) && 
                           (t.Type.Trim().Equals("Income", StringComparison.OrdinalIgnoreCase) || 
                            t.Type.Trim().Equals("Deposit", StringComparison.OrdinalIgnoreCase)))
                .Sum(t => t.Amount);

            decimal totalExpense = _transactions
                .Where(t => !string.IsNullOrEmpty(t.Type) && 
                           (t.Type.Trim().Equals("Expense", StringComparison.OrdinalIgnoreCase) || 
                            t.Type.Trim().Equals("Withdraw", StringComparison.OrdinalIgnoreCase)))
                .Sum(t => t.Amount);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                SelectedDateIncomeLabel.Text = $"{totalIncome:N0} تومان";
                SelectedDateExpenseLabel.Text = $"{totalExpense:N0} تومان";

                // Update Summary Label
                string[] monthNames = { "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند" };
                if (_selectedDate.HasValue)
                {
                    int pYear = _persianCalendar.GetYear(_selectedDate.Value);
                    int pMonth = _persianCalendar.GetMonth(_selectedDate.Value);
                    int pDay = _persianCalendar.GetDayOfMonth(_selectedDate.Value);
                    
                    CalendarSummaryLabel.Text = $"آمار روز {pDay} {monthNames[pMonth - 1]} {pYear}";
                }
                else if (_isYearView)
                {
                    CalendarSummaryLabel.Text = $"آمار کل سال {_currentYear}";
                }
                else
                {
                    CalendarSummaryLabel.Text = $"آمار ماه {monthNames[_currentMonth - 1]} {_currentYear}";
                }
            });
        }
    }
}
