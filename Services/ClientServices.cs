using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Dorak.DataTransferObject;
using Dorak.DataTransferObject.ProviderDTO;
using Dorak.Models;
using Dorak.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Repositories;

namespace Services
{
    public class ClientServices
    {
        public ClientRepository clientRepository;
        public TemperoryClientRepository temperoryClientRepository;
        public AppointmentRepository appointmentRepository;
        public CommitData commitData;
        private readonly AppointmentServices appointmentServices;
        private readonly WalletRepository walletRepository;
        private readonly IWebHostEnvironment _env;
        private readonly AccountRepository accountRepository;

        public ClientServices(ClientRepository _clientRepository, TemperoryClientRepository _temperoryClientRepository, AppointmentRepository _appointmentRepository, CommitData _commitData,AppointmentServices appointmentServices,WalletRepository _walletRepository, IWebHostEnvironment env, AccountRepository _accountRepository)
        {
            clientRepository = _clientRepository;
            temperoryClientRepository = _temperoryClientRepository;
            appointmentRepository = _appointmentRepository;
            commitData = _commitData;
            this.appointmentServices = appointmentServices;
            walletRepository = _walletRepository;
            _env = env;
            accountRepository = _accountRepository;
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
            var phoneNumber = client?.User?.PhoneNumber;
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                var temp = temperoryClientRepository
                    .GetAll().FirstOrDefault(t => t.ContactInfo == phoneNumber);

                if (temp != null)
                {
                    var appointments = appointmentRepository.GetAppointmentsByTempClientId(temp.TempClientId);
                    foreach (var appointment in appointments)
                    {
                        appointment.UserId = userId;
                        appointmentRepository.Edit(appointment);
                        commitData.SaveChanges();
                    }

                }
            }
            return IdentityResult.Success;
        }

        public ClientProfileDTO GetProfile(string userId)
        {
            var Client = clientRepository.GetById(c => c.ClientId == userId);
            var clientProfile = new ClientProfileDTO()
            {
                ID = Client.ClientId,
                Image = Client.Image,
                Name = $"{Client.FirstName} {Client.LastName}",
                Phone = Client.User.PhoneNumber,
                Email = Client.User.Email,
                Appointments = appointmentServices.GetAppointmentsByUserId(userId)
            };
            return clientProfile;
        }

        public ClientWalletAndProfileDTO GetClientWalletAndProfile(string userId)
        {
            var Client = clientRepository.GetById(c => c.ClientId == userId);
            var wallet = new ClientWalletAndProfileDTO()
            {
                ID = Client.ClientId,
                Image = Client.Image,
                Name = $"{Client.FirstName} {Client.LastName}",
                Phone = Client.User.PhoneNumber,
                Email = Client.User.Email,
                Balance = walletRepository.GetWalletByUserId(userId).Balance
            };

            return wallet;

        }

        public ClientInfoToLiveQueueDTO GetClientInfoToLiveQueue(string userId)
        {
            var Client = clientRepository.GetById(c => c.ClientId == userId);
            var clientProfile = new ClientInfoToLiveQueueDTO()
            {
                Image = Client.Image,
                Name = $"{Client.FirstName} {Client.LastName}"
  
            };
            return clientProfile;
        }


        public async Task<bool> UpdateClientProfile(string userId, UpdateClientProfileDTO model)
        {
            var client = clientRepository.GetById(c => c.ClientId == userId && !c.IsDeleted);
            if (client == null)
                return false;

            var user = client.User;
            if (user == null)
                return false;

            if (!string.IsNullOrWhiteSpace(model.FirstName))
                client.FirstName = model.FirstName;

            if (!string.IsNullOrWhiteSpace(model.LastName))
                client.LastName = model.LastName;

            if (!string.IsNullOrWhiteSpace(model.Email))
                user.Email = model.Email;

            if (!string.IsNullOrWhiteSpace(model.Phone))
                user.PhoneNumber = model.Phone;

            if (model.BirthDate != null)
                client.BirthDate = (DateOnly)model.BirthDate;

            if (model.Image != null)
            {
                

                var savedImagePath = await SaveImageAsync(model.Image, client.ClientId);  // Use client.ClientId here
                client.Image = savedImagePath;  // Assuming the 'client' model has an Image property
            }

            clientRepository.Edit(client);
            accountRepository.Edit(user);

            commitData.SaveChanges();
            //await context.SaveChangesAsync();

            return true;
        }

        public ClientDetailsDTO GetCLientProfileForUpdate(string userId)
        {
            var client = clientRepository.GetById(p => p.ClientId == userId && !p.IsDeleted);

            if (client == null || client.User == null)
                return null;

            return new ClientDetailsDTO
            {
                FirstName = client.FirstName,
                LastName = client.LastName,

                Email = client.User.Email,
                Phone = client.User.PhoneNumber,
                BirthDate = client.BirthDate,
                Image = client.Image,
                City = client.City,
                Country = client.Country,
                Street = client.Street,
                Governorate = client.Governorate
            };
        }


        private async Task<string> SaveImageAsync(IFormFile userImage, string ClientId)
        {
            if (userImage != null && userImage.Length > 0)
            {
                // المسار يكون داخل wwwroot/image/Client/{ID}
                var folderPath = Path.Combine(_env.WebRootPath, "image", "provider", ClientId);
                Directory.CreateDirectory(folderPath);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(userImage.FileName);
                var fullPath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await userImage.CopyToAsync(stream);
                }

                // هنا return path يكون من بعد wwwroot
                string imagePath = $"/image/Client/{ClientId}/{fileName}";
                return imagePath;
            }

            return null;
        }

    }
}
