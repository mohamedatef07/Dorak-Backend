using Dorak.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Dorak.ViewModels.CenterViewModel
{
    public static class CenterExtensions
    {
        public static Center ToModel(this CenterViewModel viewModel)
        {
            return new Center
            {
                CenterName = viewModel.CenterName,
                ContactNumber = viewModel.ContactNumber,
                Country = viewModel.Country,
                Governorate = viewModel.Governorate,
                City = viewModel.City,
                Street = viewModel.Street,
                Email = viewModel.Email,
                WebsiteURL = viewModel.WebsiteURL,
                Latitude = viewModel.Latitude,
                Longitude = viewModel.Longitude,
                MapURL = viewModel.MapURL
            };
        }
    }
}
