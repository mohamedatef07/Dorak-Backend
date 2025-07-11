using Data;
using Dorak.DataTransferObject;
using Dorak.Models;
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


        public bool AssignServiceToProviderCenterService(AddProviderCenterServiceDTO model)
        {

            if (string.IsNullOrEmpty(model.ProviderId) ||
                                       model.ServiceId <= 0 ||
                                       model.CenterId <= 0 ||
                                       model.Price <= 0)
            {
                return false; // Any validation failure results in false
            }

            // Check if the provider, service, or center exists in the database
            var providerExists = providerRepository.GetById(p => p.ProviderId == model.ProviderId);
            var serviceExists = servicesRepository.GetById(s => s.ServiceId == model.ServiceId);
            var centerExists = centerRepository.GetById(c => c.CenterId == model.CenterId);

            if (providerExists == null || serviceExists == null || centerExists == null)
            {
                return false; // If any entity doesn't exist, return false
            }

            // Create the new ProviderCenterService entity from the model
            var newService = new ProviderCenterService
            {
                ProviderId = model.ProviderId,
                ServiceId = model.ServiceId,
                CenterId = model.CenterId,
                Duration = providerExists.EstimatedDuration,
                Price = model.Price,
                Priority = model.Priority,
                IsDeleted = false // Assuming the service is not deleted initially
            };

            // Add the new service to the database
            providerCenterServiceRepository.Add(newService);

            // Save the changes
            commitData.SaveChanges();

            return true;


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
        public List<ProviderCenterServiceDTO> GetProviderCenterServices(int centerId)
        {
            if (centerId <= 0)
            {
                return null;
            }

            var assignments = providerCenterServiceRepository
                .GetList(pcs => pcs.CenterId == centerId && !pcs.IsDeleted)
                .Select(pcs => new ProviderCenterServiceDTO
                {
                    Id = pcs.ProviderCenterServiceId,
                    ServiceName = pcs.Service.ServiceName,
                    ProviderName = $"{pcs.Provider.FirstName} {pcs.Provider.LastName}",
                    Price = pcs.Price,
                    Duration = pcs.Duration
                })
                .ToList();
            return assignments;
        }

        public bool UpdateProviderCenterServices(int PCSId, EditProviderCenterServiceDTO model)
        {
            if (model == null)
            {
                return false;
            }
            var providerCenterService = providerCenterServiceRepository.GetById(pcs => pcs.ProviderCenterServiceId == PCSId && !pcs.IsDeleted);
            if (providerCenterService == null)
            {
                return false; // Service not found
            }
            // Update the properties of the existing service
            providerCenterService.Price = model.Price;
            // Save the changes to the database
            providerCenterServiceRepository.Edit(providerCenterService);
            commitData.SaveChanges();
            return true; // Update successful


        }
    }
}

