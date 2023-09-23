namespace MetaTicTacToe;

public interface IBotInstance
{
    public int[] DoMove(Game game);
}
public class BotInstance : IBotInstance
{
    public BotInstance()
    {
        
    }
    
    public int[] DoMove(Game game)
    {
        throw new NotImplementedException();
    }
}