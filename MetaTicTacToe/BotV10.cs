using SocketIOClient;
using SocketIOClient.Transport;

namespace MetaTicTacToe;

public sealed class BotV10 : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        const string secret = "913544ef-2b69-4ac4-b338-3700c83f88b5";

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
        (string ourSymbol, string enemySymbol) = BotUtils.GetPlayerSymbols(game);
        var lines = new WinnableLines(game.overview);

        int? forcedSectionIndex = null;
        if (forced)
        {
            forcedSectionIndex = game.forcedSection;
        }

        // Can we win the game?
        var ourWinnableMove = BotUtils.GetWinnableGameMoveV2(ourSymbol, game.board, lines, forcedSectionIndex);
        if (ourWinnableMove.Length > 0)
        {
            BotUtils.CheckIllegalMove(game, ourWinnableMove, "BotUtils.GetWinnableGameMove our winnable");
            return ourWinnableMove;
        }

        // Can we block the enemy win?
        var enemyWinnableMove = BotUtils.GetWinnableGameMoveV2(enemySymbol, game.board, lines, forcedSectionIndex);
        if (enemyWinnableMove.Length > 0)
        {
            BotUtils.CheckIllegalMove(game, enemyWinnableMove, "BotUtils.GetWinnableGameMove enemy winnable");
            return enemyWinnableMove;
        }

        return GetMoveInner(game);
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
    private static int[] GetMoveInner(Game game)
    {
        var (ourSymbol, enemySymbol) = BotUtils.GetPlayerSymbols(game);

        var forced = BotUtils.GetForced(game);
        int boardSectionIndex;
        if (forced)
        {
            boardSectionIndex = game.forcedSection!.Value;
        }
        else
        {
            //todo: hier beste metaboard choice durchführern und diese in den methoden übergeben
            var weightedMetaBoards = BotUtils.GetWeightedMetaBoardsV2(game, ourSymbol, enemySymbol);
            var bestBoards = weightedMetaBoards.GroupBy(x => x.Weight).OrderByDescending(x => x.Key).First().ToArray();
            var randomIndex = Random.Shared.Next(0, bestBoards.Length);
            boardSectionIndex = bestBoards[randomIndex].BoardIndex;
        }

        // "Best" move
        var weightedPossibleMoves = BotUtils.GetWeightedSectionMovesV4(game, boardSectionIndex, ourSymbol, enemySymbol);
        var moves = weightedPossibleMoves.GroupBy(x => x.Weight).OrderByDescending(x => x.Key).First().ToArray();
        var rndIdx = Random.Shared.Next(0, moves.Length);
        var mv = moves[rndIdx].Move;
        BotUtils.CheckIllegalMove(game, mv, "bestMove");
        return mv;

        // Random fallback
        var randomBoardSectionIndex = BotUtils.GetRandomBoardSectionIndex(game, forced);
        var boardIndex = BotUtils.GetRandomBoardMoveIndex(game, randomBoardSectionIndex);
        var move = new[] { randomBoardSectionIndex, boardIndex };
        Console.WriteLine("RANDOM MOVE!!!");
        BotUtils.CheckIllegalMove(game, move, "random move");
        return move;
    }
}

