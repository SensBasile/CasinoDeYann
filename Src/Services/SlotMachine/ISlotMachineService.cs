using CasinoDeYann.Services.SlotMachine.Models;

namespace CasinoDeYann.Services.SlotMachine;

public interface ISlotMachineService
{
    public Task<SlotMachineModel> Play(string userName, int bet);
}