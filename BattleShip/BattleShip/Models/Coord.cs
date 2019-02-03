using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BattleShip.Models
{
    public class Coord
    {
        public int Horizontal { get; set; }
        public int Vertical { get; set; }
        public FieldType CellType { get; set; }

        public Coord(int horizontal, int vertical, FieldType cellType)
        {
            Horizontal = horizontal;
            Vertical = vertical;
            CellType = cellType;
        }
    }
}