namespace MetaTicTacToe;

public interface IBotInstance
{
    public void DoMove();
}
public class BotInstance : IBotInstance
{
    public BotInstance()
    {
        
    }
    //TODO: jeder bot braucht eine speicherung der boards

    public void DoMove()
    {
        throw new NotImplementedException();
    }
}