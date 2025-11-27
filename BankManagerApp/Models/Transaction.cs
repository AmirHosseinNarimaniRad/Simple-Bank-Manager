using System;

namespace BankManagerApp.Models
{
    public enum TransactionType
    {
        Deposit,    // واریز
        Withdraw    // برداشت
    }

    public class Transaction
    {
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateTime { get; set; }
        public string Description { get; set; }

        public Transaction(TransactionType type, decimal amount, string description = "")
        {
            Type = type;
            Amount = amount;
            DateTime = DateTime.Now;
            Description = description;
        }

        public string TypeText => Type == TransactionType.Deposit ? "واریز" : "برداشت";
        
        public string AmountText => Type == TransactionType.Deposit 
            ? $"+{Amount:N0} تومان" 
            : $"-{Amount:N0} تومان";

        public string DateTimeText => DateTime.ToString("yyyy/MM/dd HH:mm");
    }
}
