using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BattleShip.Models
{
    public class Result
    {
        Coord[] coords;
        int index;

        public Coord[] Coords { get { return coords; } }

        public Result() {
        }

        public Result InitializeSize(int size)
        {
            coords = new Coord[size];
            index = 0;
            return this;
        }

        public bool AddCoord(Coord coord)
        {
            if (index == coords.Length)
                return false;
            coords[index++] = coord;
            return true;
        }
    }
}