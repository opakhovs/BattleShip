using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BattleShip.Models
{
    public class GameBoard
    {
        Coord[,] coords;
        Ship[] ships;
        bool isShipsSunk;
        int shipNumber;

        public Coord[,] Coords { get { return coords; } }
        public Ship[] Ships { get { return ships; } }
        public bool IsShipsSunk { get { return isShipsSunk; } }

        delegate void OperateOnCoords(ref Coord coord);

        public GameBoard()
        {
            shipNumber = 10;
            isShipsSunk = false;
            GenerateFreeCoords();
            GenerateShips();
            FillShipsOnTable();
        }

        public Result GetTableCoords()
        {
            Result result = new Result();
            result.InitializeSize(100);
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    result.AddCoord(coords[i, j]);
                }
            }
            return result;
        }

        public Result MakeFire(string vertical, string horizontal)
        {
            Tuple<int, int> shotCoord = new Tuple<int, int>(Int32.Parse(horizontal), Int32.Parse(vertical));
            Result result = new Result();
            result.IsHit = false;
            if (coords[shotCoord.Item1, shotCoord.Item2].CellType == FieldType.FREE)
            {
                result.InitializeSize(1).AddCoord(new Coord(shotCoord.Item1, shotCoord.Item2, FieldType.MISS));
                coords[shotCoord.Item1, shotCoord.Item2].CellType = FieldType.MISS;
            }
            else if (coords[shotCoord.Item1, shotCoord.Item2].CellType == FieldType.SHIP)
            {
                coords[shotCoord.Item1, shotCoord.Item2].CellType = FieldType.HIT;
                Ship ship = null;
                foreach (Ship temp in ships)
                {
                    for (int i = 0; i < temp.Coords.Length; i++)
                    {
                        if (temp.Coords[i].Horizontal == shotCoord.Item1 && temp.Coords[i].Vertical == shotCoord.Item2)
                        {
                            temp.Coords[i].CellType = FieldType.HIT;
                            ship = temp;
                            break;
                        }
                    }
                }
                if (!ship.IsSunk())
                    result.InitializeSize(1).AddCoord(new Coord(shotCoord.Item1, shotCoord.Item2, FieldType.HIT));
                else
                {
                    if (--shipNumber == 0)
                        isShipsSunk = true;
                    result = CalculateSizeOfHitsAndCreateResult(ship, shotCoord);
                }
                result.IsHit = true;

            }
            return result;
        }

        private Result CalculateSizeOfHitsAndCreateResult(Ship ship, Tuple<int, int> shootCoord)
        {
            int size = ship.Coords.Length;
            Result result = new Result();
            if (CheckIfShipIsOnCorner(ship, size))
                size += 2;
            else if (CheckIfShipIsOnBorderWithSmallerSide(ship, size))
                size = size * 2 + 3;
            else if (CheckIfShipIsOnBorderWithBiggerSide(ship, size))
                size = size + 4;
            else
                size = size * 2 + 6;
            size += ship.Coords.Length;
            result.InitializeSize(size);
            for (int i = 0; i < ship.Coords.Length; i++)
            {
                ship.Coords[i].CellType = FieldType.SUNK;
                result.AddCoord(ship.Coords[i]);
            }
            return FillAroundShip(result, ship);
        }

        private Result FillAroundShip(Result result, Ship ship)
        {
            for (int i = 0; i < ship.Coords.Length; i++)
            {
                if (i == 0 && ship.Coords.Length == 1)
                    FillAroundCoord(result, ship.Coords[i], null, null);
                else if (i == 0)
                    FillAroundCoord(result, ship.Coords[i], null, ship.Coords[i + 1]);
                else if (i == ship.Coords.Length - 1)
                    FillAroundCoord(result, ship.Coords[i], ship.Coords[i - 1], null);
                else
                    FillAroundCoord(result, ship.Coords[i], ship.Coords[i - 1], ship.Coords[i + 1]);
            }
            return result;
        }
        private Result FillAroundCoord(Result result, Coord currentCoord, Coord leftOrUpOfCurrent, Coord rightOrDownOfCurrent)
        {
            Coord coordOperate = new Coord(currentCoord.Horizontal - 1, currentCoord.Vertical - 1, FieldType.MISS);
            OperateOnCoords operateOnCoords = IncrementHorizontalCoord;
            do
            {
                if (coordOperate.Horizontal > -1 && coordOperate.Horizontal < 10 && coordOperate.Vertical > -1 && coordOperate.Vertical < 10 &&
                    !(leftOrUpOfCurrent != null && (coordOperate.Horizontal == leftOrUpOfCurrent.Horizontal && coordOperate.Vertical == leftOrUpOfCurrent.Vertical)) &&
                    !(rightOrDownOfCurrent != null && (coordOperate.Horizontal == rightOrDownOfCurrent.Horizontal && coordOperate.Vertical == rightOrDownOfCurrent.Vertical)))
                {
                    coords[coordOperate.Horizontal, coordOperate.Vertical].CellType = FieldType.MISS;
                    result.AddCoord(new Coord(coordOperate.Horizontal, coordOperate.Vertical, coordOperate.CellType));
                }
                operateOnCoords(ref coordOperate);
                if (coordOperate.Horizontal == currentCoord.Horizontal + 1 && coordOperate.Vertical == currentCoord.Vertical - 1)
                    operateOnCoords = IncrementVerticalCoord;
                else if (coordOperate.Horizontal == currentCoord.Horizontal + 1 && coordOperate.Vertical == currentCoord.Vertical + 1)
                    operateOnCoords = DecrementHorizontalCoord;
                else if (coordOperate.Horizontal == currentCoord.Horizontal - 1 && coordOperate.Vertical == currentCoord.Vertical + 1)
                    operateOnCoords = DecrementVerticalCoord;
            } while (!(coordOperate.Horizontal == currentCoord.Horizontal - 1 && coordOperate.Vertical == currentCoord.Vertical - 1));
            return result;
        }

        private void DecrementVerticalCoord(ref Coord coord)
        {
            coord.Vertical = coord.Vertical - 1;
        }

        private void IncrementVerticalCoord(ref Coord coord)
        {
            coord.Vertical = coord.Vertical + 1;
        }

        private void DecrementHorizontalCoord(ref Coord coord)
        {
            coord.Horizontal = coord.Horizontal - 1;
        }

        private void IncrementHorizontalCoord(ref Coord coord)
        {
            coord.Horizontal = coord.Horizontal + 1;
        }

        private bool CheckIfShipIsOnCorner(Ship ship, int size)
        {
            if ((ship.Coords[0].Horizontal == 0 && ship.Coords[0].Vertical == 0) || (ship.Coords[0].Horizontal == 9 && ship.Coords[0].Vertical == 0) ||
                (ship.Coords[0].Horizontal == 0 && ship.Coords[0].Vertical == 9) || (ship.Coords[size - 1].Horizontal == 9 && ship.Coords[size - 1].Vertical == 9))
                return true;
            return false;
        }

        private bool CheckIfShipIsOnBorderWithSmallerSide(Ship ship, int size)
        {
            if ((ship.Coords[0].Horizontal == 0 && ship.Coords[size - 1].Horizontal != 0) || (ship.Coords[0].Horizontal == 9 && ship.Coords[size - 1].Horizontal != 9) ||
                (ship.Coords[0].Vertical == 0 && ship.Coords[size - 1].Vertical != 0) || (ship.Coords[0].Vertical == 9 && ship.Coords[size - 1].Vertical != 9))
                return true;
            return false;
        }

        private bool CheckIfShipIsOnBorderWithBiggerSide(Ship ship, int size)
        {
            if ((ship.Coords[0].Horizontal == 0 && ship.Coords[size - 1].Horizontal == 0) || (ship.Coords[0].Horizontal == 9 && ship.Coords[size - 1].Horizontal == 9) ||
                (ship.Coords[0].Vertical == 0 && ship.Coords[size - 1].Vertical == 0) || (ship.Coords[0].Vertical == 9 && ship.Coords[size - 1].Vertical == 9))
                return true;
            return false;
        }

        private void FillShipsOnTable()
        {
            for (int i = 0; i < ships.Length; i++)
            {
                for (int j = 0; j < ships[i].Coords.Length; j++)
                {
                    coords[ships[i].Coords[j].Horizontal, ships[i].Coords[j].Vertical].CellType = FieldType.SHIP;
                }
            }
        }

        private void GenerateFreeCoords()
        {
            coords = new Coord[10, 10];
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    coords[i, j] = new Coord(i, j, FieldType.FREE);
                }
            }
        }

        private void GenerateShips()
        {
            ships = new Ship[10];
            ships[0] = new Ship(new Coord[] { new Coord(9, 1, FieldType.SHIP), new Coord(9, 2, FieldType.SHIP), new Coord(9, 3, FieldType.SHIP), new Coord(9, 4, FieldType.SHIP) });
            ships[1] = new Ship(new Coord[] { new Coord(4, 3, FieldType.SHIP), new Coord(5, 3, FieldType.SHIP), new Coord(6, 3, FieldType.SHIP) });
            ships[2] = new Ship(new Coord[] { new Coord(5, 9, FieldType.SHIP), new Coord(6, 9, FieldType.SHIP), new Coord(7, 9, FieldType.SHIP) });
            ships[3] = new Ship(new Coord[] { new Coord(2, 0, FieldType.SHIP), new Coord(3, 0, FieldType.SHIP) });
            ships[4] = new Ship(new Coord[] { new Coord(0, 5, FieldType.SHIP), new Coord(1, 5, FieldType.SHIP) });
            ships[5] = new Ship(new Coord[] { new Coord(9, 6, FieldType.SHIP), new Coord(9, 7, FieldType.SHIP) });
            ships[6] = new Ship(new Coord[] { new Coord(6, 0, FieldType.SHIP) });
            ships[7] = new Ship(new Coord[] { new Coord(2, 2, FieldType.SHIP) });
            ships[8] = new Ship(new Coord[] { new Coord(6, 7, FieldType.SHIP) });
            ships[9] = new Ship(new Coord[] { new Coord(2, 8, FieldType.SHIP) });
        }
    }
}