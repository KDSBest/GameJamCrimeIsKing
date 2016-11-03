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
        }

        public void SetTile(Tile.TileType type, int x, int y, GameObject occupyingObject)
        {
            this.Tiles[x, y] = new Tile(type, occupyingObject);
        }
    }
}
