namespace PWD_DSWD_SPC.Models.Registered
{
    public class UserCredentials
    {
        public int Id { get; set; }

        public Guid AccountId { get; set; }  // Foreign Key to Account

        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Avatar { get; set; }  // E.g., "/images/user-avatar.jpg"
        public string? Role { get; set; }
        
        // Navigation property
        public virtual Account Accounts { get; set; }
    }
}
