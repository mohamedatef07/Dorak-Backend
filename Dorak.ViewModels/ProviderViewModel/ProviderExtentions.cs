using Dorak.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Enums;

namespace Dorak.ViewModels
{
    public static class ProviderExtentions
    {
        public static Provider ToModel(this ProviderViewModel viewModel)
        {
            return new Provider
            {
                ProviderId = viewModel.ProviderId,
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                Specialization = viewModel.Specialization,
                Bio = viewModel.Bio,
                ExperienceYears = viewModel.ExperienceYears,
                LicenseNumber   = viewModel.LicenseNumber,
                Gender = viewModel.Gender,
                Street = viewModel.Street,
                City = viewModel.City,
                Governorate = viewModel.Governorate,
                Country = viewModel.Country,
                BirthDate = viewModel.BirthDate,
                Image = viewModel.Image,
                Availability =viewModel.Availability,
                EstimatedDuration = viewModel.EstimatedDuration
            };
        }


        public static ProviderViewModel toModelView(this Provider provider)
        {
            return new ProviderViewModel
            {
                ProviderId = provider.ProviderId,
                FirstName = provider.FirstName,
                LastName = provider.LastName,
                Specialization = provider.Specialization,
                Bio = provider.Bio,
                ExperienceYears = provider.ExperienceYears,
                LicenseNumber = provider.LicenseNumber,
                Gender = provider.Gender,
                Street = provider.Street,
                City = provider.City,
                Governorate = provider.Governorate,
                Country = provider.Country,
                BirthDate = provider.BirthDate,
                Image = provider.Image,
                Availability = provider.Availability,
                EstimatedDuration = provider.EstimatedDuration
            };
        }
    }
}
