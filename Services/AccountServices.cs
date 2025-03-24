using Dorak.Models;
using Dorak.ViewModels;
using Microsoft.AspNetCore.Identity;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class AccountServices
    {
        AccountRepository accountRepository;
        ClientRepository clientRepository;
        public AccountServices(AccountRepository _AccountRepository,
                               ClientRepository _ClientRepository)
        {
            accountRepository = _AccountRepository;
            clientRepository = _ClientRepository;
        }


        public async Task<IdentityResult> CreateAccount(UserRegisterViewModel user)
        {
            var userRes = await accountRepository.Register(user);

            if (userRes.Succeeded)
            {
                var currentUser = await accountRepository.FindByUserName(user.UserName);
                if (user.Role == "Client")
                {
                    
                    clientRepository.AddAsync(new Client() { ClientId = currentUser.Id });
                }
                

            }
            return IdentityResult.Failed();
        }
        public async Task<SignInResult> Login(UserLoginViewModel user)
        {
            return await accountRepository.Login(user);
        }
        public async Task Signout()
        {
            await accountRepository.Signout();
        }
    }
}
