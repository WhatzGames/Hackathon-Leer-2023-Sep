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
        var otherSymbol = game.players.Where(x => x.id != game.self).First().symbol;
        int playSection = 0;
        if (game.forcedSection is not null)
            playSection = game.forcedSection.Value;

        var section = game.board[playSection];
        var row0 = new[] { section[0], section[1], section[2] };
        var row1 = new[] { section[3], section[4], section[5] };
        var row2 = new[] { section[6], section[7], section[8] };

        var col0 = new[] { section[0], section[3], section[6] };
        var col1 = new[] { section[1], section[4], section[7] };
        var col2 = new[] { section[2], section[5], section[8] };

        var dia0 = new[] { section[0], section[4], section[8] };
        var dia1 = new[] { section[6], section[4], section[2] };

        int move = GetBlockingMove(otherSymbol, row0, row1, row2, col0, col1, col2, dia0, dia1);

        if (move is -1)
            return Array.Empty<int>();

        return new[] { playSection, move };
    }

    private int GetBlockingMove(string opponentSymbol, params string[][] lines)
    {
        for (var index = 0; index < lines.Length; index++)
        {
            var line = lines[index];
            if (!line.Contains(opponentSymbol))
                continue;

            bool wasLastSymbolOpponent = false;
            for (var i = 0; i < line.Length; i++)
            {
                var symbol = line[i];
                if (symbol == opponentSymbol && wasLastSymbolOpponent)
                {
                    return index+i ;
                }

                if (symbol == opponentSymbol)
                    wasLastSymbolOpponent = true;
            }
        }

        return -1;
    }

    private int[] WinField(Game game)
    {
        //do stuff 

        return Array.Empty<int>();
    }

    private int[] ChooseField(Game game)
    {
        //do stuff 

        return Array.Empty<int>();
    }
}