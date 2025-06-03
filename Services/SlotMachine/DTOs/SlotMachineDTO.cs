using CasinoDeYann.Controllers.SlotMachine.Responses;

namespace CasinoDeYann.Services.SlotMachine.DTOs
{
    public class SlotMachineDTO(int[][] grid, long money, string message)
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