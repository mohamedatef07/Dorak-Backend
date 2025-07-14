using Data;
using Dorak.Models;
using Dorak.ViewModels;
using LinqKit;
using Microsoft.IdentityModel.Tokens;

namespace Repositories
{
    public class OperatorRepository : BaseRepository<Operator>
    {
        private readonly CommitData commitData;

        public OperatorRepository(DorakContext _dbContext, CommitData _commitData) : base(_dbContext)
        {
            commitData = _commitData;
        }
        public PaginationViewModel<OperatorViewModel> Search(string searchText = "", int pageNumber = 1, int pageSize = 5)
        {
            var builder = PredicateBuilder.New<Operator>();
            var old = builder;
            if (!searchText.IsNullOrEmpty())
            {
                builder = builder.And(i => (i.FirstName.ToLower().Contains(searchText.ToLower()) || i.LastName.ToLower().Contains(searchText.ToLower()) && i.IsDeleted == false));
            }

            if (old == builder)
            {
                builder = null;
            }

            var count = base.GetList(builder).Count();
            var resultAfterPagination = base.Get(filter: builder, pageSize: pageSize, pageNumber: pageNumber).Select(p => p.toModelView()).ToList();
            return new PaginationViewModel<OperatorViewModel>
            {
                Data = resultAfterPagination,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Total = count
            };
        }
    }
}
