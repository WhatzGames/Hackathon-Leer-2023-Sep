namespace MetaTicTacToe;

public class Game
{
    public string id { get; set; }

    public List<Player> players { get; set; }

    public List<string> overview { get; set; }

    public int? forcedSection { get; set; }

    public List<List<string>> board { get; set; }

    public List<Log> log { get; set; }

    public string type { get; set; }

    public string self { get; set; }
}

public class Log
{
    public string player { get; set; }

    public List<int> move { get; set; }
}

public class Player
{
    public string id { get; set; }

    public string symbol { get; set; }

    public int score { get; set; }
}

