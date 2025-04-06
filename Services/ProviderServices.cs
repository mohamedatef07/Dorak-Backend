using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dorak.Models;
using Dorak.ViewModels;
using Dorak.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Identity;
using Repositories;

namespace Services
{
    public class ProviderServices
    {
        private ProviderRepository providerRepository;

        public ProviderServices(ProviderRepository _providerRepository)
        {
            providerRepository = _providerRepository;
        }

        //public IdentityResult CreateProvider(string userId, ProviderRegisterViewModel model)
        //{
        //    var provider = new Provider
        //    {
        //        ProviderId = userId,
        //        FirstName = model.FirstName,
        //        LastName = model.LastName,
        //        Gender = model.Gender,
        //        BirthDate = model.BirthDate,
        //        Street = model.Street,
        //        City = model.City,
        //        Governorate = model.Governorate,
        //        Country = model.Country,
        //        Image = model.Image
        //    };

        //    providerRepository.Add(provider);
        //    return 

        //}

        //public PaginationViewModel<ProviderViewModel> Search(string searchText = "", int pageNumber = 1,
        //                                                    int pageSize = 2)
        //{
        //    return providerRepository.Search(searchText, pageNumber, pageSize);
        //}
    }
}
