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

        public async void ConnectAndGetTableCoords()
        {
            var id = Context.ConnectionId;

            if (!Users.Any(x => x.ConnectionId == id))
            {
                User newUser = new User() { ConnectionId = id};
                Users.Add(newUser);
                await Clients.Client(id).displayResult(newUser.GameBoard.GetTableCoords());
            }
        }

        public async void GetGuid()
        {
            await Clients.Caller.getGuid(Guid.NewGuid().ToString());
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
                    await Clients.Group(guid.ToString()).startGame();
                }
                return true;
            }
            else
            {
                return false;
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