using BankManager.Data.Entities;
using BankManager.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BankManager.Data
{
    public class BankDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<TransactionDb> Transactions { get; set; }
        public DbSet<Category> Categories { get; set; }

        private readonly string? _databasePath;

        public BankDbContext()
        {
            // Default constructor for design-time tools
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _databasePath = Path.Combine(folder, "bankmanager.db");
        }

        public BankDbContext(string databasePath)
        {
            _databasePath = databasePath;
        }

        // Constructor for DI
        public BankDbContext(DbContextOptions<BankDbContext> options) : base(options)
        {
            _databasePath = null;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured && _databasePath != null)
            {
                optionsBuilder.UseSqlite($"Data Source={_databasePath}");
            }
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure enum to int conversion for TransactionType
            var transactionTypeConverter = new EnumToNumberConverter<TransactionType, int>();

            // ===== User Entity Configuration =====
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255);
                
                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50);
                
                entity.Property(e => e.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(255);
                
                entity.Property(e => e.PasswordResetToken)
                    .HasMaxLength(255);
                
                entity.Property(e => e.MonthlyBudget)
                    .HasPrecision(18, 2);
                
                entity.Property(e => e.CreatedAt)
                    .IsRequired();
                
                entity.Property(e => e.UpdatedAt)
                    .IsRequired();
                
                // Relationships
                entity.HasMany(e => e.Wallets)
                    .WithOne(w => w.User)
                    .HasForeignKey(w => w.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                // Indexes
                entity.HasIndex(e => e.Email)
                    .IsUnique();
                
                entity.HasIndex(e => e.Username)
                    .IsUnique();
            });

            // ===== Wallet Entity Configuration =====
            modelBuilder.Entity<Wallet>(entity =>
            {
                entity.ToTable("bank_accounts");
                
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);
                
                entity.Property(e => e.Balance)
                    .HasPrecision(18, 2);
                
                entity.Property(e => e.CreatedAt)
                    .IsRequired();
                
                entity.Property(e => e.UpdatedAt)
                    .IsRequired();
                
                // Relationships
                entity.HasMany(e => e.Transactions)
                    .WithOne(t => t.Wallet)
                    .HasForeignKey(t => t.AccountId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                // Indexes
                entity.HasIndex(e => e.UserId);
            });

            // ===== Category Entity Configuration =====
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("categories");
                
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasConversion(transactionTypeConverter);
                
                entity.Property(e => e.Icon)
                    .HasMaxLength(50);
                
                entity.Property(e => e.CreatedAt)
                    .IsRequired();
                
                entity.Property(e => e.UpdatedAt)
                    .IsRequired();
                
                // Relationships
                entity.HasMany(e => e.Transactions)
                    .WithOne(t => t.CategoryEntity)
                    .HasForeignKey(t => t.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull);
                
                // Indexes
                entity.HasIndex(e => new { e.Type, e.Name });
            });

            // ===== TransactionDb Entity Configuration =====
            modelBuilder.Entity<TransactionDb>(entity =>
            {
                entity.ToTable("transactions");
                
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasConversion(transactionTypeConverter);
                
                entity.Property(e => e.Category)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(e => e.IncomeType)
                    .HasMaxLength(100);
                
                entity.Property(e => e.Amount)
                    .HasPrecision(18, 2);
                
                entity.Property(e => e.Description)
                    .HasMaxLength(500);
                
                entity.Property(e => e.DateTime)
                    .IsRequired();
                
                entity.Property(e => e.CreatedAt)
                    .IsRequired();
                
                entity.Property(e => e.UpdatedAt)
                    .IsRequired();
                
                // Indexes
                entity.HasIndex(e => e.AccountId);
                entity.HasIndex(e => e.CategoryId);
                entity.HasIndex(e => e.DateTime);
                entity.HasIndex(e => new { e.AccountId, e.DateTime });
            });
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is User || e.Entity is Wallet || e.Entity is TransactionDb || e.Entity is Category);

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    if (entry.Entity is User u) { u.CreatedAt = DateTime.UtcNow; u.UpdatedAt = DateTime.UtcNow; }
                    if (entry.Entity is Wallet w) { w.CreatedAt = DateTime.UtcNow; w.UpdatedAt = DateTime.UtcNow; }
                    if (entry.Entity is TransactionDb t) { t.CreatedAt = DateTime.UtcNow; t.UpdatedAt = DateTime.UtcNow; }
                    if (entry.Entity is Category c) { c.CreatedAt = DateTime.UtcNow; c.UpdatedAt = DateTime.UtcNow; }
                }
                else if (entry.State == EntityState.Modified)
                {
                    if (entry.Entity is User u) u.UpdatedAt = DateTime.UtcNow;
                    if (entry.Entity is Wallet w) w.UpdatedAt = DateTime.UtcNow;
                    if (entry.Entity is TransactionDb t) t.UpdatedAt = DateTime.UtcNow;
                    if (entry.Entity is Category c) c.UpdatedAt = DateTime.UtcNow;
                }
            }
        }
    }
}
