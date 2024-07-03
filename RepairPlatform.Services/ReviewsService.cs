using Microsoft.EntityFrameworkCore;
using RepairPlatform.Entities;
using RepairPlatform.Services.DTO.Reviews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairPlatform.Services
{
    public class ReviewsService
    {
        private readonly Repairguy20118046Context _context;

        public ReviewsService(Repairguy20118046Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Review>> GetReviewsAsync()
        {
            return await _context.Reviews
                 .Include(r => r.Client)
                .Include(r => r.Group)
                .Include(r => r.Repairguy)
                .Include(r => r.Reservation) 
                .OrderByDescending(r => r.RevDateTime)
                .ToListAsync();
        }

        public async Task<Review?> GetReviewByIdAsync(int id)
        {
            return await _context.Reviews.FindAsync(id);
        }

        public async Task<Review> CreateReviewAsync(Review review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return review;
        }

        //public async Task<Review> UpdateReviewAsync(Review review)
        //{
        //    _context.Entry(review).State = EntityState.Modified;
        //    await _context.SaveChangesAsync();
        //    return review;
        //}

        //public async Task DeleteReviewAsync(int id)
        //{
        //    var review = await _context.Reviews.FindAsync(id);
        //    if (review != null)
        //    {
        //        _context.Reviews.Remove(review);
        //        await _context.SaveChangesAsync();
        //    }
        //}

        public async Task<bool> HasUserReviewedReservationAsync(int clientId, int reservationId)
        {
            return await _context.Reviews
                .AnyAsync(r => r.ClientId == clientId && r.ReservationId == reservationId);
        }
        public async Task<bool> HasUserReviewedAllReservationsAsync(int clientId, List<Reservation> reservations)
        {
            foreach (var reservation in reservations)
            {
                if (!await HasUserReviewedReservationAsync(clientId, reservation.ReservationId))
                {
                    return false;
                }
            }
            return true;
        }
        public async Task<double> GetAverageRatingByRepairguyIdAsync(int repairguyId)
        {
            var ratings = await _context.Reviews
                .Where(r => r.RepairguyId == repairguyId)
                .Select(r => (double?)r.Rating)
                .ToListAsync();

            return ratings.Average() ?? 0.0; 
        }

        public async Task<List<Review>> GetReviewsByRepairguyIdAsync(int repairguyId)
        {
            return await _context.Reviews.Where(r => r.RepairguyId == repairguyId).ToListAsync();
        }

        public async Task<List<Review>> GetPublicReviewsByRepairguyIdAsync(int repairguyId)
        {
            return await _context.Reviews
                .Include(r => r.Client) 
                .Include(r => r.Group)
                .Where(r => r.RepairguyId == repairguyId)
                .ToListAsync();
        }
    }
}
