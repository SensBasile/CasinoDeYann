using CasinoDeYann.Controllers.SlotMachine.Responses;

namespace CasinoDeYann.Services.SlotMachine.Models
{
    public class SlotMachineModel
    {
        public int[][] Grid { get; }
        public bool[][] Patterns { get; }
        public long Gain { get; }
        public long UserMoney { get; }
        public string Message { get; }

        public SlotMachineModel(int[][] grid, bool[][] patterns, long gain, long money, string message)
        {
            Grid = grid;
            Patterns = patterns;
            Gain = gain;
            UserMoney = money;
            Message = message;
        }

        public SlotMachineResponse ToResponse()
        {
            return new SlotMachineResponse(
                Grid,
                Patterns,
                Gain,
                UserMoney,
                Message
            );
        }
    }
}