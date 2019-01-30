using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BattleShip.Models;
using Microsoft.AspNet.SignalR;

namespace BattleShip.Hubs
{
    public class GameHub : Hub
    {
        private static List<User> Users = new List<User>();
    }
}