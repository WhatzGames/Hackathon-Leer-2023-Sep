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
        await BotUtils.LogResultAsync(game);
    }
    
    private async Task RoundAsync(Game game, SocketIOResponse response)
    {
        var forced = BotUtils.GetForced(game);
        var boardSectionIndex = BotUtils.GetRandomBoardSectionIndex(game, forced);
        var boardMoveIndex = BotUtils.GetRandomBoardMoveIndex(game, boardSectionIndex);
        var move = new int[] { boardSectionIndex, boardMoveIndex };
        BotUtils.CheckIllegalMove(game, move);
        await response.CallbackAsync(move);
    }
}