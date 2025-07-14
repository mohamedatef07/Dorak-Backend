using Dorak.Models;

namespace Dorak.ViewModels
{
    public static class ClientExtensions
    {
        public static ClientViewModel toModelView(this Client _client)
        {
            return new ClientViewModel
            {
                ClientId = _client.ClientId,
                FirstName = _client.FirstName,
                LastName = _client.LastName,
                Gender = _client.Gender,
                City = _client.City,
                Governorate = _client.Governorate,
                Country = _client.Country
            };
        }
    }
}
