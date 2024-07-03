using AutoMapper;
using RepairPlatform.Entities;
using RepairPlatform.Services.DTO.Admins;
using RepairPlatform.Services.DTO.Clients;
using RepairPlatform.Services.DTO.Groups;
using RepairPlatform.Services.DTO.Repairguys;
using RepairPlatform.Services.DTO.Repairs;
using RepairPlatform.Services.DTO.Reservations;
using RepairPlatform.Services.DTO.Reviews;
using RepairPlatform.Services.DTO.Towns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairPlatform.Services.Mapping
{
    public class MappingProfile : Profile 
    {
        public MappingProfile() 
        {
            CreateMap<Repairguy, RepairguyDto>().ReverseMap();
            
            CreateMap<Repairguy, RepairguySummaryDto>();
            
            CreateMap<Repair, RepairDto>();
            CreateMap<RepairDto, Repair>().ReverseMap();
            
            CreateMap<Group, GroupDto>();
            CreateMap<GroupDto, Group>().ReverseMap();
           
            CreateMap<Reservation, ReservationDto>().ReverseMap();
           
            CreateMap<Client, ClientDto>().ReverseMap();
            
            CreateMap<Review, ReviewDto>();
            
            CreateMap<Town, TownDto>();
            CreateMap<TownDto, Town>().ReverseMap();

            CreateMap<CreateRepairguyDto, Repairguy>()
            .ForMember(dest => dest.Reservations, opt => opt.Ignore())
            .ForMember(dest => dest.Reviews, opt => opt.Ignore());
            
            CreateMap<CreateClientDto, Client>()
.           ForMember(dest => dest.Reservations, opt => opt.Ignore())
            .ForMember(dest => dest.Reviews, opt => opt.Ignore());

            CreateMap<CreateRepairDto, Repair>()
            .ForMember(dest => dest.Repairguys, opt => opt.Ignore());

            CreateMap<CreateGroupDto, Group>()
            .ForMember(dest => dest.Reservations, opt => opt.Ignore())
            .ForMember(dest => dest.Reviews, opt => opt.Ignore());

            CreateMap<CreateReviewDto, Review>();

            CreateMap<CreateAdministratorDto, Administrator>();
        }
    }
}
