using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BankManagerApp.Models;

namespace BankManagerApp.ViewModels
{
    [QueryProperty(nameof(Account), "Account")]
    public class AccountDetailViewModel : INotifyPropertyChanged
    {
        private BankAccount _account;
        private string _amount;
        private string _statusMessage;

        public BankAccount Account
        {
            get => _account;
            set
            {
                _account = value;
                OnPropertyChanged();
            }
        }

        public string Amount
        {
            get => _amount;
            set
            {
                _amount = value;
                OnPropertyChanged();
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        public ICommand DepositCommand { get; }
        public ICommand WithdrawCommand { get; }

        public AccountDetailViewModel()
        {
            DepositCommand = new Command(async () => await Deposit());
            WithdrawCommand = new Command(async () => await Withdraw());
        }

        private async Task Deposit()
        {
            if (Account == null)
                return;

            if (string.IsNullOrWhiteSpace(Amount))
            {
                await ShowError("لطفاً مبلغ را وارد کنید");
                return;
            }

            if (!decimal.TryParse(Amount, out decimal amount))
            {
                await ShowError("مبلغ وارد شده معتبر نیست");
                return;
            }

            if (Account.Deposit(amount))
            {
                StatusMessage = $"✅ واریز {amount:N0} تومان با موفقیت انجام شد";
                Amount = string.Empty;
                
                await Task.Delay(2000);
                StatusMessage = string.Empty;
            }
            else
            {
                await ShowError("مبلغ باید بیشتر از صفر باشد");
            }
        }

        private async Task Withdraw()
        {
            if (Account == null)
                return;

            if (string.IsNullOrWhiteSpace(Amount))
            {
                await ShowError("لطفاً مبلغ را وارد کنید");
                return;
            }

            if (!decimal.TryParse(Amount, out decimal amount))
            {
                await ShowError("مبلغ وارد شده معتبر نیست");
                return;
            }

            if (Account.Withdraw(amount))
            {
                StatusMessage = $"✅ برداشت {amount:N0} تومان با موفقیت انجام شد";
                Amount = string.Empty;
                
                await Task.Delay(2000);
                StatusMessage = string.Empty;
            }
            else
            {
                await ShowError("موجودی کافی نیست یا مبلغ نامعتبر است");
            }
        }

        private async Task ShowError(string message)
        {
            StatusMessage = $"❌ {message}";
            await Task.Delay(3000);
            StatusMessage = string.Empty;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
