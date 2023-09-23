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


        var resultWinField = WinField(game.board[ForcedSection]);
        if ((resultWinField.Length >= 1))
        {
            BotUtils.CheckIllegalMove(game, resultWinField);
            return resultWinField;
        }

        var resultBlockOpponent = BlockOpponent(game.board[ForcedSection]);
        if ((resultBlockOpponent.Length >= 1))
        {
            BotUtils.CheckIllegalMove(game, resultBlockOpponent);
            return resultBlockOpponent;
        }

        var move = ChooseField(game);
        BotUtils.CheckIllegalMove(game, move);
        return move;
    }

    private int[] BlockOpponent(List<string> section)
    {
        var winnable = new WinnableLines(section);
        int move = GetCriticalMove(OpponentSymbol, winnable);

        if (move is -1)
            return Array.Empty<int>();

        return new[] { ForcedSection, move };
    }

    private int GetCriticalMove(string importantSymbol, WinnableLines lines)
    {
        foreach (var line in lines)
        {
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

    private int[] WinField(List<string> section)
    {
        var winnable = new WinnableLines(section);

        int move = GetCriticalMove(SelfSymbol, winnable);

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