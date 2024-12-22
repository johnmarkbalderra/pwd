using System.ComponentModel.DataAnnotations;

namespace PWD_DSWD_SPC.Models.Registered
{
    public class QrCode
    {
        public Guid QrCodeId { get; set; }

        [Required]
        [Display(Name = "Establishment Name")]
        public string EstablishmentName { get; set; } // Text property used for QR code generation

        public string Branch { get; set; } // Branch property
        public string TypeOfQRCode { get; set; } // Type of QR code (Commodities, Medicine, Both)
        public string QrCodeBase64 { get; set; } // QR Code in base64 format
        public string RegistrationUrl { get; set; } // URL linked to the QR Code

    }
}