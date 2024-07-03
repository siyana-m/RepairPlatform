using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RepairPlatform.Entities;
using RepairPlatform.Services.DTO.Admins;
using RepairPlatform.Services.DTO.Groups;
using RepairPlatform.Services.DTO.Repairs;
using RepairPlatform.Services.DTO.Towns;
using RepairPlatform.Services.DTO.Repairguys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RepairPlatform.Services
{
    public class AdministratorsService
    {
        private readonly Repairguy20118046Context _context;

        private readonly IMapper _mapper;

        public AdministratorsService(Repairguy20118046Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public List<Repairguy> GetAllRepairguys()
        {
            return _context.Repairguys.ToList();
        }

        public List<Client> GetAllClients()
        {
            return _context.Clients.ToList();
        }

        public List<Group> GetAllGroups()
        {
            return _context.Groups.ToList();
        }

        public List<Repair> GetAllRepairs()
        {
            return _context.Repairs.ToList();
        }

        public List<Town> GetAllTowns()
        {
            return _context.Towns.ToList();
        }

        public Repairguy? GetRepairguyById(int repairguyId)
        {
            return _context.Repairguys.FirstOrDefault(r => r.RepairguyId == repairguyId);
        }

        public Client? GetClientById(int clientId)
        {
            return _context.Clients.FirstOrDefault(c => c.ClientId == clientId);
        }

        public Group? GetGroupById(int groupId)
        {
            return _context.Groups.FirstOrDefault(g => g.GroupId == groupId);
        }

        public Repair? GetRepairById(int repairId)
        {
            return _context.Repairs.FirstOrDefault(rr => rr.RepairId == repairId);
        }

        public Town? GetTownById(int townId)
        {
            return _context.Towns.FirstOrDefault(t => t.Id == townId);
        }

        public void UpdateRepairguy(Repairguy repairguy)
        {
            var existingRepairguy = _context.Repairguys.FirstOrDefault(r => r.RepairguyId == repairguy.RepairguyId);
            if (existingRepairguy != null)
            {
                existingRepairguy.Rstatus = repairguy.Rstatus;
                _context.SaveChanges();
            }
        }

        public void UpdateClient(Client client)
        {
            var existingClient = _context.Clients.FirstOrDefault(c => c.ClientId == client.ClientId);
            if (existingClient != null)
            {
                existingClient.Cstatus = client.Cstatus;
                _context.SaveChanges();
            }
        }

        public void ChangeRepairguyStatus(int repairguyId, string status)
        {
            var repairguy = _context.Repairguys.FirstOrDefault(r => r.RepairguyId == repairguyId);
            if (repairguy != null)
            {
                repairguy.Rstatus = status;
                _context.SaveChanges();
            }
        }

        public void ChangeClientStatus(int clientId, string status)
        {
            var client = _context.Clients.FirstOrDefault(c => c.ClientId == clientId);
            if (client != null)
            {
                client.Cstatus = status;
                _context.SaveChanges();
            }
        }

        public void UpdateGroup(Group group)
        {
            var existingGroup = _context.Groups.FirstOrDefault(g => g.GroupId == group.GroupId);
            if (existingGroup != null)  //?
            {
                existingGroup.CatName = group.CatName;
                existingGroup.CatDescription = group.CatDescription;
                _context.SaveChanges();
            }
        }

        public void UpdateRepair(Repair repair)
        {
            var existingRepair = _context.Repairs.FirstOrDefault(rr => rr.RepairId == repair.RepairId);
            if (existingRepair != null)  //?
            {
                existingRepair.RepName = repair.RepName;
                existingRepair.RepDescription = repair.RepDescription;
                _context.SaveChanges();
            }
        }

        public void UpdateTown(Town town)
        {
            var existingTown = _context.Towns.FirstOrDefault(t => t.Id == town.Id);
            if (existingTown != null)
            {
                existingTown.Name = town.Name;
                _context.SaveChanges();
            }
        }

        //public void DeleteRepairguy(int repairguyId)
        //{
        //    var repairguy = _context.Repairguys
        //        .Include(r => r.Reservations)
        //        .Include(r => r.Reviews)
        //        .Include(r => r.Repairs)
        //        .Include(r => r.Towns)
        //        .FirstOrDefault(r => r.RepairguyId == repairguyId);

        //    if (repairguy != null)
        //    {
        //        _context.Repairguys.Remove(repairguy);
        //        _context.SaveChanges();
        //    }
        //}

        public  void CreateGroup(GroupDto groupDto)

        {
            var group = _mapper.Map<Group>(groupDto);
            try
            {
                _context.Groups.Add(group);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public void CreateRepair(RepairDto repairDto)
        {
            var repair = _mapper.Map<Repair>(repairDto);
            try
            {
                _context.Repairs.Add(repair);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public void CreateTown(TownDto townDto)
        {
            var town = _mapper.Map<Town>(townDto);
            try
            {
                _context.Towns.Add(town);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex.Message);
                throw;
            }
        }


        public List<Group> GetAllGroupsWithRepairs()
        {
            return _context.Groups
                .Include(g => g.Repairs)
                .ToList();
        }

        public Group GetGroupbyId(int groupId)
        {
            var group = _context.Groups
                 .FirstOrDefault(c => c.GroupId == groupId);

            if (group == null)
            {
                return null!;
            }

            return group;
        }



        public void DeleteClient(int clientId)
        {
            var client = _context.Clients
                .Include(c => c.Reservations)
                .Include(c => c.Reviews)
                .FirstOrDefault(c => c.ClientId == clientId);

            if (client != null)
            {
                _context.Clients.Remove(client);
                _context.SaveChanges();
            }
        }

        public void DeleteRepairguy(int repairguyId)
        {
            var repairguy = _context.Repairguys
                .Include(r => r.Reservations)
                .Include(r => r.Reviews)
                .FirstOrDefault(c => c.RepairguyId == repairguyId);

            if (repairguy != null)
            {
                _context.Repairguys.Remove(repairguy);
                _context.SaveChanges();
            }
        }

        public void DeleteGroup(int groupId)
        {
            var group = _context.Groups
                .Include(r => r.Repairs)
                .FirstOrDefault(g => g.GroupId == groupId);

            if (group != null)
            {
                _context.Repairs.RemoveRange(group.Repairs);
                _context.Groups.Remove(group);
                _context.SaveChanges();
            }
        }

        public void DeleteRepair(int repairId)
        {
            var repair = _context.Repairs
                .FirstOrDefault(rr => rr.RepairId == repairId);

            if (repair != null)
            {
                _context.Repairs.Remove(repair);
                _context.SaveChanges();
            }
        }

        public void DeleteTown(int townId)
        {
            var town = _context.Towns
                .FirstOrDefault(t => t.Id == townId);

            if (town != null)
            {
                _context.Towns.Remove(town);
                _context.SaveChanges();
            }
        }

        //public List<RepairguyPerformanceDto> GetAllRepairguysWithPerformance()
        //{
        //    var repairguys = _context.Repairguys
        //        .Include(r => r.Reservations)
        //        .Include(r => r.Reviews)
        //        .Select(r => new RepairguyPerformanceDto
        //        {
        //            RfirstName = r.RfirstName,
        //            RlastName = r.RlastName,
        //            Remail = r.Remail,
        //            AverageRating = r.Reviews.Any() ? r.Reviews.Average(rev => rev.Rating) : 0,
        //            TotalReservations = r.Reservations.Count,
        //            CompletedReservations = r.Reservations.Count(res => res.ResStatus == "Завършена")
        //        })
        //        .ToList();

        //    return repairguys;
        //}

        public List<RepairguyPerformanceDto> GetAllRepairguysWithPerformance(string? status, string? sortOption)
        {
            var query = _context.Repairguys
                .Include(r => r.Reservations)
                .Include(r => r.Reviews)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(r => r.Rstatus == status);
            }

            switch (sortOption)
            {
                case "ratingAsc":
                    query = query.OrderBy(r => r.Reviews.Any() ? r.Reviews.Average(rev => rev.Rating) : 0);
                    break;
                case "ratingDesc":
                    query = query.OrderByDescending(r => r.Reviews.Any() ? r.Reviews.Average(rev => rev.Rating) : 0);
                    break;
                case "reservationsAsc":
                    query = query.OrderBy(r => r.Reservations.Count);
                    break;
                case "reservationsDesc":
                    query = query.OrderByDescending(r => r.Reservations.Count);
                    break;
            }

            var repairguys = query
                .Select(r => new RepairguyPerformanceDto
                {
                    RepairguyId = r.RepairguyId,
                    RfirstName = r.RfirstName,
                    RlastName = r.RlastName,
                    Remail = r.Remail,
                    Rstatus = r.Rstatus,
                    AverageRating = r.Reviews.Any() ? r.Reviews.Average(rev => rev.Rating) : 0,
                    TotalReservations = r.Reservations.Count,
                    CompletedReservations = r.Reservations.Count(res => res.ResStatus == "Завършена")
                })
                .ToList();

            return repairguys;
        }

    }
}
