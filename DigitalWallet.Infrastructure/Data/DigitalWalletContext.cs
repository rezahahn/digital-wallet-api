using DigitalWallet.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace DigitalWallet.Infrastructure.Data
{
    public class DigitalWalletContext : DbContext
    {
        public DigitalWalletContext(DbContextOptions<DigitalWalletContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<LoanPayment> LoanPayments { get; set; }
        public DbSet<InvestmentProduct> InvestmentProducts { get; set; }
        public DbSet<UserInvestment> UserInvestments { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure relationships and constraints
            modelBuilder.Entity<User>()
                .HasOne(u => u.Wallet)
                .WithOne(w => w.User)
                .HasForeignKey<Wallet>(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.RelatedTransaction)
                .WithMany()
                .HasForeignKey(t => t.RelatedTransactionId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Loan>()
                .HasOne(l => l.Approver)
                .WithMany()
                .HasForeignKey(l => l.ApprovedBy)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure decimal precision
            modelBuilder.Entity<Wallet>()
                .Property(w => w.Balance)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasColumnType("decimal(18,2)");

            // Configure indexes
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.PhoneNumber)
                .IsUnique();

            modelBuilder.Entity<Transaction>()
                .HasIndex(t => t.ReferenceNumber)
                .IsUnique();

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(t => t.TransactionId);
            });

            modelBuilder.Entity<LoanPayment>(entity =>
            {
                entity.HasKey(a => a.PaymentId);
                entity.Property(a => a.PaymentId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<InvestmentProduct>(entity =>
            {
                entity.HasKey(a => a.ProductId);
                entity.Property(a => a.ProductId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<UserInvestment>(entity =>
            {
                entity.HasKey(a => a.InvestmentId);
                entity.Property(a => a.InvestmentId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasKey(a => a.LogId);
                entity.Property(a => a.LogId).ValueGeneratedOnAdd();
                entity.Property(a => a.Timestamp).HasDefaultValueSql("GETUTCDATE()");
            });
        }
    }
}
