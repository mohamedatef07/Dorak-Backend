using Dorak.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Dorak.ViewModels.ServiceViewModel
{
    public static class ServiceExtensions
    {
        public static Service ToModel(this ServiceViewModel viewModel)
        {
            return new Service
            {
                ServiceId = viewModel.ServiceId,
                ServiceName = viewModel.ServiceName,
                //Priority = viewModel.Priority,
                BasePrice = viewModel.BasePrice,


            };
        }
        public static ServiceViewModel ToViewModel(this Service model)
        {
            return new ServiceViewModel
            {
                ServiceId = model.ServiceId,
                ServiceName = model.ServiceName,
                Description = model.Description, 
                //Priority = model.,
                BasePrice = model.BasePrice
            };
        }

    }
}



