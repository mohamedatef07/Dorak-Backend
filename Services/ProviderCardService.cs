using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dorak.ViewModels;
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
                .Where(p => !p.IsDeleted)
                .Select(p => new ProviderCardViewModel
                {
                    FullName = p.FirstName + " " + p.LastName,
                    Specialization = p.Specialization,
                    City = p.City,
                    Rate = p.Rate,
                    EstimatedDuration = p.EstimatedDuration,
                    Price = p.ProviderCenterServices.Any()
                        ? p.ProviderCenterServices.Min(s => s.Price)
                        : 0
                })
                .ToList();

            return providers;
        }

        public List<ProviderCardViewModel> SearchDoctors(string? searchText, string? city, string? specialization)
        {
            var query = context.Providers
                .Where(p => !p.IsDeleted);

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(p => (p.FirstName + " " + p.LastName).ToLower().Contains(searchText.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(city))
            {
                query = query.Where(p => p.City.ToLower() == city.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(specialization))
            {
                query = query.Where(p => p.Specialization.ToLower() == specialization.ToLower());
            }

            return query.Select(p => new ProviderCardViewModel
            {
                FullName = p.FirstName + " " + p.LastName,
                Specialization = p.Specialization,
                City = p.City,
                Rate = p.Rate,
                EstimatedDuration = p.EstimatedDuration,
                Price = p.ProviderCenterServices.Any()
                    ? p.ProviderCenterServices.Min(s => s.Price)
                    : 0
            }).ToList();
        }


        public List<ProviderCardViewModel> FilterByDay(DateOnly date)
        {
            DateTime dateTime = date.ToDateTime(TimeOnly.MinValue);

            var providers = context.ProviderAssignments
                .Where(a => !a.IsDeleted
                            && a.StartDate <= DateOnly.FromDateTime(dateTime)
                            && a.EndDate >= DateOnly.FromDateTime(dateTime))
                .Select(a => a.Provider)
                .Distinct()
                .Select(p => new ProviderCardViewModel
                {
                    FullName = p.FirstName + " " + p.LastName,
                    Specialization = p.Specialization,
                    City = p.City,
                    Rate = p.Rate,
                    EstimatedDuration = p.EstimatedDuration,
                    Price = p.ProviderCenterServices.Any()
                        ? p.ProviderCenterServices.Min(s => s.Price)
                        : 0
                })
                .ToList();

            return providers;
        }







    }
}