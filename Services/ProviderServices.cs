using Data;
using Dorak.Models;
using Dorak.ViewModels;
using Repositories;

namespace Services
{
    public class ProviderServices
    {
        ProviderRepository providerRepository;
        ProviderAssignmentRepository providerAssignmentRepository;
        ProviderScheduleRepository providerScheduleRepository;
        

        public ProviderServices(ProviderRepository _providerRepository,
            ProviderAssignmentRepository _providerAssignmentRepository,
            ProviderScheduleRepository _providerScheduleRepository
            )
        {
            providerRepository = _providerRepository;
            providerAssignmentRepository = _providerAssignmentRepository;
            providerScheduleRepository = _providerScheduleRepository;
            
        }

        // ----- Assign provider to center -----

        public Provider GetProviderById(string providerId)
        {
            return providerRepository.GetById(p => p.ProviderId == providerId);
        }
        public void AssignProviderToCenter(ProviderAssignmentViewModel model)
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
            CommitData.SaveChanges();
        }

        // ----- Manage provider schedule -----
        public string ManageProviderSchedule(ProviderScheduleViewModel model)
        {
            if (model.StartDate > model.EndDate)
            {
                return "start date cannot be after end date.";
            }

            if (model.StartTime >= model.EndTime)
            {
                return "start time must be before end time.";
            }

            // ***

            var existingSchedule = providerScheduleRepository
                .GetAll()
                .FirstOrDefault(s =>
                    s.ProviderId == model.ProviderId &&
                    s.CenterId == model.CenterId &&
                    ((model.StartDate <= s.EndDate && model.EndDate >= s.StartDate) &&
                    (model.StartTime < s.EndTime && model.EndTime > s.StartTime))
                );

            if (existingSchedule != null)
            {
                return "provider already has a schedule at this time.";
            }

            var providerSchedule = new ProviderSchedule
            {
                ProviderId = model.ProviderId,
                CenterId = model.CenterId,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                MaxPatientsPerDay = model.MaxPatientsPerDay
            };

            providerScheduleRepository.Add(providerSchedule);
            CommitData.SaveChanges();

            return "schedule has been created successfully!";
        }
    }
}
