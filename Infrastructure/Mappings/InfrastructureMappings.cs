using Application.Models;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Users;

namespace Infrastructure.Mappings;

public class InfrastructureMappings : Profile
{
    public InfrastructureMappings()
    {
        CreateMap<RegistrationRequest, MoodBoardUser>().ReverseMap();
        CreateMap<AuthenticationRequest, MoodBoardUser>().ReverseMap();
        
        CreateMap<MoodBoardUser, User>()
            .ForMember(u => u.IdentityId, 
                opt => opt.MapFrom(m => m.Id))
            .ReverseMap();
    }
}