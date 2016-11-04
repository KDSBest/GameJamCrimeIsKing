using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts
{
    public static class FlowField
    {
        public static FlowFieldLocation[,] Search(Point startPosition, Grid map)
        {
            FlowFieldLocation[,] field = new FlowFieldLocation[map.Size.X, map.Size.Y];

            for (int y = 0; y < map.Size.Y; y++)
            {
                for (int x = 0; x < map.Size.X; x++)
                {
                    field[x, y] = new FlowFieldLocation()
                                  {
                                      Position = new Point(x, y)
                                  };
                }
            }

            for (int y = 0; y < map.Size.Y; y++)
            {
                for (int x = 0; x < map.Size.X; x++)
                {
                    var points = GetWalkableAdjacentSquares(new Point(x, y), map);
                    foreach (var point in points)
                    {
                        field[x, y].Neighbours.Add(field[point.X, point.Y]);
                    }
                }
            }

            field[startPosition.X, startPosition.Y].UpdateCost(0);

            return field;
        }

        public static List<Point> GetWalkableAdjacentSquares(Point position, Grid map)
        {
            var proposedLocations = new List<Point>()
            {
                new Point(position.X, position.Y - 1),
                new Point(position.X, position.Y + 1),
                new Point(position.X - 1, position.Y),
                new Point(position.X + 1, position.Y),
            };

            return proposedLocations.Where(l => l.X >= 0 && l.Y >= 0 && l.X < map.Size.X && l.Y < map.Size.Y && map.Tiles[l.X, l.Y].Type == TileType.Walkable).ToList();
        }

        public static int ComputeHScore(int x, int y, int targetX, int targetY)
        {
            return Math.Abs(targetX - x) + Math.Abs(targetY - y);
        }
    }
}