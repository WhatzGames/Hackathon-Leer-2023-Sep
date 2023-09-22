using System.Collections;

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

    public List<int?> move { get; set; }
}

public class Player
{
    public string id { get; set; }

    public string symbol { get; set; }

    public int score { get; set; }
}


public class WinnableLines : IEnumerable<(int, string)[]>
{
    private readonly (int, string)[] _row0;
    private readonly (int, string)[] _row1;
    private readonly (int, string)[] _row2;
    private readonly (int, string)[] _col0;
    private readonly (int, string)[] _col1;
    private readonly (int, string)[] _col2;
    private readonly (int, string)[] _dia0;
    private readonly (int, string)[] _dia1;
    private readonly (int, string)[][] _lines;
    public WinnableLines(List<string> section)
    {
        _row0 = new[] { (0, section[0]), (1, section[1]), (2, section[2]) };
        _row1 = new[] { (3, section[3]), (4, section[4]), (5, section[5]) };
        _row2 = new[] { (6, section[6]), (7, section[7]), (8, section[8]) };

        _col0 = new[] { (0, section[0]), (3, section[3]), (6, section[6]) };
        _col1 = new[] { (1, section[1]), (4, section[4]), (7, section[7]) };
        _col2 = new[] { (2, section[2]), (5, section[5]), (8, section[8]) };

        _dia0 = new[] { (0, section[0]), (4, section[4]), (8, section[8]) };
        _dia1 = new[] { (6, section[6]), (4, section[4]), (2, section[2]) };

        _lines = new[] { _row0, _row1, _row2, _col0, _col1, _col2, _dia0, _dia1 };
    }
    
    public IEnumerator<(int, string)[]> GetEnumerator()
    {
        return _lines.AsEnumerable().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}