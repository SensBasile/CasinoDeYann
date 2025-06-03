namespace CasinoDeYann.Controllers.SlotMachine.DTOs;

public record SlotMachineResponse(int[][] Grid, long Money, int[] Patterns, string Message);