namespace BankManager.Data.Enums
{
    /// <summary>
    /// Transaction type enumeration
    /// Stored as integer in database for better performance
    /// </summary>
    public enum TransactionType
    {
        /// <summary>
        /// Deposit/Income transaction (adds to balance)
        /// </summary>
        Deposit = 0,
        
        /// <summary>
        /// Withdraw/Expense transaction (subtracts from balance)
        /// </summary>
        Withdraw = 1
    }
}
