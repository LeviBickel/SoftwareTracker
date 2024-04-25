using System.ComponentModel.DataAnnotations;

namespace SoftwareTracker.Models
{
    public class LicenseModel
    {
        public int Id { get; set; }
        public string Manufacturer { get; set; }

        [Display(Name = "Software Title")]
        [Required]
        public string SoftwareTitle { get; set; }

        [Display(Name = "Assigned To")]
        public string AssignedServer { get; set; }

        [Display(Name = "Purchase Order #")]
        public string PurchaseOrder { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [Display(Name = "Purchase Date")]
        [DataType(DataType.Date)]
        public DateTime PurchaseDate { get; set; }

        [Display(Name = "License Type")]
        public string LicenseType { get; set; }

        [Display(Name = "License Expiration")]
        [DataType(DataType.Date)]
        public DateTime LicenseExp { get; set; }
        public bool Support { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]

        [Display(Name = "Support Expiration")]
        [DataType(DataType.Date)]
        public DateTime SupportExp { get; set; }

        [Display(Name = "Total Keys")]
        [Required]
        public int AmountofKeys { get; set; }

        [Display(Name = "Used Keys")]
        [Required]
        public int UsedKeys { get; set; }

        [Display(Name = "Remaining Keys")]
        public int RemainingKeys { get; set; }

        [Display(Name = "License Key")]
        public string LicenseKey { get; set; }

        public string AddedBy { get; set; }
        public bool Notified {  get; set; }
    }
}
