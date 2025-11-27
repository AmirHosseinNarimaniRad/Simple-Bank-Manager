using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BankManagerApp.Models
{
    public class BankAccount : INotifyPropertyChanged
    {
        private decimal _balance;
        private string _name;

        public int Id { get; set; }
        
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public decimal Balance
        {
            get => _balance;
            private set
            {
                _balance = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(BalanceText));
            }
        }

        public string BalanceText => $"{Balance:N0} تومان";

        public ObservableCollection<Transaction> Transactions { get; set; }

        public BankAccount(int id, string name)
        {
            Id = id;
            Name = name;
            Balance = 0;
            Transactions = new ObservableCollection<Transaction>();
        }

        public bool Deposit(decimal amount)
        {
            if (amount <= 0)
            {
                return false;
            }

            Balance += amount;
            Transactions.Insert(0, new Transaction(TransactionType.Deposit, amount, "واریز وجه"));
            return true;
        }

        public bool Withdraw(decimal amount)
        {
            if (amount <= 0)
            {
                return false;
            }

            if (Balance < amount)
            {
                return false;
            }

            Balance -= amount;
            Transactions.Insert(0, new Transaction(TransactionType.Withdraw, amount, "برداشت وجه"));
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
