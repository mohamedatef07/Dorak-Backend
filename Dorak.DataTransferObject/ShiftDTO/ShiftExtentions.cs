using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dorak.Models;

namespace Dorak.ViewModels
{
    public static class ShiftExtentions
    {
        public static ShiftDTO ShiftToShiftVM(this Shift _shift)
        {
            return new ShiftDTO
            {
                ShiftId = _shift.ShiftId,
                ShiftType = _shift.ShiftType,
                ShiftDate = _shift.ShiftDate,
                StartTime = _shift.StartTime,
                EndTime = _shift.EndTime,
                MaxPatientsPerDay = _shift.MaxPatientsPerDay,
                CenterId = _shift.ProviderAssignment.CenterId,
                ProviderId = _shift.ProviderAssignment.ProviderId,
                //OperatorId = _shift.OperatorId
            };
        }
    }
}
