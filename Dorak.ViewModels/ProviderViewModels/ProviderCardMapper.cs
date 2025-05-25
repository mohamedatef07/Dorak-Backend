using Dorak.Models;

namespace Dorak.ViewModels
{
    public static class ProviderCardMapper
    {
        public static ProviderCardViewModel ToCardView(this Provider provider)
        {
            return new ProviderCardViewModel
            {
                FullName = $"{provider.FirstName} {provider.LastName}",
                Specialization = provider.Specialization,
                Rate = provider.Rate,
                EstimatedDuration = provider.EstimatedDuration,
                City = provider.City,
                Price = provider.ProviderCenterServices.Any()
         ? provider.ProviderCenterServices.Min(s => s.Price)
         : 0
            };


        }
    }

}