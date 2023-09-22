using SocketIOClient;
using SocketIOClient.Transport;

namespace MetaTicTacToe;

public sealed class BotV6 : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        const string secret = "279b7dfa-da8a-4036-8324-e9293ba7733b";

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
                return ourWinnableMove;
            }

            // Can we block the enemy win?
            var enemyWinnableMove= BotUtils.GetWinnableGameMove(enemySymbol, game.board, lines);
            if (enemyWinnableMove.Length > 0)
            {
                return enemyWinnableMove;
            }

            
            // Can we block an enemy board win?
            var enemyNextMove = BotUtils.GetNextBestMove(game, enemySymbol, lines);
            if (enemyNextMove.Length > 0)
            {
                return enemyNextMove;
            }

            // Can we win any board?
            var ournextBestMove = BotUtils.GetNextBestMove(game, ourSymbol, lines);
            if (ournextBestMove.Length > 0)
            {
                return ournextBestMove;
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
    public static int[] DoMoveBotV6(Game game)
    {
        //todo:hier beste metaboard choice durchführern und diese in den methoden übergeben
        var weightedMetaBoards = BotUtils.GetWeightedMetaBoards(game);
        var bestBoardIndex = weightedMetaBoards.First().BoardIndex;
        
        // Win
        var resultWinField = BotUtils.WinFieldV2(game, bestBoardIndex);
        if (resultWinField.Length > 0)
        {
            BotUtils.CheckIllegalMove(game, resultWinField, "resultWinField");
            Console.WriteLine("Board:" + resultWinField[0] + " Feld:" + resultWinField[1]);
            return resultWinField;
        }

        // Block
        var resultBlockOpponent = BotUtils.BlockOpponentV2(game, bestBoardIndex);
        if (resultBlockOpponent.Length > 0)
        {
            BotUtils.CheckIllegalMove(game, resultBlockOpponent, "resultBlockOpponent");
            Console.WriteLine("Board:" + resultBlockOpponent[0] + " Feld:" + resultBlockOpponent[1]);
            return resultBlockOpponent;
        }

        // First move
        var (ourSymbol, enemySymbol) = BotUtils.GetPlayerSymbols(game);
        var firstMove = BotUtils.FirstMoveV3(game, bestBoardIndex);
        if (firstMove.Length > 0)
        {
            BotUtils.CheckIllegalMove(game, firstMove, "firstMove");
            return firstMove;
        }

        // Second move
        var weightedPossibleMoves = BotUtils.GetWeightedSectionMoves(game, bestBoardIndex);
        var possibleMoves = weightedPossibleMoves.Select(x => x.Move[1])
                                                 .ToArray();
        
        if (possibleMoves.Length > 0)
        {
            var metaMove = BotUtils.GetNextBestMetaMove(ourSymbol, enemySymbol, bestBoardIndex,
                possibleMoves, game);
            var valueOfField = game.board[metaMove[0]][metaMove[1]];
            if (valueOfField == "")
            {
                BotUtils.CheckIllegalMove(game, metaMove, "secondMove");
                return metaMove;
            }
        }
        
        // Most efficient local move
        

        // Random fallback
        var forced = BotUtils.GetForced(game);
        var boardSectionIndex = BotUtils.GetRandomBoardSectionIndex(game, forced);
        var boardIndex = BotUtils.GetRandomBoardMoveIndex(game, boardSectionIndex);
        var move = new[] { boardSectionIndex, boardIndex };
        Console.WriteLine("Board:" + move[0] + " Feld:" + move[1]);
        BotUtils.CheckIllegalMove(game, move, "random move");
        return move;
    }
}

