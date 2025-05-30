using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dorak.Models;

namespace Dorak.DataTransferObject.OperatorDTO
{
    public static class OperatorExtensions
    {
        public static OperatorDTO OperatorToOperatorDTO(this Operator model)
        {
            return new OperatorDTO
            {
                OperatorId = model.OperatorId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Gender = model.Gender,
                Image = model.Image,
                CenterId = model.CenterId
            };
        }
        public static List<OperatorDTO> OperatorToOperatorDTO(this List<Operator> operators)
        {
            return operators.Select(op => op.OperatorToOperatorDTO()).ToList();
        }
    }
}
