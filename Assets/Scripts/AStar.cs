using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts
{
    public static class AStar
    {
        public static AStarLocation Search(Point startPosition, Point endPosition, Grid map)
        {

            var openList = new List<AStarLocation>();
            var closedList = new List<AStarLocation>();
            AStarLocation current = null;
            var start = new AStarLocation()
            {
                Position = startPosition
            };

            var target = new AStarLocation()
            {
                Position = endPosition
            };

            int g = 0;

            // start by adding the original position to the open list
            openList.Add(start);

            while (openList.Count > 0)
            {
                // get the square with the lowest F score
                current = openList.Aggregate((curMin, x) => (curMin == null || x.F < curMin.F ? x : curMin));

                // add the current square to the closed list
                closedList.Add(current);

                // remove it from the open list
                openList.Remove(current);

                if (current.Position.X == target.Position.X && current.Position.Y == target.Position.Y)
                    return current;

                var adjacentSquares = GetWalkableAdjacentSquares(current.Position, map);
                g++;

                foreach (var adjacentSquare in adjacentSquares)
                {
                    // if this adjacent square is already in the closed list, ignore it
                    if (closedList.FirstOrDefault(l => l.Position.X == adjacentSquare.Position.X
                            && l.Position.Y == adjacentSquare.Position.Y) != null)
                        continue;

                    // if it's not in the open list...
                    if (openList.FirstOrDefault(l => l.Position.X == adjacentSquare.Position.X
                            && l.Position.Y == adjacentSquare.Position.Y) == null)
                    {
                        // compute its score, set the parent
                        adjacentSquare.G = g;
                        adjacentSquare.H = ComputeHScore(adjacentSquare.Position.X, adjacentSquare.Position.Y, target.Position.X, target.Position.Y);
                        adjacentSquare.F = adjacentSquare.G + adjacentSquare.H;
                        adjacentSquare.Parent = current;

                        // and add it to the open list
                        openList.Insert(0, adjacentSquare);
                    }
                    else
                    {
                        // test if using the current G score makes the adjacent square's F score
                        // lower, if yes update the parent because it means it's a better path
                        if (g + adjacentSquare.H < adjacentSquare.F)
                        {
                            adjacentSquare.G = g;
                            adjacentSquare.F = adjacentSquare.G + adjacentSquare.H;
                            adjacentSquare.Parent = current;
                        }
                    }
                }
            }

            return null;
        }

        public static List<AStarLocation> GetWalkableAdjacentSquares(Point position, Grid map)
        {
            var proposedLocations = new List<AStarLocation>()
            {
                new AStarLocation { Position = new Point(position.X, position.Y - 1) },
                new AStarLocation { Position = new Point(position.X, position.Y + 1) },
                new AStarLocation { Position = new Point(position.X - 1, position.Y) },
                new AStarLocation { Position = new Point(position.X + 1, position.Y) },
            };

            return proposedLocations.Where(l => l.Position.X >= 0 && l.Position.Y >= 0 && l.Position.X < map.Size.X && l.Position.Y < map.Size.Y && map.Tiles[l.Position.X, l.Position.Y].Type == TileType.Walkable).ToList();
        }

        public static int ComputeHScore(int x, int y, int targetX, int targetY)
        {
            return Math.Abs(targetX - x) + Math.Abs(targetY - y);
        }
    }
}