using CasinoDeYann.Controllers.GoldMine.Responses;

namespace CasinoDeYann.Services.GoldMineService.Models
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