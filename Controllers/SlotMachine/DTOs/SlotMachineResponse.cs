namespace CasinoDeYann.Controllers.SlotMachine.DTOs;

public record SlotMachineResponse(int[][] Grid, long Gain, int[] Patterns, string Message);