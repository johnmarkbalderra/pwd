namespace PWD_DSWD_SPC.Models.Registered
{
    public class ApprovalStatus
    {
        public int Id { get; set; }

        public Guid AccountId { get; set; }  // Foreign Key to Account
        public string Status { get; set; } = "Pending";

        public bool Requirement1 { get; set; }
        public bool Requirement2 { get; set; }
        public bool Requirement3 { get; set; }
        public bool Requirement4 { get; set; }
        public bool IsApproved { get; set; } // New property

        public virtual Account Accounts { get; set; }
    }
}
