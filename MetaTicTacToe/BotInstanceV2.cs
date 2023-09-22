using System.Runtime.InteropServices.JavaScript;

namespace MetaTicTacToe;

public class BotInstanceV2 : IBotInstance
{
    private string SelfId { get; set; }
    private string SelfSymbol { get; set; }
    private string OpponentSymbol { get; set; }

    private int? ForcedSection { get; set; }

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

        ForcedSection = game.forcedSection;


        var resultWinField = WinField(game);
        if (resultWinField.Length < 1)
        {
            return Array.Empty<int>();
        }

        var resultBlockOpponent = BlockOpponent(game);
        if (resultBlockOpponent.Length < 1)
        {
            return Array.Empty<int>();
        }

        return ChooseField(game);
    }

    private int[] BlockOpponent(Game game)
    {
        int playSection = 0;
        if (game.forcedSection is not null)
            playSection = game.forcedSection.Value;

        var section = game.board[playSection];
        var row0 = new[] { (0, section[0]), (1, section[1]), (2, section[2]) };
        var row1 = new[] { (3, section[3]), (4, section[4]), (5, section[5]) };
        var row2 = new[] { (6, section[6]), (7, section[7]), (8, section[8]) };

        var col0 = new[] { (0, section[0]), (3, section[3]), (6, section[6]) };
        var col1 = new[] { (1, section[1]), (4, section[4]), (7, section[7]) };
        var col2 = new[] { (2, section[2]), (5, section[5]), (8, section[8]) };

        var dia0 = new[] { (0, section[0]), (4, section[4]), (8, section[8]) };
        var dia1 = new[] { (6, section[6]), (4, section[4]), (2, section[2]) };

        int move = GetBlockingMove(row0, row1, row2, col0, col1, col2, dia0, dia1);

        if (move is -1)
            return Array.Empty<int>();

        return new[] { playSection, move };
    }

    private int GetBlockingMove(params (int, string)[][] lines)
    {
        for (var index = 0; index < lines.Length; index++)
        {
            var line = lines[index];
            if (line.All(x => x.Item2 != OpponentSymbol))
                continue;

            bool wasLastSymbolOpponent = false;
            for (var i = 0; i < line.Length; i++)
            {
                var pos = line[i];
                var symbol = pos.Item2;
                if (symbol == OpponentSymbol && wasLastSymbolOpponent)
                    return pos.Item1;

                if (symbol == OpponentSymbol)
                    wasLastSymbolOpponent = true;
            }
        }

        return -1;
    }

    private int[] WinField(Game game)
    {
        int playSection = 0;
        if (game.forcedSection is not null)
            playSection = game.forcedSection.Value;

        var section = game.board[playSection];
        var row0 = new[] { (0, section[0]), (1, section[1]), (2, section[2]) };
        var row1 = new[] { (3, section[3]), (4, section[4]), (5, section[5]) };
        var row2 = new[] { (6, section[6]), (7, section[7]), (8, section[8]) };

        var col0 = new[] { (0, section[0]), (3, section[3]), (6, section[6]) };
        var col1 = new[] { (1, section[1]), (4, section[4]), (7, section[7]) };
        var col2 = new[] { (2, section[2]), (5, section[5]), (8, section[8]) };

        var dia0 = new[] { (0, section[0]), (4, section[4]), (8, section[8]) };
        var dia1 = new[] { (6, section[6]), (4, section[4]), (2, section[2]) };

        int move = GetWinningMove(row0, row1, row2, col0, col1, col2, dia0, dia1);

        if (move is -1)
            return Array.Empty<int>();

        return new[] { playSection, move };
    }

    private int GetWinningMove(params (int, string)[][] lines)
    {
        for (var index = 0; index < lines.Length; index++)
        {
            var line = lines[index];
            if (line.All(x => x.Item2 != SelfSymbol))
                continue;

            bool wasLastSymbolSelf = false;
            for (var i = 0; i < line.Length; i++)
            {
                var pos = line[i];
                var symbol = pos.Item2;
                if (symbol == SelfSymbol && wasLastSymbolSelf)
                    return pos.Item1;

                if (symbol == SelfSymbol)
                    wasLastSymbolSelf = true;
            }
        }

        return -1;
    }

    private int[] ChooseField(Game game)
    {
        //do stuff 

        return Array.Empty<int>();
    }
}