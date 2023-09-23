using SocketIOClient;
using SocketIOClient.Transport;

namespace MetaTicTacToe;

public sealed class BotV5 : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        const string secret = "f2baebc8-46a1-4e70-b294-34baa407de3b";

        var client = new SocketIOClient.SocketIO("https://games.uhno.de",
            new SocketIOOptions { Transport = TransportProtocol.WebSocket });

        client.OnConnected += (sender, e) => Console.WriteLine("Connected");
        client.OnDisconnected += (_, _) => Reconnect(client);


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
        var move = GetMove(game);
        await response.CallbackAsync(move);
    }

    private static int[] GetMove(Game game)
    {
        var forced = BotUtils.GetForced(game);
        if (!forced)
        {
            (string ourSymbol, string enemySymbol) = BotUtils.GetPlayerSymbols(game);
            var lines = new WinnableLines(game.overview);
            var ourWinnableMove = BotUtils.GetWinnableGameMove(ourSymbol, game.board, lines);
            if (ourWinnableMove.Length > 0)
            {
                return ourWinnableMove;
            }
            var enemyWinnableMove= BotUtils.GetWinnableGameMove(enemySymbol, game.board, lines);
            if (enemyWinnableMove.Length > 0)
            {
                return enemyWinnableMove;
            }
            
            var enemyNextMove = BotUtils.GetNextBestMove(game, enemySymbol, lines);
            if (enemyNextMove.Length > 0)
            {
                return enemyNextMove;
            }
            var ournextBestMove = BotUtils.GetNextBestMove(game, ourSymbol, lines);
            if (ournextBestMove.Length > 0)
            {
                return ournextBestMove;
            }
        }

        return BotInstanceV4.DoMove(game);
    }
    
    private static void Reconnect(SocketIOClient.SocketIO client)
    {
        Console.WriteLine("Disconnected");
        while (!client.Connected)
        {
            Task.Delay(5000).Wait();
            Console.WriteLine("Attempting reconnect");
            client.ConnectAsync().Wait();
        }

        Console.WriteLine("Reconnected!");
    }
}