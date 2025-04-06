using Dorak.Models;
using Dorak.ViewModels.AccountViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorak.ViewModels
{
    public static class AccountExtentions
    {
        public static User ToModel(this RegisterationViewModel viewmodel)
        {
            return new User
            {
                UserName = viewmodel.UserName,
                Email = viewmodel.Email,
                PhoneNumber = viewmodel.PhoneNumber,
            };
        }
    }
}
