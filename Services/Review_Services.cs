using Data;
using Dorak.Models;
using Dorak.ViewModels;
using Microsoft.EntityFrameworkCore; 
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services
{

    public class Review_Service
    {
        private readonly ReviewRepository reviewRepository;
        private readonly CommitData commitData;

        public Review_Service(ReviewRepository _reviewRepository, CommitData _commitData)
        {
            reviewRepository = _reviewRepository;
            commitData = _commitData;
        }

        public string CreateReview(Review review)
        {
            reviewRepository.Add(review);
            commitData.SaveChanges();
            return "Review added successfully.";
        }


    }


}
