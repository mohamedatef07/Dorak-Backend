using Data;
using Dorak.Models;
using Dorak.ViewModels;
using LinqKit;
using Microsoft.IdentityModel.Tokens;

namespace Repositories
{
    public class ProviderRepository : BaseRepository<Provider>
    {
        public ProviderRepository(DorakContext context) : base(context)
        {
            
        }
        public PaginationViewModel<ProviderViewModel> Search(string searchText = "", int pageNumber = 1,
        int pageSize = 2)
        {
            var builder = PredicateBuilder.New<Provider>();
            var old = builder;
            if (!searchText.IsNullOrEmpty())
            {
                builder = builder.And(i => i.FirstName.ToLower().Contains(searchText.ToLower()) || i.LastName.ToLower().Contains(searchText.ToLower()) || i.City.ToLower().Contains(searchText.ToLower()));
            }

            if (old == builder)
            {
                builder = null;
            }

            var count = base.GetList(builder).Count();
            var resultAfterPagination = base.Get(filter: builder, pageSize: pageSize, pageNumber: pageNumber).Select(p => p.toModelView()).ToList();
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
