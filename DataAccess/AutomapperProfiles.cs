using AutoMapper;


namespace CasinoDeYann.DataAccess;

public class AutomapperProfiles : Profile
{
    public AutomapperProfiles()
    {
        CreateMap<Dbo.User, EfModels.TUser>().ReverseMap();
    }
}
