using System.ComponentModel.DataAnnotations;

namespace PWD_DSWD_SPC.Models.Registered
{
    public class AdminCredentials
    {
        public int Id { get; set; } // Primary Key

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength(100)]
        public string Password { get; set; }
    }
}
