using UnityEngine;

namespace Assets.Scripts
{
    public class Tile
    {
        public TileType Type;

        public GameObject OccupyingObject;

        public Tile(TileType type, GameObject occupyingObject)
        {
            this.Type = type;
            this.OccupyingObject = occupyingObject;
        }
    }
}