using Data;
using Dorak.Models;
using Dorak.ViewModels;
using Dorak.ViewModels.AccountViewModels;
using Dorak.ViewModels.CenterViewModel;
using LinqKit;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
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



        public PaginationViewModel<ProviderViewModel> GetProvidersOfCenter(int CenterId)
        {
            //var center = GetById(CenterId);
            //var result = center.ProviderAssignments.Where(c => c.CenterId == CenterId&&c.IsDeleted==false).Select(res => res.Provider.toModelView()).ToList();
            var result = ProviderSearch(CenterId);
            return result;
        }


        //error
        public async Task<IdentityResult> AddProviderAsync(RegisterationViewModel user)
        {
            var userRes = await accountServices.CreateAccount(user);


            commitData.SaveChanges();

            return userRes;
        }

        //delete provider from center
        public string DeleteProviderfromCenter(String ProviderId)
        {
            var assignment = providerAssignmentRepository.GetAssignmentByProviderId(ProviderId);
            assignment.IsDeleted = true;
            providerAssignmentRepository.Edit(assignment);
            commitData.SaveChanges();

            return "Provider Deleted Succesfully!";
        }

       
        public PaginationViewModel<ProviderViewModel> ProviderSearch( int centerId, string searchText = "", int pageNumber = 1,
        int pageSize = 2)
        {
            var builder = PredicateBuilder.New<Provider>();
            var old = builder;
            if (!searchText.IsNullOrEmpty())
            {
                builder = builder.And(i => (i.FirstName.ToLower().Contains(searchText.ToLower()) || i.LastName.ToLower().Contains(searchText.ToLower()) || i.City.ToLower().Contains(searchText.ToLower()) && i.IsDeleted == false));
                builder= builder.And(i=>i.ProviderAssignments.Select(p=>p.CenterId==centerId).FirstOrDefault());
            }

            if (old == builder)
            {
                builder = null;
            }

            var count = providerRepository.GetList(builder).Count();
            var resultAfterPagination = providerRepository.Get(filter: builder, pageSize: pageSize, pageNumber: pageNumber).Select(p => p.toModelView()).ToList();
            return new PaginationViewModel<ProviderViewModel>
            {
                Data = resultAfterPagination,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Total = count
            };
        }



    }
}
