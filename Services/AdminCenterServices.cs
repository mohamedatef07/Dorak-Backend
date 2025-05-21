using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Dorak.Models;
using Dorak.ViewModels;
using Microsoft.AspNetCore.Identity;
using Repositories;

namespace Services
{
    public class AdminCenterServices
    {
        public AdminCenterRepository adminCenterRepository;
        public CommitData commitData;
        public AdminCenterServices(AdminCenterRepository _adminCenterRepository, CommitData _commitData)
        {
            adminCenterRepository = _adminCenterRepository;
            commitData = _commitData;
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
            commitData.SaveChanges();
            return IdentityResult.Success;
        }
    }
}
