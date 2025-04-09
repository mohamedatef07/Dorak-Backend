using Data;
using Dorak.Models;
using Dorak.ViewModels;
using Dorak.ViewModels.ServiceViewModel;
using Repositories;
using System.Linq.Expressions;

namespace Services
{
    public class S_Services
    {
        public ServicesRepository servicesRepository;
        public CommitData commitData;
        public S_Services(ServicesRepository _servicesRepository, CommitData _commitData)
        {
            servicesRepository = _servicesRepository;
            commitData = _commitData;
        }
        public List<Service> GetAll()
        {
            return servicesRepository.GetAll().ToList();
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
    }
}
