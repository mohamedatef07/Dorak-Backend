using Data;
using Dorak.Models;
using Dorak.ViewModels;
using Repositories;
using System.Linq.Expressions;

namespace Services
{
    public class CenterServices
    {
        public CenterRepository centerRepository;
        public CommitData commitData;
        public CenterServices(CenterRepository _centerRepository, CommitData _commitData)
        {
            centerRepository = _centerRepository;
            commitData = _commitData;
        }
        public List<Center> GetAll()
        {
            return centerRepository.GetAll().ToList();
        }
        public Center GetById(int id)
        {
            return centerRepository.GetById(c => c.CenterId == id);
        }
        public bool Delete(int id)
        {
            var center = centerRepository.GetById(c => c.CenterId == id);
            if (center != null)
            {
                centerRepository.Delete(center);
                commitData.SaveChanges();
                return true;
            }
            else
            {
                return false;  
            }
        }
        public bool Active(int id)
        {
            var center = centerRepository.GetById(c => c.CenterId == id);
            if (center != null)
            {
                //center.CenterStatus = CenterStatus.Active;
                centerRepository.Edit(center);
                commitData.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool Inactive(int id)
        {
            var center = centerRepository.GetById(c => c.CenterId == id);
            if (center != null)
            {
                //center.CenterStatus = CenterStatus.Inactive;
                centerRepository.Edit(center);
                commitData.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
        public PaginationViewModel<CenterViewModel> Search(string searchText = "", int pageNumber = 1,
                                                            int pageSize = 5)
        {
            return centerRepository.Search(searchText, pageNumber, pageSize);
        }
    }
}
