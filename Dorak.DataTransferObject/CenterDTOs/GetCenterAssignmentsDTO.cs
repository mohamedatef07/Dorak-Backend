using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorak.DataTransferObject.CenterDTOs
{
    public class GetCenterAssignmentsDTO
    {
        public int AssignmentId { get; set; }
        public int CenterId { get; set; }
        public DateOnly? StartDate { get; set; }  
        public DateOnly? EndDate { get; set; }
        public bool IsDeleted { get; set; }

    }
}
