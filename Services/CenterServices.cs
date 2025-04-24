using Data;
using Dorak.Models;
using Dorak.ViewModels;
using Dorak.ViewModels.AccountViewModels;
using Dorak.ViewModels.CenterViewModel;
using LinqKit;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Models.Enums;
using Repositories;
using System.Data.Entity.Core.Common;
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
        private readonly ProviderServices providerServices;

        public CenterServices(CenterRepository _centerRepository,
            CommitData _commitData,
            ProviderAssignmentRepository _providerAssignmentRepository,
            ProviderRepository _providerRepository,
            AccountServices _accountServices, ProviderServices _providerServices)
        {
            centerRepository = _centerRepository;
            commitData = _commitData;
            providerAssignmentRepository = _providerAssignmentRepository;
            providerRepository = _providerRepository;
            accountServices = _accountServices;
            providerServices = _providerServices;
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
        public async Task<IdentityResult> AddProviderAsync(RegisterationViewModel user, int centerId, DateOnly _startdate, DateOnly _enddate, AssignmentType _assignmentType)
        {
            try
            {
                var userRes = await accountServices.CreateAccount(user);

                if (userRes.Succeeded)
                {
                    var providerId = await accountServices.getIdByUserName(user.UserName);

                    if (!string.IsNullOrEmpty(providerId))
                    {
                        var providerAssignmentViewModel = new ProviderAssignmentViewModel
                        {
                            AssignmentType = _assignmentType,
                            CenterId = centerId,
                            EndDate = _enddate,
                            StartDate = _startdate,
                            ProviderId = providerId
                        };

                        providerServices.AssignProviderToCenter(providerAssignmentViewModel);
                        commitData.SaveChanges();
                    }
                }

                return userRes;
            }
            catch (Exception ex)
            {
                // log exception or handle error
                return IdentityResult.Failed(new IdentityError { Description = $"Error: {ex.Message}" });
            }
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


        public PaginationViewModel<ProviderViewModel> ProviderSearch(int centerId, string searchText = "", int pageNumber = 1,
        int pageSize = 2)
        {
            var builder = PredicateBuilder.New<Provider>(true);
            builder = builder.And(i => i.ProviderAssignments.Any(p => p.CenterId == centerId && p.IsDeleted == false));
            if (!searchText.IsNullOrEmpty())
            {
                builder = builder.And(i => (i.FirstName.ToLower().Contains(searchText.ToLower()) ||
                i.LastName.ToLower().Contains(searchText.ToLower()) ||
                i.City.ToLower().Contains(searchText.ToLower())
                && i.IsDeleted == false));
            }
            else
            {
                builder = builder.And(i => i.IsDeleted == false);
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
