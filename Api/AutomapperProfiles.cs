using AutoMapper;
using CasinoDeYann.Api.DataAccess.Dbo;
using CasinoDeYann.Api.DataAccess.EFModels;
using CasinoDeYann.Api.Services;

namespace CasinoDeYann.Api;

public class AutomapperProfiles : Profile
{
    public AutomapperProfiles()
    {
        CreateMap<User, TUser>().ReverseMap();
        CreateMap<Stats, TStats>().ReverseMap();
    }
}
