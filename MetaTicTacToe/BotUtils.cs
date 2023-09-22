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
        if (forced)
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
        for (int i = 0; i < chosenBoard.Count; i++)
        {
            var freeIndexes = new List<int>();
            var symbol = chosenBoard[i];
            if (symbol == "")
            {
                freeIndexes.Add(i);
            }
            
            var random = Random.Shared.Next(0, freeIndexes.Count);
            moveIndex = freeIndexes[random];
        }

        return moveIndex;
    }
    
    public static void CheckIllegalMove(Game game, int[] move)
    {
        int idx1 = move[0];
        int idx2 = move[1];

        bool idx1Valid = game.overview[idx1] == "";
        bool idx2Valid = game.board[idx1][idx2] == "";

        if (!idx1Valid || !idx2Valid)
        {
            Console.WriteLine("Invalid move!");
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
}