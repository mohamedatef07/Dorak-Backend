using Data;
using Dorak.DataTransferObject;
using Dorak.Models;
using Repositories;

namespace Services
{
    public class ReviewServices
    {
        private readonly CommitData _commitData;
        private readonly ReviewRepository _reviewRepository;
        private readonly ProviderRepository _providerRepository;

        public ReviewServices(CommitData commitData, ReviewRepository reviewRepository, ProviderRepository providerRepository)
        {
            _commitData = commitData;
            _reviewRepository = reviewRepository;
            _providerRepository = providerRepository;
        }

        public bool CreateReview(Review review)
        {
            if (review == null)
            {
                return false;
            }

            _reviewRepository.Add(review);
            _commitData.SaveChanges();
            return true;
        }
        public bool DeleteReview(int reviewId)
        {
            if (reviewId <= 0)
            {
                return false;
            }
            var review = _reviewRepository.GetById(rev => rev.ReviewId == reviewId);
            if (review == null)
            {
                return false;
            }
            review.IsDeleted = true;
            _reviewRepository.Edit(review);
            _commitData.SaveChanges();
            return true;
        }

        public void UpdateAllProvidersAverageRating()
        {
            var providers = _providerRepository.GetList(p => !p.IsDeleted);

            foreach (var provider in providers)
            {
                var reviews = _reviewRepository.GetList(r => r.ProviderId == provider.ProviderId && !r.IsDeleted)
                    .ToList();
                if (reviews.Any())
                {
                    provider.Rate = reviews.Average(r => r.Rating);
                }
                else
                {
                    provider.Rate = 0;
                }
            }
            _commitData.SaveChanges();
        }


        public List<ReviewByProviderDTO> GetReviewsForProvider(string providerId)
        {
            return _reviewRepository.GetList(r => r.ProviderId == providerId && !r.IsDeleted)
                .Select(r => new ReviewByProviderDTO
                {
                    ProviderName = $"{r.Provider.FirstName} {r.Provider.LastName}",
                    ClientName = $"{r.Client.FirstName} {r.Client.LastName}",
                    Review = r.Description,
                    ClientId = r.ClientId,
                    Rate = r.Rating,
                    Date = r.Date
                }).ToList();
        }

        public PaginationApiResponse<List<ReviewByClientDTO>> GetReviewsForClient(string ClientId, int pageNumber = 1, int pageSize = 10)
        {
            var reviews = _reviewRepository.GetList(r => r.ClientId == ClientId && !r.IsDeleted).OrderByDescending(r => r.Date)
                .Select(r => new ReviewByClientDTO
                {
                    ProviderName = $"{r.Provider.FirstName} {r.Provider.LastName}",
                    Review = r.Description,
                    Rate = r.Rating,
                    Date = r.Date
                }).ToList();

            var totalRecords = reviews.Count();
            var paginationResponse = new PaginationApiResponse<List<ReviewByClientDTO>>(
            success: true,
            message: "Client reviews retrieved successfully.",
            status: 200,
            data: reviews,
            totalRecords: totalRecords,
            currentPage: pageNumber,
            pageSize: pageSize);
            return paginationResponse;
        }
    }
}
