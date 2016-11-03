using UnityEngine;

public class Tile
{
    public enum TileType
    {
        Walkable,
        Wall,
        Door
    }

    public TileType Type;

    public GameObject OccupyingObject;

    public Tile(TileType type, GameObject occupyingObject)
    {
        this.Type = type;
        this.OccupyingObject = occupyingObject;
    }
}
