using System;
using System.Linq;

using UnityEditor;

using UnityEngine;

namespace Assets.Scripts
{
    public class Grid
    {
        public Tile[,] Tiles;

        public Point Size;

        private const char FloorChar = ' ';
        private const char WallChar = '#';

        public Grid(string mapFileContent)
        {
            int mapX = mapFileContent.IndexOfAny(new char[]
                                      {
                                          '\r',
                                          '\n'
                                      });

            string onlyWithAllowedChars = new string(mapFileContent.Where(x => x == FloorChar || x == WallChar).ToArray());
            Debug.Log("X Count: " + mapX);
            int mapY = onlyWithAllowedChars.Length / mapX;
            Debug.Log("Y Count: " + mapY);

            this.Size = new Point(mapX, mapY);
            this.Tiles = new Tile[this.Size.X, this.Size.Y];
            for (int y = 0; y < mapY; y++)
            {
                for (int x = 0; x < mapX; x++)
                {
                    this.Tiles[x, y] = new Tile(TileType.Walkable, null);

                    switch (onlyWithAllowedChars[x + y * mapX])
                    {
                        case '#':
                            this.Tiles[x, y].Type = TileType.Wall;
                            break;
                    }
                }
            }
        }

        public Grid(Point size)
        {
            this.Size = size;
            this.Tiles = new Tile[this.Size.X, this.Size.Y];
            for (int x = 0; x < this.Size.X; x++)
            {
                for (int y = 0; y < this.Size.Y; y++)
                {
                    this.Tiles[x, y] = new Tile(TileType.Walkable, null);
                }
            }
        }

        public Grid(Point size, Tile[,] tiles)
        {
            this.Size = size;
            this.Tiles = tiles;
        }

        public void SetTile(TileType type, int x, int y, GameObject occupyingObject)
        {
            this.Tiles[x, y] = new Tile(type, occupyingObject);
        }

        public void GeneratedMapVisibles(GameObject floor, GameObject wall, GameObject wallL, GameObject wallT, GameObject wallX)
        {
            for (int x = 0; x < this.Size.X; x++)
            {
                for (int y = 0; y < this.Size.Y; y++)
                {
                    if (this.Tiles[x, y].OccupyingObject != null)
                        GameObject.Destroy(this.Tiles[x, y].OccupyingObject);

                    switch (this.Tiles[x, y].Type)
                    {
                        case TileType.Walkable:
                            this.Tiles[x, y].OccupyingObject = GameObject.Instantiate(floor);
                            this.Tiles[x, y].OccupyingObject.transform.position = new Vector3(x, 0, y);
                            break;
                        case TileType.Wall:
                            WallType wallType = this.CalculateWallType(x, y);

                            switch (wallType)
                            {
                                case WallType.I:
                                    this.Tiles[x, y].OccupyingObject = GameObject.Instantiate(wall);
                                    break;
                                case WallType.L:
                                    this.Tiles[x, y].OccupyingObject = GameObject.Instantiate(wallL);
                                    break;
                                case WallType.T:
                                    this.Tiles[x, y].OccupyingObject = GameObject.Instantiate(wallT);
                                    break;
                                case WallType.X:
                                    this.Tiles[x, y].OccupyingObject = GameObject.Instantiate(wallX);
                                    break;
                            }
                            this.Tiles[x, y].OccupyingObject.transform.position = new Vector3(x, 0, y);
                            break;
                    }
                }
            }
        }

        private WallType CalculateWallType(int x, int y)
        {
            bool up = false;
            bool down = false;
            bool left = false;
            bool right = false;

            if (x - 1 >= 0)
            {
                left = this.Tiles[x - 1, y].Type == TileType.Wall;
            }

            if (y - 1 >= 0)
            {
                down = this.Tiles[x, y - 1].Type == TileType.Wall;
            }

            if (x + 1 < this.Size.X)
            {
                right = this.Tiles[x + 1, y].Type == TileType.Wall;
            }

            if (y + 1 < this.Size.Y)
            {
                up = this.Tiles[x, y + 1].Type == TileType.Wall;
            }

            if (up && down && left && right)
                return WallType.X;

            if (up && down && left)
                return WallType.T;

            if (up && down && right)
                return WallType.T;

            if (up && left && right)
                return WallType.T;

            if (down && left && right)
                return WallType.T;

            if (up && left)
                return WallType.L;

            if (up && right)
                return WallType.L;

            if (down && left)
                return WallType.L;

            if (down && right)
                return WallType.L;

            return WallType.I;
        }
    }
}
