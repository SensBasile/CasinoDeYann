using CasinoDeYann.Src.Controllers.SlotMachine.Responses;

namespace CasinoDeYann.Src.Services.SlotMachine.Models
{
    public class SlotMachineModel(int[][] grid, bool[][] patterns, long gain, long money, string message)
    {
        public SlotMachineResponse ToResponse()
        {
            return new SlotMachineResponse(
                    grid,
                    patterns,
                    gain,
                    money,
                    message
                );
        }
    }

}