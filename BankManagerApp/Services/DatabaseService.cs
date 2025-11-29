using SQLite;
using BankManagerApp.Models;

namespace BankManagerApp.Services
{
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _database;

        public DatabaseService()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "bankmanager.db3");
            _database = new SQLiteAsyncConnection(dbPath);
            
            // Create tables
            _database.CreateTableAsync<User>().Wait();
            _database.CreateTableAsync<BankAccountDb>().Wait();
            _database.CreateTableAsync<TransactionDb>().Wait();
        }

        // User Operations
        public Task<List<User>> GetUsersAsync()
        {
            return _database.Table<User>().ToListAsync();
        }

        public Task<User> GetUserByPhoneAsync(string phoneNumber)
        {
            return _database.Table<User>()
                .Where(u => u.PhoneNumber == phoneNumber)
                .FirstOrDefaultAsync();
        }

        public Task<User> GetUserByIdAsync(int id)
        {
            return _database.Table<User>()
                .Where(u => u.Id == id)
                .FirstOrDefaultAsync();
        }

        public Task<int> SaveUserAsync(User user)
        {
            if (user.Id != 0)
            {
                return _database.UpdateAsync(user);
            }
            else
            {
                return _database.InsertAsync(user);
            }
        }

        public Task<int> DeleteUserAsync(User user)
        {
            return _database.DeleteAsync(user);
        }

        // BankAccount Operations
        public Task<List<BankAccountDb>> GetAccountsAsync(int userId)
        {
            return _database.Table<BankAccountDb>()
                .Where(a => a.UserId == userId)
                .ToListAsync();
        }

        public Task<BankAccountDb> GetAccountByIdAsync(int id)
        {
            return _database.Table<BankAccountDb>()
                .Where(a => a.Id == id)
                .FirstOrDefaultAsync();
        }

        public Task<int> SaveAccountAsync(BankAccountDb account)
        {
            if (account.Id != 0)
            {
                return _database.UpdateAsync(account);
            }
            else
            {
                return _database.InsertAsync(account);
            }
        }

        public Task<int> DeleteAccountAsync(BankAccountDb account)
        {
            return _database.DeleteAsync(account);
        }

        // Transaction Operations
        public Task<List<TransactionDb>> GetTransactionsAsync(int accountId)
        {
            return _database.Table<TransactionDb>()
                .Where(t => t.AccountId == accountId)
                .OrderByDescending(t => t.DateTime)
                .ToListAsync();
        }

        public Task<int> SaveTransactionAsync(TransactionDb transaction)
        {
            return _database.InsertAsync(transaction);
        }

        public Task<int> DeleteTransactionAsync(TransactionDb transaction)
        {
            return _database.DeleteAsync(transaction);
        }

        // Helper method to update account balance
        public async Task<bool> UpdateAccountBalanceAsync(int accountId, decimal newBalance)
        {
            var account = await GetAccountByIdAsync(accountId);
            if (account != null)
            {
                account.Balance = newBalance;
                await SaveAccountAsync(account);
                return true;
            }
            return false;
        }
    }
}
