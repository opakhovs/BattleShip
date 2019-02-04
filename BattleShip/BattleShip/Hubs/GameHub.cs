using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using BattleShip.Models;
using Microsoft.AspNet.SignalR;

namespace BattleShip.Hubs
{
    public class GameHub : Hub
    {
        private static List<User> Users = new List<User>();
        private static List<Game> Games = new List<Game>();

        public void Connect()
        {
            var id = Context.ConnectionId;

            if (!Users.Any(x => x.ConnectionId == id))
            {
                User newUser = new User(id);
                Users.Add(newUser);
            }
        }

        public async Task<bool> StartGame(Guid guid)
        {
            var idOfPlayer = Context.ConnectionId;
            var newPlayer = Users.Find(u => u.ConnectionId == idOfPlayer);

            var game = getGameByGuid(guid);
            if (game.AddPlayer(newPlayer))
            {
                await Groups.Add(idOfPlayer, guid.ToString());

                if (game.HasTwoPlayers())
                {
                    await Clients.Client(game.FirstPlayer.ConnectionId).startGame(isOurTurn: true);
                    await Clients.Client(game.SecondPlayer.ConnectionId).startGame(isOurTurn: false);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        
        public void TempMethod()
        {
            Clients.All.tempMethod();
        }

        public void Shoot(string vertical, string horizontal)
        {
            var playerWhoShoot = Users.Find(u => u.ConnectionId == Context.ConnectionId);
            var game = Games.Find(g => (g.FirstPlayer == playerWhoShoot) || (g.SecondPlayer == playerWhoShoot));
            var gameGuid = game.Guid.ToString();
            var opponent = (game.FirstPlayer.ConnectionId == playerWhoShoot.ConnectionId) ? game.SecondPlayer : game.FirstPlayer;
            var result = opponent.GameBoard.MakeFire(vertical, horizontal);
            Clients.All.displayResult(result);
            if (!result.IsHit)
            {
                Clients.Caller.changeTurn(false);
                Clients.OthersInGroup(gameGuid).changeTurn(true);
            }
        }

        private Game getGameByGuid(Guid guid)
        {
            var game = Games.Find(g => g.Guid == guid);
            if (game == null)
            {
                game = new Game() { Guid = guid };
                Games.Add(game);
            }
            return game;
        }
    }
}