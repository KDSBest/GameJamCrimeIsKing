using UnityEngine;

namespace Assets.Scripts
{
    public class Tile
    {
        public TileType Type;

        public GameObject OccupyingObject;

        public bool IsDirectionTile = false;

        public int HP = 1;

        public Tile(TileType type, GameObject occupyingObject)
        {
            this.Type = type;
            this.OccupyingObject = occupyingObject;
        }
    }
}