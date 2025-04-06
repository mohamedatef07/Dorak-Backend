using Data;
using Dorak.Models;
using Dorak.ViewModels;
using Models.Enums;
using Repositories;
using System;
using System.Threading.Tasks;

namespace Services
{
    public class ProviderServices
    {
        ProviderRepository providerRepository;
        ProviderAssignmentRepository providerAssignmentRepository;


        public ProviderServices(ProviderRepository _providerRepository, ProviderAssignmentRepository _providerAssignmentRepository)
        {
            providerRepository = _providerRepository;
            providerAssignmentRepository = _providerAssignmentRepository;
        }

        // ----- assign provider to center -----

        // get provider by id to assign to center later
        public Provider GetProviderById(string providerId)
        {
            return providerRepository.GetById(p => p.ProviderId == providerId);
        }
        public List<Provider> GetAllProviders()
        {
            return providerRepository.GetAll().ToList();
        }
        public void EditProvider(Provider provider)
        {
            providerRepository.Edit(provider);
        }
        public void DeleteProvider(Provider provider)
        {
            providerRepository.Delete(provider);
        }

        public void AssignProviderToCenter(string providerId, int centerId, DateTime startDate, DateTime endDate, ProviderType assignmentType)
        {
            var assignment = new ProviderAssignment
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
                ProviderId = providerId,
                CenterId = centerId,
                StartDate = startDate,
                EndDate = endDate,
                AssignmentType = assignmentType
            };

            providerAssignmentRepository.Add(assignment);

        }

        public PaginationViewModel<ProviderViewModel> Search(string searchText = "", int pageNumber = 1,
                                                            int pageSize = 2)
        {
            return providerRepository.Search(searchText, pageNumber, pageSize);
        }

    }
}
