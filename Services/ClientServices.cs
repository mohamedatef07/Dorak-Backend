using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Dorak.DataTransferObject;
using Dorak.Models;
using Dorak.ViewModels;
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

        public ClientServices(ClientRepository _clientRepository, TemperoryClientRepository _temperoryClientRepository, AppointmentRepository _appointmentRepository, CommitData _commitData,AppointmentServices appointmentServices)
        {
            clientRepository = _clientRepository;
            temperoryClientRepository = _temperoryClientRepository;
            appointmentRepository = _appointmentRepository;
            commitData = _commitData;
            this.appointmentServices = appointmentServices;
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
                Appointments = appointmentServices.GetAppointmentsByUserId(userId)
            };
            return clientProfile;
        }

    }
}
