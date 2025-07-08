using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dorak.Models;

namespace Dorak.DataTransferObject.CenterDTO
{
    public static class CenterMappingExtensions
    {
        public static CenterDTO_ CenterToDTO(this Center center)
        {
            if (center == null) return null;

            return new CenterDTO_
            {
                CenterName = center.CenterName,
                ContactNumber = center.ContactNumber,
                Street = center.Street,
                City = center.City,
                Governorate = center.Governorate,
                Country = center.Country,
                Email = center.Email,
                WebsiteURL = center.WebsiteURL,
                Latitude = center.Latitude,
                Longitude = center.Longitude,
                MapURL = center.MapURL,
                IsDeleted = center.IsDeleted,
                CenterStatus = center.CenterStatus
            };
        }

        public static Center CenterDTOToCenter(this CenterDTO_ dto)
        {
            if (dto == null) return null;
            return new Center
            {
                CenterName = dto.CenterName,
                ContactNumber = dto.ContactNumber,
                Street = dto.Street,
                City = dto.City,
                Governorate = dto.Governorate,
                Country = dto.Country,
                Email = dto.Email,
                WebsiteURL = dto.WebsiteURL,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                MapURL = dto.MapURL,
                IsDeleted = dto.IsDeleted,
            };
        }
    }
}
