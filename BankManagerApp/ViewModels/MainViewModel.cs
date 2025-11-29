using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BankManagerApp.Models;
using BankManagerApp.Views;

namespace BankManagerApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private BankAccount _selectedAccount;
        private int _nextId = 1;

        public ObservableCollection<BankAccount> Accounts { get; set; }

        public BankAccount SelectedAccount
        {
            get => _selectedAccount;
            set
            {
                _selectedAccount = value;
                OnPropertyChanged();
            }
        }

        public ICommand CreateAccountCommand { get; }
        public ICommand SelectAccountCommand { get; }

        public MainViewModel()
        {
            Accounts = new ObservableCollection<BankAccount>();
            CreateAccountCommand = new Command(async () => await CreateAccount());
            SelectAccountCommand = new Command<BankAccount>(async (account) => await SelectAccount(account));
            
            // حساب‌های نمونه برای تست
            CreateSampleAccounts();
        }

        private void CreateSampleAccounts()
        {
            var account1 = new BankAccount(_nextId++, "حساب جاری");
            account1.Deposit(5000000);
            Accounts.Add(account1);

            var account2 = new BankAccount(_nextId++, "حساب پس‌انداز");
            account2.Deposit(10000000);
            account2.Withdraw(2000000);
            Accounts.Add(account2);
        }

        private async Task CreateAccount()
        {
            var page = Shell.Current?.CurrentPage;
            if (page == null) return;
            
            string accountName = await page.DisplayPromptAsync(
                "حساب جدید",
                "نام حساب را وارد کنید:",
                placeholder: "مثال: حساب جاری",
                maxLength: 50);

            if (!string.IsNullOrWhiteSpace(accountName))
            {
                var newAccount = new BankAccount(_nextId++, accountName);
                Accounts.Add(newAccount);
                SelectedAccount = newAccount;
                
                await page.DisplayAlert(
                    "موفق",
                    $"حساب '{accountName}' با موفقیت ایجاد شد",
                    "باشه");
            }
        }

        private async Task SelectAccount(BankAccount account)
        {
            if (account != null)
            {
                await Shell.Current.GoToAsync(nameof(AccountDetailPage), new Dictionary<string, object>
                {
                    { "Account", account }
                });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
