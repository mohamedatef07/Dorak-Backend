using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Dorak.Models;
using Dorak.ViewModels;
using Dorak.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Identity;
using Repositories;

namespace Services
{
    public class OperatorServices
    {
        public OperatorRepository operatorRepository;
        public ClientRepository clientRepository;
        private readonly AppointmentRepository appointmentRepository;
        public CommitData commitData;
        public OperatorServices(OperatorRepository _operatorRepository, CommitData _commitData, AppointmentRepository _appointmentRepository, ClientRepository _clientRepository)
        {
            operatorRepository = _operatorRepository;
            clientRepository = _clientRepository;
            appointmentRepository = _appointmentRepository;
            this.commitData = _commitData;
        }

        public async Task<IdentityResult> CreateOperator(string userId, OperatorViewModel model)
        {
            var _operator = new Operator
            {
                OperatorId = userId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                //Gender = model.Gender,
                Image = model.Image
            };

            operatorRepository.Add(_operator);
            commitData.SaveChanges();
            return IdentityResult.Success;
        }

        public Appointment CreateAppointment(AppointmentViewModel model)
        {
            var appointment = new Appointment
            {
                AppointmentDate = model.AppointmentDate,
                AppointmentStatus = model.AppointmentStatus,
                Type = model.Type,
                Fees = model.Fees,
                AdditionalFees = model.AdditionalFees,
                EstimatedTime = model.EstimatedTime,
                ExactTime = model.ExactTime,
                EndTime = model.EndTime,
                OperatorId = model.OperatorId,
                LiveQueueId = model.LiveQueueId,
                ShiftId = model.ShiftId,
                UserId = model.UserId,
                TemporaryClientId = model.TemporaryClientId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            return appointmentRepository.CreateAppoinment(appointment);
        }
    }
}
