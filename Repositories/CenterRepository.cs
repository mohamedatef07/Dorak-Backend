using Data;
using Dorak.DataTransferObject;
using Dorak.Models;
using Dorak.ViewModels;
using LinqKit;
using Microsoft.IdentityModel.Tokens;

namespace Repositories
{
    public class CenterRepository : BaseRepository<Center>
    {
        public CommitData commitData;
        public CenterRepository(DorakContext context, CommitData _commitData) : base(context)
        {
            commitData = _commitData;
        }

        public async Task<Center> CreateCenter(CenterDTO_ center)
        {
            if (center is null)
                return null;

            var newCenter = new Center
            {
                CenterName = center.CenterName,
                ContactNumber = center.ContactNumber,
                Street = center.Street,
                City = center.City,
                Governorate = center.Governorate,
                Country = center.Country,
                Email = center.Email,
                WebsiteURL = center.WebsiteURL,
                Latitude = center.Latitude,
                Longitude = center.Longitude,
                MapURL = center.MapURL,
                CenterStatus = center.CenterStatus,
                IsDeleted = center.IsDeleted
            };

            Add(newCenter);
            commitData.SaveChanges();
            return newCenter;
        }

        public PaginationViewModel<CenterViewModel> Search(string searchText = "", int pageNumber = 1,
        int pageSize = 5)
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
