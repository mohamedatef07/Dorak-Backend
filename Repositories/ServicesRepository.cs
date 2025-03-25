using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Dorak.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class ServicesRepository : BaseRepository<Service>
    {
        public ServicesRepository(DorakContext dbContext) : base(dbContext)
        {
        }
        public List<Service> GetServicesByCenterId(int centerId)
        {
            var serviceIds = DbContext.CenterServices
                             .Where(cs => cs.CenterId == centerId)
                             .Select(cs => cs.ServiceId)
            .ToList();

            return DbContext.Services
                           .Where(s => serviceIds.Contains(s.ServiceId))
                           .ToList();
        }

        public List<Service> SearchServices(string keyword)
        {
            return DbContext.Services
                .Where(s => s.ServiceName.Contains(keyword) || s.Description.Contains(keyword))
                .ToList();
        }

        public List<Service> FilterServicesByPrice(decimal minPrice, decimal maxPrice)
        {
            return DbContext.Services
                .Where(s => s.BasePrice >= minPrice && s.BasePrice <= maxPrice)
                .ToList();
        }

        public bool IsServiceAvailableInCenter(int serviceId, int centerId)
        {
            return DbContext.CenterServices.Any(cs => cs.ServiceId == serviceId && cs.CenterId == centerId);
        }


    }
}

