using Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorak.DataTransferObject
{

    public class WorkingDates
    {
        public DateOnly startDate { get; set; }
        public DateOnly endDate { get; set; }

    }
    public class ProviderAssignmentDTO
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; } = "Password123@";
        public string ConfirmPassword { get; set; } = "Password123@";
        public string Role { get; set; } = "Provider";
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public GenderType Gender { get; set; }
        public DateOnly? BirthDate { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? Governorate { get; set; }
        public string? Country { get; set; }
        public string? Image { get; set; }
        public string? Specialization { get; set; }
        public string? LicenseNumber { get; set; }
        public AssignmentType AssignmentType { get; set; }
        public int CenterId { get; set; }
        public List<WorkingDates> WorkingDates { get; set; }


    }
}
