using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Dorak.Models;
using Dorak.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Identity;
using Repositories;

namespace Services
{
    public class ClientServices
    {
        public ClientRepository clientRepository;
        public CommitData commitData;
        public ClientServices(ClientRepository _clientRepository,CommitData _commitData) 
        { 
            clientRepository = _clientRepository;
            commitData = _commitData;
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
            commitData.SaveChanges();
            return IdentityResult.Success;
        }
    }
}
