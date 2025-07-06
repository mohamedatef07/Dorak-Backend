using Data;
using Dorak.DataTransferObject;
using Dorak.DataTransferObject.ProviderDTO;
using Dorak.Models;
using Dorak.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Models.Enums;
using Repositories;




namespace Services
{
    public class ProviderServices
    {
        private readonly ProviderRepository providerRepository;
        private readonly ProviderAssignmentRepository providerAssignmentRepository;
        private readonly ShiftRepository shiftRepository;
        private readonly ProviderCenterServiceRepository providerCenterServiceRepository;
        private readonly ServicesRepository servicesRepository;
        private readonly CenterRepository centerRepository;
        private readonly AccountServices accountServices;
        private readonly UserManager<User> userManager;
        private readonly CommitData commitData;
        private readonly AccountRepository accountRepository;
        private readonly DorakContext context;
        private readonly IWebHostEnvironment env;

        public ProviderServices(
            ProviderRepository _providerRepository,
            ProviderAssignmentRepository _providerAssignmentRepository,
            ShiftRepository _shiftRepository,
            ProviderCenterServiceRepository _providerCenterServiceRepository,
            AccountRepository _accountRepository,
            CenterRepository _centerRepository,
            UserManager<User> _userManager,
            CommitData _commitData,
            ServicesRepository _servicesRepository,
            DorakContext _context,
            IWebHostEnvironment _env
            )
        {
            providerRepository = _providerRepository;
            providerAssignmentRepository = _providerAssignmentRepository;
            shiftRepository = _shiftRepository;
            providerCenterServiceRepository = _providerCenterServiceRepository;
            centerRepository = _centerRepository;
            accountRepository = _accountRepository;
            userManager = _userManager;
            commitData = _commitData;
            servicesRepository = _servicesRepository;
            context = _context;
            env = _env;
        }

        public async Task<ProviderViewModel> GetProviderDetailsById(string providerId)
        {
            var provider = providerRepository.GetById(p => p.ProviderId == providerId && !p.IsDeleted);
            if (provider == null)
            {
                return null;
            }
            var user = await userManager.FindByIdAsync(providerId);
            return new ProviderViewModel
            {
                AssignmentId = 0,
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
                EstimatedDuration = provider.EstimatedDuration,
                //AddDate = provider.AddDate,
                Email = user?.Email,
                PhoneNumber = user?.PhoneNumber
            };
        }

        public async Task<IdentityResult> CreateProvider(string userId, ProviderRegisterViewModel model)
        {
            try
            {


                var _provider = new Provider
                {
                    ProviderId = userId,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Gender = model.Gender,
                    BirthDate = model.BirthDate ?? DateOnly.FromDateTime(DateTime.MinValue),
                    Street = model.Street,
                    City = model.City,
                    Governorate = model.Governorate,
                    Country = model.Country,
                    ExperienceYears = model.ExperienceYears,
                    LicenseNumber = model.LicenseNumber,
                    Bio = model.Bio,
                    EstimatedDuration = model.EstimatedDuration,
                    Specialization = model.Specialization,
                    Image = model.Image
                };

                providerRepository.Add(_provider);
                commitData.SaveChanges();

                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Description = $"Error: {ex.Message}" });
            }
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

        public IEnumerable<object> GetProviderAssignments(string providerId, int centerId)
        {
            if (string.IsNullOrEmpty(providerId) || centerId <= 0)
            {
                return Enumerable.Empty<object>();
            }

            var assignments = providerAssignmentRepository
                .GetList(pa => pa.ProviderId == providerId && pa.CenterId == centerId && !pa.IsDeleted && pa.StartDate.HasValue && pa.EndDate.HasValue)
                .Select(pa => new
                {
                    startDate = pa.StartDate.Value.ToString("yyyy-MM-dd"),
                    endDate = pa.EndDate.Value.ToString("yyyy-MM-dd")
                })
                .Distinct()
                .ToList();
            return assignments;
        }

        public IEnumerable<object> GetAllProviderAssignments(string providerId)
        {
            if (string.IsNullOrEmpty(providerId))
            {
                return Enumerable.Empty<object>();
            }

            var assignments = providerAssignmentRepository
                .GetList(pa => pa.ProviderId == providerId && !pa.IsDeleted && pa.StartDate.HasValue && pa.EndDate.HasValue)
                .Select(pa => new
                {
                    startDate = pa.StartDate.Value.ToString("yyyy-MM-dd"),
                    endDate = pa.EndDate.Value.ToString("yyyy-MM-dd")
                })
                .Distinct()
                .ToList();
            return assignments;
        }
        public string AssignProviderToCenter(ProviderAssignmentViewModel model)
        {
            if (string.IsNullOrEmpty(model.ProviderId) || !providerRepository.GetAll().Any(p => p.ProviderId == model.ProviderId))
            {
                return "Invalid ProviderId: The specified provider does not exist.";
            }

            if (!centerRepository.GetAll().Any(c => c.CenterId == model.CenterId))
            {
                return "Invalid CenterId: The specified center does not exist.";
            }

            DateTime utcNow = DateTime.UtcNow;
            DateOnly today = DateOnly.FromDateTime(utcNow.Date);

            if (model.EndDate.HasValue && model.EndDate < model.StartDate)
            {
                return "End date cannot be before start date.";
            }

            var overlappingAssignment = providerAssignmentRepository.GetAll()
                .Any(pa => pa.ProviderId == model.ProviderId && pa.CenterId == model.CenterId && !pa.IsDeleted &&
                           pa.StartDate <= (model.EndDate ?? pa.StartDate) && (pa.EndDate ?? pa.StartDate) >= model.StartDate);
            if (overlappingAssignment)
            {
                return "Provider is already assigned to this center for the specified date range.";
            }

            var assignment = new ProviderAssignment
            {
                ProviderId = model.ProviderId,
                CenterId = model.CenterId,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                AssignmentType = model.AssignmentType,
                IsDeleted = false
            };

            try
            {
                providerAssignmentRepository.Add(assignment);
                commitData.SaveChanges();
            }
            catch (Exception ex)
            {
                return $"Failed to save assignment: {ex.Message}. Inner Exception: {ex.InnerException?.Message ?? "None"}";
            }

            try
            {
                CreateShift(model.Shifts.ToList(), assignment);
            }
            catch (Exception ex)
            {
                return $"Failed to create shift: {ex.Message}";
            }

            return "Provider assigned successfully!";
        }
        public string AssignProviderToCenterWithWorkingDays(WeeklyProviderAssignmentViewModel model)
        {
            if (model.WorkingDays == null || !model.WorkingDays.Any())
                return "Please select at least one working day.";

            if (model.StartDate == null)
                return "Please provide a start date.";

            if (DateOnly.FromDateTime(model.StartDate.Value) < DateOnly.FromDateTime(DateTime.Now))
                return "Start date cannot be in the past.";

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
                CreateShift(model.Shifts.ToList(), assignment);
            }

            return "Weekly assignment completed successfully.";
        }

        //public string RescheduleAssignment(RescheduleAssignmentViewModel model)
        //{
        //    var existingAssignments = providerAssignmentRepository.GetAll()
        //        .Where(pa => pa.ProviderId == model.ProviderId && pa.CenterId == model.CenterId && !pa.IsDeleted)
        //        .ToList();

        //    if (model.WorkingDays == null || !model.WorkingDays.Any() || model.EndDate != null)
        //    {
        //        if (!existingAssignments.Any())
        //        {
        //            return "No existing assignments found to reschedule.";
        //        }

        //        var assignmentToUpdate = existingAssignments.First();
        //        assignmentToUpdate.StartDate = model.StartDate;
        //        assignmentToUpdate.EndDate = model.EndDate;
        //        assignmentToUpdate.AssignmentType = AssignmentType.Visiting;

        //        var existingShifts = shiftRepository.GetAll()
        //            .Where(s => s.ProviderAssignmentId == assignmentToUpdate.AssignmentId && !s.IsDeleted)
        //            .ToList();

        //        if (model.Shifts != null && model.Shifts.Any())
        //        {
        //            var existingShiftsByDate = existingShifts.ToLookup(s => s.ShiftDate);

        //            foreach (var shiftViewModel in model.Shifts)
        //            {
        //                var matchingShift = existingShiftsByDate[shiftViewModel.ShiftDate]
        //                    .FirstOrDefault(s => s.StartTime == shiftViewModel.StartTime && s.EndTime == shiftViewModel.EndTime);

        //                if (matchingShift != null)
        //                {
        //                    matchingShift.StartTime = shiftViewModel.StartTime;
        //                    matchingShift.EndTime = shiftViewModel.EndTime;
        //                    matchingShift.MaxPatientsPerDay = shiftViewModel.MaxPatientsPerDay;
        //                }
        //                else
        //                {
        //                    CreateShift(new List<ShiftViewModel> { shiftViewModel }, assignmentToUpdate);
        //                }
        //            }

        //            var newShiftDates = model.Shifts.Select(s => s.ShiftDate).Distinct().ToList();
        //            foreach (var shift in existingShifts)
        //            {
        //                if (!newShiftDates.Contains(shift.ShiftDate) ||
        //                    !model.Shifts.Any(s => s.ShiftDate == shift.ShiftDate && s.StartTime == shift.StartTime && s.EndTime == shift.EndTime))
        //                {
        //                    shift.IsDeleted = true;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            foreach (var shift in existingShifts)
        //            {
        //                shift.IsDeleted = true;
        //            }
        //        }

        //        commitData.SaveChanges();
        //        return "Manually assignment rescheduled successfully.";
        //    }
        //    else
        //    {
        //        foreach (var oldAssignment in existingAssignments)
        //        {
        //            var oldShifts = shiftRepository.GetAll()
        //                .Where(s => s.ProviderAssignmentId == oldAssignment.AssignmentId)
        //                .ToList();

        //            foreach (var shift in oldShifts)
        //            {
        //                shift.IsDeleted = true;
        //            }

        //            oldAssignment.IsDeleted = true;
        //        }

        //        commitData.SaveChanges();

        //        List<ProviderAssignment> newAssignments = new();
        //        DateOnly currentDate = model.StartDate.Value;
        //        DateOnly? rangeStart = null;

        //        for (int i = 0; i < 28; i++)
        //        {
        //            int dow = (int)currentDate.DayOfWeek;

        //            if (model.WorkingDays.Contains(dow))
        //            {
        //                rangeStart ??= currentDate;
        //            }
        //            else if (rangeStart != null)
        //            {
        //                newAssignments.Add(new ProviderAssignment
        //                {
        //                    ProviderId = model.ProviderId,
        //                    CenterId = model.CenterId,
        //                    AssignmentType = AssignmentType.Permanent,
        //                    StartDate = rangeStart.Value,
        //                    EndDate = currentDate.AddDays(-1),
        //                    IsDeleted = false
        //                });

        //                rangeStart = null;
        //            }

        //            currentDate = currentDate.AddDays(1);
        //        }

        //        if (rangeStart != null)
        //        {
        //            newAssignments.Add(new ProviderAssignment
        //            {
        //                ProviderId = model.ProviderId,
        //                CenterId = model.CenterId,
        //                AssignmentType = AssignmentType.Permanent,
        //                StartDate = rangeStart.Value,
        //                EndDate = currentDate.AddDays(-1),
        //                IsDeleted = false
        //            });
        //        }

        //        foreach (var assignment in newAssignments)
        //        {
        //            providerAssignmentRepository.Add(assignment);
        //        }

        //        commitData.SaveChanges();

        //        if (model.Shifts != null)
        //        {
        //            foreach (var assignment in newAssignments)
        //            {
        //                CreateShift(model.Shifts.ToList(), assignment);
        //            }
        //        }

        //        return "Weekly assignment rescheduled successfully.";
        //    }
        //}

        public string RescheduleAssignment(RescheduleAssignmentViewModel model)
        {
            // Validate ProviderId and CenterId
            if (string.IsNullOrEmpty(model.ProviderId) || !providerRepository.GetAll().Any(p => p.ProviderId == model.ProviderId))
            {
                return "Invalid ProviderId: The specified provider does not exist.";
            }

            if (!centerRepository.GetAll().Any(c => c.CenterId == model.CenterId))
            {
                return "Invalid CenterId: The specified center does not exist.";
            }

            // Validate date range
            if (model.EndDate.HasValue && model.EndDate < model.StartDate)
            {
                return "End date cannot be before start date.";
            }

            // Get existing assignments for the provider and center
            var existingAssignments = providerAssignmentRepository.GetAll()
                .Where(pa => pa.ProviderId == model.ProviderId && pa.CenterId == model.CenterId && !pa.IsDeleted)
                .ToList();

            // Soft-delete existing assignments and their shifts
            foreach (var oldAssignment in existingAssignments)
            {
                var oldShifts = shiftRepository.GetAll()
                    .Where(s => s.ProviderAssignmentId == oldAssignment.AssignmentId && !s.IsDeleted)
                    .ToList();

                foreach (var shift in oldShifts)
                {
                    shift.IsDeleted = true;
                }

                oldAssignment.IsDeleted = true;
            }

            // Save soft-deletions
            try
            {
                commitData.SaveChanges();
            }
            catch (Exception ex)
            {
                return $"Failed to soft-delete existing assignments: {ex.Message}. Inner Exception: {ex.InnerException?.Message ?? "None"}";
            }

            // Handle weekly mode
            if (model.WorkingDays != null && model.WorkingDays.Any() && model.EndDate == null)
            {
                List<ProviderAssignment> newAssignments = new();
                DateOnly currentDate = model.StartDate.Value;
                DateOnly? rangeStart = null;

                // Generate assignments for a 28-day period based on WorkingDays
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

                // Save new assignments
                foreach (var assignment in newAssignments)
                {
                    providerAssignmentRepository.Add(assignment);
                }

                try
                {
                    commitData.SaveChanges();
                }
                catch (Exception ex)
                {
                    return $"Failed to save new assignments: {ex.Message}. Inner Exception: {ex.InnerException?.Message ?? "None"}";
                }

                // Create shifts for new assignments
                if (model.Shifts != null && model.Shifts.Any())
                {
                    foreach (var assignment in newAssignments)
                    {
                        try
                        {
                            CreateShift(model.Shifts.ToList(), assignment);
                        }
                        catch (Exception ex)
                        {
                            return $"Failed to create shifts: {ex.Message}. Inner Exception: {ex.InnerException?.Message ?? "None"}";
                        }
                    }
                }

                return "Weekly assignment rescheduled successfully.";
            }
            else // Manual mode
            {
                // Create a new assignment
                var newAssignment = new ProviderAssignment
                {
                    ProviderId = model.ProviderId,
                    CenterId = model.CenterId,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    AssignmentType = AssignmentType.Visiting,
                    IsDeleted = false
                };

                try
                {
                    providerAssignmentRepository.Add(newAssignment);
                    commitData.SaveChanges();
                }
                catch (Exception ex)
                {
                    return $"Failed to save new assignment: {ex.Message}. Inner Exception: {ex.InnerException?.Message ?? "None"}";
                }

                // Create shifts for the new assignment
                if (model.Shifts != null && model.Shifts.Any())
                {
                    try
                    {
                        CreateShift(model.Shifts.ToList(), newAssignment);
                    }
                    catch (Exception ex)
                    {
                        return $"Failed to create shifts: {ex.Message}. Inner Exception: {ex.InnerException?.Message ?? "None"}";
                    }
                }

                return "Manually assignment rescheduled successfully.";
            }
        }


        // background service 

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

                DateOnly? startDate = latestAssignment.EndDate.HasValue
                ? latestAssignment.EndDate.Value.AddDays(-30)
                : null;

                if (!startDate.HasValue) continue;

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
                        StartDate = betweenDays.StartDate.Value.AddDays(28),
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
                        CreateShift(new List<ShiftViewModel> { shiftView }, newAssignment);
                    }
                }
            }

            return "Weekly assignments regenerated successfully.";
        }
        public GetProviderMainInfoDTO GetProviderMainInfo(Provider provider)
        {
            return new GetProviderMainInfoDTO
            {
                FullName = $"{provider.FirstName} {provider.LastName}",
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
                            ShiftId = shift.ShiftId,
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
                List<ProviderCenterService> centerServices = center.ProviderCenterServices.Where(pcs => pcs.ProviderId == provider.ProviderId && pcs.CenterId == center.CenterId).ToList();
                GetProviderCenterServicesDTO providerCenterServicesDTO;
                List<GetProviderSrvicesDTO> providerSrvicesDTO = new List<GetProviderSrvicesDTO>();
                providerSrvicesDTO = centerServices.Select(service => new GetProviderSrvicesDTO
                {
                    ServiceId = service.ServiceId,
                    ServiceName = service.Service.ServiceName,
                    Price = service.Price,
                    Duration = service.Duration,
                }).ToList();
                providerCenterServicesDTO = new GetProviderCenterServicesDTO
                {
                    CenterId = center.CenterId,
                    CenterName = center.CenterName,
                    Latitude = center.Latitude,
                    Longitude = center.Longitude,
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


            var existingService = providerCenterServiceRepository.GetAll()
                .Any(pcs => pcs.ProviderId == model.ProviderId && pcs.CenterId == model.CenterId &&
                            pcs.ServiceId == model.ServiceId && !pcs.IsDeleted);
            if (existingService)
                return "This service is already assigned to the provider at this center.";


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
        //public void CreateShift(List<ShiftViewModel> shifts, ProviderAssignment assignment)
        //{
        //    DateOnly currentDate = assignment.StartDate.Value;
        //    DateOnly endDate = assignment.EndDate ?? currentDate;

        //    foreach (var model in shifts)
        //    {
        //        try
        //        {
        //            // Validate time range
        //            if (model.StartTime >= model.EndTime)
        //                continue; // Skip invalid shifts instead of exiting the method

        //            // Validate MaxPatientsPerDay
        //            if (model.MaxPatientsPerDay.HasValue && model.MaxPatientsPerDay < 0)
        //                continue; // Skip invalid shifts

        //            // Validate ShiftDate is within assignment range
        //            if (model.ShiftDate < currentDate || model.ShiftDate > endDate)
        //                continue; // Skip if date is out of range

        //            // Check for existing shift with the same details
        //            var existingShift = shiftRepository.GetAll()
        //                .FirstOrDefault(s => s.ProviderAssignmentId == assignment.AssignmentId &&
        //                                     s.ShiftDate == model.ShiftDate &&
        //                                     s.StartTime == model.StartTime &&
        //                                     s.EndTime == model.EndTime &&
        //                                     !s.IsDeleted);

        //            if (existingShift != null)
        //            {
        //                // Update existing shift
        //                existingShift.ShiftType = model.ShiftType;
        //                existingShift.MaxPatientsPerDay = model.MaxPatientsPerDay;
        //                existingShift.OperatorId = model.OperatorId;
        //            }
        //            else
        //            {
        //                // Create new shift
        //                var shift = new Shift
        //                {
        //                    ProviderAssignmentId = assignment.AssignmentId,
        //                    ShiftType = model.ShiftType,
        //                    StartTime = model.StartTime,
        //                    EndTime = model.EndTime,
        //                    MaxPatientsPerDay = model.MaxPatientsPerDay,
        //                    IsDeleted = false,
        //                    ShiftDate = model.ShiftDate,
        //                    OperatorId = model.OperatorId
        //                };
        //                shiftRepository.Add(shift);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            // Log the error (e.g., to a file or logging service)
        //            Console.WriteLine($"Error creating/updating shift for date {model.ShiftDate}: {ex.Message}");
        //            continue; // Proceed with next shift
        //        }
        //    }

        //    commitData.SaveChanges(); // Save all changes at the end
        //}

        public void CreateShift(List<ShiftViewModel> shifts, ProviderAssignment assignment)
        {
            if (shifts == null || !shifts.Any())
            {
                Console.WriteLine("No shifts provided to create.");
                return;
            }

            DateOnly currentDate = assignment.StartDate.Value;
            DateOnly endDate = assignment.EndDate ?? currentDate;

            foreach (var model in shifts)
            {
                try
                {
                    // Validate time range
                    if (model.StartTime >= model.EndTime)
                    {
                        Console.WriteLine($"Skipping shift for date {model.ShiftDate}: Invalid time range (StartTime: {model.StartTime}, EndTime: {model.EndTime})");
                        continue;
                    }

                    // Validate MaxPatientsPerDay
                    if (model.MaxPatientsPerDay.HasValue && model.MaxPatientsPerDay < 0)
                    {
                        Console.WriteLine($"Skipping shift for date {model.ShiftDate}: Invalid MaxPatientsPerDay ({model.MaxPatientsPerDay})");
                        continue;
                    }

                    // Validate ShiftDate is within assignment range
                    if (model.ShiftDate < currentDate || model.ShiftDate > endDate)
                    {
                        Console.WriteLine($"Skipping shift for date {model.ShiftDate}: Date is outside assignment range ({currentDate} to {endDate})");
                        continue;
                    }

                    // Validate OperatorId
                    if (string.IsNullOrEmpty(model.OperatorId))
                    {
                        Console.WriteLine($"Skipping shift for date {model.ShiftDate}: OperatorId is null or empty");
                        continue;
                    }

                    // Validate ShiftType
                    if (!Enum.IsDefined(typeof(ShiftType), model.ShiftType))
                    {
                        Console.WriteLine($"Skipping shift for date {model.ShiftDate}: Invalid ShiftType ({model.ShiftType})");
                        continue;
                    }

                    // Check for existing shift (should not exist since we soft-deleted all)
                    var existingShift = shiftRepository.GetAll()
                        .FirstOrDefault(s => s.ProviderAssignmentId == assignment.AssignmentId &&
                                             s.ShiftDate == model.ShiftDate &&
                                             s.StartTime == model.StartTime &&
                                             s.EndTime == model.EndTime &&
                                             !s.IsDeleted);

                    if (existingShift != null)
                    {
                        // Update existing shift
                        existingShift.ShiftType = model.ShiftType;
                        existingShift.MaxPatientsPerDay = model.MaxPatientsPerDay;
                        existingShift.OperatorId = model.OperatorId;
                        Console.WriteLine($"Updated existing shift for date {model.ShiftDate}");
                    }
                    else
                    {
                        // Create new shift
                        var shift = new Shift
                        {
                            ProviderAssignmentId = assignment.AssignmentId,
                            ShiftType = model.ShiftType,
                            StartTime = model.StartTime,
                            EndTime = model.EndTime,
                            MaxPatientsPerDay = model.MaxPatientsPerDay,
                            IsDeleted = false,
                            ShiftDate = model.ShiftDate,
                            OperatorId = model.OperatorId
                        };
                        shiftRepository.Add(shift);
                        Console.WriteLine($"Added new shift for date {model.ShiftDate}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating/updating shift for date {model.ShiftDate}: {ex.Message}. Inner Exception: {ex.InnerException?.Message ?? "None"}");
                    continue;
                }
            }

            try
            {
                commitData.SaveChanges();
                Console.WriteLine("Successfully saved all shifts.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save shifts: {ex.Message}. Inner Exception: {ex.InnerException?.Message ?? "None"}");
            }
        }

        public PaginationViewModel<ProviderViewModel> Search(string searchText = "", int pageNumber = 1,
                                                            int pageSize = 2)
        {
            return providerRepository.Search(searchText, pageNumber, pageSize);
        }
        private async Task<string> SaveImageAsync(ProviderProfileDTO user, string providerId)
        {
            if (user.ImageFile != null && user.ImageFile.Length > 0)
            {
                // المسار يكون داخل wwwroot/image/provider/{ID}
                var folderPath = Path.Combine(env.WebRootPath, "image", "provider", providerId);
                Directory.CreateDirectory(folderPath);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(user.ImageFile.FileName);
                var fullPath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await user.ImageFile.CopyToAsync(stream);
                }

                // هنا return path يكون من بعد wwwroot
                string imagePath = $"/image/provider/{providerId}/{fileName}";
                return imagePath;
            }

            return null;
        }
        public async Task<string> UpdateDoctorProfile(string userId, UpdateProviderProfileDTO model)
        {
            var provider = providerRepository.GetById(p => p.ProviderId == userId && !p.IsDeleted);
            if (provider == null)
                return "Provider not found.";

            var user = provider.User;
            if (user == null)
                return "User data not found.";

            if (!string.IsNullOrWhiteSpace(model.FirstName))
                provider.FirstName = model.FirstName;

            if (!string.IsNullOrWhiteSpace(model.LastName))
                provider.LastName = model.LastName;

            if (!string.IsNullOrWhiteSpace(model.Email))
                user.Email = model.Email;

            if (!string.IsNullOrWhiteSpace(model.Phone))
                user.PhoneNumber = model.Phone;

            if (model.BirthDate != default)
                provider.BirthDate = model.BirthDate;

            if (model.Image != null)
            {
                var imageDto = new ProviderProfileDTO
                {
                    Role = "Provider",
                    ImageFile = model.Image
                };

                var savedImagePath = await SaveImageAsync(imageDto, provider.ProviderId);
                provider.Image = savedImagePath;
            }

            providerRepository.Edit(provider);
            accountRepository.Edit(user);

            await context.SaveChangesAsync();

            return "Profile updated successfully.";
        }
        public bool UpdateProfessionalInfo(string userId, UpdateProviderProfessionalInfoDTO model)
        {
            var provider = providerRepository.GetById(p => p.ProviderId == userId && !p.IsDeleted);
            if (provider == null)
                return false;

            provider.Specialization = model.Specialization;
            provider.ExperienceYears = model.ExperienceYears;
            provider.LicenseNumber = model.LicenseNumber;
            provider.Bio = model.Bio;
            providerRepository.Edit(provider);
            commitData.SaveChanges();
            return true;
        }
        public List<ProviderCardViewModel> GetProviderCards()
        {
            var providers = context.Providers
                .Where(p => !p.IsDeleted)
                .Select(p => new ProviderCardViewModel
                {
                    id = p.ProviderId,
                    Image = p.Image,
                    FullName = $"{p.FirstName} {p.LastName}",
                    Specialization = p.Specialization,
                    City = p.City,
                    Rate = p.Rate,
                    EstimatedDuration = p.EstimatedDuration,
                    Price = p.ProviderCenterServices.Any()
                        ? p.ProviderCenterServices.Min(s => s.Price)
                        : 0
                })
                .ToList();

            return providers;
        }
        public List<ProviderCardViewModel> SearchProviders(string? searchText, string? city, string? specialization)
        {
            var query = context.Providers
                .Where(p => !p.IsDeleted);

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(p => (p.FirstName + " " + p.LastName).ToLower().Contains(searchText.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(city))
            {
                query = query.Where(p => p.City.ToLower() == city.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(specialization))
            {
                query = query.Where(p => p.Specialization.ToLower() == specialization.ToLower());
            }

            return query.Select(p => new ProviderCardViewModel
            {
                FullName = $"{p.FirstName} {p.LastName}",
                Specialization = p.Specialization,
                City = p.City,
                Rate = p.Rate,
                EstimatedDuration = p.EstimatedDuration,
                Price = p.ProviderCenterServices.Any()
                    ? p.ProviderCenterServices.Min(s => s.Price)
                    : 0
            }).ToList();
        }


        public List<ProviderCardViewModel> FilterProviders(FilterProviderDTO filter)
        {
            var query = context.Providers
                .Where(p => !p.IsDeleted);

            if (filter.Gender.HasValue)
                query = query.Where(p => p.Gender == (GenderType)filter.Gender.Value);

            if (filter.Title.HasValue)
                query = query.Where(p => p.providerTitle == (ProviderTitle)filter.Title.Value);

            if (!string.IsNullOrWhiteSpace(filter.City))
                query = query.Where(p => p.City.ToLower().Contains(filter.City.ToLower()));

            if (filter.MinRate.HasValue)
                query = query.Where(p => p.Rate >= (decimal)filter.MinRate.Value);

            if (filter.MaxRate.HasValue)
                query = query.Where(p => p.Rate <= (decimal)filter.MaxRate.Value);

            if (filter.MinPrice.HasValue || filter.MaxPrice.HasValue)
            {
                query = query.Where(p => p.ProviderCenterServices.Any(s =>
                    (!filter.MinPrice.HasValue || s.Price >= filter.MinPrice.Value) &&
                    (!filter.MaxPrice.HasValue || s.Price <= filter.MaxPrice.Value)
                ));
            }

            if (filter.AvailableDate.HasValue)
            {
                var date = filter.AvailableDate.Value;
                query = query.Where(p => p.ProviderAssignments.Any(a =>
                    a.StartDate <= date && a.EndDate >= date
                ));
            }

            return query
                .ToList()
                .Select(p => p.ToCardView())
                .ToList();
        }

        public List<ProviderCardViewModel> GetTopRatedProviders(int count = 3)
        {
            return context.Providers
                .OrderByDescending(p => p.Rate)
                .Take(count)
                .Select(p => new ProviderCardViewModel
                {
                    id = p.ProviderId,
                    FullName = $"{p.FirstName} {p.LastName}",
                    Specialization = p.Specialization,
                    Rate = p.Rate,
                    Image = p.Image,
                }).ToList();
        }

        public ProviderProfileDTO GetProviderProfile(string userId)
        {
            var provider = providerRepository.GetById(p => p.ProviderId == userId && !p.IsDeleted);

            if (provider == null || provider.User == null)
            {
                return null;
            }

            return new ProviderProfileDTO
            {
                ID = provider.ProviderId,
                FullName = $"{provider.FirstName} {provider.LastName}",
                Email = provider.User.Email,
                Phone = provider.User.PhoneNumber,
                Gender = provider.Gender,
                BirthDate = provider.BirthDate,
                Image = provider.Image,
                Specialization = provider.Specialization,
                Experience = provider.ExperienceYears,
                MedicalLicenseNumber = provider.LicenseNumber,
                About = provider.Bio
            };
        }

        public GeneralStatisticsDTO GetGeneralStatistics(string providerId)
        {
            var provider = providerRepository.GetProviderById(providerId);
            if (provider == null)
            {
                return null;
            }
            int TotalAppointments = 0;
            int TotalUrgentCases = 0;
            int PatientsTreatedToday = 0;
            int PatientsInQueue = 0;
            foreach (var pcs in provider.ProviderCenterServices)
            {
                TotalAppointments += pcs.Appointments.Count();
                TotalUrgentCases += pcs.Appointments.Where(app => app.AppointmentType == AppointmentType.Urgent).Count();
                PatientsTreatedToday += pcs.Appointments.Where(app => app.AppointmentStatus == AppointmentStatus.Confirmed && app.Shift.ShiftDate == DateOnly.FromDateTime(DateTime.Today)).Count();
                PatientsInQueue += pcs.Appointments.Select(app => app.Shift).Where(sh => sh.ShiftType == ShiftType.OnGoing).Select(sh => sh.LiveQueues).Count();
            }
            return new GeneralStatisticsDTO
            {
                TotalAppointments = TotalAppointments,
                TotalUrgentCases = TotalUrgentCases,
                PatientsInQueue = PatientsInQueue,
                PatientsTreatedToday = PatientsInQueue,
                AverageEstimatedTime = provider.EstimatedDuration,
            };
        }
    }
}







