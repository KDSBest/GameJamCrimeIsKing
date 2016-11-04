using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;

using UnityEngine;

namespace Assets.Scripts
{
    public class Grid
    {

        private List<TileType> NoVisionBlockers = new List<TileType>()
                                                  {
                                                      TileType.Wall,
                                                      TileType.Door,
                                                      TileType.DoorFrame
                                                  };
        public Tile[,] Tiles;

        public Point Size;

        public List<Tile> PossibleTreasureTiles = new List<Tile>();
        public List<Point> PossibleThiefSpawns = new List<Point>();

        private const char FloorChar = ' ';
        private const char WallChar = '#';
        private const char BedHeadChar = 'B';
        private const char BedFootChar = 'b';
        private const char DoorChar = 'D';
        private const char DoorFrameChar = 'd';
        private const char ThiefChar = 'T';
        private const char GuardChar = 'G';
        private const char CupboardChar = 'S';
        private const char WalkableDirectionChar = '~';

        private const int CupboardHP = 15;

        private GameObject GetVisionBlocker(GameObject visionBlocker, GameObject parent, int x, int y)
        {
            GameObject go = GameObject.Instantiate(visionBlocker);
            go.transform.position = new Vector3(x, 0, y);
            go.transform.SetParent(parent.transform);

            return go;
        }

        public Grid(string mapFileContent, GameObject visionBlocker, GameObject parent)
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
                    this.Tiles[x, y] = new Tile(TileType.Walkable, null, GetVisionBlocker(visionBlocker, parent, x, y));

                    switch (onlyWithAllowedChars[x + y * mapX])
                    {
                        case WallChar:
                            this.Tiles[x, y].Type = TileType.Wall;
                            break;
                        case BedHeadChar:
                            this.Tiles[x, y].Type = TileType.BedHead;
                            this.PossibleTreasureTiles.Add(this.Tiles[x, y]);
                            break;
                        case BedFootChar:
                            this.Tiles[x, y].Type = TileType.BedFoot;
                            break;
                        case DoorFrameChar:
                            this.Tiles[x, y].Type = TileType.DoorFrame;
                            break;
                        case DoorChar:
                            this.Tiles[x, y].Type = TileType.Door;
                            this.Tiles[x, y].WasDoor = true;
                            break;
                        case ThiefChar:
                            this.Tiles[x, y].Type = TileType.Walkable;
                            this.PossibleThiefSpawns.Add(new Point(x, y));
                            break;
                        case GuardChar:
                            this.Tiles[x, y].Type = TileType.Guard;
                            break;
                        case CupboardChar:
                            this.Tiles[x, y].Type = TileType.Cupboard;
                            this.PossibleTreasureTiles.Add(this.Tiles[x, y]);
                            break;
                        case WalkableDirectionChar:
                            this.Tiles[x, y].Type = TileType.Walkable;
                            this.Tiles[x, y].IsDirectionTile = true;
                            break;
                    }

                    if (this.NoVisionBlockers.Contains(this.Tiles[x, y].Type))
                        RemoveVisionBlocker(y, x);
                }
            }
        }

        private void RemoveVisionBlocker(int y, int x)
        {
            GameObject.Destroy(this.Tiles[x, y].VisionBlocker);
            this.Tiles[x, y].VisionBlocker = null;
        }

        public Grid(Point size, Tile[,] tiles)
        {
            this.Size = size;
            this.Tiles = tiles;
        }

        public void GeneratedMapVisibles(GameObject parent, GameObject floor, GameObject wall, GameObject wallL, GameObject wallT, GameObject wallX, GameObject bed, GameObject door, GameObject cupboard)
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
                        case TileType.Wall:
                            this.ProcessWall(wall, wallL, wallT, wallX, x, y);
                            break;
                        case TileType.Door:
                            ProcessDoor(door, x, y);
                            break;
                        case TileType.Cupboard:
                            ProcessCupboard(cupboard, x, y);
                            break;
                        case TileType.BedFoot:
                        case TileType.DoorFrame:
                            break;
                    }
                }
            }

            for (int x = 0; x < this.Size.X; x++)
            {
                for (int y = 0; y < this.Size.Y; y++)
                {
                    if (this.Tiles[x, y].OccupyingObject != null)
                        this.Tiles[x, y].OccupyingObject.transform.SetParent(parent.transform);
                }
            }
        }

        private void ProcessCupboard(GameObject cupboard, int x, int y)
        {
            int rotation = this.CalculateCupboard(x, y);

            this.Tiles[x, y].OccupyingObject = GameObject.Instantiate(cupboard);

            this.Tiles[x, y].OccupyingObject.transform.rotation = Quaternion.AngleAxis(rotation, Vector3.up);
            this.Tiles[x, y].OccupyingObject.transform.position = new Vector3(x, this.Tiles[x, y].OccupyingObject.transform.position.y, y);
            this.Tiles[x, y].HP = CupboardHP;
        }

        private void ProcessDoor(GameObject door, int x, int y)
        {
            var doorResult = this.CalculateDoorType(x, y);

            this.Tiles[x, y].OccupyingObject = GameObject.Instantiate(door);
            Vector2 doorPosition = new Vector2(x, y);
            doorPosition += (Vector2)doorResult.Frames[0];
            doorPosition += (Vector2)doorResult.Frames[1];
            doorPosition /= 3;

            this.Tiles[x, y].OccupyingObject.transform.rotation = Quaternion.AngleAxis(doorResult.Rotation, Vector3.up);
            this.Tiles[x, y].OccupyingObject.transform.position = new Vector3(doorPosition.x, this.Tiles[x, y].OccupyingObject.transform.position.y, doorPosition.y);
            this.Tiles[doorResult.Frames[0].X, doorResult.Frames[0].Y].OccupyingObject = this.Tiles[x, y].OccupyingObject;
            this.Tiles[doorResult.Frames[1].X, doorResult.Frames[1].Y].OccupyingObject = this.Tiles[x, y].OccupyingObject;
            this.LinkTiles(this.Tiles[x, y], this.Tiles[doorResult.Frames[0].X, doorResult.Frames[0].Y], this.Tiles[doorResult.Frames[1].X, doorResult.Frames[1].Y]);
        }

        private void LinkTiles(params Tile[] tiles)
        {
            for (int i = 0; i < tiles.Length; i++)
            {
                for (int ii = 0; ii < tiles.Length; ii++)
                {
                    if (i == ii)
                        continue;

                    tiles[i].LinkedTiles.Add(tiles[ii]);
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
            Vector2 bedPosition = new Vector2(x, y);
            bedPosition += (Vector2)bedResult.FootPosition;
            bedPosition /= 2;

            this.Tiles[x, y].OccupyingObject.transform.rotation = Quaternion.AngleAxis(bedResult.Rotation, Vector3.up);
            this.Tiles[x, y].OccupyingObject.transform.position = new Vector3(bedPosition.x, this.Tiles[x, y].OccupyingObject.transform.position.y, bedPosition.y);
            this.Tiles[bedResult.FootPosition.X, bedResult.FootPosition.Y].OccupyingObject = this.Tiles[x, y].OccupyingObject;
            this.LinkTiles(this.Tiles[x, y], this.Tiles[bedResult.FootPosition.X, bedResult.FootPosition.Y]);
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
                    Rotation = 180,
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

        private int CalculateCupboard(int x, int y)
        {
            bool up = false;
            bool down = false;
            bool left = false;
            bool right = false;

            if (x - 1 >= 0)
            {
                left = this.Tiles[x - 1, y].IsDirectionTile;
                if (left)
                    return -90;
            }

            if (y - 1 >= 0)
            {
                down = this.Tiles[x, y - 1].IsDirectionTile;
                if (down)
                    return 180;
            }

            if (x + 1 < this.Size.X)
            {
                right = this.Tiles[x + 1, y].IsDirectionTile;
                if (right)
                    return 90;
            }

            if (y + 1 < this.Size.Y)
            {
                up = this.Tiles[x, y + 1].IsDirectionTile;
                if (up)
                    return 0;
            }

            return 0;
        }

        private DoorTypeResult CalculateDoorType(int x, int y)
        {
            bool up = false;
            bool down = false;
            bool left = false;
            bool right = false;

            if (x - 1 >= 0)
            {
                left = this.Tiles[x - 1, y].Type == TileType.DoorFrame;
            }

            if (y - 1 >= 0)
            {
                down = this.Tiles[x, y - 1].Type == TileType.DoorFrame;
            }

            if (x + 1 < this.Size.X)
            {
                right = this.Tiles[x + 1, y].Type == TileType.DoorFrame;
            }

            if (y + 1 < this.Size.Y)
            {
                up = this.Tiles[x, y + 1].Type == TileType.DoorFrame;
            }

            if (up && down)
                return new DoorTypeResult()
                {
                    Frames = new Point[]
                                    {
                                        new Point(x, y - 1),
                                        new Point(x, y + 1)
                                    },
                    Rotation = 0
                };

            return new DoorTypeResult()
            {
                Frames = new Point[]
                                {
                                        new Point(x - 1, y),
                                        new Point(x + 1, y)
                                },
                Rotation = 90
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
                return new WallTypeResult()
                {
                    Rotation = 0,
                    Type = WallType.T
                };

            if (up && down && right)
                return new WallTypeResult()
                {
                    Rotation = 180,
                    Type = WallType.T
                };

            if (up && left && right)
                return new WallTypeResult()
                {
                    Rotation = 90,
                    Type = WallType.T
                };

            if (down && left && right)
                return new WallTypeResult()
                {
                    Rotation = -90,
                    Type = WallType.T
                };

            if (up && left)
                return new WallTypeResult()
                {
                    Rotation = 90,
                    Type = WallType.L
                };

            if (up && right)
                return new WallTypeResult()
                {
                    Rotation = 180,
                    Type = WallType.L
                };

            if (down && left)
                return new WallTypeResult()
                {
                    Rotation = 0,
                    Type = WallType.L
                };

            if (down && right)
                return new WallTypeResult()
                {
                    Rotation = -90,
                    Type = WallType.L
                };

            if (up || down)
                return WallType.I;

            return new WallTypeResult()
            {
                Type = WallType.I,
                Rotation = 90
            };
        }

        public void Traverse(Func<int, int, Tile, bool> func)
        {
            for (int y = 0; y < this.Size.Y; y++)
            {
                for (int x = 0; x < this.Size.X; x++)
                {
                    if (!func(x, y, this.Tiles[x, y]))
                        return;
                }
            }
        }
    }
}
