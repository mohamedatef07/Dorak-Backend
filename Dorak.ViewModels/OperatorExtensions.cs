using Dorak.Models;

namespace Dorak.ViewModels
{
    public static class OperatorExtensions
    {
        public static OperatorViewModel toModelView(this Operator _operator)
        {
            return new OperatorViewModel
            {
                FirstName = _operator.FirstName,
                LastName = _operator.LastName,
                CenterName = _operator.Center?.CenterName,
                Gender = _operator.Gender,
                OperatorId = _operator.OperatorId,
            };
        }
    }
}
