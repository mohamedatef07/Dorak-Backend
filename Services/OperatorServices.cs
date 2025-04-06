using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dorak.Models;
using Dorak.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Identity;
using Repositories;

namespace Services
{
    public class OperatorServices
    {
        private OperatorRepository operatorRepository;

        public OperatorServices(OperatorRepository _operatorRepository)
        {
            operatorRepository = _operatorRepository;
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
            operatorRepository.SaveChanges();
            return IdentityResult.Success;
        }
    }
}
