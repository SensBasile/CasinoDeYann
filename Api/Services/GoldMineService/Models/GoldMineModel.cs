using CasinoDeYann.Api.Controllers.GoldMine.Responses;

namespace CasinoDeYann.Api.Services.GoldMineService.Models
{
    public class GoldMineModel(bool credited)
    {
        public GoldMineResponse ToResponse()
        {
            return new GoldMineResponse(
                credited
            );
        }
    }

}