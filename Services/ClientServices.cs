using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dorak.Models;
using Dorak.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Identity;
using Repositories;

namespace Services
{
    public class ClientServices
    {
        private ClientRepository clientRepository;

        public ClientServices(ClientRepository _clientRepository) 
        { 
            clientRepository = _clientRepository;
        }

        public async Task<IdentityResult> CreateClient(string userId, ClientRegisterViewModel model)
        {
            var client = new Client
            {
                ClientId = userId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Gender = model.Gender,
                BirthDate = model.BirthDate,
                Street = model.Street,
                City = model.City,
                Governorate = model.Governorate,
                Country = model.Country,
                Image = model.Image
            };

            clientRepository.Add(client);
            clientRepository.SaveChanges();
            return IdentityResult.Success;
        }
    }
}
