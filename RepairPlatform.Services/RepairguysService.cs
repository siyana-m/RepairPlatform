using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RepairPlatform.Entities;
using RepairPlatform.Services.DTO.Clients;
using RepairPlatform.Services.DTO.Repairguys;
using RepairPlatform.Services.DTO.Repairs;
using RepairPlatform.Services.DTO.Reservations;
using RepairPlatform.Services.DTO.Reviews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace RepairPlatform.Services
{
    public class RepairguysService
    {
        private readonly Repairguy20118046Context _dbContext;

        private readonly IMapper _mapper;

        private readonly UserManager<AspNetUsers> _userManager;
        private readonly SignInManager<AspNetUsers> _signInManager;

        public RepairguysService(Repairguy20118046Context dbContext, IMapper mapper, UserManager<AspNetUsers> userManager, SignInManager<AspNetUsers> signInManager)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<List<RepairguyDto>> GetAll()
        {
            var repairguys = await _dbContext.Repairguys
            .Include(r => r.Reservations)
            .Include(r => r.Reviews)
            .Include(r => r.Repairs)
            .Include(r => r.Town)
            .ToListAsync();
                    
            return _mapper.Map <List<RepairguyDto>>(repairguys);

        }

        public async Task<List<RepairguyDto>> Search(string searchTerm)
        {
            var repairguys = await _dbContext.Repairguys
            .Include(r => r.Reservations)
            .Include(r => r.Reviews)
            .Include(r => r.Repairs)
            .Include(r => r.Town)
            .Where(r => r.RfirstName.Contains(searchTerm) || r.RlastName.Contains(searchTerm))
            .ToListAsync();

            return _mapper.Map<List<RepairguyDto>>(repairguys);
        }


        public async Task<RepairguyDto> GetById(int repairguyId)
        {
            var repairguy = await _dbContext.Repairguys
            .Include(r => r.Reservations)
            .Include(r => r.Reviews)
            .Include(r => r.Repairs).ThenInclude(r => r.Groups)
             .Include(r => r.Town)
            .FirstOrDefaultAsync(r => r.RepairguyId == repairguyId);

            if (repairguy == null)
            {
                return null!;
            }

            repairguy.Groups = await GetRepairGroups(repairguyId);
            return _mapper.Map<RepairguyDto>(repairguy);

        }


        public async Task<List<string>> GetRepairGroups(int repairguyId)
        {
            var groups = await _dbContext.Groups.FromSqlRaw(@"
                SELECT DISTINCT g.CatName 
                FROM [20118046].[RepairguyRepairs] rgr
                JOIN [20118046].[RepairGroups] rg ON rgr.RepairId = rg.RepairId
                JOIN [20118046].[Group] g ON rg.GroupId = g.GroupId
                WHERE rgr.RepairguyId = {0}", repairguyId)
                .Select(g => g.CatName)
                .ToListAsync();

            return groups;
        }


       public  async Task<IdentityResult> Create(CreateRepairguyDto repairguyDto)
        {
            var repairguy = _mapper.Map<Repairguy>(repairguyDto);
            //repairguy.Repairs = _dbContext.Repairs.Where(g => repairguyDto!.Repairs!.Contains(g.RepairId)).ToList();

            var appUser = new AspNetUsers { UserName = repairguy.Remail, Email = repairguy.Remail };
            var result = await _userManager.CreateAsync(appUser, repairguy.Rpassword);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(appUser, "repairguy");
                repairguy.UserId = appUser.Id;
                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, appUser.Id),
                    new(ClaimTypes.Name, appUser.UserName),
                    new(ClaimTypes.Email, appUser.Email),
                    new(ClaimTypes.Role, "repairguy")
                };
                var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
                var principal = new ClaimsPrincipal(identity);
                await _signInManager.SignInWithClaimsAsync(appUser, isPersistent: false, claims);


                await _dbContext.Repairguys.AddAsync(repairguy);
                await _dbContext.SaveChangesAsync();
               // return _mapper.Map<RepairguyDto>(repairguy);
            }

            return result; 
        }
        public async Task<RepairguyDto> Update(int id, RepairguyDto repairguyDto)
        {
            var repairguy = await _dbContext.Repairguys.FindAsync(id);
            if (repairguy == null)
            {
                return null!;
            }
            _mapper.Map(repairguyDto, repairguy);
            _dbContext.Entry(repairguy).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return _mapper.Map<RepairguyDto>(repairguy);
        }
        public async Task<RepairguyDto> UpdatePhoto(int id, byte[] photoData)
        {
            var repairguy = await _dbContext.Repairguys.FindAsync(id);
            if (repairguy == null)
            {
                return null!;
            }
            repairguy.Rphoto = photoData;
            await _dbContext.SaveChangesAsync();
            return _mapper.Map<RepairguyDto>(repairguy);
        }
        public async Task<bool> Delete(int id)
        {
            var repairguy = await _dbContext.Repairguys.FindAsync(id);
            if (repairguy == null)
            {
                return false;
            }
            _dbContext.Repairguys.Remove(repairguy);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<ReservationDto>> GetRepairguyReservations(int repairguyId)
        {
            var repairguy = await _dbContext.Repairguys
            .Include(r => r.Reservations)
            .FirstOrDefaultAsync(b => b.RepairguyId == repairguyId);
            if (repairguy == null)
            {
                return null!;
            }
            var orders = repairguy.Reservations.ToList();
            return _mapper.Map<List<ReservationDto>>(orders);
        }

        public async Task<RepairguyDto> AuthenticateRepairguyAsync(string email, string userId)
        {
            var repairguy = await _dbContext.Repairguys
                .FirstOrDefaultAsync(c => c.Remail == email && c.UserId == userId);

            if (repairguy == null)
            {
                return null!;
            }

            return _mapper.Map<RepairguyDto>(repairguy);
        }

        public async Task<RepairguyDto?> GetByEmail(string email)
        {
            var repairguy = await _dbContext.Repairguys
                .FirstOrDefaultAsync(r => r.Remail == email);

            if (repairguy == null)
            {
                return null!;
            }

            return _mapper.Map<RepairguyDto>(repairguy);
        }

        public async Task<RepairguyDto?> GetByUserId(string userId)
        {
            var repairguy = await _dbContext.Repairguys
                            .Include(r => r.Reservations).ThenInclude(reservation => reservation.Client)
                            .Include(r => r.Reservations).ThenInclude(reservation => reservation.Group)
                            .Include(r => r.Reviews)
                            .Include(r => r.Repairs)
                            .Include(r => r.Town)
                            .FirstOrDefaultAsync(c => c.UserId == userId);

            if (repairguy == null)
            {
                return null!;
            }
            return _mapper.Map<RepairguyDto>(repairguy);
        }

        public async Task<List<RepairguyDto>> GetActiveRepairguysAsync()
        {
            var activeRepairguys = await _dbContext.Repairguys
                .Where(r => r.Rstatus == "Active")
                .ToListAsync();

            return activeRepairguys.Select(r => new RepairguyDto
            {
                RepairguyId = r.RepairguyId,
                RfirstName = r.RfirstName,
                RlastName = r.RlastName,
                Rstatus = r.Rstatus,
                Rdescription = r.Rdescription
            }).ToList();
        }


        public async Task<List<Repairguy>> GetRepairguysWithGroupsAsync()
        {
            var repairguys = await _dbContext.Repairguys.ToListAsync();
            return repairguys;
        }
    }
}
