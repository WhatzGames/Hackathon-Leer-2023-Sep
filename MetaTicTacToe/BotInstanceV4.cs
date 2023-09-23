namespace MetaTicTacToe;

public class BotInstanceV4
{
    public static int[] DoMove(Game game)
    {
        // Win
        var resultWinField = BotUtils.WinField(game);
        if (resultWinField.Length > 0)
        {
            BotUtils.CheckIllegalMove(game, resultWinField, "resultWinField");
            Console.WriteLine("Board:" + resultWinField[0] + " Feld:" + resultWinField[1]);
            return resultWinField;
        }

        // Block
        var resultBlockOpponent = BotUtils.BlockOpponent(game);
        if (resultBlockOpponent.Length > 0)
        {
            BotUtils.CheckIllegalMove(game, resultBlockOpponent, "resultBlockOpponent");
            Console.WriteLine("Board:" + resultBlockOpponent[0] + " Feld:" + resultBlockOpponent[1]);
            return resultBlockOpponent;
        }

        // First move
        var (ourSymbol, enemySymbol) = BotUtils.GetPlayerSymbols(game);
        var firstMove = BotUtils.FirstMove2(game);
        if (firstMove.Positions.Length > 0)
        {
            var metaMove = BotUtils.GetNextBestMetaMove(ourSymbol, enemySymbol, firstMove.SectionIndex,
                firstMove.Positions, game);
            BotUtils.CheckIllegalMove(game, metaMove, "firstMove");
            return metaMove;
        }

        // Second move
        var secondMove = BotUtils.SecondMove2(game);
        if (secondMove.Positions.Length > 0)
        {
            var metaMove = BotUtils.GetNextBestMetaMove(ourSymbol, enemySymbol, secondMove.SectionIndex,
                secondMove.Positions, game);
            var valueOfField = game.board[metaMove[0]][metaMove[1]];
            if (valueOfField == "")
            {
                BotUtils.CheckIllegalMove(game, metaMove, "secondMove");
                return metaMove;
            }
        }
        
        // Most efficient local move
        

        // Random fallback
        var forced = BotUtils.GetForced(game);
        var boardSectionIndex = BotUtils.GetRandomBoardSectionIndex(game, forced);
        var boardIndex = BotUtils.GetRandomBoardMoveIndex(game, boardSectionIndex);
        var move = new[] { boardSectionIndex, boardIndex };
        Console.WriteLine("Board:" + move[0] + " Feld:" + move[1]);
        BotUtils.CheckIllegalMove(game, move, "random move");
        return move;
    }
}