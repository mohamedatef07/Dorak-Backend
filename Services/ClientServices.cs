using Data;
using Dorak.DataTransferObject;
using Dorak.DataTransferObject.ClientDTO;
using Dorak.Models;
using Dorak.ViewModels;
using Dorak.ViewModels.AccountViewModels;
using Dorak.ViewModels.ServiceViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Models.Enums;
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
        private readonly UserManager<User> userManager;

        public ClientServices(ClientRepository _clientRepository, TemperoryClientRepository _temperoryClientRepository, AppointmentRepository _appointmentRepository, CommitData _commitData, AppointmentServices appointmentServices, WalletRepository _walletRepository, IWebHostEnvironment env, AccountRepository _accountRepository, UserManager<User> _userManager)
        {
            clientRepository = _clientRepository;
            temperoryClientRepository = _temperoryClientRepository;
            appointmentRepository = _appointmentRepository;
            commitData = _commitData;
            this.appointmentServices = appointmentServices;
            walletRepository = _walletRepository;
            _env = env;
            accountRepository = _accountRepository;
            userManager = _userManager;


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

        //public async Task<IdentityResult> ClientRegister(NewClientViewModel model)
        //{

        //    var client = new Client
        //    {

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

        //    clientRepository.Add(client);
        //    commitData.SaveChanges();
        //    return IdentityResult.Success;
        //}

        public async Task<IdentityResult> ClientRegister(NewClientViewModel model)
        {
            // Create Identity User
            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };

            // Create user with password
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return result;
            }

            // Assign role to user
            var roleResult = await userManager.AddToRoleAsync(user, model.Role);
            if (!roleResult.Succeeded)
            {
                // Optionally, delete the user if role assignment fails
                await userManager.DeleteAsync(user);
                return roleResult;
            }

            // Create Client entity
            var client = new Client
            {
                ClientId = user.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Gender = model.Gender,
                BirthDate = model.BirthDate,
                Street = model.Street,
                City = model.City,
                Governorate = model.Governorate,
                Country = model.Country,
                Image = model.Image,
                User = user
            };


            clientRepository.Add(client);
            commitData.SaveChanges();

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
                City = Client.City,
                Country = Client.Country,
                Governorate = Client.Governorate,
                Street = Client.Street,
            };
            return clientProfile;
        }

        public ClientWalletAndProfileDTO GetClientWalletAndProfile(string userId)
        {
            var walletEntity = walletRepository.GetWalletByUserId(userId);

            var Client = clientRepository.GetById(c => c.ClientId == userId);
            var wallet = new ClientWalletAndProfileDTO()
            {
                ID = Client.ClientId,
                Image = Client.Image,
                Name = $"{Client.FirstName} {Client.LastName}",
                Phone = Client.User.PhoneNumber,
                Email = Client.User.Email,
                Balance = walletEntity?.Balance ?? 0
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

            if (model.City != null)
                client.City = model.City;

            if (model.Country != null)
                client.Country = model.Country;

            if (model.Street != null)
                client.Street = model.Street;

            if (model.Governorate != null)
                client.Governorate = model.Governorate;


            if (model.Image != null)
            {
                var savedImagePath = await SaveImageAsync(model.Image, client.User.Id.ToString());  // Use client.ClientId here
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
                ID = client.ClientId,
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
                var folderPath = Path.Combine(_env.WebRootPath, "image", "Client", ClientId);
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
        public GetClientAppointmentStatisticsDTO GetGeneralAppoinmentStatistics(string userId)
        {
            var client = clientRepository.GetById(cl => cl.ClientId == userId && !cl.IsDeleted);
            var appoinmentStatistics = new GetClientAppointmentStatisticsDTO
            {
                TotalAppointments = client.User.Appointments.Count(),
                CompletedAppointments = client.User.Appointments.Where(app => app.AppointmentStatus == AppointmentStatus.Completed).Count(),
                PendingAppointments = client.User.Appointments.Where(app => app.AppointmentStatus == AppointmentStatus.Pending).Count(),
            };
            return appoinmentStatistics;
        }
        public Client GetClientById(string clientId)
        {
            return clientRepository.GetById(c => c.ClientId == clientId && c.IsDeleted == false);
        }
        public PaginationViewModel<ClientViewModel> Search(string searchText = "", int pageNumber = 1,
                                                    int pageSize = 10)
        {
            return clientRepository.Search(searchText, pageNumber, pageSize);
        }
        public bool EditClient(Client provider)
        {
            if (provider == null)
            {
                return false;
            }
            clientRepository.Edit(provider);
            commitData.SaveChanges();
            return true;
        }
        public bool DeleteClient(Client provider)
        {
            if (provider == null)
            {
                return false;
            }
            provider.IsDeleted = true;
            clientRepository.Edit(provider);
            commitData.SaveChanges();
            return true;
        }

    }
}
