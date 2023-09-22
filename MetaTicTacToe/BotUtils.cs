using System.Text;
using System.Text.Json;

namespace MetaTicTacToe;

public static class BotUtils
{
    public static bool GetForced(Game game)
    {
        if (game.forcedSection.HasValue)
        {
            int index = game.forcedSection.Value;
            if (game.overview[index] == "")
            {
                return true;
            }
        }

        return false;
    }

    public static int GetRandomBoardSectionIndex(Game game, bool forced)
    {
        if (forced && game.overview[game.forcedSection!.Value] == "")
        {
            return game.forcedSection!.Value;
        }
        
        var freeIndexes = new List<int>();
        for (int i = 0; i < game.overview.Count; i++)
        {
            var symbol = game.overview[i];
            if (symbol == "")
            {
                freeIndexes.Add(i);
            }
        }

        var random = Random.Shared.Next(0, freeIndexes.Count);
        return freeIndexes[random];
    }
    
    public static int GetRandomBoardMoveIndex(Game game, int boardSectionIndex)
    {
        int moveIndex = 0;
        var chosenBoard = game.board[boardSectionIndex];
        var freeIndexes = new List<int>();
        for (int i = 0; i < chosenBoard.Count; i++)
        {
            var symbol = chosenBoard[i];
            if (symbol == "")
            {
                freeIndexes.Add(i);
            }
        }
        
        var random = Random.Shared.Next(0, freeIndexes.Count);
        moveIndex = freeIndexes[random];
        return moveIndex;
    }

    public static (string ourSymbol, string enemySymbol) GetPlayerSymbols(Game game)
    {
        var ourSymbol = game.players.First(x => x.id == game.self).symbol;
        var enemySymbol = game.players.First(x => x.id != game.self).symbol;
        return (ourSymbol, enemySymbol);
    }

    public static int[] FirstMove(Game game)
    {
        var forced = GetForced(game);
        var boardSectionIndex = GetRandomBoardSectionIndex(game, forced);
        var board = game.board[boardSectionIndex];
        var isFirstMove = board.All(x => x == "");
        if (!isFirstMove)
        {
            return Array.Empty<int>();
        }

        var indexes = new int[] {1, 3, 5, 7};
        var random = Random.Shared.Next(0, indexes.Length);
        var boardIndex = indexes[random];
        return new[] {boardSectionIndex, boardIndex};
    }

    public static int[] SecondMove(Game game)
    {
        var forced = GetForced(game);
        var boardSectionIndex = GetRandomBoardSectionIndex(game, forced);
        var board = game.board[boardSectionIndex];
        var Symbols = GetPlayerSymbols(game);
        var ourSymbol = Symbols.ourSymbol;

        if (board[1] == ourSymbol)
        {
            if (board[3] == ourSymbol)
            {
                if (board[0] == ourSymbol)
                {
                    return Array.Empty<int>();
                }

                return new[] {boardSectionIndex, 0};
            }

            if (board[5] == ourSymbol)
            {
                if (board[2] == ourSymbol)
                {
                    return Array.Empty<int>();
                }

                return new[] {boardSectionIndex, 2};
            }

            return new[] {boardSectionIndex, 5};
            return new[] {boardSectionIndex, 3};
        }

        if (board[3] == ourSymbol)
        {
            if (board[1] == ourSymbol)
            {
                if (board[0] == ourSymbol)
                {
                    return Array.Empty<int>();
                }

                return new[] {boardSectionIndex, 0};
            }

            if (board[7] == ourSymbol)
            {
                if (board[6] == ourSymbol)
                {
                    return Array.Empty<int>();
                }

                return new[] {boardSectionIndex, 6};
            }

            return new[] {boardSectionIndex, 1};
            return new[] {boardSectionIndex, 7};
        }

        if (board[5] == ourSymbol)
        {
            if (board[1] == ourSymbol)
            {
                if (board[2] == ourSymbol)
                {
                    return Array.Empty<int>();
                }

                return new[] {boardSectionIndex, 2};
            }

            if (board[7] == ourSymbol)
            {
                if (board[8] == ourSymbol)
                {
                    return Array.Empty<int>();
                }

                return new[] {boardSectionIndex, 8};
            }

            return new[] {boardSectionIndex, 1};
            return new[] {boardSectionIndex, 7};
        }

        if (board[7] == ourSymbol)
        {
            if (board[3] == ourSymbol)
            {
                if (board[6] == ourSymbol)
                {
                    return Array.Empty<int>();
                }

                return new[] {boardSectionIndex, 6};
            }

            if (board[5] == ourSymbol)
            {
                if (board[8] == ourSymbol)
                {
                    return Array.Empty<int>();
                }

                return new[] {boardSectionIndex, 8};
            }

            return new[] {boardSectionIndex, 5};
            return new[] {boardSectionIndex, 3};
        }




        return Array.Empty<int>();

    }

    public static void CheckIllegalMove(Game game, int[] move, string whereDoIComeFrom = "")
    {
        int idx1 = move[0];
        int idx2 = move[1];

        bool idx1Valid = game.overview[idx1] == "";
        bool idx2Valid = game.board[idx1][idx2] == "";

        if (!idx1Valid || !idx2Valid)
        {
            Console.WriteLine("Invalid move!" + whereDoIComeFrom);
        }
    }
    
    public static async Task LogResultAsync(Game game)
    {
        var player = game.players.First(x => x.id == game.self);
        bool win = player.score == 1;
        Console.WriteLine($"Won game? {win}");
        var exeDir = AppDomain.CurrentDomain.BaseDirectory;
        var logPath = Path.Combine(exeDir, "games");
        Directory.CreateDirectory(logPath);
        var json = JsonSerializer.Serialize(game);
        var filePath = Path.Combine(logPath, $"{game.id}.json");
        await File.WriteAllTextAsync(filePath, json, Encoding.UTF8);
    }

    public static int[] BlockOpponent(Game game)
    {
        var forced = GetForced(game);
        var playSection = GetRandomBoardSectionIndex(game, forced);
        var symbols = GetPlayerSymbols(game);

        var section = game.board[playSection];
        var row0 = new[] { (0, section[0]), (1, section[1]), (2, section[2]) };
        var row1 = new[] { (3, section[3]), (4, section[4]), (5, section[5]) };
        var row2 = new[] { (6, section[6]), (7, section[7]), (8, section[8]) };

        var col0 = new[] { (0, section[0]), (3, section[3]), (6, section[6]) };
        var col1 = new[] { (1, section[1]), (4, section[4]), (7, section[7]) };
        var col2 = new[] { (2, section[2]), (5, section[5]), (8, section[8]) };

        var dia0 = new[] { (0, section[0]), (4, section[4]), (8, section[8]) };
        var dia1 = new[] { (6, section[6]), (4, section[4]), (2, section[2]) };

        int move = GetCriticalMove(symbols.enemySymbol, row0, row1, row2, col0, col1, col2, dia0, dia1);

        if (move is -1)
            return Array.Empty<int>();

        return new[] { playSection, move };
    }
    
    public static int[] WinField(Game game)
    {
        var forced = GetForced(game);
        var playSection = GetRandomBoardSectionIndex(game, forced);
        var symbols = GetPlayerSymbols(game);

        var section = game.board[playSection];
        var row0 = new[] { (0, section[0]), (1, section[1]), (2, section[2]) };
        var row1 = new[] { (3, section[3]), (4, section[4]), (5, section[5]) };
        var row2 = new[] { (6, section[6]), (7, section[7]), (8, section[8]) };

        var col0 = new[] { (0, section[0]), (3, section[3]), (6, section[6]) };
        var col1 = new[] { (1, section[1]), (4, section[4]), (7, section[7]) };
        var col2 = new[] { (2, section[2]), (5, section[5]), (8, section[8]) };

        var dia0 = new[] { (0, section[0]), (4, section[4]), (8, section[8]) };
        var dia1 = new[] { (6, section[6]), (4, section[4]), (2, section[2]) };

        int move = GetCriticalMove(symbols.ourSymbol, row0, row1, row2, col0, col1, col2, dia0, dia1);

        if (move is -1)
            return Array.Empty<int>();

        return new[] { playSection, move };
    }
    
    private static int GetCriticalMove(string importantSymbol, params (int, string)[][] lines)
    {
        for (var index = 0; index < lines.Length; index++)
        {
            var line = lines[index];
            var opCount = line.Count(x => x.Item2 == importantSymbol);
            if (opCount < 2)
                continue;

            for (var i = 0; i < line.Length; i++)
            {
                var pos = line[i];
                var symbol = pos.Item2;
                if (string.IsNullOrWhiteSpace(symbol))
                    return pos.Item1;
            }
        }

        return -1;
    }
}