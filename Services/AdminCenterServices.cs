using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dorak.Models;
using Dorak.ViewModels;
using Dorak.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Identity;
using Repositories;

namespace Services
{
    public class AdminCenterServices
    {
        private AdminCenterRepository adminCenterRepository;

        public AdminCenterServices(AdminCenterRepository _adminCenterRepository)
        {
            adminCenterRepository = _adminCenterRepository;
        }

        public async Task<IdentityResult> CreateAdminCenter(string userId, AdminCenterViewModel model)
        {
            var admin = new AdminCenterManagement
            {
                AdminId = userId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Gender = model.Gender,
                Image = model.Image
            };

            adminCenterRepository.Add(admin);
            adminCenterRepository.SaveChanges();
            return IdentityResult.Success;
        }
    }
}
