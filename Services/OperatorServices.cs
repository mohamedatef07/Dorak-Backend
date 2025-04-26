using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Dorak.Models;
using Dorak.Models;
using Dorak.Models.Models.Wallet;
using Dorak.ViewModels;
using Microsoft.AspNetCore.Identity;
using Models.Enums;
using Repositories;

namespace Services
{
    public class OperatorServices
    {
        public OperatorRepository operatorRepository;
        public ClientRepository clientRepository;
        private readonly AppointmentRepository appointmentRepository;
        private readonly ShiftRepository shiftRepository;
        public CommitData commitData;
        public OperatorServices(OperatorRepository _operatorRepository, CommitData _commitData, AppointmentRepository _appointmentRepository, ClientRepository _clientRepository, ShiftRepository _shiftRepository)
        {
            shiftRepository = _shiftRepository;
            operatorRepository = _operatorRepository;
            clientRepository = _clientRepository;
            appointmentRepository = _appointmentRepository;
            commitData = _commitData;
        }
        public async Task<IdentityResult> CreateOperator(string userId, OperatorViewModel model)
        {
            var _operator = new Operator
            {
                OperatorId = userId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Gender = model.Gender,
                Image = model.Image
            };

            operatorRepository.Add(_operator);
            commitData.SaveChanges();
            return IdentityResult.Success;
        }
        public bool SoftDelete(string operatorId)
        {
            var SelectedOperator = operatorRepository.GetById(o => o.OperatorId == operatorId);

            if (SelectedOperator != null)
            {
                SelectedOperator.IsDeleted = true;
                operatorRepository.Edit(SelectedOperator);
                commitData.SaveChanges();
                return true;
            }
            return false;
        }
        public bool RestoreOperator(string operatorId)
        {
            var SelectedOperator = operatorRepository.GetById(o => o.OperatorId == operatorId);

            if (SelectedOperator != null && SelectedOperator.IsDeleted == true)
            {
                SelectedOperator.IsDeleted = false;
                operatorRepository.Edit(SelectedOperator);
                commitData.SaveChanges();
                return true;
            }
            return false;
        }
        public bool IsExist(string operatorId)
        {
            var SelectedOperator = operatorRepository.GetById(o => o.OperatorId == operatorId);

            if (SelectedOperator == null)
                return false;

            return true;
        }
        public IQueryable<Operator> GetAllOperators()
        {
            var operators = operatorRepository.GetAll().Where(t => t.IsDeleted != true);
            if (operators != null)
            {
                return operators;
            }
            return null;
        }
        public Appointment CreateAppointment(AppointmentViewModel model)
        {
            var appointment = new Appointment
            {
                AppointmentDate = model.AppointmentDate,
                AppointmentStatus = model.AppointmentStatus,
                ClientType = model.ClientType,
                Fees = model.Fees,
                AdditionalFees = model.AdditionalFees,
                EstimatedTime = model.EstimatedTime,
                ExactTime = model.ExactTime,
                EndTime = model.EndTime,
                OperatorId = model.OperatorId,

                ShiftId = model.ShiftId,
                UserId = model.UserId,
                TemporaryClientId = model.TemporaryClientId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            return appointmentRepository.CreateAppoinment(appointment);
        }     //NOT DONE>>>>>>>>
        public bool StartShift(int ShiftId, string operatorId)
        {
            DateTime currentTime = DateTime.Now;
            TimeOnly timeNow = TimeOnly.FromDateTime(currentTime);

            Shift shift = shiftRepository.GetById(s => s.ShiftId == ShiftId);

            if (shift != null)
            {
                shift.OperatorId = operatorId;
                shift.ExactStartTime = timeNow;
                shift.ShiftType = ShiftType.OnGoing;
                shiftRepository.Edit(shift);
                commitData.SaveChanges();
            }
            else
            {
                return false;
            }
            IQueryable<Appointment> appointments = appointmentRepository.GetAllAppointmentForShift(ShiftId);


            return true;
        }
    }
}
