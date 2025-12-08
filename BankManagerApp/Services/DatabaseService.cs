using BankManager.Data;
using BankManager.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankManagerApp.Services
{
    public class DatabaseService
    {
        public readonly BankDbContext _context;

        public DatabaseService(BankDbContext context)
        {
            _context = context;
        }

        public async Task Init()
        {
            // Ensure database is created and migrated
            await _context.Database.MigrateAsync();
        }

        // User methods
        public async Task<List<User>> GetUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetUserAsync(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<int> SaveUserAsync(User user)
        {
            if (user.Id == 0)
                _context.Users.Add(user);
            else
                _context.Users.Update(user);

            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteUserAsync(User user)
        {
            _context.Users.Remove(user);
            return await _context.SaveChangesAsync();
        }

        // Wallet methods
        public async Task<List<Wallet>> GetWalletsAsync(int userId)
        {
            return await _context.Wallets
                .Where(a => a.UserId == userId)
                .ToListAsync();
        }

        public async Task<Wallet> GetWalletAsync(int id)
        {
            return await _context.Wallets
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<int> SaveWalletAsync(Wallet wallet)
        {
            if (wallet.Id == 0)
                _context.Wallets.Add(wallet);
            else
                _context.Wallets.Update(wallet);

            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteWalletAsync(Wallet wallet)
        {
            _context.Wallets.Remove(wallet);
            return await _context.SaveChangesAsync();
        }

        // Transaction methods
        public async Task<List<TransactionDb>> GetTransactionsAsync(int walletId)
        {
            return await _context.Transactions
                .Where(t => t.AccountId == walletId)
                .OrderByDescending(t => t.DateTime)
                .ToListAsync();
        }

        public async Task<int> SaveTransactionAsync(TransactionDb transaction)
        {
            if (transaction.Id == 0)
                _context.Transactions.Add(transaction);
            else
                _context.Transactions.Update(transaction);

            return await _context.SaveChangesAsync();
        }

        // Helper to update wallet balance
        public async Task UpdateWalletBalanceAsync(int walletId, decimal amount)
        {
            var wallet = await GetWalletAsync(walletId);
            if (wallet != null)
            {
                wallet.Balance += amount;
                await SaveWalletAsync(wallet);
            }
        }

        // Category methods
        public async Task<List<Category>> GetCategoriesAsync(BankManager.Data.Enums.TransactionType type)
        {
            return await _context.Categories
                .Where(c => c.Type == type)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories
                .OrderBy(c => c.Type)
                .ThenBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Category?> GetCategoryAsync(int id)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<int> SaveCategoryAsync(Category category)
        {
            if (category.Id == 0)
                _context.Categories.Add(category);
            else
                _context.Categories.Update(category);

            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteCategoryAsync(Category category)
        {
            _context.Categories.Remove(category);
            return await _context.SaveChangesAsync();
        }
    }
}
