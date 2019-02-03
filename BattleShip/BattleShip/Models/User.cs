using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BattleShip.Models
{
    public class User
    {
        public string ConnectionId { get; set; }
        public GameBoard GameBoard { get; set; }

        public User() {
            GameBoard = new GameBoard();
        }

        public User (string connectionId)
        {
            ConnectionId = connectionId;
            GameBoard = new GameBoard();
        }
    }
}