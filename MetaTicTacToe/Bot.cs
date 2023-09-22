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
                    Result(game);
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
    
    private void Result(Game game)
    {
        // TODO Log results?
    }
    
    private async Task RoundAsync(Game game, SocketIOResponse response)
    {
        var move = GetNextMove(game);
        await response.CallbackAsync(move);
    }

    private int[] GetNextMove(Game game)
    {
        // Forced value?
        bool forced = false;
        if (game.forcedSection.HasValue)
        {
            int index = game.forcedSection.Value;
            if (game.overview[index] == "")
            {
                forced = true;
            }
        }

        // Get board section
        int chosenBoardIndex = game.forcedSection!.Value;
        if (!forced)
        {
            for (int i = 0; i < game.overview.Count; i++)
            {
                var symbol = game.overview[i];
                if (symbol == "")
                {
                    chosenBoardIndex = i;
                    break;
                }
            }
        }
        
        // Get move index
        int moveIndex = 0;
        var chosenBoard = game.board[chosenBoardIndex];
        for (int i = 0; i < chosenBoard.Count; i++)
        {
            var symbol = chosenBoard[i];
            if (symbol == "")
            {
                moveIndex = i;
                break;
            }
        }

        return new int[] { chosenBoardIndex, moveIndex };
    }
}