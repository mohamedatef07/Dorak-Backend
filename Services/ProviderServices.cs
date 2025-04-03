using Data;
using Dorak.Models;
using Dorak.ViewModels;
using Models.Enums;
using Repositories;
using System;
using System.Threading.Tasks;

namespace Services
{
    public class ProviderServices
    {
        ProviderRepository providerRepository;
        ProviderAssignmentRepository providerAssignmentRepository;

        
        public ProviderServices(ProviderRepository _providerRepository, ProviderAssignmentRepository _providerAssignmentRepository)
        {
            providerRepository = _providerRepository;
            providerAssignmentRepository = _providerAssignmentRepository;
        }

        // ----- assign provider to center -----

        // get provider by id to assign to center later
        public Provider GetProviderById(string providerId)
        {
            return providerRepository.GetById(p => p.ProviderId == providerId);
        }

        public async Task AssignProviderToCenterAsync(string providerId, int centerId, DateTime startDate, DateTime endDate, ProviderType assignmentType)
        {
            var assignment = new ProviderAssignment
            {
                ProviderId = providerId,
                CenterId = centerId,
                StartDate = startDate,
                EndDate = endDate,
                AssignmentType = assignmentType
            };

            providerAssignmentRepository.Add(assignment);

             CommitData.SaveChanges();
        }
    }
}
