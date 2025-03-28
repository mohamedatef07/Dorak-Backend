using Dorak.Models;
using Dorak.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Repositories;


namespace Services
{
    public class AccountServices
    {
        AccountRepository accountRepository;
        ClientRepository clientRepository;
        ProviderRepository providerRepository;
        private readonly IConfiguration configuration;
        public AccountServices(AccountRepository _AccountRepository,
                               ClientRepository _ClientRepository,
                               ProviderRepository _ProviderRepository,
                               IConfiguration _configuration)
        {
            accountRepository = _AccountRepository;
            clientRepository = _ClientRepository;
            providerRepository = _ProviderRepository;
            configuration = _configuration;
        }


        public async Task<IdentityResult> CreateAccount(UserRegisterViewModel user)
        {
            var userRes = await accountRepository.Register(user);

            if (userRes.Succeeded)
            {
                var currentUser = await accountRepository.FindByUserName(user.UserName);
                if (user.Role == "Admin")
                {
                    clientRepository.Add(new Client() { ClientId = currentUser.Id });
                }
                else if(user.Role == "Operator")
                {
                    OperatorRepo
                }
                else if (user.Role == "Provider")
                {
                    providerRepository.Add(new Provider() { ProviderId = currentUser.Id });
                }
                else if (user.Role == "Client")
                {
                    clientRepository.Add(new Client() { ClientId = currentUser.Id });
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
