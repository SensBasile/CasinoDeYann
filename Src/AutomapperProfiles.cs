using AutoMapper;
using CasinoDeYann.DataAccess.Dbo;
using CasinoDeYann.DataAccess.EFModels;

namespace CasinoDeYann;

public class AutomapperProfiles : Profile
{
    public AutomapperProfiles()
    {
        CreateMap<User, TUser>().ReverseMap();
        CreateMap<Stats, TStats>()
            .ForMember(dest => dest.User, opt => opt.Ignore()).ReverseMap();
    }
}
