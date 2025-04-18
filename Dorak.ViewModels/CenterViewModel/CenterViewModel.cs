using Dorak.Models;
using Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Dorak.ViewModels.CenterViewModel
{
    public class CenterViewModel
    {
        public int CenterId { get; set; }
        public string CenterName { get; set; }
        public string ContactNumber { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Governorate { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string WebsiteURL { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string MapURL { get; set; }
        public bool IsDeleted { get; set; }
        public CenterStatus CenterStatus { get; set; }
    }
}
