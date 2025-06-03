namespace CasinoDeYann.Api.Controllers.SlotMachine.Responses;

public record SlotMachineResponse(int[][] Grid, bool[][] Patterns, long Gain, long Money, string Message);