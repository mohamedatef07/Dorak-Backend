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
            // Validation: Check for past dates
            if (model.StartDate < DateOnly.FromDateTime(DateTime.Now))
                return "Start date cannot be in the past.";

            if (model.EndDate.HasValue && model.EndDate < model.StartDate)
                return "End date cannot be before start date.";

            // Validation: Check for duplicate assignment (same provider and center with overlapping dates)
            var overlappingAssignment = providerAssignmentRepository.GetAll()
                .Any(pa => pa.ProviderId == model.ProviderId && pa.CenterId == model.CenterId && !pa.IsDeleted &&
                           pa.StartDate <= (model.EndDate ?? pa.StartDate) && (pa.EndDate ?? pa.StartDate) >= model.StartDate);
            if (overlappingAssignment)
                return "Provider is already assigned to this center for the specified date range.";

            var assignment = new ProviderAssignment
            {
                ProviderId = model.ProviderId,
                CenterId = model.CenterId,
                StartDate = model.StartDate,
                EndDate = model.EndDate, // This works since EndDate is now DateOnly? and matches the entity's expected type
                AssignmentType = model.AssignmentType,
                IsDeleted = false
            };

            providerAssignmentRepository.Add(assignment);
            commitData.SaveChanges();

            foreach (ShiftViewModel shift in model.Shifts)
            {
                CreateShift(shift, assignment);
            }

            return "Provider assigned successfully!";
        }

        // Assign provider to center weekly - for permanent provider
        public string AssignProviderToCenterWithWorkingDays(WeeklyProviderAssignmentViewModel model)
        {
            if (model.WorkingDays == null || !model.WorkingDays.Any())
                return "Please select at least one working day.";

            if (model.StartDate == null)
                return "Please provide a start date.";

            // Validation: Check for past start date
            if (DateOnly.FromDateTime(model.StartDate.Value) < DateOnly.FromDateTime(DateTime.Now))
                return "Start date cannot be in the past.";

            // Validation: Check for duplicate assignments (same provider and center with overlapping dates)
            DateOnly startDate = DateOnly.FromDateTime(model.StartDate.Value);
            DateOnly endDate = startDate.AddDays(27); 
            var overlappingAssignment = providerAssignmentRepository.GetAll()
                .Any(pa => pa.ProviderId == model.ProviderId && pa.CenterId == model.CenterId && !pa.IsDeleted &&
                           pa.StartDate <= endDate && (pa.EndDate ?? endDate) >= startDate);
            if (overlappingAssignment)
                return "Provider is already assigned to this center for the specified date range.";

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


        // Reschedule provider assignment
        public string RescheduleAssignment(RescheduleAssignmentViewModel model)
        {
            var existingAssignments = providerAssignmentRepository.GetAll()
                .Where(pa => pa.ProviderId == model.ProviderId && pa.CenterId == model.CenterId && !pa.IsDeleted)
                .ToList();

            
            if (model.WorkingDays == null || !model.WorkingDays.Any() || model.EndDate != null)
            {
                if (!existingAssignments.Any())
                {
                    return "No existing assignments found to reschedule.";
                }

                
                var assignmentToUpdate = existingAssignments.First();
                assignmentToUpdate.StartDate = model.StartDate;
                assignmentToUpdate.EndDate = model.EndDate;
                assignmentToUpdate.AssignmentType = AssignmentType.Visiting;

                
                var existingShifts = shiftRepository.GetAll()
                    .Where(s => s.ProviderAssignmentId == assignmentToUpdate.AssignmentId && !s.IsDeleted)
                    .ToList();

               
                if (model.Shifts != null && model.Shifts.Any())
                {
                    for (int i = 0; i < model.Shifts.Count; i++)
                    {
                        var shiftViewModel = model.Shifts[i];
                        if (i < existingShifts.Count)
                        {
                            
                            var shiftToUpdate = existingShifts[i];
                            shiftToUpdate.StartTime = shiftViewModel.StartTime;
                            shiftToUpdate.EndTime = shiftViewModel.EndTime;
                        }
                        else
                        {
                           
                            CreateShift(shiftViewModel, assignmentToUpdate);
                        }
                    }

                    
                    for (int i = model.Shifts.Count; i < existingShifts.Count; i++)
                    {
                        existingShifts[i].IsDeleted = true;
                    }
                }
                else
                {
                    
                    foreach (var shift in existingShifts)
                    {
                        shift.IsDeleted = true;
                    }
                }

                commitData.SaveChanges();
                return "Manual assignment rescheduled successfully.";
            }
            else
            {
                
                foreach (var oldAssignment in existingAssignments)
                {
                    var oldShifts = shiftRepository.GetAll()
                        .Where(s => s.ProviderAssignmentId == oldAssignment.AssignmentId)
                        .ToList();

                    foreach (var shift in oldShifts)
                    {
                        shift.IsDeleted = true;
                    }

                    oldAssignment.IsDeleted = true;
                }

                commitData.SaveChanges();

                List<ProviderAssignment> newAssignments = new();
                DateOnly currentDate = model.StartDate;
                DateOnly? rangeStart = null;

                for (int i = 0; i < 28; i++)
                {
                    int dow = (int)currentDate.DayOfWeek;

                    if (model.WorkingDays.Contains(dow))
                    {
                        rangeStart ??= currentDate;
                    }
                    else if (rangeStart != null)
                    {
                        newAssignments.Add(new ProviderAssignment
                        {
                            ProviderId = model.ProviderId,
                            CenterId = model.CenterId,
                            AssignmentType = AssignmentType.Permanent,
                            StartDate = rangeStart.Value,
                            EndDate = currentDate.AddDays(-1),
                            IsDeleted = false
                        });

                        rangeStart = null;
                    }

                    currentDate = currentDate.AddDays(1);
                }

                if (rangeStart != null)
                {
                    newAssignments.Add(new ProviderAssignment
                    {
                        ProviderId = model.ProviderId,
                        CenterId = model.CenterId,
                        AssignmentType = AssignmentType.Permanent,
                        StartDate = rangeStart.Value,
                        EndDate = currentDate.AddDays(-1),
                        IsDeleted = false
                    });
                }

                foreach (var assignment in newAssignments)
                {
                    providerAssignmentRepository.Add(assignment);
                }

                commitData.SaveChanges();

                if (model.Shifts != null)
                {
                    foreach (var assignment in newAssignments)
                    {
                        foreach (var shift in model.Shifts)
                        {
                            CreateShift(shift, assignment);
                        }
                    }
                }

                return "Weekly assignment rescheduled successfully.";
            }
        }
    
        // background service to rescedule 

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

                var startDate = latestAssignment.EndDate.Value.AddDays(-30);

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
                        EndDate = betweenDays.EndDate.Value.AddDays(28),
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
            var providerAssignments = provider.ProviderAssignments.Where(pa => pa.StartDate >= DateOnly.FromDateTime(DateTime.Now) || pa.EndDate >= DateOnly.FromDateTime(DateTime.Now));
            List<GetProviderBookingInfoDTO> bookingInfo = new List<GetProviderBookingInfoDTO>();
            List<Shift> shifts = new List<Shift>();
            foreach (var providerAssignment in providerAssignments)
            {
                shifts = shiftRepository.GetAllShiftsByAssignmentId(providerAssignment.AssignmentId);
                foreach (var shift in shifts)
                {
                    if(shift.ShiftDate >= DateOnly.FromDateTime(DateTime.Now.AddMonths(1)))
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
        public List<GetProviderCenterServicesDTO> GetCenterServices(Provider provider)
        {
            var uniqueProviderCenterServices = provider.ProviderCenterServices.Where(pcs => pcs.ProviderId == provider.ProviderId).DistinctBy(pcs => pcs.CenterId).ToList();
            List<GetProviderCenterServicesDTO> pcs = new List<GetProviderCenterServicesDTO>();
            foreach (var providerCenterService in uniqueProviderCenterServices)
            {
                Center center = providerCenterService.Center;
                List<Service> services = center.ProviderCenterServices.Where(pcs => pcs.ProviderId == provider.ProviderId && pcs.CenterId == center.CenterId).Select(pcs => pcs.Service).ToList();
                GetProviderCenterServicesDTO providerCenterServicesDTO;
                List<GetProviderSrvicesDTO> providerSrvicesDTO = new List<GetProviderSrvicesDTO>();
                providerSrvicesDTO = services.Select(service => new GetProviderSrvicesDTO
                {
                    ServiceId = service.ServiceId,
                    ServiceName = service.ServiceName,
                }).ToList();
                providerCenterServicesDTO = new GetProviderCenterServicesDTO
                {
                    CenterId = center.CenterId,
                    Services = providerSrvicesDTO
                };
                pcs.Add(providerCenterServicesDTO);
            }
            return pcs;
        }

        // Assign service to center
        public string AssignServiceToCenter(AssignProviderCenterServiceViewModel model)
        {
            var isAssigned = providerAssignmentRepository.GetAll()
                .Any(a => a.ProviderId == model.ProviderId && a.CenterId == model.CenterId && !a.IsDeleted);

            if (!isAssigned)
                return "Provider is not assigned to the selected center.";

            // Validation: Check for duplicate service assignment
            var existingService = providerCenterServiceRepository.GetAll()
                .Any(pcs => pcs.ProviderId == model.ProviderId && pcs.CenterId == model.CenterId &&
                            pcs.ServiceId == model.ServiceId && !pcs.IsDeleted);
            if (existingService)
                return "This service is already assigned to the provider at this center.";

            // Validation: Ensure duration and price are positive
            if (model.Duration <= 0)
                return "Duration must be greater than zero.";
            if (model.Price < 0)
                return "Price cannot be negative.";

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
            // Validation: Ensure start time is before end time
            if (model.StartTime >= model.EndTime)
                return; // Silently return since this is a void method

            // Validation: Ensure max patients is non-negative
            if (model.MaxPatientsPerDay.HasValue && model.MaxPatientsPerDay < 0)
                return;

            // Validation: Check for overlapping shifts on each date
            DateOnly currentDate = assignment.StartDate;
            DateOnly endDate = assignment.EndDate ?? currentDate; // Use StartDate if EndDate is null

            while (currentDate <= endDate)
            {
                var existingShifts = shiftRepository.GetAll()
                    .Where(s => s.ProviderAssignmentId == assignment.AssignmentId &&
                                s.ShiftDate == currentDate && !s.IsDeleted)
                    .ToList();

                // Check if the new shift overlaps with any existing shift on this date
                bool hasOverlap = existingShifts.Any(existingShift =>
                    model.StartTime < existingShift.EndTime && existingShift.StartTime < model.EndTime);

                if (hasOverlap)
                    return;

                var shift = new Shift
                {
                    ProviderAssignmentId = assignment.AssignmentId,
                    ShiftType = model.ShiftType,
                    StartTime = model.StartTime,
                    EndTime = model.EndTime,
                    MaxPatientsPerDay = model.MaxPatientsPerDay,
                    IsDeleted = false,
                    ShiftDate = currentDate,
                    OperatorId = model.OperatorId
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


    




