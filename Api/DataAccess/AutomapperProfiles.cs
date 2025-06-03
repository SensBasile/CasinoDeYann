using AutoMapper;
using CasinoDeYann.Api.DataAccess.Dbo;
using CasinoDeYann.Api.DataAccess.EFModels;

namespace CasinoDeYann.Api.DataAccess;

public class AutomapperProfiles : Profile
{
    public AutomapperProfiles()
    {
        CreateMap<User, TUser>().ReverseMap();
        CreateMap<Stats, TStats>().ReverseMap();
    }
}
