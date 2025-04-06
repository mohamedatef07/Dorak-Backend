using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dorak.Models;
using Dorak.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Identity;
using Repositories;

namespace Services
{
    public class ProviderServices
    {
        private ProviderRepository providerRepository;

        public ProviderServices(ProviderRepository _providerRepository)
        {
            providerRepository = _providerRepository;
        }

        public async Task<IdentityResult> CreateProvider(string userId, ProviderRegisterViewModel model)
        {
            var provider = new Provider
            {
                ProviderId = userId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Gender = model.Gender,
                BirthDate = model.BirthDate,
                Street = model.Street,
                City = model.City,
                Governorate = model.Governorate,
                Country = model.Country,
                ExperienceYears =model.ExperienceYears,
                LicenseNumber = model.LicenseNumber,
                Description = model.Description,
                EstimatedDuration = model.EstimatedDuration,
                Availability = model.Availability,
                //RATE
                Specialization = model.Specialization,
                PicName = model.Image,
            };

            providerRepository.Add(provider);
            providerRepository.SaveChanges();
            return IdentityResult.Success;
        }
    }
}
