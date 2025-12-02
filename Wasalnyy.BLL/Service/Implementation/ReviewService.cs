using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Wasalnyy.BLL.DTO.ReviewandReports;
using Wasalnyy.BLL.DTO.Trip;
using Wasalnyy.BLL.Exceptions;
using Wasalnyy.BLL.Validators;
using Wasalnyy.DAL.Repo.Abstraction;
using Wasalnyy.DAL.Entities;

namespace Wasalnyy.BLL.Service.Implementation
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepo _reviewRepo;
        private readonly IMapper _mapper;
        private readonly ReviewServiceValidator _valid;
        private readonly ITripService _trip;

        public ReviewService(IReviewRepo reviewRepo, IMapper mapper, ReviewServiceValidator valid, ITripService trip)
        {
            _reviewRepo = reviewRepo ?? throw new ArgumentNullException(nameof(reviewRepo));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _trip = trip ?? throw new ArgumentNullException(nameof(trip));
            _valid = valid ?? throw new ArgumentNullException(nameof(valid));
        }

        
        public async Task<ReturnReviewDto> AddReviewAsync(CreateReviewDto dto, string currentUserId)
        {
           

            var tripDto = await _trip.GetByIdAsync(dto.TripId);
            if (tripDto == null)
                throw new NotFoundException("Trip not found");

           
            ReviewerType reviewerType;
            if (tripDto.DriverId == currentUserId)
            {
                reviewerType = ReviewerType.Driver;  // Current user is the driver
            }
            else if (tripDto.RiderId == currentUserId)
            {
                reviewerType = ReviewerType.Rider;   // Current user is the rider
            }
            else
            {
                throw new UnauthorizedAccessException("You are not part of this trip");
            }

            
            if (tripDto.TripStatus != "Ended")
                throw new ValidationException("You cannot review a trip that is not completed.");

           
            var hasReviewed = await _reviewRepo.HasReviewedTripAsync(
                dto.TripId,
                currentUserId,
                reviewerType);  

            if (hasReviewed)
                throw new ValidationException("You have already reviewed this trip");

            var review = new Review
            {
                Id = Guid.NewGuid(),
                TripId = dto.TripId,
                DriverId = tripDto.DriverId,
                RiderId = tripDto.RiderId,
                Comment = dto.Comment?.Trim(),
                Stars = dto.Stars,
                ReviewerType = reviewerType,  
                CreatedAt = DateTime.UtcNow
            };

            await _reviewRepo.AddAsync(review);
            await _reviewRepo.SaveChangesAsync();

            return _mapper.Map<ReturnReviewDto>(review);
        }

        
      
       
        public async Task<ReturnReviewDto> UpdateReviewAsync(UpdateReviewDto dto, string currentUserId)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Update DTO cannot be null");

            if (string.IsNullOrEmpty(currentUserId))
                throw new ArgumentNullException(nameof(currentUserId), "Current user ID cannot be null or empty");

            
            _valid.UpdateReviewValidator(dto);

            var review = await _reviewRepo.GetReviewById(dto.Id);
            if (review == null)
                throw new NotFoundException("Review not found");

            
            if (!IsReviewAuthor(review, currentUserId))
                throw new UnauthorizedAccessException("You can only update your own reviews");

            // Update fields
            review.Comment = dto.Comment?.Trim();
            review.Stars = (double)dto.Stars;

            await _reviewRepo.UpdateAsync(review);
            await _reviewRepo.SaveChangesAsync();

            return _mapper.Map<ReturnReviewDto>(review);
        }

        
        
        
        public async Task<bool> DeleteReviewAsync(Guid reviewId, string currentUserId)
        {
            if (reviewId == Guid.Empty)
                throw new ArgumentNullException(nameof(reviewId), "Review ID cannot be empty");

            if (string.IsNullOrEmpty(currentUserId))
                throw new ArgumentNullException(nameof(currentUserId), "Current user ID cannot be null or empty");

            var review = await _reviewRepo.GetReviewById(reviewId);
            if (review == null)
                return false;

            
            if (!IsReviewAuthor(review, currentUserId))
                throw new UnauthorizedAccessException("You can only delete your own reviews");

            await _reviewRepo.DeleteAsync(reviewId);
            await _reviewRepo.SaveChangesAsync();

            return true;
        }

       
      
        public async Task<IEnumerable<ReturnReviewDto>> GetDriverReviewsAsync(string driverId)
        {
            if (string.IsNullOrEmpty(driverId))
                throw new ArgumentNullException(nameof(driverId), "Driver ID cannot be null or empty");

            var reviews = await _reviewRepo.GetDriverReviewsAsync(driverId);
            return _mapper.Map<IEnumerable<ReturnReviewDto>>(reviews);
        }

        
        // Get average rating for a driver (last 500 reviews)
        
        public async Task<double> GetDriverAverageRatingAsync(string driverId)
        {
            if (string.IsNullOrEmpty(driverId))
                throw new ArgumentNullException(nameof(driverId), "Driver ID cannot be null or empty");

            return await _reviewRepo.GetDriverAverageRatingAsync(driverId);
        }

        
        
        
        public async Task<IEnumerable<ReturnReviewDto>> GetRiderReviewsAsync(string riderId)
        {
            if (string.IsNullOrEmpty(riderId))
                throw new ArgumentNullException(nameof(riderId), "Rider ID cannot be null or empty");

            var reviews = await _reviewRepo.GetRiderReviewsAsync(riderId);
            return _mapper.Map<IEnumerable<ReturnReviewDto>>(reviews);
        }

        
       
       
        public async Task<double> GetRiderAverageRatingAsync(string riderId)
        {
            if (string.IsNullOrEmpty(riderId))
                throw new ArgumentNullException(nameof(riderId), "Rider ID cannot be null or empty");

            return await _reviewRepo.GetRiderAverageRatingAsync(riderId);
        }

        
       
        
        public async Task<ReturnReviewDto> GetReviewByIdAsync(Guid reviewId)
        {
            if (reviewId == Guid.Empty)
                throw new ArgumentNullException(nameof(reviewId), "Review ID cannot be empty");

            var review = await _reviewRepo.GetReviewById(reviewId);
            if (review == null)
                throw new NotFoundException("Review not found");

            return _mapper.Map<ReturnReviewDto>(review);
        }

        
        
        
        public async Task<IEnumerable<ReturnReviewDto>> GetMyReviewsAsync(string currentUserId, ReviewerType reviewerType)
        {
            if (string.IsNullOrEmpty(currentUserId))
                throw new ArgumentNullException(nameof(currentUserId), "Current user ID cannot be null or empty");

            var reviews = await _reviewRepo.GetReviewsByReviewerAsync(currentUserId, reviewerType);
            return _mapper.Map<IEnumerable<ReturnReviewDto>>(reviews);
        }

        
        
        
        public async Task<ReviewStatisticsDto> GetReviewStatisticsAsync(string userId, ReviewerType userType)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId), "User ID cannot be null or empty");

            var stats = await _reviewRepo.GetReviewStatisticsAsync(userId, userType);
            return _mapper.Map<ReviewStatisticsDto>(stats);
        }

        
      
        
        public async Task<ReturnReviewDto> GetReviewForViewAsync(Guid reviewId, string currentUserId)
        {
            if (reviewId == Guid.Empty)
                throw new ArgumentNullException(nameof(reviewId), "Review ID cannot be empty");

            var review = await _reviewRepo.GetReviewById(reviewId);
            if (review == null)
                throw new NotFoundException("Review not found");

            

            return _mapper.Map<ReturnReviewDto>(review);
        }

        

        
        
       
     

        
      
        private bool IsReviewAuthor(Review review, string currentUserId)
        {
            return (review.ReviewerType == ReviewerType.Rider && review.RiderId == currentUserId) ||
                   (review.ReviewerType == ReviewerType.Driver && review.DriverId == currentUserId);
        }
    }
}