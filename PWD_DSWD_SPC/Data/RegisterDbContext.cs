using Microsoft.EntityFrameworkCore;
using PWD_DSWD_SPC.Models.Registered;

namespace PWD_DSWD_SPC.Data
{
    public class RegisterDbContext : DbContext
    {
        public RegisterDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<ApprovalStatus> Status { get; set; }
        public DbSet<UserCredentials> UserCredential { get; set; }
        public DbSet<AdminCredentials> AdminCredential {  get; set; } 
        public DbSet<QrCode> QrCodes { get; set; } // QR codes for establishments
        public DbSet<CommodityTransaction> CommodityTransactions { get; set; }
        public DbSet<CommodityItem> CommodityItems { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Medicine.MedicineTransaction> MedicineTransactions { get; set; }
        public DbSet<Medicine.MedicineTransactionLedger> MedicineTransactionLedgers { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Account and ApprovalStatus - One-to-One relationship
            modelBuilder.Entity<Account>()
                .HasOne(a => a.Status)
                .WithOne(s => s.Accounts)
                .HasForeignKey<ApprovalStatus>(s => s.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            // Account and UserCredentials - One-to-One relationship
            modelBuilder.Entity<Account>()
                .HasOne(a => a.UserCredential)
                .WithOne(uc => uc.Accounts)
                .HasForeignKey<UserCredentials>(uc => uc.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            // CommodityTransaction and CommodityItem - One-to-Many relationship
            modelBuilder.Entity<CommodityTransaction>()
                .HasMany(ct => ct.Items)
                .WithOne(ci => ci.CommodityTransaction)
                .HasForeignKey(ci => ci.TransactionId)
                .OnDelete(DeleteBehavior.Restrict);

            // CommodityTransaction and Account - One-to-Many relationship
            modelBuilder.Entity<CommodityTransaction>()
                .HasOne(ct => ct.Account)
                .WithMany(a => a.CommodityTransactions)
                .HasForeignKey(ct => ct.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            // Account and MedicineTransaction - One-to-Many relationship
            modelBuilder.Entity<Medicine.MedicineTransaction>()
                .HasOne(mt => mt.Account)
                .WithMany(a => a.MedicineTransactions)
                .HasForeignKey(mt => mt.AccountId)
                .OnDelete(DeleteBehavior.Restrict); // Use Restrict to prevent cascading delete if desired

            // Account and MedicineTransactionLedger - One-to-Many relationship
            modelBuilder.Entity<Medicine.MedicineTransactionLedger>()
                .HasOne(ml => ml.Account)
                .WithMany(a => a.MedicineTransactionLedgers)
                .HasForeignKey(ml => ml.AccountId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete of ledger when Account is deleted

            // MedicineTransactionLedger and MedicineTransaction - One-to-Many relationship
            modelBuilder.Entity<Medicine.MedicineTransactionLedger>()
                .HasMany(ml => ml.Transactions)
                .WithOne(mt => mt.MedicineTransactionLedger) // Assuming MedicineTransaction has a navigation property to Ledger
                .HasForeignKey(mt => mt.LedgerId) // Specify the foreign key for clarity
                .OnDelete(DeleteBehavior.Cascade);

            // Decimal precision and scale for Price in CommodityItem
            modelBuilder.Entity<CommodityItem>()
                .Property(ci => ci.Price)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<CommodityItem>()
                .Property(ci => ci.TotalPrice)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<CommodityItem>()
                .Property(ci => ci.DiscountedPrice)
                .HasColumnType("decimal(18, 2)");

            // Specify decimal precision and scale for RemainingDiscount in CommodityTransaction
            modelBuilder.Entity<CommodityTransaction>()
                .Property(ct => ct.RemainingDiscount)
                .HasColumnType("decimal(18, 2)");

            // Specify decimal precision and scale for Price fields in MedicineTransaction
            modelBuilder.Entity<Medicine.MedicineTransaction>()
                .Property(mt => mt.Price)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Medicine.MedicineTransaction>()
                .Property(mt => mt.TotalPrice)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Medicine.MedicineTransaction>()
                .Property(mt => mt.DiscountedPrice)
                .HasColumnType("decimal(18, 2)");


            base.OnModelCreating(modelBuilder);
        }
    }
}
