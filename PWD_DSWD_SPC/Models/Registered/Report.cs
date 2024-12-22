namespace PWD_DSWD_SPC.Models.Registered
{
    public class Report
    {
        public int ReportId { get; set; }
        public Guid AccountId { get; set; } // Linked to the logged-in user
        public string ProblemDescription { get; set; }
        public string Establishment { get; set; }
        public string Branch { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Acknowledged { get; set; } = false;

        // Navigation property to link Report with Account
        public virtual Account Accounts { get; set; }
    }
}
