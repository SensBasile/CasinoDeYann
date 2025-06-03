using CasinoDeYann.Api.DataAccess.Interfaces;
using CasinoDeYann.Api.Services.SlotMachine.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CasinoDeYann.Api.Services.SlotMachine;

public class SlotMachineService(IUsersRepository usersRepository)
{
    private const int W = 5;
    private const int H = 5;
    private const int SymbolsNumber = 9;
    private const int MinAlign = 3;

    private const int WildIndex = 7;

    private readonly float[] _alignCoeff = [2f, 3f, 4f]; // 3 aligned, 4 aligned, 5 aligned
    private readonly float _vCoeff = 5f;
    private readonly float[] _mCoeff = [2f, 3.5f, 5f];
    
    private readonly float[] _symbolsCoeff = [
        1f, // bell
        2f, // cherry
        5f, // diamond
        3f, // hearth
        4f, // horseshoe
        7f, // seven
        2.5f, // watermelon
        10f, // wildcard
        15f, // yann
    ];

    private readonly Random _random = new();

    public async Task<SlotMachineModel> Play(string userName, int bet)
    {
        var callingUser = usersRepository.GetOneByName(userName);
        if (callingUser.Money < bet) throw new BadHttpRequestException("You don't have enough money");
        callingUser = await usersRepository.AddMoney(callingUser.Username, -bet);
        
        var grid = new List<int[]>();
        
        for (int i = 0; i < H; i++)
        {
            var row = new List<int>();
            for (int j = 0; j < W; j++)
            {
                row.Add(_random.Next(SymbolsNumber));
            }
            grid.Add(row.ToArray());
        }
        
        var patterns = Enumerable.Range(0, H).Select(_ => new bool[W]).ToArray();
        var gain = ComputeGain(grid.ToArray(), bet, patterns);
        
        callingUser = await usersRepository.AddMoney(callingUser.Username, gain);
        
        return new SlotMachineModel(
            grid.ToArray(),
            patterns,
            gain,
            callingUser.Money,
            gain > 0 ? "Bravo vous avez gagné !!!" : "Retentez votre chance"
            );
    }

    private float CheckAlignRow(int[] row, bool[] patterns)
    {
        int rowSymbol = row[0];
        int acc = 1;
        for (int i = 1; i < W; i++)
        {
            if (row[i] != rowSymbol && row[i] != WildIndex)
            {
                if (rowSymbol != WildIndex) break;
                rowSymbol = row[i];
            }
            acc++;
        }

        if (acc < MinAlign) return 0f;

        for (int i = 0; i < acc; i++) patterns[i] = true;
        return _alignCoeff[acc - MinAlign] * _symbolsCoeff[rowSymbol]; 
    }

    private float CheckAlignCol(int[][] grid, int col, bool[][] patterns)
    {
        int colSymbol = grid[0][col];
        int acc = 1;
        for (int i = 1; i < H; i++)
        {
            if (grid[i][col] != colSymbol && grid[i][col] != WildIndex)
            {
                if (colSymbol != WildIndex) break;
                colSymbol = grid[i][col];
            }
            acc++;
        }
        
        if (acc < MinAlign) return 0f;
        
        for (int i = 0; i < acc; i++) patterns[i][col] = true;
        return _alignCoeff[acc - MinAlign] * _symbolsCoeff[grid[0][col]];
    }

    private float CheckAlignDiagUp(int[][] grid, int starting_row, bool[][] patterns)
    {
        return 0f; // TODO
    }
    
    private float CheckAlignDiagDown(int[][] grid, int starting_row, bool[][] patterns)
    {
        return 0f; // TODO
    }

    private long ComputeGain(int[][] grid, long bet, bool[][] patterns)
    {
        long sum = 0;
        for (int i = 0; i < H; i++)
        {
            sum += (long) Math.Floor(bet * CheckAlignRow(grid[i], patterns[i]));
        }

        for (int i = 0; i < W; i++)
        {
            sum += (long) Math.Floor(bet * CheckAlignCol(grid, i, patterns));
        }

        for (int i = 0; H - (i + 1) >= MinAlign; i++)
        {
            sum += (long) Math.Floor(bet * CheckAlignDiagDown(grid, i, patterns));
            sum += (long) Math.Floor(bet * CheckAlignDiagUp(grid, H - 1 - i, patterns));
        }
        return sum;
    }
}