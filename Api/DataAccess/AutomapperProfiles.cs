using AutoMapper;
using CasinoDeYann.Api.DataAccess.EFModels;

namespace CasinoDeYann.Api.DataAccess;

public class AutomapperProfiles : Profile
{
    public AutomapperProfiles()
    {
        CreateMap<Dbo.User, TUser>().ReverseMap();
    }
}
