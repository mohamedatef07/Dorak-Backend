using System.ComponentModel.DataAnnotations;

namespace Dorak.Models
{
    public class ContactFormModel
    {
        [Required]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }
        public string? Phone { get; set; }          
        public string? CenterName { get; set; }       
        public string InquiryType { get; set; }       

        public string Subject { get; set; }

        [Required]
        public string Message { get; set; }
    }
}
