using Data;
using Dorak.Models;
using Dorak.ViewModels;
using Dorak.ViewModels.Service;
using Dorak.ViewModels.ServiceViewModel;
using LinqKit;
using Microsoft.IdentityModel.Tokens;

namespace Repositories
{
    public class ServicesRepository : BaseRepository<Service>
    {
        public ServicesRepository(DorakContext context) : base(context)
        {

        }
        public PaginationViewModel<ServiceViewModel> Search(string searchText = "", int pageNumber = 1,
        int pageSize = 2)
        {
            var builder = PredicateBuilder.New<Service>();
            var old = builder;
            if (!searchText.IsNullOrEmpty())
            {
                builder = builder.And(S => S.ServiceName.ToLower().Contains(searchText.ToLower()));
            }
            if (old == builder)
            {
                builder = null;
            }
            var count = base.GetList(builder).Count();
            var resultAfterPagination = base.Get(filter: builder, pageSize: pageSize, pageNumber: pageNumber).Select(s => s.ToViewModel()).ToList();
            return new PaginationViewModel<ServiceViewModel>
            {
                Data = resultAfterPagination,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Total = count
            };
        }
    }
}
