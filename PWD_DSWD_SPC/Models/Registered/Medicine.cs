using System;
using System.Collections.Generic;  // Ensure you have this for ICollection
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PWD_DSWD_SPC.Models.Registered
{
    public class Medicine
    {
        public class MedicineTransaction
        {
            [Key]
            public Guid MedTransactionId { get; set; } // Unique identifier for each transaction

            [ForeignKey("Account")]
            public Guid AccountId { get; set; } // Foreign key to Account

            public string MedicineName { get; set; }
            public int PrescribedQuantity { get; set; }
            public int PurchasedQuantity { get; set; }
            public int RemainingBalance { get; set; }
            public decimal Price { get; set; }
            public decimal TotalPrice { get; set; }
            public decimal DiscountedPrice { get; set; }
            public DateTime DatePurchased { get; set; }
            public string AttendingPhysician { get; set; }
            public string PTRNo { get; set; }
            public string Signature { get; set; }
            public string EstablishmentName { get; set; }
            public string Branch { get; set; }

            [InverseProperty("MedicineTransactions")]
            public virtual Account Account { get; set; } // Navigation property

            // Add navigation property to link back to MedicineTransactionLedger
            public Guid? LedgerId { get; set; }  // Foreign key to MedicineTransactionLedger (nullable)
            [ForeignKey("LedgerId")]
            public virtual MedicineTransactionLedger MedicineTransactionLedger { get; set; }  // Navigation property
        }

        public class MedicineTransactionLedger
        {
            [Key]
            public Guid LedgerId { get; set; }

            [ForeignKey("Account")]
            public Guid AccountId { get; set; }

            [InverseProperty("MedicineTransactionLedgers")]
            public virtual Account Account { get; set; } // Navigation property

            public virtual ICollection<MedicineTransaction> Transactions { get; set; } = new List<MedicineTransaction>();
        }
    }
}
