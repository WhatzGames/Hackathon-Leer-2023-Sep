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

    public static List<WeightedMove> GetWeightedSectionMoves(Game game, int boardSectionIndex)
    {
        var board = game.board[boardSectionIndex];

        // Get all possible winning triplets.
        var row0 = new[] { (0, board[0]), (1, board[1]), (2, board[2]) };
        var row1 = new[] { (3, board[3]), (4, board[4]), (5, board[5]) };
        var row2 = new[] { (6, board[6]), (7, board[7]), (8, board[8]) };

        var col0 = new[] { (0, board[0]), (3, board[3]), (6, board[6]) };
        var col1 = new[] { (1, board[1]), (4, board[4]), (7, board[7]) };
        var col2 = new[] { (2, board[2]), (5, board[5]), (8, board[8]) };

        var dia0 = new[] { (0, board[0]), (4, board[4]), (8, board[8]) };
        var dia1 = new[] { (6, board[6]), (4, board[4]), (2, board[2]) };

        var permutations = new (int index, string symbol)[][]
        {
            row0, row1, row2, col0, col1, col2, dia0, dia1
        };

        var symbols = GetPlayerSymbols(game);
        
        int[] weights = 
          { 0, 0, 0, 
            0, 0, 0,
            0, 0, 0 };

        foreach (var triplet in permutations)
        {
            // triplet is already full
            if (triplet.All(x => x.symbol != ""))
            {
                continue;
            }
            
            // Can't win triplet anymore
            if (triplet.Any(x => x.symbol == symbols.enemySymbol))
            {
                continue;
            }

            // Empty triplet does not lead to winning move on next turn
            if (triplet.All(x => x.symbol == ""))
            {
                continue;
            }

            // Now we have one of our symbols and two empty ones.
            var empty = triplet.Where(x => x.symbol == "");
            foreach (var e in empty)
            {
                
                weights[e.index] += 10;

                // Corners are always better so we increase the weight a bit
                if (e.index is 0 or 2 or 6 or 8)
                {
                    weights[e.index] += 1;
                }
                
                // The middle is worth even more than corners 
                if (e.index is 4)
                {
                    weights[e.index] += 100;
                }
            }
        }
        
        // This should mark all already set fields as invalid
        for (var i = 0; i < board.Count; i++)
        {
            var symbol = board[i];
            if (symbol != "")
            {
                weights[i] = -1;
            }
        }

        var moves = new List<WeightedMove>();
        for (int i = 0; i < weights.Length; i++ )
        {
            var weight = weights[i];
            
            // Do not return impossible moves 
            if (weight == -1)
            {
                continue;
            }
            
            var move = new []{boardSectionIndex, i};
            moves.Add(new WeightedMove(move, weight));
        }
        
        // 
        
        var orderedMoves = moves.OrderByDescending(x => x.Weight).ToList();
        return orderedMoves;
    }
    
    public static List<WeightedBoard> GetWeightedMetaBoards(Game game)
    {
        var board = game.overview;

        // Get all possible winning triplets.
        var row0 = new[] { (0, board[0]), (1, board[1]), (2, board[2]) };
        var row1 = new[] { (3, board[3]), (4, board[4]), (5, board[5]) };
        var row2 = new[] { (6, board[6]), (7, board[7]), (8, board[8]) };

        var col0 = new[] { (0, board[0]), (3, board[3]), (6, board[6]) };
        var col1 = new[] { (1, board[1]), (4, board[4]), (7, board[7]) };
        var col2 = new[] { (2, board[2]), (5, board[5]), (8, board[8]) };

        var dia0 = new[] { (0, board[0]), (4, board[4]), (8, board[8]) };
        var dia1 = new[] { (6, board[6]), (4, board[4]), (2, board[2]) };

        var permutations = new (int index, string symbol)[][]
        {
            row0, row1, row2, col0, col1, col2, dia0, dia1
        };

        var symbols = GetPlayerSymbols(game);
        
        int[] weights = 
          { 0, 0, 0, 
            0, 0, 0,
            0, 0, 0 };

        foreach (var triplet in permutations)
        {
            // triplet is already full
            if (triplet.All(x => x.symbol != ""))
            {
                continue;
            }
            
            // Can't win triplet anymore
            if (triplet.Any(x => x.symbol == symbols.enemySymbol || x.symbol == "-"))
            {
                continue;
            }

            // Empty triplet does not lead to winning move on next turn
            if (triplet.All(x => x.symbol == ""))
            {
                continue;
            }

            // We have two symbols in the triplet and one is still empty
            if (triplet.Count(x => x.symbol == symbols.ourSymbol) == 2)
            {
                var t = triplet.First(x => x.symbol == "");
                weights[t.index] = 1000;
                continue;
            }

            // Now we have one of our symbols and two empty ones.
            var empty = triplet.Where(x => x.symbol == "");
            foreach (var e in empty)
            {
                weights[e.index] += 10;

                // Corners are always better so we increase the weight a bit
                if (e.index is 0 or 2 or 6 or 8)
                {
                    weights[e.index] += 1;
                }

                // The middle is worth even more than corners 
                if (e.index is 4)
                {
                    weights[e.index] += 100;
                }
            }
        }
        
        // This should mark all already set fields as invalid
        for (var i = 0; i < board.Count; i++)
        {
            var symbol = board[i];
            if (symbol != "")
            {
                weights[i] = -1;
            }
        }

        var boards = new List<WeightedBoard>();
        for (int i = 0; i < weights.Length; i++ )
        {
            var weight = weights[i];
            
            // Do not return impossible moves 
            if (weight == -1)
            {
                continue;
            }
            
            boards.Add(new WeightedBoard(i, weight));
        }

        var orderedBoards = boards.OrderByDescending(x => x.Weight).ToList();
        return orderedBoards;
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

        var indexes = new int[] { 1, 3, 5, 7 };
        var random = Random.Shared.Next(0, indexes.Length);
        var boardIndex = indexes[random];
        return new[] { boardSectionIndex, boardIndex };
    }

    public static (int SectionIndex, int[] Positions) FirstMove2(Game game)
    {
        var forced = GetForced(game);
        var boardSectionIndex = GetRandomBoardSectionIndex(game, forced);
        var board = game.board[boardSectionIndex];
        var isFirstMove = board.All(x => x == "");
        if (!isFirstMove)
        {
            return (boardSectionIndex, Array.Empty<int>());
        }

        var indexes = new[] { 1, 3, 5, 7 };
        return (boardSectionIndex, indexes);
    }
    
    public static int[] FirstMoveV3(Game game, int boardSectionIndex)
    {
        var board = game.board[boardSectionIndex];
        var isFirstMove = board.All(x => x == "");
        if (!isFirstMove)
        {
            return Array.Empty<int>();
        }

        // 4 = Middle
        return new[] {boardSectionIndex, 4};
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

                return new[] { boardSectionIndex, 0 };
            }

            if (board[5] == ourSymbol)
            {
                if (board[2] == ourSymbol)
                {
                    return Array.Empty<int>();
                }

                return new[] { boardSectionIndex, 2 };
            }

            return new[] { boardSectionIndex, 5 };
            return new[] { boardSectionIndex, 3 };
        }

        if (board[3] == ourSymbol)
        {
            if (board[1] == ourSymbol)
            {
                if (board[0] == ourSymbol)
                {
                    return Array.Empty<int>();
                }

                return new[] { boardSectionIndex, 0 };
            }

            if (board[7] == ourSymbol)
            {
                if (board[6] == ourSymbol)
                {
                    return Array.Empty<int>();
                }

                return new[] { boardSectionIndex, 6 };
            }

            return new[] { boardSectionIndex, 1 };
            return new[] { boardSectionIndex, 7 };
        }

        if (board[5] == ourSymbol)
        {
            if (board[1] == ourSymbol)
            {
                if (board[2] == ourSymbol)
                {
                    return Array.Empty<int>();
                }

                return new[] { boardSectionIndex, 2 };
            }

            if (board[7] == ourSymbol)
            {
                if (board[8] == ourSymbol)
                {
                    return Array.Empty<int>();
                }

                return new[] { boardSectionIndex, 8 };
            }

            return new[] { boardSectionIndex, 1 };
            return new[] { boardSectionIndex, 7 };
        }

        if (board[7] == ourSymbol)
        {
            if (board[3] == ourSymbol)
            {
                if (board[6] == ourSymbol)
                {
                    return Array.Empty<int>();
                }

                return new[] { boardSectionIndex, 6 };
            }

            if (board[5] == ourSymbol)
            {
                if (board[8] == ourSymbol)
                {
                    return Array.Empty<int>();
                }

                return new[] { boardSectionIndex, 8 };
            }

            return new[] { boardSectionIndex, 5 };
            return new[] { boardSectionIndex, 3 };
        }


        return Array.Empty<int>();
    }

    public static (int SectionIndex, int[] Positions) SecondMove2(Game game)
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
                    return (boardSectionIndex, Array.Empty<int>());
                }

                return (boardSectionIndex, new[] { 0 });
            }

            if (board[5] == ourSymbol)
            {
                if (board[2] == ourSymbol)
                {
                    return (boardSectionIndex, Array.Empty<int>());
                }

                return (boardSectionIndex, new[] { 2 });
            }

            return (boardSectionIndex, new[] { 5, 3 });
        }

        if (board[3] == ourSymbol)
        {
            if (board[1] == ourSymbol)
            {
                if (board[0] == ourSymbol)
                {
                    return (boardSectionIndex, Array.Empty<int>());
                }

                return (boardSectionIndex, new[] { 0 });
            }

            if (board[7] == ourSymbol)
            {
                if (board[6] == ourSymbol)
                {
                    return (boardSectionIndex, Array.Empty<int>());
                }

                return (boardSectionIndex, new[] { 6 });
            }

            return (boardSectionIndex, new[] { 1, 7 });
        }

        if (board[5] == ourSymbol)
        {
            if (board[1] == ourSymbol)
            {
                if (board[2] == ourSymbol)
                {
                    return (boardSectionIndex, Array.Empty<int>());
                }

                return (boardSectionIndex, new[] { 2 });
            }

            if (board[7] == ourSymbol)
            {
                if (board[8] == ourSymbol)
                {
                    return (boardSectionIndex, Array.Empty<int>());
                }

                return (boardSectionIndex, new[] { 8 });
            }

            return (boardSectionIndex, new[] { 1, 7 });
        }

        if (board[7] == ourSymbol)
        {
            if (board[3] == ourSymbol)
            {
                if (board[6] == ourSymbol)
                {
                    return (boardSectionIndex, Array.Empty<int>());
                }

                return (boardSectionIndex, new[] { 6 });
            }

            if (board[5] == ourSymbol)
            {
                if (board[8] == ourSymbol)
                {
                    return (boardSectionIndex, Array.Empty<int>());
                }

                return (boardSectionIndex, new[] { 8 });
            }

            return (boardSectionIndex, new[] { 5, 3 });
        }

        return (boardSectionIndex, Array.Empty<int>());
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
        var enemy = game.players.First(x => x.id != game.self);
        var win = player.score switch
        {
            1 => "WON!",
            0 when enemy.score is 1 => "LOST! :(",
            _ => "STALEMATE!"
        };
        Console.WriteLine($"Game Result: {win}");
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
        var bestBoardIndex = GetRandomBoardSectionIndex(game, forced);
        var symbols = GetPlayerSymbols(game);

        var section = game.board[bestBoardIndex];
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

        return new[] { bestBoardIndex, move };
    }
    
    public static int[] BlockOpponentV2(Game game, int bestBoardIndex)
    {
        var forced = GetForced(game);
        var symbols = GetPlayerSymbols(game);

        var section = game.board[bestBoardIndex];
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

        return new[] { bestBoardIndex, move };
    }

    public static int[] WinField(Game game)
    {
        var forced = GetForced(game);
        var bestBoardIndex = GetRandomBoardSectionIndex(game, forced);
        var symbols = GetPlayerSymbols(game);

        var section = game.board[bestBoardIndex];
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

        return new[] { bestBoardIndex, move };
    }
    public static int[] WinFieldV2(Game game, int bestBoardIndex)
    {
        var forced = GetForced(game);
        var symbols = GetPlayerSymbols(game);

        var section = game.board[bestBoardIndex];
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

        return new[] { bestBoardIndex, move };
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

    public static int GetCriticalMove(string importantSymbol, WinnableLines lines)
    {
        foreach (var line in lines)
        {
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

    public static bool HasClosingPosition(List<string> section, string symbol)
    {
        var winnableLines = new WinnableLines(section);
        return GetCriticalMove(symbol, winnableLines) >= 0;
    }

    public static int[] GetNextBestMetaMove(string selfSymbol, string opponentSymbol, int sectionIndex, int[] positions,
        Game game)
    {
        //todo: hier noch die weights verarbeiten, die BotUtils.GetWeightedSectionMoves() einfließen lassen
        foreach (var position in positions)
        {
            var section = game.board[position];
            if (HasClosingPosition(section, selfSymbol) || HasClosingPosition(section, opponentSymbol))
                continue;
            if (!string.IsNullOrWhiteSpace(game.overview[position]))
                continue;
            return new[] { sectionIndex, position };
        }

        return new[] { sectionIndex, positions[0] };
    }

    public static int[] GetWinnableGameMove(string symbol, List<List<string>> board, WinnableLines metaLines)
    {
        var winnableSection = GetCriticalMove(symbol, metaLines);
        if (winnableSection >= 0)
        {
            var winnableLines = new WinnableLines(board[winnableSection]);
            var winnableMoveIndex = GetCriticalMove(symbol, winnableLines);
            if (winnableMoveIndex >= 0)
            {
                return new[] {winnableSection, winnableMoveIndex};
            }
        }

        return Array.Empty<int>();
    }

    public static int[] GetNextBestMove(Game game, string symbol, WinnableLines lines)
    {
        List<int[]> possibleMoves = new List<int[]>();
        for (var i = 0; i < game.overview.Count; i++)
        {
            if(!string.IsNullOrWhiteSpace(game.overview[i]))
                continue;
            var sectionLines = new WinnableLines(game.board[i]);
            var position = GetCriticalMove(symbol, sectionLines);
            if (position >= 0)
            {
                possibleMoves.Add(new []{i, position});
            }
        }

        var nexBestSections = lines.GetNextBestPositions(symbol).ToHashSet();
        foreach (int[] possibleMove in possibleMoves)
        {
            //todo: when mittelfeld eines der next best section, dann nimm das, statt das erst beste!!
            //muss noch gebaut werden
            if (nexBestSections.Contains(possibleMove[0]))
                return possibleMove;
        }

        return possibleMoves.FirstOrDefault(Array.Empty<int>());
    }
}

public record WeightedMove(int[] Move, double Weight);
public record WeightedBoard(int BoardIndex, double Weight);
