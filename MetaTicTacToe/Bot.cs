using System.Text;
using System.Text.Json;
using SocketIOClient;
using SocketIOClient.Transport;

namespace MetaTicTacToe;

public sealed class Bot : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        const string secret = "e2d5483c-546b-422b-b883-6f099d7efcb7";
        
        var client = new SocketIOClient.SocketIO("https://games.uhno.de",
            new SocketIOOptions {Transport = TransportProtocol.WebSocket});

        client.OnConnected += (sender, e) => Console.WriteLine("Connected");
        client.On("disconnect", (e) => Console.WriteLine("Disconnected"));
        
        await client.ConnectAsync();
        
        Console.WriteLine("Trying to Authenticate");
        await client.EmitAsync("authenticate",
            (success) => Console.WriteLine($"Authentication successful: {success}"), secret);

        client.On("data", async (response) =>
        {
            var game = response.GetValue<Game>();
            switch (game.type)
            {
                case "INIT":
                    Console.WriteLine("Init");
                    Init(game);
                    break;
                case "RESULT":
                    Console.WriteLine("Result");
                    await ResultAsync(game);
                    break;
                case "ROUND":
                    Console.WriteLine("Round");
                    await RoundAsync(game, response);
                    break;
            }
        });

        while (true)
        {
            await Task.Delay(5_000, stoppingToken);
            Console.WriteLine("Still alive");
        }
    }

    private void Init(Game game)
    {
    }
    
    private async Task ResultAsync(Game game)
    {
        var player = game.players.First(x => x.id == game.self);
        bool win = player.score == 1;
        Console.WriteLine($"Won game? {win}");

        await LogResultAsync(game);
    }
    
    private async Task RoundAsync(Game game, SocketIOResponse response)
    {
        var forced = GetForced(game);
        var boardSectionIndex = GetBoardSectionIndex(game, forced);
        var boardMoveIndex = GetBoardMoveIndex(game, boardSectionIndex);
        var move = new int[] { boardSectionIndex, boardMoveIndex };

        // CheckIllegalMove();
        
        await response.CallbackAsync(move);
    }

    private bool GetForced(Game game)
    {
        if (game.forcedSection.HasValue)
        {
            int index = game.forcedSection.Value;
            if (game.overview[index] == "")
            {
                return true;
            }
        }

        return false;
    }

    private int GetBoardSectionIndex(Game game, bool forced)
    {
        if (forced)
        {
            return game.forcedSection!.Value;
        }
        
        var freeIndexes = new List<int>();
        for (int i = 0; i < game.overview.Count; i++)
        {
            var symbol = game.overview[i];
            if (symbol == "")
            {
                freeIndexes.Add(i);
            }
        }

        var random = Random.Shared.Next(0, freeIndexes.Count);
        return freeIndexes[random];
    }

    private int GetBoardMoveIndex(Game game, int boardSectionIndex)
    {
        int moveIndex = 0;
        var chosenBoard = game.board[boardSectionIndex];
        for (int i = 0; i < chosenBoard.Count; i++)
        {
            var freeIndexes = new List<int>();
            var symbol = chosenBoard[i];
            if (symbol == "")
            {
                freeIndexes.Add(i);
            }
            
            var random = Random.Shared.Next(0, freeIndexes.Count);
            moveIndex = freeIndexes[random];
        }

        return moveIndex;
    }

    private void CheckIllegalMove(Game game, int[] move)
    {
        int idx1 = move[0];
        int idx2 = move[1];
    }

    private async Task LogResultAsync(Game game)
    {
        var exeDir = AppDomain.CurrentDomain.BaseDirectory;
        var logPath = Path.Combine(exeDir, "games");
        Directory.CreateDirectory(logPath);
        var json = JsonSerializer.Serialize(game);
        var filePath = Path.Combine(logPath, $"{game.id}.json");
        await File.WriteAllTextAsync(filePath, json, Encoding.UTF8);
    }
}