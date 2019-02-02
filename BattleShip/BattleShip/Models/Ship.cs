using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BattleShip.Models
{
    public class Ship
    {
        public Coord[] Coords { get; }
        public bool IsHorizontal { get; }

        public Ship(Coord[] coords)
        {
            Coords = coords;
            IsHorizontal = HorizontalCheck();
        }

        public bool IsSunk()
        {
            foreach (Coord coord in Coords)
            {
                if (coord.CellType != FieldType.HIT)
                    return false;
            }
            return true;
        }

        private bool HorizontalCheck()
        {
            if (Coords.Length == 1)
                return true;
            if (Coords[0].Horizontal == Coords[Coords.Length - 1].Horizontal)
                return true;
            return false;
        }
    }
}