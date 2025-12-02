using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BankManager.Data.Entities;
using BankManagerApp.Views;

namespace BankManagerApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Wallet> _accounts;
        public ObservableCollection<Wallet> Accounts
        {
            get => _accounts;
            set
            {
                _accounts = value;
                OnPropertyChanged();
            }
        }

        private Wallet _selectedAccount;
        public Wallet SelectedAccount
        {
            get => _selectedAccount;
            set
            {
                _selectedAccount = value;
                if (_selectedAccount != null)
                {
                    OnAccountSelected(_selectedAccount);
                    _selectedAccount = null; // Reset selection
                    OnPropertyChanged();
                }
            }
        }

        public ICommand CreateAccountCommand { get; }
        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            Accounts = new ObservableCollection<Wallet>();
            CreateAccountCommand = new Command(OnCreateAccount);
            // LoadAccounts(); // In a real app, we'd load here or in OnAppearing
        }

        private async void OnCreateAccount()
        {
            string accountName = await Shell.Current.DisplayPromptAsync("New Wallet", "Enter wallet name:");
            if (!string.IsNullOrWhiteSpace(accountName))
            {
                var newAccount = new Wallet { Name = accountName, Balance = 0, CreatedAt = DateTime.Now };
                Accounts.Add(newAccount);
                // Note: In a real MVVM app, we'd call a service to save to DB here
            }
        }

        private async void OnAccountSelected(Wallet account)
        {
            await Shell.Current.GoToAsync($"AccountDetailPage?accountId={account.Id}");
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
