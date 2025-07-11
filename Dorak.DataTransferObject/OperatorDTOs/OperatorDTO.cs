using Models.Enums;

namespace Dorak.DataTransferObject
{
    public class OperatorDTO
    {
        public string OperatorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public GenderType Gender { get; set; }
        public string? Image { get; set; }
        public int? CenterId { get; set; }
    }
}
