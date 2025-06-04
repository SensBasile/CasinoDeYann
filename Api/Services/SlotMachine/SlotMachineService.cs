using CasinoDeYann.Api.DataAccess.Interfaces;
using CasinoDeYann.Api.Services.SlotMachine.Models;
using CasinoDeYann.Api.Services.Stats;

namespace CasinoDeYann.Api.Services.SlotMachine;

public class SlotMachineService(IUsersRepository usersRepository, StatsService statsService)
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
        var callingUser = await usersRepository.GetOneByName(userName);
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
        
        await statsService.Create(new GameHistoryEntryModel(callingUser.Id, DateTime.Now, "Slot Machine", bet, gain));
        
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

    private float CheckAlignDiagDown(int[][] grid, int startingRow, bool[][] patterns)
    {
        int row = startingRow;
        int col = 0;
        int diagSymbol = grid[row][col];
        int acc = 1;

        // On parcourt (r,c) = (starting_row + k, 0 + k) tant que c < W et r < H
        for (int k = 1; k < W; k++)
        {
            int r = startingRow + k;
            int c = k;
            if (r >= H) break;

            if (grid[r][c] != diagSymbol && grid[r][c] != WildIndex)
            {
                if (diagSymbol != WildIndex) break;
                diagSymbol = grid[r][c];
            }
            acc++;
        }

        if (acc < MinAlign) return 0f;

        // On marque les acc premières cases de la diagonale
        for (int k = 0; k < acc; k++)
        {
            int r = startingRow + k;
            int c = k;
            patterns[r][c] = true;
        }
        return _alignCoeff[acc - MinAlign] * _symbolsCoeff[diagSymbol];
    }

    private float CheckAlignDiagUp(int[][] grid, int startingRow, bool[][] patterns)
    {
        int row = startingRow;
        int col = 0;
        int diagSymbol = grid[row][col];
        int acc = 1;

        // On parcourt (r,c) = (starting_row - k, 0 + k) tant que c < W et r >= 0
        for (int k = 1; k < W; k++)
        {
            int r = startingRow - k;
            int c = k;
            if (r < 0) break;

            if (grid[r][c] != diagSymbol && grid[r][c] != WildIndex)
            {
                if (diagSymbol != WildIndex) break;
                diagSymbol = grid[r][c];
            }
            acc++;
        }

        if (acc < MinAlign) return 0f;

        // On marque les acc premières cases de la diagonale “vers le haut”
        for (int k = 0; k < acc; k++)
        {
            int r = startingRow - k;
            int c = k;
            patterns[r][c] = true;
        }
        return _alignCoeff[acc - MinAlign] * _symbolsCoeff[diagSymbol];
    }

    private float CheckVDown(int[][] grid, int startingRow, bool[][] patterns)
    {
        // on ne peut tracer une branche V de taille “3” vers le bas que si starting_row + 2 < H
        if (startingRow + (MinAlign - 1) > H - 1) 
            return 0f;
        
        // Définition du chemin : pour col = 0..4, row = starting_row + (col <= 2 ? col : 4 - col)
        int row0 = startingRow;
        int colSymbol = grid[row0][0];
        int acc = 1;

        for (int c = 1; c < W; c++)
        {
            int delta = c <= 2 ? c : (W - 1 - c);
            int r = startingRow + delta;
            if (r < 0 || r >= H) break; // en principe non nécessaire grâce à la condition ci-dessus

            if (grid[r][c] != colSymbol && grid[r][c] != WildIndex)
            {
                if (colSymbol != WildIndex) break;
                colSymbol = grid[r][c];
            }
            acc++;
        }

        if (acc < MinAlign) return 0f;

        // On marque les acc premières positions du “V” depuis l’extrémité gauche
        for (int c = 0; c < acc; c++)
        {
            int delta = c <= 2 ? c : (W - 1 - c);
            int r = startingRow + delta;
            patterns[r][c] = true;
        }
        return _vCoeff * _symbolsCoeff[colSymbol];
    }

    private float CheckVUp(int[][] grid, int startingRow, bool[][] patterns)
    {
        // on ne peut tracer une branche V vers le haut que si starting_row - (MinAlign - 1) >= 0
        if (startingRow - (MinAlign - 1) < 0)
            return 0f;
        
        int row0 = startingRow;
        int colSymbol = grid[row0][0];
        int acc = 1;

        for (int c = 1; c < W; c++)
        {
            int delta = c <= 2 ? c : (W - 1 - c);
            int r = startingRow - delta;
            if (r < 0 || r >= H) break;

            if (grid[r][c] != colSymbol && grid[r][c] != WildIndex)
            {
                if (colSymbol != WildIndex) break;
                colSymbol = grid[r][c];
            }
            acc++;
        }

        if (acc < MinAlign) return 0f;

        for (int c = 0; c < acc; c++)
        {
            int delta = c <= 2 ? c : (W - 1 - c);
            int r = startingRow - delta;
            patterns[r][c] = true;
        }
        return _vCoeff * _symbolsCoeff[colSymbol];
    }

    private float CheckMUp(int[][] grid, int startingRow, bool[][] patterns)
    {
        // Le “creux” de l’M se trouve à starting_row + 2, il faut starting_row + 2 < H
        if (startingRow + 2 >= H) 
            return 0f;

        int row0 = startingRow;
        int colSymbol = grid[row0][0];
        int acc = 1;

        // Pour chaque colonne c = 1..4, on détermine r selon c % 2
        for (int c = 1; c < W; c++)
        {
            int r = (c % 2 == 1) 
                    ? (startingRow + 2)   // pour les colonnes impaires : le “creux”
                    : startingRow;        // pour les colonnes paires : la “crête”
            if (r < 0 || r >= H) break;

            if (grid[r][c] != colSymbol && grid[r][c] != WildIndex)
            {
                if (colSymbol != WildIndex) break;
                colSymbol = grid[r][c];
            }
            acc++;
        }

        if (acc < MinAlign) return 0f;

        // On marque les acc premières positions de la forme MUp
        for (int c = 0; c < acc; c++)
        {
            int r = (c % 2 == 1) 
                    ? (startingRow + 2) 
                    : startingRow;
            patterns[r][c] = true;
        }

        // acc vaut 3, 4 ou 5 → on indexe mCoeff[acc - MinAlign]
        return _mCoeff[acc - MinAlign] * _symbolsCoeff[colSymbol];
    }

    private float CheckMDown(int[][] grid, int startingRow, bool[][] patterns)
    {
        // Le “sommet” de l’M inversé se trouve à starting_row - 2
        if (startingRow - 2 < 0) 
            return 0f;

        int row0 = startingRow;
        int colSymbol = grid[row0][0];
        int acc = 1;

        for (int c = 1; c < W; c++)
        {
            int r = (c % 2 == 1) 
                    ? (startingRow - 2)  // pour les colonnes impaires : le “sommet” inversé
                    : startingRow;       // pour les colonnes paires : la “base”
            if (r < 0 || r >= H) break;

            if (grid[r][c] != colSymbol && grid[r][c] != WildIndex)
            {
                if (colSymbol != WildIndex) break;
                colSymbol = grid[r][c];
            }
            acc++;
        }

        if (acc < MinAlign) return 0f;

        for (int c = 0; c < acc; c++)
        {
            int r = (c % 2 == 1) 
                    ? (startingRow - 2) 
                    : startingRow;
            patterns[r][c] = true;
        }
        return _mCoeff[acc - MinAlign] * _symbolsCoeff[colSymbol];
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
            sum += (long) Math.Floor(bet * CheckVDown(grid, i, patterns));
            sum += (long) Math.Floor(bet * CheckAlignDiagDown(grid, i, patterns));
            sum += (long) Math.Floor(bet * CheckVUp(grid, i, patterns));
            sum += (long) Math.Floor(bet * CheckAlignDiagUp(grid, H - 1 - i, patterns));
        }

        for (int i = 0; i + 1 < H; i++)
        {
            sum += (long) Math.Floor(bet * CheckMUp(grid, i, patterns));
            sum += (long) Math.Floor(bet * CheckMDown(grid, H - 1 - i, patterns));
        }
        return sum;
    }
}