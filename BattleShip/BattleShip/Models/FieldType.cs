using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BattleShip.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FieldType
    {
        FREE, SHIP, MISS, HIT, SUNK
    }
}