﻿namespace MetaTicTacToe;

public interface IBotInstance
{
    public void DoMove(Game game);
}
public class BotInstance : IBotInstance
{
    public BotInstance()
    {
        
    }
    
    public void DoMove(Game game)
    {
        throw new NotImplementedException();
    }
}