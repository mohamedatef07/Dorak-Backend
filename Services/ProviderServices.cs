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
using System.Linq;

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

        // Assign provider to center
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
        public List<GetProviderBookingInfoViewModel> GetProviderBookingInfo(string providerId)
        {
            var provider = GetProviderById(providerId);
            var providerAssignments = provider.ProviderAssignments.Where(pa => pa.StartDate <= DateTime.Now && pa.EndDate >= DateTime.Now);
            List<GetProviderBookingInfoViewModel> shifts = new List<GetProviderBookingInfoViewModel>();
            Shift shift;
            bool IsMonth = false;
            foreach (var providerAss in providerAssignments)
            {
                shift = shiftRepository.GetProviderAssignmentById(providerAss.AssignmentId);
                var start = providerAss.StartDate.Value;
                var end = providerAss.EndDate.Value;
                for (DateTime i = start; i <=  end; i = i.AddDays(1)) 
                {
                    if (i.Date >= DateTime.Now.AddMonths(1))
                    {
                        IsMonth = true;
                        break;
                    }
                    else if(i > DateTime.Now)
                    {
                        var NewShift = new GetProviderBookingInfoViewModel()
                        {
                            Date = i.Date.ToString(),
                            StartTime = shift.StartTime,
                            EndTime = shift.EndTime,
                            ShiftType = shift.ShiftType,
                            CenterId = providerAss.CenterId,
                            ServiceId = providerAss.Provider.ProviderCenterServices.Where(pas => pas.ProviderId == providerId).Select(se =>se.ServiceId ).ToList(),
                        };
                        shifts.Add(NewShift);
                    }        
                }
                if (IsMonth) 
                {
                    break;
                }
            }
            var providerCenterService = provider.ProviderCenterServices.FirstOrDefault(provider => provider.ProviderId == providerId);
            return shifts;
        }


        // Assign service to center
        public string AssignServiceToCenter(AssignProviderCenterServiceViewModel model)
        {
            var isAssigned = providerAssignmentRepository.GetAll()
                .Any(a => a.ProviderId == model.ProviderId && a.CenterId == model.CenterId);

            if (!isAssigned)
                return "Provider is not assigned to the selected center.";


            var providerCenterService = new ProviderCenterService
            {
                ProviderId = model.ProviderId,
                ServiceId = model.ServiceId,
                CenterId = model.CenterId,
                Duration = model.Duration,
                Price = model.Price,
                Priority = model.Priority,
                IsDeleted = false
            };

            providerCenterServiceRepository.Add(providerCenterService);
            commitData.SaveChanges();

            return "Service successfully assigned to center for the provider!";
        }

        // Create shift
        public string CreateShift(ShiftViewModel model)
        {
            var assignment = providerAssignmentRepository.GetById(a => a.AssignmentId == model.ProviderAssignmentId);
            if (assignment == null)
                return "Invalid provider assignment ID.";

            var shift = new Shift
            {
                ProviderAssignmentId = model.ProviderAssignmentId,
                ShiftType = model.ShiftType,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                MaxPatientsPerDay = model.MaxPatientsPerDay,
                IsDeleted = false,

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
