using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class Grid
    {
        public Tile[,] Tiles;

        public Point Size;

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

        public void SetTile(TileType type, int x, int y, GameObject occupyingObject)
        {
            this.Tiles[x, y] = new Tile(type, occupyingObject);
        }
    }
}
