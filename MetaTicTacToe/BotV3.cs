using SocketIOClient;
using SocketIOClient.Transport;

namespace MetaTicTacToe;

public sealed class BotV3 : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        const string secret = "34389667-88c2-4100-beef-78c7d758a7ec";
        
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
        var move = GetMove(game);
        await response.CallbackAsync(move);
    }

    private static int[] GetMove(Game game)
    {
        var resultWinField = BotUtils.WinField(game);
        if (resultWinField.Length > 0)
        {
            BotUtils.CheckIllegalMove(game, resultWinField, "resultWinField");
            Console.WriteLine("Board:" + resultWinField[0] + " Feld:" + resultWinField[1]);
            return resultWinField;
        }

        var resultBlockOpponent = BotUtils.BlockOpponent(game);
        if (resultBlockOpponent.Length > 0)
        {
            BotUtils.CheckIllegalMove(game, resultBlockOpponent, "resultBlockOpponent");
            Console.WriteLine("Board:" + resultBlockOpponent[0] + " Feld:" + resultBlockOpponent[1]);
            return resultBlockOpponent;
        }

        var firstMove = BotUtils.FirstMove(game);
        if (firstMove.Length > 0)
        {
            BotUtils.CheckIllegalMove(game, firstMove, "firstMove");
            Console.WriteLine("Board:" + firstMove[0] + " Feld:" + firstMove[1]);
            return firstMove;
        }

        var secondMove = BotUtils.SecondMove(game);
        if (secondMove.Length > 0)
        {
            var valueOfField = game.board[secondMove[0]][secondMove[1]];
            if (valueOfField == "")
            {
                BotUtils.CheckIllegalMove(game, secondMove, "secondMove");
                Console.WriteLine("Board:" + secondMove[0] + " Feld:" + secondMove[1]);
                return secondMove;
            }


        }



        // Random fallback
        var forced = BotUtils.GetForced(game);
        var boardSectionIndex = BotUtils.GetRandomBoardSectionIndex(game, forced);
        var boardIndex = BotUtils.GetRandomBoardMoveIndex(game, boardSectionIndex);
        var move = new[] {boardSectionIndex, boardIndex};
        Console.WriteLine("Board:" + move[0] + " Feld:" + move[1]);
        BotUtils.CheckIllegalMove(game, move, "random move");
        return move;
    }
}