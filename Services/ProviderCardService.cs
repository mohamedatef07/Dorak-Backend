using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dorak.ViewModels.DoctorCardVMs;
using Data;
using Repositories;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    public class ProviderCardService
    {
        private readonly DorakContext context;

        public ProviderCardService(DorakContext _context)
        {
            context = _context;
        }

        public List<ProviderCardViewModel> GetDoctorCards()
        {
            var providers = context.Providers
            .Include(p => p.ProviderCenterServices)
             .Where(p => !p.IsDeleted)
             .Select(p => p.ToCardView())
             .ToList();

            return providers;
        }
        public List<ProviderCardViewModel> SearchDoctors(string? searchText, string? city, string? specialization)
        {
            var query = context.Providers
                .Include(p => p.ProviderCenterServices)
                .Where(p => !p.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                searchText = searchText.ToLower();
                query = query.Where(p => (p.FirstName + " " + p.LastName).ToLower().Contains(searchText));
            }

            if (!string.IsNullOrWhiteSpace(city))
            {
                city = city.ToLower();
                query = query.Where(p => p.City.ToLower() == city);
            }

            if (!string.IsNullOrWhiteSpace(specialization))
            {
                specialization = specialization.ToLower();
                query = query.Where(p => p.Specialization.ToLower() == specialization);
            }

            return query.Select(p => p.ToCardView()).ToList();
        }

        public List<ProviderCardViewModel> FilterByDay(DateOnly date)
        {
            DateTime dateTime = date.ToDateTime(TimeOnly.MinValue);

            var assignments = context.ProviderAssignments
                .Include(a => a.Provider)
                    .ThenInclude(p => p.ProviderCenterServices)
                .Where(a => !a.IsDeleted
                            && a.StartDate <= dateTime
                            && a.EndDate >= dateTime)
                .ToList();

            var uniqueProviders = assignments
                .Select(a => a.Provider)
                .Distinct()
                .ToList();

            return uniqueProviders
                .Select(p => p.ToCardView())
                .ToList();
        }






    }
}
