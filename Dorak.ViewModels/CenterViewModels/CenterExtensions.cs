

using Dorak.Models;

namespace Dorak.ViewModels
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
                MapURL = viewModel.MapURL,
                CenterStatus = viewModel.CenterStatus

            };
        }
        public static CenterViewModel ToViewModel(this Center model)
        {
            return new CenterViewModel
            {
                CenterId = model.CenterId,
                CenterName = model.CenterName,
                ContactNumber = model.ContactNumber,
                Country = model.Country,
                Governorate = model.Governorate,
                City = model.City,
                Street = model.Street,
                Email = model.Email,
                WebsiteURL = model.WebsiteURL,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                MapURL = model.MapURL,
                CenterStatus = model.CenterStatus
                
            };
        }
    }
}
