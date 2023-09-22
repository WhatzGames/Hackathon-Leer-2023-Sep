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

        await Task.Delay(TimeSpan.MaxValue, stoppingToken);
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
        await response.CallbackAsync(new int[0,0]);
    }
}