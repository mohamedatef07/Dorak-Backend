using Data;
using Dorak.Models;
using Dorak.Models.Models.Wallet;
using Dorak.ViewModels;
using Dorak.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Identity;
using Models.Enums;
using Repositories;
using System;
using System.Threading.Tasks;

namespace Services
{
    public class ProviderServices
    {
        public ProviderRepository providerRepository;
        public ProviderAssignmentRepository providerAssignmentRepository;
        public ShiftRepository shiftRepository;
        public ProviderCenterServiceRepository providerCenterServiceRepository;
        public CommitData commitData;

        public ProviderServices(
            ProviderRepository _providerRepository,
            ProviderAssignmentRepository _providerAssignmentRepository,
            ShiftRepository _shiftRepository,
            ProviderCenterServiceRepository _providerCenterServiceRepository,
            CommitData _commitData)
        {
            providerRepository = _providerRepository;
            providerAssignmentRepository = _providerAssignmentRepository;
            shiftRepository = _shiftRepository;
            providerCenterServiceRepository = _providerCenterServiceRepository;
            commitData = _commitData;
        }

        // Creating a New User-Provider 
        public async Task<IdentityResult> CreateProvider(string userId, ProviderRegisterViewModel model)
        {
            var _provider = new Provider
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
                ExperienceYears = model.ExperienceYears,
                LicenseNumber = model.LicenseNumber,
                Bio = model.Description,
                EstimatedDuration = model.EstimatedDuration,
                Availability = model.Availability,
                //RATE
                Specialization = model.Specialization,
                Image = model.Image,
            };

            providerRepository.Add(_provider);
            commitData.SaveChanges();
            return IdentityResult.Success;
        }


        // ----- assign provider to center -----
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
            commitData.SaveChanges();
        }
        public void DeleteProvider(Provider provider)
        {
            providerRepository.Delete(provider);
        }

        public string AssignProviderToCenter(ProviderAssignmentViewModel model)
        {
            var assignment = new ProviderAssignment
            {
                ProviderId = model.ProviderId,
                CenterId = model.CenterId,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                AssignmentType = model.AssignmentType
            };

            providerAssignmentRepository.Add(assignment);
            commitData.SaveChanges();

            return "Provider assigned Succesfully!";
        }

        public GetProviderBookingInfoViewModel GetProviderBookingInfo(string providerId)
        {
            var provider = GetProviderById(providerId);
            var providerAssignment = provider.ProviderAssignments.FirstOrDefault(pa => pa.StartDate <= DateTime.Now && pa.EndDate >= DateTime.Now);
            var providerService = provider.ProviderServices.FirstOrDefault(ps => ps.ProviderId == providerId);
            var shift = shiftRepository.GetProviderAssignmentById(providerAssignment.AssignmentId);
            return new GetProviderBookingInfoViewModel
            {
                CenterName = providerAssignment.Center.CenterName,
                ServiceName = providerService.Service.ServiceName,
                BasePrice = providerService.Service.BasePrice,
                ShiftType = shift.ShiftType
            };
        }

        public string AssignServiceToCenter(AssignProviderCenterServiceViewModel model)
        {

            var isAssigned = providerAssignmentRepository.GetAll()
                .Any(a => a.ProviderId == model.ProviderId && a.CenterId == model.CenterId);

            if (!isAssigned)
                return "Provider is not assigned to the selected center.";


            var provider = providerRepository.GetById(p => p.ProviderId == model.ProviderId);
            var providerService = provider.ProviderServices.FirstOrDefault(s => s.ServiceId == model.ServiceId);

            if (providerService == null)
                return "Provider does not offer the selected service.";

            var pcs = new ProviderCenterService
            {
                ProviderServiceId = providerService.ProviderServiceId,
                CenterId = model.CenterId,
                Duration = model.Duration > 0 ? model.Duration : providerService.Duration,
                Price = model.Price > 0 ? model.Price : providerService.CustomPrice,
                Priority = model.Priority,
                IsDeleted = false
            };

            providerCenterServiceRepository.Add(pcs);
            commitData.SaveChanges();

            return "Service successfully assigned to center for the provider!";
        }

        public string CreateShift(ShiftViewModel model)
        {
            var assignment = providerAssignmentRepository.GetById(a => a.AssignmentId == model.ProviderAssignmentId);
            if (assignment == null)
            {
                return "Invalid provider assignment ID.";
            }

            var shift = new Shift
            {
                ProviderAssignmentId = model.ProviderAssignmentId,
                ShiftType = model.ShiftType,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                MaxPatientsPerDay = model.MaxPatientsPerDay,
                IsDeleted = false
            };

            shiftRepository.Add(shift);
            commitData.SaveChanges();


            return "Shift created successfully!";
        }
        public PaginationViewModel<ProviderViewModel> Search(string searchText = "", int pageNumber = 1,
                                                            int pageSize = 2)
        {
            return providerRepository.Search(searchText, pageNumber, pageSize);
        }

    }
}
