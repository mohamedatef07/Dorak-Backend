using Data;
using Dorak.Models;
using Dorak.ViewModels;
using Microsoft.EntityFrameworkCore;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using Dorak.DataTransferObject.ClientDTO;

namespace Services
{
    public class Review_Service
    {
        private readonly CommitData commitData;
        private readonly DorakContext context;

        public Review_Service(CommitData _commitData, DorakContext _context)
        {
            commitData = _commitData;
            context = _context;
        }

        public string CreateReview(Review review)
        {
            context.Review.Add(review);
            commitData.SaveChanges();

            return "Review added successfully.";
        }

        public void UpdateAllProvidersAverageRating()
        {
            var providers = context.Providers.ToList();

            foreach (var provider in providers)
            {
                var reviews = context.Review
                    .Where(r => r.ProviderId == provider.ProviderId)
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

            commitData.SaveChanges();
        }


        public List<ReviewByProviderDTO> GetReviewsForProvider(string providerId)
        {
            return context.Review
                .Where(r => r.ProviderId == providerId)
                .Select(r => new ReviewByProviderDTO
                {
                    ProviderName = r.Provider.FirstName + " " + r.Provider.LastName,
                    ClientName = r.Client.FirstName + " " + r.Client.LastName,
                    Review = r.Description,
                    ClientId = r.ClientId
                }).ToList();
        }

        public List<ReviewbyclientDTO> GetReviewsForClient(string ClientId)
        {
            return context.Review
                .Where(r => r.ClientId == ClientId)
                .Select(r => new ReviewbyclientDTO
                {
                    ProviderName = r.Provider.FirstName + " " + r.Provider.LastName,
                    Review = r.Description,
                    Providerid = r.ProviderId,
                    Rate=r.Rating
                }).ToList();
        }
    }
}
