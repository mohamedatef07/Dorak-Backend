using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dorak.ViewModels;
using Data;
using Repositories;
using Microsoft.EntityFrameworkCore;
using Dorak.DataTransferObject.ProviderDTO;

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

        //filterprovider
        public List<ProviderCardViewModel> FilterProviders(FilterProviderDTO filter)
        {
            var query = context.Providers
                .Where(p => !p.IsDeleted);

            // Title (enum)
            if (filter.Title.HasValue)
                query = query.Where(p => p.providerTitle == filter.Title.Value);

            // Gender (enum)
            if (filter.Gender.HasValue)
                query = query.Where(p => p.Gender == filter.Gender.Value);

            // City (string)
            if (!string.IsNullOrWhiteSpace(filter.City))
                query = query.Where(p => p.City.ToLower().Contains(filter.City.ToLower()));

            // Rate (decimal)
            if (filter.MinRate.HasValue)
                query = query.Where(p => p.Rate >= (decimal)filter.MinRate.Value);

            if (filter.MaxRate.HasValue)
                query = query.Where(p => p.Rate <= (decimal)filter.MaxRate.Value);

            // Price (decimal) - from ProviderCenterServices
            if (filter.MinPrice.HasValue || filter.MaxPrice.HasValue)
            {
                query = query.Where(p => p.ProviderCenterServices.Any(s =>
                    (!filter.MinPrice.HasValue || s.Price >= filter.MinPrice.Value) &&
                    (!filter.MaxPrice.HasValue || s.Price <= filter.MaxPrice.Value)
                ));
            }

            // Availability (DateOnly or DateTime range)
            if (filter.AvailableDate.HasValue)
            {
                var targetDate = filter.AvailableDate.Value;
                query = query.Where(p => p.ProviderAssignments.Any(a =>
                    a.StartDate <= targetDate && a.EndDate >= targetDate
                ));
            }

            return query
                .Select(p => p.ToCardView())
                .ToList();
        }






    }
}
