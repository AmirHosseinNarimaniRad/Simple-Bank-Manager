using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BankManager.Data
{
    public class BankDbContextFactory : IDesignTimeDbContextFactory<BankDbContext>
    {
        public BankDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BankDbContext>();
            
            // This is only for design-time (migrations), so the path doesn't matter much
            // as long as it's valid.
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var dbPath = Path.Combine(folder, "bankmanager.db");
            
            optionsBuilder.UseSqlite($"Data Source={dbPath}");

            return new BankDbContext(optionsBuilder.Options);
        }
    }
}
