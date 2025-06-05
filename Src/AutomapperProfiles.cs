using AutoMapper;
using CasinoDeYann.Src.DataAccess.Dbo;
using CasinoDeYann.Src.DataAccess.EFModels;

namespace CasinoDeYann.Src;

public class AutomapperProfiles : Profile
{
    public AutomapperProfiles()
    {
        CreateMap<User, TUser>().ReverseMap();
        CreateMap<Stats, TStats>()
            .ForMember(dest => dest.User, opt => opt.Ignore()).ReverseMap();
    }
}
