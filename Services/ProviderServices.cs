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

            foreach (ShiftViewModel shift in model.Shifts )
            {

                CreateShift(shift,assignment);

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
        

    }
}
