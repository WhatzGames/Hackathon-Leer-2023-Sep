using SocketIOClient;
using SocketIOClient.Transport;

namespace MetaTicTacToe;

public sealed class BotV4 : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        const string secret = "e685de0b-8ded-4a76-9cfc-d42214261688";

        var client = new SocketIOClient.SocketIO("https://games.uhno.de",
            new SocketIOOptions { Transport = TransportProtocol.WebSocket });

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
        var move = GetMove(game);
        await response.CallbackAsync(move);
    }

    private static int[] GetMove(Game game)
    {
        // Win
        var resultWinField = BotUtils.WinField(game);
        if (resultWinField.Length > 0)
        {
            BotUtils.CheckIllegalMove(game, resultWinField, "resultWinField");
            Console.WriteLine("Board:" + resultWinField[0] + " Feld:" + resultWinField[1]);
            return resultWinField;
        }

        // Block
        var resultBlockOpponent = BotUtils.BlockOpponent(game);
        if (resultBlockOpponent.Length > 0)
        {
            BotUtils.CheckIllegalMove(game, resultBlockOpponent, "resultBlockOpponent");
            Console.WriteLine("Board:" + resultBlockOpponent[0] + " Feld:" + resultBlockOpponent[1]);
            return resultBlockOpponent;
        }

        // First move
        var (ourSymbol, enemySymbol) = BotUtils.GetPlayerSymbols(game);
        var firstMove = BotUtils.FirstMove2(game);
        if (firstMove.Positions.Length > 0)
        {
            var metaMove = BotUtils.GetNextBestMetaMove(ourSymbol, enemySymbol, firstMove.SectionIndex,
                firstMove.Positions, game);
            BotUtils.CheckIllegalMove(game, metaMove, "firstMove");
            return metaMove;
        }

        // Second move
        var secondMove = BotUtils.SecondMove2(game);
        if (secondMove.Positions.Length > 0)
        {
            var metaMove = BotUtils.GetNextBestMetaMove(ourSymbol, enemySymbol, secondMove.SectionIndex,
                secondMove.Positions, game);
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