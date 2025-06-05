using CasinoDeYann.Src.Controllers.GoldMine.Responses;

namespace CasinoDeYann.Src.Services.GoldMineService.Models
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