using Data;
using Dorak.Models;
using Dorak.ViewModels;
using Dorak.ViewModels.AccountViewModels;
using Dorak.ViewModels.CenterViewModel;
using Microsoft.AspNetCore.Identity;
using Repositories;
using System.Linq.Expressions;

namespace Services
{
    public class CenterServices
    {
        private readonly CenterRepository centerRepository;
        public CommitData commitData;
        private readonly ProviderAssignmentRepository providerAssignmentRepository;
        private readonly ProviderRepository providerRepository;
        private readonly AccountServices accountServices;

        public CenterServices(CenterRepository _centerRepository,
            CommitData _commitData,
            ProviderAssignmentRepository _providerAssignmentRepository,
            ProviderRepository _providerRepository,
            AccountServices _accountServices)
        {
            centerRepository = _centerRepository;
            commitData = _commitData;
            providerAssignmentRepository = _providerAssignmentRepository;
            providerRepository = _providerRepository;
            accountServices = _accountServices;
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
            commitData.SaveChanges();
        }
        public bool Delete(int id)
        {
            try
            {
                var center = centerRepository.GetById(c => c.CenterId == id);
                if (center != null)
                {
                    centerRepository.Delete(center);
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

        public bool Active(int id)
        {
            var center = centerRepository.GetById(c => c.CenterId == id);
            if (center != null)
            {
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
                                                            int pageSize = 2)
        {
            return centerRepository.Search(searchText, pageNumber, pageSize);
        }



        public List<ProviderViewModel> GetProvidersOfCenter(int CenterId)
        {
            var center = GetById(CenterId);
            var result = center.ProviderAssignments.Where(c => c.CenterId == CenterId).Select(res => res.Provider.toModelView()).ToList();
            return result;
        }

        public async Task<IdentityResult> AddProviderAsync(RegisterationViewModel user)
        {
            var userRes = await accountServices.CreateAccount(user);


            commitData.SaveChanges();

            return userRes;
        }
    }
}
