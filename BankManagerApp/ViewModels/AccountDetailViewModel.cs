using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BankManager.Data.Entities;

namespace BankManagerApp.ViewModels
{
    [QueryProperty(nameof(AccountId), "accountId")]
    public class AccountDetailViewModel : INotifyPropertyChanged
    {
        private int _accountId;
        public int AccountId
        {
            get => _accountId;
            set
            {
                _accountId = value;
                LoadAccount(value);
            }
        }

        private Wallet _account;
        public Wallet Account
        {
            get => _account;
            set
            {
                _account = value;
                OnPropertyChanged();
            }
        }

        private string _amount;
        public string Amount
        {
            get => _amount;
            set
            {
                _amount = value;
                OnPropertyChanged();
            }
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        private Color _statusColor;
        public Color StatusColor
        {
            get => _statusColor;
            set
            {
                _statusColor = value;
                OnPropertyChanged();
            }
        }

        public ICommand DepositCommand { get; }
        public ICommand WithdrawCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public AccountDetailViewModel()
        {
            DepositCommand = new Command(OnDeposit);
            WithdrawCommand = new Command(OnWithdraw);
        }

        private void LoadAccount(int id)
        {
            // In a real app, load from database
            // For now, we rely on the View to load data or inject a service here
        }

        private void OnDeposit()
        {
            if (decimal.TryParse(Amount, out decimal value) && value > 0)
            {
                if (Account != null)
                {
                    Account.Balance += value;
                    StatusMessage = $"Deposited {value:N0}";
                    StatusColor = Colors.Green;
                    Amount = string.Empty;
                }
            }
            else
            {
                StatusMessage = "Invalid Amount";
                StatusColor = Colors.Red;
            }
        }

        private void OnWithdraw()
        {
            if (decimal.TryParse(Amount, out decimal value) && value > 0)
            {
                if (Account != null)
                {
                    if (Account.Balance >= value)
                    {
                        Account.Balance -= value;
                        StatusMessage = $"Withdrawn {value:N0}";
                        StatusColor = Colors.Green;
                        Amount = string.Empty;
                    }
                    else
                    {
                        StatusMessage = "Insufficient Funds";
                        StatusColor = Colors.Red;
                    }
                }
            }
            else
            {
                StatusMessage = "Invalid Amount";
                StatusColor = Colors.Red;
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
