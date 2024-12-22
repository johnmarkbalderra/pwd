using System.Net;

namespace PWD_DSWD_SPC.Models.Registered
{
    public class Account
    {
        public Guid Id { get; set; }

        public string ApplicantType { get; set; }
        public string? DisabilityNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string? suffix { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string CivilStatus { get; set; }
        public string FatherLastName { get; set; }
        public string FatherFirstName { get; set; }
        public string FatherMiddleName { get; set; }
        public string MotherLastName { get; set; }
        public string MotherFirstName { get; set; }
        public string MotherMiddleName { get; set; }
        public string GuardianLastName { get; set; }
        public string GuardianFirstName { get; set; }
        public string GuardianMiddleName { get; set; }
        public string TypeOfDisability { get; set; }
        public string CauseOfDisability { get; set; }
        public string HouseNoAndStreet { get; set; }
        public string AD { get; set; }
        public string Barangay { get; set; }
        public string Municipality { get; set; }
        public string Province { get; set; }
        public string Region { get; set; }
        public string? LandlineNo { get; set; }
        public string MobileNo { get; set; }
        public string EmailAddress { get; set; }
        public string EducationalAttainment { get; set; }
        public string? StatusOfEmployment { get; set; }
        public string? CategoryOfEmployment { get; set; }
        public string? TypeOfEmployment { get; set; }
        public string? Occupation { get; set; }
        public string? OtherOccupation { get; set; }
        public string? OrganizationAffiliated { get; set; }
        public string? ContactPerson { get; set; }
        public string? OfficeAddress { get; set; }
        public string? OfficeTelNo { get; set; }
        public string? SSSNo { get; set; }
        public string? GSISNo { get; set; }
        public string? PagIBIGNo { get; set; }
        public string? PSNNo { get; set; }
        public string? PhilHealthNo { get; set; }
        public string AccomplishByLastName { get; set; }
        public string AccomplishByFirstName { get; set; }
        public string AccomplishByMiddleName { get; set; }
        public string ReferenceNumber { get; set; }

        public DateTime? ValidUntil { get; set; }  // Expiration Date
        public DateTime? DisapprovalDate { get; set; } // Nullable to track disapproval date
        public bool IsExpired => ValidUntil.HasValue && ValidUntil.Value < DateTime.Now;  // Derived property

        public virtual ApprovalStatus Status { get; set; }
        public virtual UserCredentials UserCredential { get; set; }
        public virtual ICollection<CommodityTransaction> CommodityTransactions { get; set; } = new List<CommodityTransaction>();
        public virtual ICollection<CommodityItem> CommodityItems { get; set; } = new List<CommodityItem>();
        public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
        public virtual ICollection<Medicine.MedicineTransaction> MedicineTransactions { get; set; } = new List<Medicine.MedicineTransaction>();
        public virtual ICollection<Medicine.MedicineTransactionLedger> MedicineTransactionLedgers { get; set; } = new List<Medicine.MedicineTransactionLedger>();

    }
}
