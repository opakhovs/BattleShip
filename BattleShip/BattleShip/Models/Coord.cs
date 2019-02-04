using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BattleShip.Models
{
    public class Coord
    {
        [JsonProperty("Horizontal")]
        public int Horizontal { get; set; }
        [JsonProperty("Vertical")]
        public int Vertical { get; set; }
        [JsonProperty("FieldType")]
        public FieldType CellType { get; set; }

        public Coord(int horizontal, int vertical, FieldType cellType)
        {
            Horizontal = horizontal;
            Vertical = vertical;
            CellType = cellType;
        }
    }
}