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

using Dorak.ViewModels.ShiftViewModel;

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

            for (int i = 0; i < 30; i++)
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
                            StartDate = rangeStart.Value.ToDateTime(TimeOnly.MinValue),
                            EndDate = currentDate.AddDays(-1).ToDateTime(TimeOnly.MinValue),
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
                    StartDate = rangeStart.Value.ToDateTime(TimeOnly.MinValue),
                    EndDate = currentDate.AddDays(-1).ToDateTime(TimeOnly.MinValue),
                    IsDeleted = false
                });
            }

            foreach (var assignment in assignments)
            {
                providerAssignmentRepository.Add(assignment);
            }

            commitData.SaveChanges();
            return "Weekly assignment completed successfully.";
        }

        // background service to reassign 
        public string RegenerateWeeklyAssignments()
        {
            var weeklyAssignments = providerAssignmentRepository.GetAll()
                .Where(pa => pa.AssignmentType == AssignmentType.Permanent && pa.StartDate.HasValue && pa.EndDate.HasValue && pa.IsDeleted == false)
                .ToList();

            if (!weeklyAssignments.Any())
                return "No weekly assignments found.";

            foreach (var existing in weeklyAssignments)
            {
                string providerId = existing.ProviderId;
                int centerId = existing.CenterId;

                List<int> workingDays = new List<int>();
                var start = existing.StartDate.Value;
                var end = existing.EndDate.Value;
                for (DateTime date = start; date <= end; date = date.AddDays(1))
                {
                    int dayOfWeek = (int)date.DayOfWeek;
                    if (!workingDays.Contains(dayOfWeek))
                        workingDays.Add(dayOfWeek);
                }

                DateOnly startDate = DateOnly.FromDateTime(DateTime.Today);
                DateOnly currentDate = startDate;
                DateOnly? rangeStart = null;
                List<ProviderAssignment> newAssignments = new();

                for (int i = 0; i < 30; i++)
                {
                    int dow = (int)currentDate.DayOfWeek;

                    if (workingDays.Contains(dow))
                    {
                        rangeStart ??= currentDate;
                    }
                    else if (rangeStart != null)
                    {
                        var startDT = rangeStart.Value.ToDateTime(TimeOnly.MinValue);
                        var endDT = currentDate.AddDays(-1).ToDateTime(TimeOnly.MinValue);

                        bool alreadyExists = providerAssignmentRepository.GetAll()
                            .Any(a => a.ProviderId == providerId && a.CenterId == centerId &&
                                      a.StartDate == startDT && a.EndDate == endDT &&
                                      a.AssignmentType == AssignmentType.Permanent && a.IsDeleted == false);

                        if (!alreadyExists)
                        {
                            newAssignments.Add(new ProviderAssignment
                            {
                                ProviderId = providerId,
                                CenterId = centerId,
                                StartDate = startDT,
                                EndDate = endDT,
                                AssignmentType = AssignmentType.Permanent,
                                IsDeleted = false
                            });
                        }

                        rangeStart = null;
                    }

                    currentDate = currentDate.AddDays(1);
                }


                if (rangeStart != null)
                {
                    var startDT = rangeStart.Value.ToDateTime(TimeOnly.MinValue);
                    var endDT = currentDate.AddDays(-1).ToDateTime(TimeOnly.MinValue);

                    bool alreadyExists = providerAssignmentRepository.GetAll()
                        .Any(a => a.ProviderId == providerId && a.CenterId == centerId &&
                                  a.StartDate == startDT && a.EndDate == endDT &&
                                  a.AssignmentType == AssignmentType.Permanent && a.IsDeleted == false);

                    if (!alreadyExists)
                    {
                        newAssignments.Add(new ProviderAssignment
                        {
                            ProviderId = providerId,
                            CenterId = centerId,
                            StartDate = startDT,
                            EndDate = endDT,
                            AssignmentType = AssignmentType.Permanent,
                            IsDeleted = false
                        });
                    }
                }

                foreach (var assignment in newAssignments)
                {
                    providerAssignmentRepository.Add(assignment);
                }
            }

            commitData.SaveChanges();
            return "30-day weekly assignments regenerated successfully.";
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
            var providerAssignments = provider.ProviderAssignments;//.Where(pa => pa.StartDate <= DateTime.Now && pa.EndDate >= DateTime.Now);
            List<GetProviderBookingInfoDTO> shifts = new List<GetProviderBookingInfoDTO>();
            Shift shift;
            bool IsMonthPassed = false;
            foreach (var providerAssignment in providerAssignments)
            {

                shift = shiftRepository.GetShiftByAssignmentId(providerAssignment.AssignmentId);
                var start = providerAssignment.StartDate.Value;
                var end = providerAssignment.EndDate.Value;
                for (DateTime i = start; i <= end; i = i.AddDays(1))
                {
                    if (i.Date >= DateTime.Now.AddMonths(1))
                    {
                        IsMonthPassed = true;
                        break;
                    }
                    else if (i > DateTime.Now)
                    {
                        if (shift == null) //null ref exception because shift is null
                        {
                            continue;
                        }
                        var newShift = new GetProviderBookingInfoDTO()
                        {
                            Date = i.Date.ToString(),
                            StartTime = shift.StartTime,
                            EndTime = shift.EndTime,
                            ShiftType = shift.ShiftType,
                            CenterId = providerAssignment.CenterId,
                            ServiceId = providerAssignment.Provider.ProviderCenterServices.Where(pas => pas.ProviderId == provider.ProviderId).Select(se => se.ServiceId).ToList(),
                        };
                        shifts.Add(newShift);
                    }
                }
                if (IsMonthPassed)
                {
                    break;
                }
            }
            return shifts;
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
                var newShift = new GetShiftDetailsDTO
                {
                    CenterId = shift.ProviderAssignment.CenterId,
                    ShiftId = shift.ShiftId,
                    ShiftType = shift.ShiftType,
                    StartTime = shift.StartTime,
                    EndTime = shift.EndTime,
                    TotalAppointments = shift.Appointments.Count(),
                    EstimatedDuration = shift.EstimatedDuration,
                    ApprovedAppointments = shift.Appointments.Where(app => app.AppointmentStatus == AppointmentStatus.Confirmed).Count(),
                    PendingAppointments = shift.Appointments.Where(app => app.AppointmentStatus == AppointmentStatus.Pending).Count()
                };
            return newShift;
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
            //var start =assignment.StartDate;
            //var end =assignment.EndDate;
            //int duration = (int)(end.Value.CompareTo(start));
            //for (int i = 0; i <= duration; i++)
            //{
                var shift = new Shift
                {
                    ProviderAssignmentId = model.ProviderAssignmentId,
                    ShiftType = model.ShiftType,
                    StartTime = model.StartTime,
                    EndTime = model.EndTime,
                    MaxPatientsPerDay = model.MaxPatientsPerDay,
                    ShiftDate=(DateTime)assignment.StartDate,
                    IsDeleted = false,

                };

                shiftRepository.Add(shift);
                commitData.SaveChanges();
            //}
            return "Shift created successfully!";
        }
        public PaginationViewModel<ProviderViewModel> Search(string searchText = "", int pageNumber = 1,
                                                            int pageSize = 2)
        {
            return providerRepository.Search(searchText, pageNumber, pageSize);
        }

    }
}
