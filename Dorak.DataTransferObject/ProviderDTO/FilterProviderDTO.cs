using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Enums;

namespace Dorak.DataTransferObject.ProviderDTO
{
    public class FilterProviderDTO
    {
        public ProviderTitle? Title { get; set; } 
        public GenderType? Gender { get; set; } 

        public string? City { get; set; }

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public double? MinRate { get; set; }
        public double? MaxRate { get; set; }

        public DateOnly? AvailableDate { get; set; } 

    }
}
