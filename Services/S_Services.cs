using Data;
using Dorak.DataTransferObject;
using Dorak.Models;
using Dorak.Models.Enums;
using Dorak.ViewModels;
using Dorak.ViewModels.ServiceViewModel;
using Repositories;

namespace Services
{
    public class S_Services
    {
        public ServicesRepository servicesRepository;
        public CommitData commitData;
        private readonly ProviderCenterServiceRepository providerCenterServiceRepository;
        private readonly ProviderRepository providerRepository;
        private readonly CenterRepository centerRepository;

        public S_Services(ServicesRepository _servicesRepository,
                          CommitData _commitData,
                          ProviderCenterServiceRepository providerCenterServiceRepository,
                          ProviderRepository providerRepository,
                          CenterRepository centerRepository)
        {
            servicesRepository = _servicesRepository;
            commitData = _commitData;
            this.providerCenterServiceRepository = providerCenterServiceRepository;
            this.providerRepository = providerRepository;
            this.centerRepository = centerRepository;
        }
        public List<Service> GetAll()
        {
            return servicesRepository.GetAll().Where(s => !s.IsDeleted).ToList();
        }
        public Service GetById(int id)
        {
            return servicesRepository.GetById(S => S.ServiceId == id);
        }
        public void Edit(Service entity)
        {
            servicesRepository.Edit(entity);
            commitData.SaveChanges();
        }
        public bool Delete(int id)
        {
            try
            {
                var service = servicesRepository.GetById(S => S.ServiceId == id);
                if (service != null)
                {
                    servicesRepository.Delete(service);
                    commitData.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void Add(Service entity)
        {
            servicesRepository.Add(entity);
            commitData.SaveChanges();
        }

        public PaginationViewModel<ServiceViewModel> Search(string searchText = "", int pageNumber = 1,
                                                            int pageSize = 2)
        {
            return servicesRepository.Search(searchText, pageNumber, pageSize);
        }


        public AssignServiceResult AssignServiceToProviderCenterService(AddProviderCenterServiceDTO model)
        {
            // 1. Input Validation
            if (string.IsNullOrEmpty(model.ProviderId) ||
                model.ServiceId <= 0 ||
                model.CenterId <= 0 ||
                model.Price <= 0)
            {
                return AssignServiceResult.InvalidInput; // Specific error for invalid input
            }

            // 2. Check if the provider, service, or center exists in the database
            var providerExists = providerRepository.GetById(p => p.ProviderId == model.ProviderId);
            var serviceExists = servicesRepository.GetById(s => s.ServiceId == model.ServiceId);
            var centerExists = centerRepository.GetById(c => c.CenterId == model.CenterId);

            if (providerExists == null)
            {
                return AssignServiceResult.ProviderNotFound; // Specific error for provider not found
            }
            if (serviceExists == null)
            {
                return AssignServiceResult.ServiceNotFound; // Specific error for service not found
            }
            if (centerExists == null)
            {
                return AssignServiceResult.CenterNotFound; // Specific error for center not found
            }

            // 3. Check for existing assignment to prevent duplication
            var existingAssignment = providerCenterServiceRepository.GetById(
                p => p.ProviderId == model.ProviderId &&
                p.ServiceId == model.ServiceId &&
                p.CenterId == model.CenterId
            );

            if (existingAssignment != null)
            {
                return AssignServiceResult.AssignmentAlreadyExists; // Specific error for duplicate
            }

            try
            {
                // 4. Create the new ProviderCenterService entity from the model
                var newService = new ProviderCenterService
                {
                    ProviderId = model.ProviderId,
                    ServiceId = model.ServiceId,
                    CenterId = model.CenterId,
                    Duration = providerExists.EstimatedDuration, // Assuming EstimatedDuration is on providerExists
                    Price = model.Price,
                    Priority = model.Priority,
                    IsDeleted = false
                };

                // 5. Add the new service to the database
                providerCenterServiceRepository.Add(newService);

                // 6. Save the changes
                commitData.SaveChanges();

                return AssignServiceResult.Success; // Success!
            }
            catch (Exception ex)
            {
                // Log the exception (e.g., using a logging framework)
                // logger.LogError(ex, "Error assigning service to provider center.");
                return AssignServiceResult.UnknownError; // Catch any unexpected errors during DB operations
            }
        }
        public List<ServicesDTO> GetAllServiceDropDown()
        {
            return servicesRepository.GetAll()
                                     .Where(s => !s.IsDeleted)
                                     .Select(s => new ServicesDTO
                                     {
                                         ServiceId = s.ServiceId,
                                         BasePrice = s.BasePrice,
                                         ServiceName = s.ServiceName
                                     }).ToList();
        }

    }
}

