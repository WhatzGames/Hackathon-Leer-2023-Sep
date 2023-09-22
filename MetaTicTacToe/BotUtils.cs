namespace MetaTicTacToe;

public static class BotUtils
{
    public static bool GetForcedUtils(Game game)
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

    private static int GetBoardSectionIndexUtils(Game game, bool forced)
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
    
    private static int GetBoardMoveIndexUtils(Game game, int boardSectionIndex)
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
    
    private static void CheckIllegalMoveUtils(Game game, int[] move)
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
}