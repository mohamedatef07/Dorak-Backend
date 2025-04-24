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
using Dorak.DataTransferObject;
using System.Data.Entity.Core.Common;
using Dorak.DataTransferObject.ShiftDTO;
using Dorak.DataTransferObject.ProviderDTO;




namespace Services
{
    public class ProviderServices
    {
        public ProviderRepository providerRepository;
        public ProviderAssignmentRepository providerAssignmentRepository;
        public ShiftRepository shiftRepository;
        public ProviderCenterServiceRepository providerCenterServiceRepository;
        public ServicesRepository servicesRepository;
        public UserManager<User> userManager;
        public CommitData commitData;
        public ProviderServices(
            ProviderRepository _providerRepository,
            ProviderAssignmentRepository _providerAssignmentRepository,
            ShiftRepository _shiftRepository,
            ProviderCenterServiceRepository _providerCenterServiceRepository,
            UserManager<User> _userManager,
            CommitData _commitData, ServicesRepository _servicesRepository)

        {
            providerRepository = _providerRepository;
            providerAssignmentRepository = _providerAssignmentRepository;
            shiftRepository = _shiftRepository;
            providerCenterServiceRepository = _providerCenterServiceRepository;
            userManager = _userManager;
            commitData = _commitData;
            servicesRepository = _servicesRepository;
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
                Bio = model.Bio,
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
            commitData.SaveChanges();
        }

        // Assign provider to center manually - for visitor provider
        public string AssignProviderToCenter(ProviderAssignmentViewModel model)
        {
            var assignment = new ProviderAssignment
            {
                ProviderId = model.ProviderId,
                CenterId = model.CenterId,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                AssignmentType = model.AssignmentType,
                IsDeleted = false

            };

            providerAssignmentRepository.Add(assignment);
            commitData.SaveChanges();

            foreach (ShiftViewModel shift in model.Shifts)
            {

                CreateShift(shift, assignment);

            }

            return "Provider assigned Succesfully!";
        }

        // Assign provider to center weekly - for permanent provider
        public string AssignProviderToCenterWithWorkingDays(WeeklyProviderAssignmentViewModel model)
        {

            if (model.WorkingDays == null || !model.WorkingDays.Any())
                return "Please select at least one working day.";

            if (model.StartDate == null)
                return "Please provide a start date.";

            DateOnly startDate = DateOnly.FromDateTime(model.StartDate.Value);
            DateOnly currentDate = startDate;
            DateOnly? rangeStart = null;

            List<ProviderAssignment> assignments = new List<ProviderAssignment>();


            for (int i = 0; i < 28; i++)
            {
                int dayOfWeek = (int)currentDate.DayOfWeek;

                if (model.WorkingDays.Contains(dayOfWeek))
                {
                    if (rangeStart == null)
                    {
                        rangeStart = currentDate;
                    }
                }
                else
                {
                    if (rangeStart != null)
                    {
                        assignments.Add(new ProviderAssignment
                        {
                            ProviderId = model.ProviderId,
                            CenterId = model.CenterId,
                            AssignmentType = model.AssignmentType,
                            StartDate = rangeStart.Value,
                            EndDate = currentDate.AddDays(-1),
                            IsDeleted = false
                        });

                        rangeStart = null;
                    }
                }

                currentDate = currentDate.AddDays(1);
            }

            if (rangeStart != null)
            {
                assignments.Add(new ProviderAssignment
                {
                    ProviderId = model.ProviderId,
                    CenterId = model.CenterId,
                    AssignmentType = model.AssignmentType,
                    StartDate = rangeStart.Value,
                    EndDate = currentDate.AddDays(-1),
                    IsDeleted = false
                });
            }


            foreach (var assignment in assignments)
            {
                providerAssignmentRepository.Add(assignment);
            }
            commitData.SaveChanges();


            foreach (var assignment in assignments)
            {
                foreach (ShiftViewModel shift in model.Shifts)
                {
                    CreateShift(shift, assignment);
                }
            }

            return "Weekly assignment completed successfully.";
        }

        // background service to reassign 
        public string RegenerateWeeklyAssignments()
        {
            var weeklyAssignments = providerAssignmentRepository.GetAll()
                .Where(pa => pa.AssignmentType == AssignmentType.Permanent && !pa.IsDeleted)
                .ToList();

            if (!weeklyAssignments.Any())
                return "No weekly assignments found.";

            var distinctProviders = weeklyAssignments
                .Select(pa => pa.ProviderId)
                .Distinct()
                .ToList();

            foreach (var providerId in distinctProviders)
            {
                var latestAssignment = providerAssignmentRepository.GetAll()
                    .Where(pa => pa.ProviderId == providerId)
                    .OrderByDescending(pa => pa.EndDate)
                    .FirstOrDefault();

                if (latestAssignment == null) continue;

                var startDate = latestAssignment.EndDate.AddDays(-30);

                var betweenAssignments = providerAssignmentRepository.GetAll()
                    .Where(pa => pa.ProviderId == providerId && pa.StartDate >= startDate)
                    .ToList();

                foreach (var betweenDays in betweenAssignments)
                {

                    var newAssignment = new ProviderAssignment
                    {

                        ProviderId = betweenDays.ProviderId,
                        CenterId = betweenDays.CenterId,
                        AssignmentType = betweenDays.AssignmentType,
                        StartDate = betweenDays.StartDate.AddDays(28),
                        EndDate = betweenDays.EndDate.AddDays(28),
                        IsDeleted = false
                    };


                    providerAssignmentRepository.Add(newAssignment);
                    commitData.SaveChanges();


                    var existingShifts = shiftRepository.GetAll()
                        .Where(s => s.ProviderAssignmentId == betweenDays.AssignmentId && !s.IsDeleted)
                        .ToList();

                    foreach (var shift in existingShifts)
                    {

                        var shiftExists = shiftRepository.GetAll()
                        .Any(s => s.ProviderAssignmentId == newAssignment.AssignmentId
                            && s.ShiftType == shift.ShiftType
                            && s.StartTime == shift.StartTime
                            && s.EndTime == shift.EndTime
                            && !s.IsDeleted);

                        if (shiftExists) continue;

                        ShiftViewModel shiftView = new ShiftViewModel
                        {
                            ShiftType = shift.ShiftType,
                            StartTime = shift.StartTime,
                            EndTime = shift.EndTime,
                            MaxPatientsPerDay = shift.MaxPatientsPerDay
                        };
                        CreateShift(shiftView, newAssignment);
                    }
                }
            }

            return "Weekly assignments regenerated successfully.";
        }

        //Get provider main Information 
        public GetProviderMainInfoDTO GetProviderMainInfo(Provider provider)
        {
            return new GetProviderMainInfoDTO
            {
                FirstName = provider.FirstName,
                LastName = provider.LastName,
                Specialization = provider.Specialization,
                Bio = provider.Bio,
                Rate = provider.Rate,
                Image = provider.Image
            };
        }
        //Get Provider Booking Information
        public List<GetProviderBookingInfoDTO> GetProviderBookingInfo(Provider provider)
        {
            var providerAssignments = provider.ProviderAssignments.Where(pa => pa.StartDate >= DateTime.Now || pa.EndDate >= DateTime.Now);
            List<GetProviderBookingInfoDTO> bookingInfo = new List<GetProviderBookingInfoDTO>();
            List<Shift> shifts = new List<Shift>();
            foreach (var providerAssignment in providerAssignments)
            {
                shifts = shiftRepository.GetAllShiftsByAssignmentId(providerAssignment.AssignmentId);
                foreach (var shift in shifts)
                {
                    if (shift.ShiftDate >= DateOnly.FromDateTime(DateTime.Now.AddMonths(1)))
                    {
                        break;
                    }
                    else if (shift.ShiftDate >= DateOnly.FromDateTime(DateTime.Now))
                    {
                        var newShift = new GetProviderBookingInfoDTO()
                        {
                            StartTime = shift.StartTime,
                            EndTime = shift.EndTime,
                            ShiftType = shift.ShiftType,
                            CenterId = providerAssignment.CenterId,
                            Date = shift.ShiftDate
                        };
                        bookingInfo.Add(newShift);
                    }
                }
            }
            return bookingInfo;
        }
        // Get schedule setails
        public List<GetProviderScheduleDetailsDTO> GetScheduleDetails(Provider provider)
        {
            var providerAssignments = providerAssignmentRepository.GetCurrentAssignmentsForProvider(provider.ProviderId);
            List<GetProviderScheduleDetailsDTO> shiftDetails = new List<GetProviderScheduleDetailsDTO>();
            List<Shift> shifts = new List<Shift>();
            foreach (var providerAssignment in providerAssignments)
            {
                shifts = shiftRepository.GetAllShiftsByAssignmentId(providerAssignment.AssignmentId);

                foreach (var shift in shifts)
                {
                    var newShift = new GetProviderScheduleDetailsDTO
                    {
                        CenterId = shift.ProviderAssignment.CenterId,
                        ShiftId = shift.ShiftId,
                        ShiftType = shift.ShiftType,
                        StartTime = shift.StartTime,
                        EndTime = shift.EndTime,
                        ShiftDate = shift.ShiftDate
                    };
                    shiftDetails.Add(newShift);
                }
            }
            return shiftDetails;
        }
        // Get shift details
        public GetShiftDetailsDTO GetShiftDetails(int shiftId)
        {
            Shift shift = shiftRepository.GetById(s => s.ShiftId == shiftId);
            GetShiftDetailsDTO shiftDetails = new GetShiftDetailsDTO
            {
                CenterId = shift.ProviderAssignment.CenterId,
                ShiftId = shift.ShiftId,
                ShiftType = shift.ShiftType,
                StartTime = shift.StartTime,
                EndTime = shift.EndTime,
                EstimatedDuration = shift.EstimatedDuration,
                TotalAppointments = shift.Appointments.Count(),
                ApprovedAppointments = shift.Appointments.Where(app => app.AppointmentStatus == AppointmentStatus.Confirmed).Count(),
                PendingAppointments = shift.Appointments.Where(app => app.AppointmentStatus == AppointmentStatus.Pending).Count()
            };
            return shiftDetails;
        }
        // Get center Services
        public List<GetCenterServicesShiftDTO> GetCenterServices(Provider provider)
        {
            var providerCenterServices = provider.ProviderCenterServices.Where(pcs => pcs.ProviderId == provider.ProviderId).ToList();
            List<GetCenterServicesShiftDTO> pcs = new List<GetCenterServicesShiftDTO>();

            foreach (var providerCenterService in providerCenterServices)
            {
                GetCenterServicesShiftDTO centerServicesShiftDTO = new GetCenterServicesShiftDTO
                {
                    Center = providerCenterService.Center,
                    Services = providerCenterServices.Where(pcs => pcs.ProviderId == provider.ProviderId && pcs.CenterId == providerCenterService.Center.CenterId).Select(pcs => pcs.Service).ToList()
                };
                pcs.Add(centerServicesShiftDTO);
            }
            return pcs;
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
        public void CreateShift(ShiftViewModel model, ProviderAssignment assignment)
        {

            DateOnly currentDate = assignment.StartDate;
            DateOnly endDate = assignment.EndDate;

            while (currentDate <= endDate)
            {
                var shift = new Shift
                {

                    ProviderAssignmentId = assignment.AssignmentId,
                    ShiftType = model.ShiftType,
                    StartTime = model.StartTime,
                    EndTime = model.EndTime,
                    MaxPatientsPerDay = model.MaxPatientsPerDay,
                    IsDeleted = false,
                    ShiftDate = currentDate
                };

                shiftRepository.Add(shift);
                currentDate = currentDate.AddDays(1);
            }

            commitData.SaveChanges();
        }
        public PaginationViewModel<ProviderViewModel> Search(string searchText = "", int pageNumber = 1,
                                                            int pageSize = 2)
        {
            return providerRepository.Search(searchText, pageNumber, pageSize);
        }

        //////


        public async Task<string> UpdateDoctorProfile(UpdateProviderProfileDTO model)
        {
            var provider = providerRepository.GetById(p => p.ProviderId == model.ProviderId);
            if (provider == null)
                return "Provider not found.";

            // Update Provider Info
            provider.FirstName = model.FirstName;
            provider.LastName = model.LastName;
            provider.BirthDate = model.BirthDate;
            provider.Gender = model.Gender;
            provider.Image = model.Image;

            providerRepository.Edit(provider);

            // Update Identity User Info
            var user = await userManager.FindByIdAsync(model.ProviderId);
            if (user != null)
            {
                user.Email = model.Email;
                user.PhoneNumber = model.Phone;

                var updateResult = await userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    var errorMessages = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                    return $"Failed to update user account: {errorMessages}";
                }
            }
            else
            {
                return "User not found in Identity.";
            }

            commitData.SaveChanges();

            return "Doctor profile updated successfully.";

        }

        public string UpdateProfessionalInfo(UpdateProviderProfessionalInfoDTO model)
        {
            var provider = providerRepository.GetById(p => p.ProviderId == model.ProviderId);
            if (provider == null)
                return "Provider not found.";

            provider.Specialization = model.Specialization;
            provider.ExperienceYears = model.ExperienceYears;
            provider.LicenseNumber = model.LicenseNumber;
            provider.Bio = model.Bio;

            providerRepository.Edit(provider);
            commitData.SaveChanges();

            return "Professional info updated successfully.";
        }




    }
}


