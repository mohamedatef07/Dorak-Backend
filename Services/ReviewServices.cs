using Data;
using Dorak.DataTransferObject;
using Dorak.DataTransferObject.ReviewDTOs;
using Dorak.Models;
using Repositories;
using System.Linq.Expressions;

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
        public bool EditReview(EditReviewDTO updatedReview)
        {
            if (updatedReview == null)
            {
                return false;
            }
            var review = _reviewRepository.GetById(rev => rev.ReviewId == updatedReview.ReviewId);
            if (review == null)
            {
                return false;
            }
            review.Description = updatedReview.Review;
            review.Rating = updatedReview.Rate;
            _reviewRepository.Edit(review);
            _commitData.SaveChanges();
            return true;
        }

        public void UpdateAllProvidersAverageRating()
        {
            var providers = _providerRepository.GetList(p => !p.IsDeleted).ToList();

            var providerAverages = _reviewRepository.GetList(r => !r.IsDeleted).GroupBy(r => r.ProviderId)
                .Select(g => new
                {
                    ProviderId = g.Key,
                    Rating = g.Average(r => r.Rating)
                }).ToList();

            var averageDict = providerAverages.ToDictionary(x => x.ProviderId, x => x.Rating);

            foreach (var provider in providers)
            {
                if (averageDict.TryGetValue(provider.ProviderId, out var averageRate))
                {
                    provider.Rate = averageRate;
                }
                else
                {
                    provider.Rate = 0;
                }
            }
            _commitData.SaveChanges();
        }

        public PaginationApiResponse<List<GetAllProviderReviewsDTO>> GetReviewsForProvider(string providerId, int pageNumber = 1, int pageSize = 10)
        {
            Expression<Func<Review, bool>> filter = rev => rev.ProviderId == providerId && !rev.IsDeleted;
            var reviews = _reviewRepository.GetAllOrderedByExpression(filter, pageSize, pageNumber, query => query.OrderByDescending(rev => rev.Date))
                                             .Select(r => new GetAllProviderReviewsDTO
                                             {
                                                 ClientName = $"{r.Client.FirstName} {r.Client.LastName}",
                                                 Review = r.Description,
                                                 ClientId = r.ClientId,
                                                 Rate = r.Rating,
                                                 Date = r.Date
                                             }).ToList();

            var totalRecords = _reviewRepository.GetList(filter).Count();
            var paginationResponse = new PaginationApiResponse<List<GetAllProviderReviewsDTO>>(
            success: true,
            message: "Provider reviews retrieved successfully.",
            status: 200,
            data: reviews,
            totalRecords: totalRecords,
            currentPage: pageNumber,
            pageSize: pageSize);
            return paginationResponse;
        }

        public PaginationApiResponse<List<ReviewByClientDTO>> GetReviewsForClient(string ClientId, int pageNumber = 1, int pageSize = 10)
        {
            Expression<Func<Review, bool>> filter = rev => rev.ClientId == ClientId && !rev.IsDeleted;
            var reviews = _reviewRepository.GetAllOrderedByExpression(filter, pageSize, pageNumber, query => query.OrderByDescending(rev => rev.Date))

                         .Select(r => new ReviewByClientDTO
                         {
                             ReviewId = r.ReviewId,
                             ProviderName = $"{r.Provider.FirstName} {r.Provider.LastName}",
                             Review = r.Description,
                             Rate = r.Rating,
                             Date = r.Date
                         }).ToList();

            var totalRecords = _reviewRepository.GetList(filter).Count();
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
