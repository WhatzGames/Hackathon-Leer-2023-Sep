using System.Runtime.InteropServices.JavaScript;

namespace MetaTicTacToe;

public class BotInstanceV2 : IBotInstance
{
    private string SelfId { get; set; }
    private string SelfSymbol { get; set; }
    private string OpponentSymbol { get; set; }

    private int ForcedSection { get; set; }

    public BotInstanceV2()
    {
    }

    public int[] DoMove(Game game)
    {
        //set game properties
        SelfId = game.self;

        SelfSymbol = game.players.Where(x => x.id == SelfId)
            .Select(y => y.symbol).ToString()!;

        OpponentSymbol = game.players.Where(x => x.id != SelfId)
            .Select(y => y.symbol).ToString()!;

        if(BotUtils.GetForced(game))
            ForcedSection = game.forcedSection!.Value;
        else
        {
            ForcedSection = GetFirstFreeId(game.overview);
        }


        var resultWinField = WinField(game);
        if ((resultWinField.Length >= 1))
        {
            BotUtils.CheckIllegalMove(game, resultWinField);
            return resultWinField;
        }

        var resultBlockOpponent = BlockOpponent(game);
        if ((resultBlockOpponent.Length >= 1))
        {
            BotUtils.CheckIllegalMove(game, resultBlockOpponent);
            return resultBlockOpponent;
        }

        var move = ChooseField(game);
        BotUtils.CheckIllegalMove(game, move);
        return move;
    }

    private int[] BlockOpponent(Game game)
    {
        var section = game.board[ForcedSection];
        var row0 = new[] { (0, section[0]), (1, section[1]), (2, section[2]) };
        var row1 = new[] { (3, section[3]), (4, section[4]), (5, section[5]) };
        var row2 = new[] { (6, section[6]), (7, section[7]), (8, section[8]) };

        var col0 = new[] { (0, section[0]), (3, section[3]), (6, section[6]) };
        var col1 = new[] { (1, section[1]), (4, section[4]), (7, section[7]) };
        var col2 = new[] { (2, section[2]), (5, section[5]), (8, section[8]) };

        var dia0 = new[] { (0, section[0]), (4, section[4]), (8, section[8]) };
        var dia1 = new[] { (6, section[6]), (4, section[4]), (2, section[2]) };

        int move = GetCriticalMove(OpponentSymbol,row0, row1, row2, col0, col1, col2, dia0, dia1);

        if (move is -1)
            return Array.Empty<int>();

        return new[] { ForcedSection, move };
    }

    private int GetCriticalMove(string importantSymbol, params (int, string)[][] lines)
    {
        for (var index = 0; index < lines.Length; index++)
        {
            var line = lines[index];
            var opCount = line.Count(x => x.Item2 == importantSymbol);
            if (opCount<2)
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

    private int[] WinField(Game game)
    {
        var section = game.board[ForcedSection];
        var row0 = new[] { (0, section[0]), (1, section[1]), (2, section[2]) };
        var row1 = new[] { (3, section[3]), (4, section[4]), (5, section[5]) };
        var row2 = new[] { (6, section[6]), (7, section[7]), (8, section[8]) };

        var col0 = new[] { (0, section[0]), (3, section[3]), (6, section[6]) };
        var col1 = new[] { (1, section[1]), (4, section[4]), (7, section[7]) };
        var col2 = new[] { (2, section[2]), (5, section[5]), (8, section[8]) };

        var dia0 = new[] { (0, section[0]), (4, section[4]), (8, section[8]) };
        var dia1 = new[] { (6, section[6]), (4, section[4]), (2, section[2]) };

        int move = GetCriticalMove(SelfSymbol, row0, row1, row2, col0, col1, col2, dia0, dia1);

        if (move is -1)
            return Array.Empty<int>();

        return new[] { ForcedSection, move };
    }

    private int[] ChooseField(Game game)
    {
        return new []{ForcedSection, GetFirstFreeId(game.board[ForcedSection])};
    }

    private int GetFirstFreeId(List<string> board)
    {
        for (int i = 0; i < board.Count; i++)
        {
            if (string.IsNullOrWhiteSpace(board[i]))
                return i;
        }

        return 0;
    }
}