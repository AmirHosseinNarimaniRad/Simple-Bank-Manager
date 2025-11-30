using SQLite;
using BankManagerApp.Models;

namespace BankManagerApp.Services
{
    public static class Constants
    {
        public const string DatabaseFilename = "namakdoon.db3";

        public const SQLite.SQLiteOpenFlags Flags =
            SQLite.SQLiteOpenFlags.ReadWrite |
            SQLite.SQLiteOpenFlags.Create |
            SQLite.SQLiteOpenFlags.SharedCache;

        public static string DatabasePath =>
            Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
    }

    public class DatabaseService
    {
        private SQLiteAsyncConnection _database;

        public DatabaseService()
        {
        }

        public async Task Init()
        {
            if (_database is not null)
                return;

            _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            Console.WriteLine($"*** DATABASE PATH: {Constants.DatabasePath} ***");
            var result = await _database.CreateTablesAsync<User, Wallet, TransactionDb>();
        }

        // User methods
        public async Task<List<User>> GetUsersAsync()
        {
            await Init();
            return await _database.Table<User>().ToListAsync();
        }

        public async Task<User> GetUserByPhoneAsync(string phoneNumber)
        {
            await Init();
            return await _database.Table<User>().Where(u => u.PhoneNumber == phoneNumber).FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            await Init();
            return await _database.Table<User>().Where(u => u.Id == id).FirstOrDefaultAsync();
        }

        public async Task<int> SaveUserAsync(User user)
        {
            await Init();
            if (user.Id != 0)
                return await _database.UpdateAsync(user);
            else
                return await _database.InsertAsync(user);
        }

        public async Task<int> DeleteUserAsync(User user)
        {
            await Init();
            return await _database.DeleteAsync(user);
        }

        // Wallet methods
        public async Task<List<Wallet>> GetWalletsAsync(int userId)
        {
            await Init();
            return await _database.Table<Wallet>()
                .Where(a => a.UserId == userId)
                .ToListAsync();
        }

        public async Task<Wallet> GetWalletAsync(int id)
        {
            await Init();
            return await _database.Table<Wallet>()
                .Where(a => a.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<int> SaveWalletAsync(Wallet wallet)
        {
            await Init();
            if (wallet.Id != 0)
                return await _database.UpdateAsync(wallet);
            else
                return await _database.InsertAsync(wallet);
        }

        public async Task<int> DeleteWalletAsync(Wallet wallet)
        {
            await Init();
            return await _database.DeleteAsync(wallet);
        }

        // Transaction methods
        public async Task<List<TransactionDb>> GetTransactionsAsync(int walletId)
        {
            await Init();
            return await _database.Table<TransactionDb>()
                .Where(t => t.AccountId == walletId)
                .OrderByDescending(t => t.DateTime)
                .ToListAsync();
        }

        public async Task<int> SaveTransactionAsync(TransactionDb transaction)
        {
            await Init();
            return await _database.InsertAsync(transaction);
        }

        // Helper to update wallet balance
        public async Task UpdateWalletBalanceAsync(int walletId, decimal amount)
        {
            await Init();
            var wallet = await GetWalletAsync(walletId);
            if (wallet != null)
            {
                wallet.Balance += amount;
                await SaveWalletAsync(wallet);
            }
        }
    }
}
