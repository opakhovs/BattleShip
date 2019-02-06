using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Web;
using BattleShip.Models;
using Microsoft.AspNet.SignalR;

namespace BattleShip.Hubs
{
    public class GameHub : Hub
    {
        private static List<User> Users = new List<User>();
        private static List<Game> Games = new List<Game>();

        public async Task ConnectAndGetTableCoords()
        {
            var id = Context.ConnectionId;

            if (!Users.Any(x => x.ConnectionId == id))
            {
                User newUser = new User(id);
                Users.Add(newUser);
                await Clients.Client(id).displayResult(newUser.GameBoard.GetTableCoords(), true);
            }
        }

        public async Task GetGuid()
        {
            await Clients.Caller.getGuid(Guid.NewGuid().ToString());
        } 

        public async Task<bool> StartGame(Guid guid)
        {
            var idOfPlayer = Context.ConnectionId;
            var newPlayer = Users.Find(u => u.ConnectionId == idOfPlayer);

            var game = getGameByGuid(guid);
            foreach (Game gameDelete in Games)
            {
                if (game != gameDelete && (gameDelete.FirstPlayer == newPlayer || gameDelete.SecondPlayer == newPlayer))
                {
                    Games.Remove(gameDelete);
                    break;
                }
            }
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

        public async Task Shoot(string vertical, string horizontal)
        {
            var playerWhoShoot = Users.Find(u => u.ConnectionId == Context.ConnectionId);
            var game = Games.Find(g => ((g.FirstPlayer == playerWhoShoot) || (g.SecondPlayer == playerWhoShoot)) && ((g.FirstPlayer != null) || (g.SecondPlayer != null)));
            var gameGuid = game.Guid.ToString();
            var opponent = (game.FirstPlayer.ConnectionId == playerWhoShoot.ConnectionId) ? game.SecondPlayer : game.FirstPlayer;
            var result = opponent.GameBoard.MakeFire(vertical, horizontal);
            Clients.Caller.displayResult(result, false);
            Clients.OthersInGroup(gameGuid).displayResult(result, true);
            if (!result.IsHit)
            {
                Clients.Caller.changeTurn(false);
                Clients.OthersInGroup(gameGuid).changeTurn(true);
            }
            if (opponent.GameBoard.IsShipsSunk)
            {
                await Clients.Caller.showWin(isWin : true);
                await Clients.OthersInGroup(gameGuid).showWin(isWin : false);
                Users.Remove(opponent);
                Users.Remove((game.FirstPlayer.ConnectionId == playerWhoShoot.ConnectionId) ? game.FirstPlayer : game.SecondPlayer);
                Games.Remove(game);
            }
        }

        public async Task endGameUserCancelGame(Guid guid)
        {
            var game = Games.Find(g => (g.Guid.CompareTo(guid) == 0));
            Users.Remove(game.FirstPlayer);
            Users.Remove(game.SecondPlayer);
            Games.Remove(game);
            await Clients.OthersInGroup(guid.ToString()).makeReload();
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