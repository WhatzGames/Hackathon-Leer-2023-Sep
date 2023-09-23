using SocketIOClient;
using SocketIOClient.Transport;

namespace MetaTicTacToe;

public sealed class BotV9 : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        const string secret = "1c5d99b8-8b9d-4073-beb9-c9f24a660975";

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
        
        // Can we win any board?
        var ournextBestMove = BotUtils.GetNextBestMoveV3(game, ourSymbol, enemySymbol, forcedSectionIndex);
        if (ournextBestMove.Length > 0)
        {
            BotUtils.CheckIllegalMove(game, ournextBestMove, "BotUtils.GetNextBestMove ournextbestmove");
            return ournextBestMove;
        }

        // Can we block an enemy board win?
        var enemyNextMove = BotUtils.GetNextBestMoveV3(game, enemySymbol, ourSymbol, forcedSectionIndex);
        if (enemyNextMove.Length > 0)
        {
            BotUtils.CheckIllegalMove(game, enemyNextMove, "BotUtils.GetNextBestMove enemynextmove");
            return enemyNextMove;
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
            var weightedMetaBoards = BotUtils.GetCombinedWeightedMetaBoards(game, ourSymbol, enemySymbol);
            var bestBoards = weightedMetaBoards.GroupBy(x => x.Weight).OrderByDescending(x => x.Key).First().ToArray();
            var randomIndex = Random.Shared.Next(0, bestBoards.Length);
            boardSectionIndex = bestBoards[randomIndex].BoardIndex;
        }

        // "Best" move
        var weightedPossibleMoves = BotUtils.GetCombinedWeightedSectionMoves(game, boardSectionIndex, ourSymbol, enemySymbol);
        var possibleMoves = weightedPossibleMoves.Select(x => x.Move[1]).ToArray();
        if (possibleMoves.Length > 0)
        {
            var metaMove = BotUtils.GetNextBestMetaMove(ourSymbol, enemySymbol, boardSectionIndex,
                possibleMoves, game);
            BotUtils.CheckIllegalMove(game, metaMove, "bestMove");
            return metaMove;
        }

        // Random fallback
        var randomBoardSectionIndex = BotUtils.GetRandomBoardSectionIndex(game, forced);
        var boardIndex = BotUtils.GetRandomBoardMoveIndex(game, randomBoardSectionIndex);
        var move = new[] { randomBoardSectionIndex, boardIndex };
        Console.WriteLine("RANDOM MOVE!!!");
        BotUtils.CheckIllegalMove(game, move, "random move");
        return move;
    }
}

