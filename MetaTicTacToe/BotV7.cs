using SocketIOClient;
using SocketIOClient.Transport;

namespace MetaTicTacToe;

public sealed class BotV7 : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        const string secret = "42bfa196-c918-4e7b-9eb0-cb5233be99d4";

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
            
            // Can we win the game?
            var ourWinnableMove = BotUtils.GetWinnableGameMove(ourSymbol, game.board, lines);
            if (ourWinnableMove.Length > 0)
            {
                BotUtils.CheckIllegalMove(game, ourWinnableMove, "BotUtils.GetWinnableGameMove our winnable");
                return ourWinnableMove;
            }

            // Can we block the enemy win?
            var enemyWinnableMove= BotUtils.GetWinnableGameMove(enemySymbol, game.board, lines);
            if (enemyWinnableMove.Length > 0)
            {
                BotUtils.CheckIllegalMove(game, enemyWinnableMove, "BotUtils.GetWinnableGameMove enemy winable");
                return enemyWinnableMove;
            }

            // Can we win any board?
            var ournextBestMove = BotUtils.GetNextBestMove(game, ourSymbol, lines);
            if (ournextBestMove.Length > 0)
            {
                BotUtils.CheckIllegalMove(game, ournextBestMove, "BotUtils.GetNextBestMove ournextbestmove");
                return ournextBestMove;
            }

            // Can we block an enemy board win?
            var enemyNextMove = BotUtils.GetNextBestMove(game, enemySymbol, lines);
            if (enemyNextMove.Length > 0)
            {
                BotUtils.CheckIllegalMove(game, enemyNextMove, "BotUtils.GetNextBestMove enemynextmove");
                return enemyNextMove;
            }
        }

        return DoMoveBotV6(game);
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
    private static int[] DoMoveBotV6(Game game)
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
            var weightedMetaBoards = BotUtils.GetWeightedMetaBoards(game, ourSymbol, enemySymbol);
            var bestBoards = weightedMetaBoards.GroupBy(x => x.Weight).OrderByDescending(x => x.Key).First().ToArray();
            var randomIndex = Random.Shared.Next(0, bestBoards.Length);
            boardSectionIndex = bestBoards[randomIndex].BoardIndex;
        }

        // Win
        var resultWinField = BotUtils.WinFieldV2(game, boardSectionIndex);
        if (resultWinField.Length > 0)
        {
            BotUtils.CheckIllegalMove(game, resultWinField, "resultWinField");
            Console.WriteLine("Board:" + resultWinField[0] + " Feld:" + resultWinField[1]);
            return resultWinField;
        }

        // Block
        var resultBlockOpponent = BotUtils.BlockOpponentV2(game, boardSectionIndex);
        if (resultBlockOpponent.Length > 0)
        {
            BotUtils.CheckIllegalMove(game, resultBlockOpponent, "resultBlockOpponent");
            Console.WriteLine("Board:" + resultBlockOpponent[0] + " Feld:" + resultBlockOpponent[1]);
            return resultBlockOpponent;
        }

        // "Best" move
        var weightedPossibleMoves = BotUtils.GetWeightedSectionMoves(game, boardSectionIndex);
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
        Console.WriteLine("Board:" + move[0] + " Feld:" + move[1]);
        BotUtils.CheckIllegalMove(game, move, "random move");
        return move;
    }
}

