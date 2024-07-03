using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RepairPlatform.Entities;
using RepairPlatform.Services.DTO.Clients;
using RepairPlatform.Services.DTO.Reservations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RepairPlatform.Services
{
    public class ClientsService
    {
        private readonly Repairguy20118046Context _dbContext;
        private readonly IMapper _mapper;
        private readonly UserManager<AspNetUsers> _userManager;
        private readonly SignInManager<AspNetUsers> _signInManager;

        public ClientsService(Repairguy20118046Context dbContext, IMapper mapper, UserManager<AspNetUsers> userManager, SignInManager<AspNetUsers> signInManager)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<List<ClientDto>> GetAll()
        {
            var clients = await _dbContext.Clients.ToListAsync();
            return _mapper.Map<List<ClientDto>>(clients);
        }

        public async Task<ClientDto> GetById(int clientId)
        {
            var client = await _dbContext.Clients.FindAsync(clientId);
            if (client == null)
            {
                return null!;
            }
            return _mapper.Map<ClientDto>(client);
        }

        public async Task<ClientDto> Create(CreateClientDto clientDto)
        {
            var client = _mapper.Map<Client>(clientDto);

            var appUser = new AspNetUsers { UserName = client.Cemail, Email = client.Cemail };
            var result = await _userManager.CreateAsync(appUser, client.Cpassword);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(appUser, "client");
                client.UserId = appUser.Id;

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, appUser.Id),
                    new Claim(ClaimTypes.Name, appUser.UserName),
                    new Claim(ClaimTypes.Email, appUser.Email),
                    new Claim(ClaimTypes.Role, "client")
                };
                var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
                var principal = new ClaimsPrincipal(identity);
                await _signInManager.SignInWithClaimsAsync(appUser, isPersistent: false, claims);       
            }

            await _dbContext.Clients.AddAsync(client);
            await _dbContext.SaveChangesAsync();
            return _mapper.Map<ClientDto>(client);
        }

        public async Task<ClientDto> Update(int id, ClientDto clientDto)
        {
            var client = await _dbContext.Clients.FindAsync(id);
            if (client == null)
            {
                return null!;
            }
            _mapper.Map(clientDto, client);
            _dbContext.Entry(client).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return _mapper.Map<ClientDto>(client);
        }

        public async Task<bool> Delete(int id)
        {
            var client = await _dbContext.Clients.FindAsync(id);
            if (client == null)
            {
                return false;
            }
            _dbContext.Clients.Remove(client);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<ClientDto> AuthenticateClientAsync(string email, string userId)
        {
            var client = await _dbContext.Clients
                .FirstOrDefaultAsync(c => c.Cemail == email && c.UserId == userId);

            if (client == null)
            {
                return null!;
            }

            return _mapper.Map<ClientDto>(client);
        }

        // New method to get client by email or phone
        public async Task<ClientDto?> GetByEmailOrPhone(string email, string phone)
        {
            var client = await _dbContext.Clients
                .FirstOrDefaultAsync(c => c.Cemail == email || c.Ctelephone == phone);

            if (client == null)
            {
                return null!;
            }

            return _mapper.Map<ClientDto>(client);
        }

        public async Task<ClientDto?> GetByUserId(string userId)
        {
            //var client = await _dbContext.Clients.FirstOrDefaultAsync(c => c.UserId == userId );
            //if (client == null)
            //{
            //    return null!;
            //}
            //return _mapper.Map<ClientDto>(client);

            var client = await _dbContext.Clients
                            .Include(c => c.Reservations)
                            .ThenInclude(r => r.Group)
                                .Include(c => c.Reservations)
                                    .ThenInclude(r => r.Repairguy)
                                        .ThenInclude(rg => rg.Town)
                            .FirstOrDefaultAsync(c => c.UserId == userId);

            if (client == null)
            {
                return null!;
            }

            var clientDto = _mapper.Map<ClientDto>(client);

            clientDto.Reservations = client.Reservations.Select(r => new Reservation
            {
                ReservationId = r.ReservationId,
                ResName = r.ResName,
                Group = r.Group,
                ResDateTime = r.ResDateTime,
                ResLocation = r.ResLocation,
                ResComment = r.ResComment,
                ResStatus = r.ResStatus,
                Repairguy = r.Repairguy 
            }).ToList();

            return clientDto;
        }
    }
}
