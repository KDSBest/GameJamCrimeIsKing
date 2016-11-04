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
        private const char BedHeadChar = 'B';
        private const char BedFootChar = 'b';

        public Grid(string mapFileContent)
        {
            int mapX = mapFileContent.IndexOfAny(new char[]
                                      {
                                          '\r',
                                          '\n'
                                      });

            string onlyWithAllowedChars = new string(mapFileContent.Where(x => x != '\r' && x != '\n').ToArray());
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
                        case WallChar:
                            this.Tiles[x, y].Type = TileType.Wall;
                            break;
                        case BedHeadChar:
                            this.Tiles[x, y].Type = TileType.BedHead;
                            break;
                        case BedFootChar:
                            this.Tiles[x, y].Type = TileType.BedFoot;
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

        public void GeneratedMapVisibles(GameObject floor, GameObject wall, GameObject wallL, GameObject wallT, GameObject wallX, GameObject bed)
        {
            for (int x = 0; x < this.Size.X; x++)
            {
                for (int y = 0; y < this.Size.Y; y++)
                {
                    if (this.Tiles[x, y].OccupyingObject != null)
                        GameObject.Destroy(this.Tiles[x, y].OccupyingObject);
                }
            }

            for (int x = 0; x < this.Size.X; x++)
            {
                for (int y = 0; y < this.Size.Y; y++)
                {
                    switch (this.Tiles[x, y].Type)
                    {
                        case TileType.Walkable:
                            ProcessWalkable(floor, x, y);
                            break;
                        case TileType.BedHead:
                            this.ProcessBedHead(bed, x, y);
                            break;
                        case TileType.BedFoot:
                            break;
                        case TileType.Wall:
                            this.ProcessWall(wall, wallL, wallT, wallX, x, y);
                            break;
                    }
                }
            }
        }

        private void ProcessWall(GameObject wall, GameObject wallL, GameObject wallT, GameObject wallX, int x, int y)
        {
            var wallResult = this.CalculateWallType(x, y);

            switch (wallResult.Type)
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
            this.Tiles[x, y].OccupyingObject.transform.rotation = Quaternion.AngleAxis(wallResult.Rotation, Vector3.up);
            this.Tiles[x, y].OccupyingObject.transform.position = new Vector3(x, this.Tiles[x, y].OccupyingObject.transform.position.y, y);
        }

        private void ProcessBedHead(GameObject bed, int x, int y)
        {
            var bedResult = this.CalculateBedType(x, y);

            this.Tiles[x, y].OccupyingObject = GameObject.Instantiate(bed);
            Debug.Log(this.Tiles[x, y].OccupyingObject.GetInstanceID());
            Debug.Log(x + " " + y + " foot: " + bedResult.FootPosition.X + " " + bedResult.FootPosition.Y + " rotation: " + bedResult.Rotation);
            Vector2 bedPosition = new Vector2(x, y);
            bedPosition += new Vector2(bedResult.FootPosition.X, bedResult.FootPosition.Y);
            bedPosition /= 2;

            this.Tiles[x, y].OccupyingObject.transform.rotation = Quaternion.AngleAxis(bedResult.Rotation, Vector3.up);
            this.Tiles[x, y].OccupyingObject.transform.position = new Vector3(bedPosition.x, this.Tiles[x, y].OccupyingObject.transform.position.y, bedPosition.y);
            this.Tiles[bedResult.FootPosition.X, bedResult.FootPosition.Y].OccupyingObject = this.Tiles[x, y].OccupyingObject;
        }

        private void ProcessWalkable(GameObject floor, int x, int y)
        {
            this.Tiles[x, y].OccupyingObject = GameObject.Instantiate(floor);
            this.Tiles[x, y].OccupyingObject.transform.position = new Vector3(x, 0, y);
        }

        private BedTypeResult CalculateBedType(int x, int y)
        {
            bool up = false;
            bool down = false;
            bool left = false;
            bool right = false;

            if (x - 1 >= 0 && this.Tiles[x - 1, y].Type == TileType.BedFoot)
            {
                return new BedTypeResult()
                {
                    Rotation =  180,
                    FootPosition = new Point(x - 1, y)
                };
            }

            if (y - 1 >= 0 && this.Tiles[x, y - 1].Type == TileType.BedFoot)
            {
                return new BedTypeResult()
                {
                    Rotation = 90,
                    FootPosition = new Point(x, y - 1)
                };
            }

            if (x + 1 < this.Size.X && this.Tiles[x + 1, y].Type == TileType.BedFoot)
            {
                return new BedTypeResult()
                {
                    Rotation = 0,
                    FootPosition = new Point(x + 1, y)
                };
            }

            return new BedTypeResult()
            {
                Rotation = -90,
                FootPosition = new Point(x, y + 1)
            };
        }

        private WallTypeResult CalculateWallType(int x, int y)
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

            if (up || down)
                return WallType.I;

            return new WallTypeResult()
            {
                Type = WallType.I,
                Rotation = 90
            };
        }
    }
}
