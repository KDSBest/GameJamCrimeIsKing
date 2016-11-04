using System.Collections.Generic;

using UnityEngine;

namespace Assets.Scripts
{
    public class Tile
    {
        public TileType Type;

        public GameObject OccupyingObject;

        public bool IsDirectionTile = false;

        public int HP = 1;

        public List<Tile> LinkedTiles = new List<Tile>();

        public bool HasTreasure { get; private set; }

        public bool WasDoor = false;

        public void SetTreasure(bool treasure)
        {
            this.HasTreasure = treasure;
            foreach (var tile in this.LinkedTiles)
                tile.HasTreasure = treasure;
        }

        public void RemoveHP(int hp)
        {
            this.HP -= hp;
            if (this.HP < 0)
                this.HP = 0;
            foreach (var tile in this.LinkedTiles)
                tile.HP = this.HP;
        }

        public Tile(TileType type, GameObject occupyingObject)
        {
            this.Type = type;
            this.OccupyingObject = occupyingObject;
        }
    }
}