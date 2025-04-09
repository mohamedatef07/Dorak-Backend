using Data;
using Dorak.Models;
using Dorak.Models.Models.Wallet;
using Dorak.ViewModels;
using Repositories;
using System.Linq;

namespace Services
{
    public class ProviderServices
    {
        ProviderRepository providerRepository;
        ProviderAssignmentRepository providerAssignmentRepository;
        ProviderCenterServiceRepository providerCenterServiceRepository;
        ShiftRepository shiftRepository;
        CommitData commitData;

        public ProviderServices(
            ProviderRepository _providerRepository,
            ProviderAssignmentRepository _providerAssignmentRepository,
            ProviderCenterServiceRepository _providerCenterServiceRepository,
            ShiftRepository _shiftRepository,
            CommitData _commitData)
        {
            providerRepository = _providerRepository;
            providerAssignmentRepository = _providerAssignmentRepository;
            providerCenterServiceRepository = _providerCenterServiceRepository;
            shiftRepository = _shiftRepository;
            commitData = _commitData;
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

            return "Provider assigned successfully!";
        }

        // Assign service to center (replacing ProviderService with direct ServiceId)
        public string AssignServiceToCenter(AssignProviderCenterServiceViewModel model)
        {
            var isAssigned = providerAssignmentRepository.GetAll()
                .Any(a => a.ProviderId == model.ProviderId && a.CenterId == model.CenterId);

            if (!isAssigned)
                return "Provider is not assigned to the selected center.";

            // Create the ProviderCenterService directly
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
                IsDeleted = false
            };

            shiftRepository.Add(shift);
            commitData.SaveChanges();

            return "Shift created successfully!";
        }
    }
}
