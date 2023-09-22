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
        //do stuff

        return Array.Empty<int>();
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