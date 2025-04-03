using Data;
using Dorak.Models;
using Repositories;
using System.Linq.Expressions;

namespace Services
{
    public class CenterServices
    {
        public CenterRepository centerRepository;
        public CenterServices(CenterRepository _centerRepository)
        {
            centerRepository = _centerRepository;
        }
        public List<Center> GetAll()
        {
            return centerRepository.GetAll().ToList();
        }
        public Center GetById(int id)
        {
            return centerRepository.GetById(c => c.CenterId == id);
        }
        public void Edit(Center entity)
        {
            centerRepository.Edit(entity);
            CommitData.SaveChanges();
        }
        public void Delete(int id)
        {
            var center = centerRepository.GetById(c => c.CenterId == id);
            if (center != null)
            {
                centerRepository.Delete(center);
                CommitData.SaveChanges();
            }
        }
        public IQueryable<Center> FilterBy(Expression<Func<Center, bool>> filtereq, string Order_ColName, bool isAscending)
        {
            return centerRepository.FilterBy(filtereq, Order_ColName, isAscending);
        }
    }
}
