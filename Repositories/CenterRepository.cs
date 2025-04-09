using Data;
using Dorak.Models;
using Dorak.ViewModels;
using Dorak.ViewModels.CenterViewModel;
using LinqKit;
using Microsoft.IdentityModel.Tokens;

namespace Repositories
{
    public class CenterRepository : BaseRepository<Center>
    {
        public CenterRepository(DorakContext context):base(context)
        {
            
        }
        public PaginationViewModel<CenterViewModel> Search(string searchText = "", int pageNumber = 1,
        int pageSize = 2)
        {
            var builder = PredicateBuilder.New<Center>();
            var old = builder;
            if (!searchText.IsNullOrEmpty())
            {
                builder = builder.And(i => i.CenterName.ToLower().Contains(searchText.ToLower()) || i.City.ToLower().Contains(searchText.ToLower()));
            }
            if (old == builder)
            {
                builder = null;
            }
            var count = base.GetList(builder).Count();
            var resultAfterPagination = base.Get(filter: builder, pageSize: pageSize, pageNumber: pageNumber).Select(p => p.ToViewModel()).ToList();
            return new PaginationViewModel<CenterViewModel>
            {
                Data = resultAfterPagination,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Total = count
            };
        }
    }
}
