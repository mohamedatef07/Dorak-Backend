using Dorak.Models;
using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Dorak.ViewModels;

namespace Repositories
{
    public class AccountRepository : BaseRepository<User>
    {
        private UserManager<User> UserManager;
        private SignInManager<User> signInManager;
        public AccountRepository(DorakContext dbContext, UserManager<User> _UserManager,
            SignInManager<User> _signInManager) : base(dbContext)
        {
            UserManager = _UserManager;
            signInManager = _signInManager;
        }



        public async Task<IdentityResult> Register(RegisterationViewModel userRegister)
        {
            var userEntity = userRegister.ToModel();

            var res = await UserManager.CreateAsync(userEntity, userRegister.Password);
            if (res.Succeeded)
            {
                userEntity.EmailConfirmed = true;
                await UserManager.UpdateAsync(userEntity); 

                res = await UserManager.AddToRoleAsync(userEntity, userRegister.Role);

                return res;
            }

            return res;
        }

        public async Task<SignInResult> Login(UserLoginViewModel vmodel)
        {
            //if correct Email
            var User = await UserManager.FindByEmailAsync(vmodel.UserName);
            if (User != null)
                return await signInManager.PasswordSignInAsync(User, vmodel.Password, true, true);
            else
                return await signInManager.PasswordSignInAsync(vmodel.UserName, vmodel.Password, true, true);
        }

        public async Task<User> FindByUserName(string userName)
        {
            return await UserManager.FindByNameAsync(userName);
        }

        public async Task<User> FindByEmail(string userName)
        {
            return await UserManager.FindByEmailAsync(userName);
        }

        public async Task<IList<string>> GetUserRoles(User user)
        {
            return await UserManager.GetRolesAsync(user);
        }

        public async Task Signout()
        {
            await signInManager.SignOutAsync();
        }

    }

}