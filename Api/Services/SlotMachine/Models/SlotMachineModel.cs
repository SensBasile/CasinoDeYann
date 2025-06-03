using CasinoDeYann.Api.Controllers.SlotMachine.Responses;

namespace CasinoDeYann.Api.Services.SlotMachine.Models
{
    public class SlotMachineModel(int[][] grid, long money, string message)
    {
        public SlotMachineResponse ToResponse()
        {
            return new SlotMachineResponse(
                    grid,
                    money,
                    message
                );
        }
    }

}