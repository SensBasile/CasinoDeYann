namespace CasinoDeYann.Controllers.HorseRace.DTOs;

public record HorseRaceResponse(int[] Results, Pace[][] Speeds, long Gain, long Money, string Message);