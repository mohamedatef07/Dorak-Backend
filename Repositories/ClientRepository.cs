using Data;
using Dorak.Models;
using Dorak.ViewModels;
using LinqKit;
using Microsoft.IdentityModel.Tokens;

namespace Repositories
{
    public class ClientRepository : BaseRepository<Client>
    {
        public ClientRepository(DorakContext context) : base(context) { }
        public PaginationViewModel<ClientViewModel> Search(string searchText = "", int pageNumber = 1, int pageSize = 5)
        {
            var builder = PredicateBuilder.New<Client>();
            var old = builder;
            if (!searchText.IsNullOrEmpty())
            {
                builder = builder.And(i => (i.FirstName.ToLower().Contains(searchText.ToLower()) || i.LastName.ToLower().Contains(searchText.ToLower()) || i.City.ToLower().Contains(searchText.ToLower()) && i.IsDeleted == false));
            }

            if (old == builder)
            {
                builder = null;
            }

            var count = base.GetList(builder).Count();
            var resultAfterPagination = base.Get(filter: builder, pageSize: pageSize, pageNumber: pageNumber).Select(o => o.toModelView()).ToList();
            return new PaginationViewModel<ClientViewModel>
            {
                Data = resultAfterPagination,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Total = count
            };
        }
    }
}
