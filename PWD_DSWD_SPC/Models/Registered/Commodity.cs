using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PWD_DSWD_SPC.Models.Registered
{

    public class CommodityItem
    {
        [Key]
        public Guid CommodityItemId { get; set; } // Primary key for each item

        public Guid TransactionId { get; set; } // Foreign key to CommodityTransaction

        public Guid AccountId { get; set; } // Foreign key to Account

        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        // Set precision and scale for TotalPrice and DiscountedPrice
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalPrice { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal DiscountedPrice { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow; // Date the item was added
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow; // Date the item was last updated

        // Navigation property for the transaction
        [ForeignKey("TransactionId")]
        public CommodityTransaction CommodityTransaction { get; set; }

    }

    public class CommodityTransaction
    {
        [Key]
        public Guid TransactionId { get; set; } // Primary key for the transaction

        public Guid AccountId { get; set; } // Foreign key to Account

        public Account Account { get; set; } // Navigation property for Account

        public List<CommodityItem> Items { get; set; } = new List<CommodityItem>();

        public decimal TotalAmount => CalculateTotalAmount();

        public decimal RemainingDiscount { get; set; } = 2500m;

        // New properties for Establishment and Branch
        public string EstablishmentName { get; set; }
        public string BranchName { get; set; }

        public decimal ExceedAmount => TotalAmount > RemainingDiscount ? TotalAmount - RemainingDiscount : 0;
        public string Signature { get; set; } // E-signature for the transaction
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow; // Transaction creation date
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow; // Last modified date

        private decimal CalculateTotalAmount()
        {
            decimal total = 0;
            foreach (var item in Items)
            {
                total += item.DiscountedPrice;
            }
            return total;
        }
    }
}