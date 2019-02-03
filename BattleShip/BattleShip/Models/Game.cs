using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BattleShip.Models
{
    public class Game
    {
        public Guid Guid { get; set; }
        public User FirstPlayer { get; set; }
        public User SecondPlayer { get; set; }

        public bool AddPlayer(User newPlayer)
        {
            if (HasTwoPlayers() || FirstPlayer == newPlayer || SecondPlayer == newPlayer)
                return false;
            
            var result = (FirstPlayer == null) ? FirstPlayer = newPlayer : SecondPlayer = newPlayer;
            return true;
        }

        public bool IsEmpty()
        {
            var result = (FirstPlayer == null && SecondPlayer == null) ? true : false;
            return result;
        }

        public bool HasTwoPlayers()
        {
            var result = (FirstPlayer == null || SecondPlayer == null) ? false : true;
            return result;
        }
    }
}