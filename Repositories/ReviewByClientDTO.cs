namespace Repositories
{
    public class ReviewByClientDTO
    {
        public string Review { get; internal set; }
        public string ProviderName { get; internal set; }
        public string ProviderId { get; internal set; }
        public decimal Rate { get; internal set; }
    }
}