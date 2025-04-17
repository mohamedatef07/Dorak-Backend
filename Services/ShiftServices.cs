using Dorak.Models;
using Dorak.Models.Models.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ShiftServices
    {
        public ShiftRepository shiftRepository;
        public ShiftServices(ShiftRepository _shiftRepository)
        {
            shiftRepository = _shiftRepository;
        }
        public Shift GetShiftById(int shiftId)
        {
            return shiftRepository.GetById(shift => shift.ShiftId == shiftId);
        }
    }
}
