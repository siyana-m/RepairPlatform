using RepairPlatform.Entities;
using RepairPlatform.Services.DTO.Reservations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepairPlatform.Services.DTO.Repairguys;
using Microsoft.EntityFrameworkCore.Internal;

namespace RepairPlatform.Services
{
    public class ReservationsService
    {
        private readonly Repairguy20118046Context _dbContext;

        public ReservationsService(Repairguy20118046Context dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<ReservationDto> GetReservationById(int reservationId)
        {
            var reservation = await _dbContext.Reservations
                .Include(r => r.Repairguy)
                .FirstOrDefaultAsync(r => r.ReservationId == reservationId).ConfigureAwait(false);
            var group = await _dbContext.Groups.FindAsync(reservation!.GroupId).ConfigureAwait(false);

            return new ReservationDto
            {
                ReservationId = reservation!.ReservationId,
                ResName = reservation.ResName,
                Group = group!,
                ResDateTime = reservation.ResDateTime,
                ResLocation = reservation.ResLocation,
                ResComment = reservation.ResComment,
                RepairguyId = reservation.RepairguyId
            };
        }

        public async Task<int> CreateReservation(ReservationDto reservationDto)
        {
            var reservation = new Reservation
            {
                ReservationId = reservationDto.ReservationId,
                ResName = reservationDto.ResName,
                Group = reservationDto.Group,
                ResDateTime = reservationDto.ResDateTime,
                ResLocation = reservationDto.ResLocation,
                ResComment = reservationDto.ResComment,
                RepairguyId = reservationDto.RepairguyId,
                GroupId = reservationDto.GroupId,
                ClientId = reservationDto.ClientId
            };

            _dbContext.Reservations.Add(reservation);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);

            return reservation.ReservationId;
        }

        public async Task<Reservation> ConfirmReservation(int reservationId)
        {
            var reservation = await _dbContext.Reservations.FindAsync(reservationId).ConfigureAwait(false);

            // Assuming confirming means just ensuring it exists without further changes
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return reservation!;
        }

        public async Task<bool> CancelReservation(int reservationId)
        {
            var reservation = await _dbContext.Reservations.FindAsync(reservationId).ConfigureAwait(false);
            if (reservation == null) return false;

            _dbContext.Reservations.Remove(reservation);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return true;
        }

        

        public async Task<List<ReservationDto>> GetReservationsByRepairguyIdAsync(int repairguyId)
        {
            var reservations = await _dbContext.Reservations
                .Where(r => r.RepairguyId == repairguyId)
                .Select(r => new ReservationDto
                {
                    ReservationId = r.ReservationId,
                    RepairguyId = r.RepairguyId,
                    ClientId = r.ClientId,
                    ResName = r.ResName,
                    ResLocation = r.ResLocation,
                    ResDateTime = r.ResDateTime,
                    Client = r.Client,
                    Group = r.Group
                })
                .ToListAsync().ConfigureAwait(false);

            return reservations!;
        }


        public async Task<List<ReservationDto>> GetReservationsByClientIdAsync(int clientId)
        {
            var reservations = await _dbContext.Reservations
                .Include(r => r.Group)
                .Include(r => r.Repairguy) // Include Repairguy
                 .ThenInclude(rg => rg.User)
                .Where(r => r.ClientId == clientId)
                .ToListAsync()
                .ConfigureAwait(false);


            return reservations.Select(r => new ReservationDto
            {
                ClientId = r.ClientId,
                ReservationId = r.ReservationId,
                ResName = r.ResName,
                Group = r.Group,
                ResDateTime = r.ResDateTime,
                ResLocation = r.ResLocation,
                ResComment = r.ResComment,
                ResStatus = r.ResStatus,
                RepairguyId = r.RepairguyId,
                Repairguy = r.Repairguy
            }).ToList();
        }

        public async Task<Reservation?> GetReservationByClientAndRepairguyAsync(int clientId, int repairguyId)
        {
            return await _dbContext.Reservations
                .Include(r => r.Group)
                .FirstOrDefaultAsync(r => r.ClientId == clientId && r.RepairguyId == repairguyId).ConfigureAwait(false);
        }

        public async Task<List<Reservation>> GetReservationsByClientForRepairguyAsync(int clientId, int repairguyId)
        {
            return await _dbContext.Reservations
                .Include(r => r.Group)
                .Where(r => r.ClientId == clientId && r.RepairguyId == repairguyId)
                .ToListAsync().ConfigureAwait(false);
        }


        public async Task ChangeReservationStatus(int reservationId, string status)
        {
            var reservation = await _dbContext.Reservations.FindAsync(reservationId).ConfigureAwait(false);
            if (reservation != null)
            {
                reservation.ResStatus = status;
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public async Task<bool> DeleteReservationByClientAsync(int reservationId)
        {
            // Load the reservation along with its associated reviews
            var reservation = await _dbContext.Reservations
                .Include(r => r.Reviews)
                .FirstOrDefaultAsync(r => r.ReservationId == reservationId);

            if (reservation == null)
            {
                //throw new Exception("Reservation not found");
                return false;
            }

            _dbContext.Reviews.RemoveRange(reservation.Reviews);

            _dbContext.Reservations.Remove(reservation);

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<Reservation>> GetReservationsExportForRepairguyAsync(int repairguyId, string? status, string? sortOption)
        {
            var reservationsQuery = _dbContext.Reservations
                .Where(r => r.RepairguyId == repairguyId)
                .Include(r => r.Client)
                .Include(r => r.Group)
                .AsQueryable();
          
            if (!string.IsNullOrEmpty(status))
            {
                if (status == "В очакване")
                {
                    reservationsQuery = reservationsQuery.Where(r => r.ResStatus == null || r.ResStatus == "В очакване");
                }
                else
                {
                    reservationsQuery = reservationsQuery.Where(r => r.ResStatus == status);
                }
            }

            switch (sortOption)
            {
                case "dateAsc":
                    reservationsQuery = reservationsQuery.OrderBy(r => r.ResDateTime);
                    break;
                case "dateDesc":
                    reservationsQuery = reservationsQuery.OrderByDescending(r => r.ResDateTime);
                    break;
            }

            return await reservationsQuery.ToListAsync();
        }

    }
}
