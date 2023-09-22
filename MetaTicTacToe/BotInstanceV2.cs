using System.Runtime.InteropServices.JavaScript;

namespace MetaTicTacToe;

public class BotInstanceV2 : IBotInstance
{
    public BotInstanceV2()
    {
        
    }

    public int[] DoMove(Game game)
    {
        var resultWinField = WinField(game);
        if (resultWinField.Length < 1)
        {
            return Array.Empty<int>();
        }
        
        var resultBlockOpponent = BlockOpponent(game);
        if (resultBlockOpponent.Length <1)
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