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
using System.Globalization;


namespace Services
{
    public class ProviderServices
    {
        public ProviderRepository providerRepository;
        public ProviderAssignmentRepository providerAssignmentRepository;
        public ShiftRepository shiftRepository;
        public ProviderCenterServiceRepository providerCenterServiceRepository;
        public ServicesRepository servicesRepository;
        public CenterRepository centerRepository;
        public AccountServices accountServices;
        public UserManager<User> userManager;
        public CommitData commitData;
        private readonly DorakContext context;



        public ProviderServices(
            ProviderRepository _providerRepository,
            ProviderAssignmentRepository _providerAssignmentRepository,
            ShiftRepository _shiftRepository,
            ProviderCenterServiceRepository _providerCenterServiceRepository,
            CenterRepository _centerRepository,
            // AccountServices _accountServices,
            UserManager<User> _userManager,
            CommitData _commitData , 
            ServicesRepository _servicesRepository,
            DorakContext _context
            )

        {
            providerRepository = _providerRepository;
            providerAssignmentRepository = _providerAssignmentRepository;
            shiftRepository = _shiftRepository;
            providerCenterServiceRepository = _providerCenterServiceRepository;
            centerRepository = _centerRepository;
            // accountServices = _accountServices;
            userManager = _userManager;
            commitData = _commitData;
            servicesRepository = _servicesRepository;
            context = _context;
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
                Availability = provider.Availability,
                EstimatedDuration = provider.EstimatedDuration,
                //AddDate = provider.AddDate,
                Email = user?.Email,
                PhoneNumber = user?.PhoneNumber
            };
        }

        // Creating a New User-Provider 
        //public async Task<IdentityResult> CreateProvider(string userId, ProviderRegisterViewModel model)
        //{
        //    var _provider = new Provider
        //    {
        //        ProviderId = userId,
        //        FirstName = model.FirstName,
        //        LastName = model.LastName,
        //        Gender = model.Gender,
        //        BirthDate = model.BirthDate,
        //        Street = model.Street,
        //        City = model.City,
        //        Governorate = model.Governorate,
        //        Country = model.Country,
        //        ExperienceYears = model.ExperienceYears,
        //        LicenseNumber = model.LicenseNumber,
        //        Bio = model.Bio,
        //        EstimatedDuration = model.EstimatedDuration,
        //        Availability = model.Availability,
        //        //RATE
        //        Specialization = model.Specialization,
        //        Image = model.Image,
        //    };

        //    providerRepository.Add(_provider);
        //    commitData.SaveChanges();
        //    return IdentityResult.Success;
        //}

        //public async Task<string> GetProviderID(ProviderAssignmentDTO providerDto)
        //{
        //    try
        //    {
        //        Console.WriteLine($"AddProviderAsync started at {DateTime.Now} (UTC: {DateTime.UtcNow}): centerId={providerDto.CenterId}, startDate={providerDto.WorkingDates?.FirstOrDefault()?.startDate}, endDate={providerDto.WorkingDates?.FirstOrDefault()?.endDate}, assignmentType={providerDto.AssignmentType}");


        //        var user = new RegisterationViewModel
        //        {
        //            UserName = providerDto.UserName,
        //            Email = providerDto.Email,
        //            PhoneNumber = providerDto.PhoneNumber,
        //            Password = providerDto.Password,
        //            ConfirmPassword = providerDto.ConfirmPassword,
        //            Role = providerDto.Role,
        //            FirstName = providerDto.FirstName,
        //            LastName = providerDto.LastName,
        //            Gender = providerDto.Gender,
        //            BirthDate = providerDto.BirthDate,
        //            Street = providerDto.Street,
        //            City = providerDto.City,
        //            Governorate = providerDto.Governorate,
        //            Country = providerDto.Country,
        //            Image = providerDto.Image,
        //            Specialization = providerDto.Specialization,
        //            LicenseNumber = providerDto.LicenseNumber
        //        };

        //        var userRes = await accountServices.CreateAccount(user);
        //        Console.WriteLine($"CreateAccount result at {DateTime.Now}: Succeeded={userRes.Succeeded}, Errors={string.Join(", ", userRes.Errors.Select(e => e.Description) ?? new[] { "No errors provided" })}");

        //        if (userRes.Succeeded)
        //        {
        //            var providerId = await accountServices.getIdByUserName(providerDto.UserName);

        //            return providerId;
        //        }

        //        return $"Not Valid";
        //    }

        //    catch
        //    {
        //        return $"Not Valid";
        //    }

        //}

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
                    BirthDate = model.BirthDate ?? DateOnly.FromDateTime(DateTime.MinValue), // Use nullable fallback
                    Street = model.Street,
                    City = model.City,
                    Governorate = model.Governorate,
                    Country = model.Country,
                    ExperienceYears = model.ExperienceYears,
                    LicenseNumber = model.LicenseNumber,
                    Bio = model.Bio,
                    EstimatedDuration = model.EstimatedDuration,
                    Availability = model.Availability,
                    Specialization = model.Specialization,
                    Image = model.Image
                };

                providerRepository.Add(_provider);
                commitData.SaveChanges();
                
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in CreateProvider: {ex.Message}");
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

        // Assign provider to center manually - for visitor provider
        //public string AssignProviderToCenter(ProviderAssignmentViewModel model)
        //{
        //    // Validation: Check for past dates
        //    if (model.StartDate < DateOnly.FromDateTime(DateTime.Now))
        //        return "Start date cannot be in the past.";

        //    if (model.EndDate.HasValue && model.EndDate < model.StartDate)
        //        return "End date cannot be before start date.";

        //    // Validation: Check for duplicate assignment (same provider and center with overlapping dates)
        //    var overlappingAssignment = providerAssignmentRepository.GetAll()
        //        .Any(pa => pa.ProviderId == model.ProviderId && pa.CenterId == model.CenterId && !pa.IsDeleted &&
        //                   pa.StartDate <= (model.EndDate ?? pa.StartDate) && (pa.EndDate ?? pa.StartDate) >= model.StartDate);
        //    if (overlappingAssignment)
        //        return "Provider is already assigned to this center for the specified date range.";

        //    var assignment = new ProviderAssignment
        //    {
        //        ProviderId = model.ProviderId,
        //        CenterId = model.CenterId,
        //        StartDate = model.StartDate,
        //        EndDate = model.EndDate, // This works since EndDate is now DateOnly? and matches the entity's expected type
        //        AssignmentType = model.AssignmentType,
        //        IsDeleted = false
        //    };

        //    providerAssignmentRepository.Add(assignment);
        //    commitData.SaveChanges();

        //    foreach (ShiftViewModel shift in model.Shifts)
        //    {
        //        CreateShift(shift, assignment);
        //    }

        //    return "Provider assigned successfully!";
        //}


        // manually
        public string AssignProviderToCenter(ProviderAssignmentViewModel model)
        {
            // Log the input and server time for debugging
            Console.WriteLine($"AssignProviderToCenter called at {DateTime.UtcNow} (Local: {DateTime.Now})");

            // Log the raw model data as received
            Console.WriteLine($"Raw Input - StartDate: {model.StartDate.ToString()}, EndDate: {(model.EndDate.HasValue ? model.EndDate.ToString() : "null")}, " +
                             $"ProviderId: {model.ProviderId}, CenterId: {model.CenterId}");

            // Validate ProviderId
            if (string.IsNullOrEmpty(model.ProviderId) || !providerRepository.GetAll().Any(p => p.ProviderId == model.ProviderId))
            {
                Console.WriteLine($"Validation failed: ProviderId {model.ProviderId} does not exist in Providers table.");
                return "Invalid ProviderId: The specified provider does not exist.";
            }

            // Validate CenterId
            if (!centerRepository.GetAll().Any(c => c.CenterId == model.CenterId))
            {
                Console.WriteLine($"Validation failed: CenterId {model.CenterId} does not exist in Centers table.");
                return "Invalid CenterId: The specified center does not exist.";
            }

            // Log server time details
            DateTime utcNow = DateTime.UtcNow;
            DateOnly today = DateOnly.FromDateTime(utcNow.Date); // Normalize to midnight UTC
            Console.WriteLine($"Server UTC Now: {utcNow}, Today (UTC): {today}, Parsed StartDate: {model.StartDate}");

            // Validation: Check if EndDate is before StartDate
            if (model.EndDate.HasValue && model.EndDate < model.StartDate)
            {
                Console.WriteLine($"Validation failed: EndDate {model.EndDate} is before StartDate {model.StartDate}");
                return "End date cannot be before start date.";
            }

            // Validation: Check for duplicate assignment (same provider and center with overlapping dates)
            var overlappingAssignment = providerAssignmentRepository.GetAll()
                .Any(pa => pa.ProviderId == model.ProviderId && pa.CenterId == model.CenterId && !pa.IsDeleted &&
                           pa.StartDate <= (model.EndDate ?? pa.StartDate) && (pa.EndDate ?? pa.StartDate) >= model.StartDate);
            if (overlappingAssignment)
            {
                Console.WriteLine("Validation failed: Overlapping assignment found");
                return "Provider is already assigned to this center for the specified date range.";
            }

            // Create the assignment
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
                Console.WriteLine($"Assignment saved successfully for ProviderId: {model.ProviderId}, CenterId: {model.CenterId}");
            }
            catch (Exception ex)
            {
                // Log the full exception details, including inner exceptions
                Console.WriteLine($"SaveChanges failed: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    Console.WriteLine($"Inner Exception StackTrace: {ex.InnerException.StackTrace}");
                }
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return $"Failed to save assignment: {ex.Message}. Inner Exception: {ex.InnerException?.Message ?? "None"}";
            }

            // Add shifts if any
            foreach (ShiftViewModel shift in model.Shifts)
            {
                try
                {
                    CreateShift(shift, assignment);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to create shift for assignment: {ex.Message}, StackTrace: {ex.StackTrace}");
                    return $"Failed to create shift: {ex.Message}";
                }
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
                DateOnly currentDate = model.StartDate.Value;
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
            DateOnly currentDate = assignment.StartDate.Value;
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


        public List<ProviderCardViewModel> GetProviderCards()
        {
            var providers = context.Providers
                .Where(p => !p.IsDeleted)
                .Select(p => new ProviderCardViewModel
                {
                    FullName = p.FirstName + " " + p.LastName,
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
                FullName = p.FirstName + " " + p.LastName,
                Specialization = p.Specialization,
                City = p.City,
                Rate = p.Rate,
                EstimatedDuration = p.EstimatedDuration,
                Price = p.ProviderCenterServices.Any()
                    ? p.ProviderCenterServices.Min(s => s.Price)
                    : 0
            }).ToList();
        }

        public List<ProviderCardViewModel> FilterByDay(DateOnly date)
        {
            DateTime dateTime = date.ToDateTime(TimeOnly.MinValue);

            var providers = context.ProviderAssignments
                .Where(a => !a.IsDeleted
                            && a.StartDate <= DateOnly.FromDateTime(dateTime)
                            && a.EndDate >= DateOnly.FromDateTime(dateTime))
                .Select(a => a.Provider)
                .Distinct()
                .Select(p => new ProviderCardViewModel
                {
                    FullName = p.FirstName + " " + p.LastName,
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



    }
}


    




