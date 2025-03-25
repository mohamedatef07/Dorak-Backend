using Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories
{
    public class RoleRepository : BaseRepository<IdentityRole>
    {
        // asp.net core identity role manager
        private readonly RoleManager<IdentityRole> roleManager;

        public RoleRepository(DorakContext dbContext, RoleManager<IdentityRole> _roleManager)
            : base(dbContext)
        {
            roleManager = _roleManager;
        }

        // check if role exists
        public async Task<bool> RoleExistsAsync(string roleName)
        {
            return await roleManager.RoleExistsAsync(roleName);
        }

        // add role
        public async Task<IdentityResult> AddRoleAsync(string roleName)
        {
            if (!await RoleExistsAsync(roleName)) // to make sure role does not exist before adding
            {
                return await roleManager.CreateAsync(new IdentityRole(roleName));
            }
            return IdentityResult.Failed(new IdentityError { Description = "Role already exists." });
        }

        // get role by name
        public async Task<IdentityRole> GetRoleByNameAsync(string roleName)
        {
            return await roleManager.FindByNameAsync(roleName);
        }

        // get all roles
        public async Task<List<IdentityRole>> GetAllRolesAsync()
        {
            return await roleManager.Roles.ToListAsync();
        }

        // delete role
        public async Task<IdentityResult> DeleteRoleAsync(string roleName)
        {
            var role = await roleManager.FindByNameAsync(roleName);

            if (role == null) // Prevent exception if role does not exist
            {
                return IdentityResult.Failed(new IdentityError { Description = "Role not found." });
            }

            return await roleManager.DeleteAsync(role);
        }
    }
}
