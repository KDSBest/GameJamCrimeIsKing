using Assets.Scripts;

using DG.Tweening;

using UnityEngine;
using UnityEngine.UI;

public class CriminalController : BaseController
{
    public GameObject Criminal;

    public ActorSkill[] Skills = new ActorSkill[4];

    public Moba_Camera mobaCam;

    private Point currentMoveEndPoint;

    private int currentMoveActionCost;

    private Vector3[] currentMoveWaypoints;

    public int Treasures = 0;

    public Text TreasureText;

    public override void MoveTo(Point currentPosition, int actionPointCost, Vector3[] waypoints)
    {
        this.currentMoveEndPoint = currentPosition;
        this.currentMoveActionCost = actionPointCost;
        this.currentMoveWaypoints = waypoints;

        this.Criminal.transform.DOPath(waypoints, waypoints.Length * 0.2f, PathType.CatmullRom, PathMode.TopDown2D, 5, Color.cyan);
        this.Invoke("UpdateWalkableTiles", waypoints.Length * 0.2f);
        Bootstrap.Instance.Map.Tiles[this.currentMoveEndPoint.X, this.currentMoveEndPoint.Y].Type = TileType.Thief;
    }

    public void UpdateWalkableTiles()
    {
        base.MoveTo(this.currentMoveEndPoint, this.currentMoveActionCost, this.currentMoveWaypoints);
    }

    public void Update()
    {
        if (!this.HasTurnToken)
        {
            return;
        }

        this.SelectionGrid.Select(this);

        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            this.ExecuteSkill(0);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            this.ExecuteSkill(1);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            this.ExecuteSkill(2);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            this.ExecuteSkill(3);
        }
    }

    protected override void CheckAdjacentTiles()
    {
        base.CheckAdjacentTiles();
        var positionToCheck = this.CurrentPosition + new Point(1, 0);
        if (this.IsValidTilePosition(positionToCheck.X, positionToCheck.Y))
        {
            ProcessAdjacentTile(positionToCheck, Bootstrap.Instance.Map.Tiles[positionToCheck.X, positionToCheck.Y]);
        }

        positionToCheck = this.CurrentPosition + new Point(-1, 0);
        if (this.IsValidTilePosition(positionToCheck.X, positionToCheck.Y))
        {
            ProcessAdjacentTile(positionToCheck, Bootstrap.Instance.Map.Tiles[positionToCheck.X, positionToCheck.Y]);
        }

        positionToCheck = this.CurrentPosition + new Point(0, 1);
        if (this.IsValidTilePosition(positionToCheck.X, positionToCheck.Y))
        {
            ProcessAdjacentTile(positionToCheck, Bootstrap.Instance.Map.Tiles[positionToCheck.X, positionToCheck.Y]);
        }

        positionToCheck = this.CurrentPosition + new Point(0, -1);
        if (this.IsValidTilePosition(positionToCheck.X, positionToCheck.Y))
        {
            ProcessAdjacentTile(positionToCheck, Bootstrap.Instance.Map.Tiles[positionToCheck.X, positionToCheck.Y]);
        }
    }

    public override void StartTurn()
    {
        base.StartTurn();
        this.mobaCam.settings.lockTargetTransform = this.transform;
        this.mobaCam.settings.cameraLocked = true;

        this.CheckAdjacentTiles();
    }

    private void GiveTreasure()
    {
        this.Treasures++;
        this.TreasureText.text = this.Treasures.ToString();
    }

    public void ProcessAdjacentTile(Point position, Tile tile)
    {
        Point positionCopyIntoClosure = position;
        var tileCopyIntoClosure = tile;
        switch (tile.Type)
        {
            case TileType.Wall:
            case TileType.DoorFrame:
            case TileType.Thief:
            case TileType.Guard:
                break;
            case TileType.Walkable:

                if (tileCopyIntoClosure.WasDoor)
                {
                    SpawnButton(positionCopyIntoClosure, tileCopyIntoClosure, () =>
                    {
                        tileCopyIntoClosure.Type = TileType.Door;
                        tileCopyIntoClosure.HP = 1;
                    }, "Close Door (" + tileCopyIntoClosure.HP + ")");
                }
                break;
            case TileType.Door:
                SpawnButton(positionCopyIntoClosure, tileCopyIntoClosure, () =>
                {
                    tileCopyIntoClosure.Type = TileType.Walkable;
                    tileCopyIntoClosure.HP = 1;
                }, "Open Door (" + tileCopyIntoClosure.HP + ")");
                break;
            case TileType.BedHead:
            case TileType.BedFoot:
            case TileType.Cupboard:
                if (tileCopyIntoClosure.HP > 0 || tileCopyIntoClosure.HasTreasure)
                {
                    SpawnButton(positionCopyIntoClosure, tileCopyIntoClosure, () =>
                    {
                        if (tileCopyIntoClosure.HasTreasure)
                        {
                            tileCopyIntoClosure.SetTreasure(false);
                            this.GiveTreasure();
                            Debug.Log("You got " + this.Treasures + " Treasures.");
                        }
                        else
                        {
                            Debug.Log("Nothing found!");
                        }
                    }, "Search (" + tileCopyIntoClosure.HP + ")");
                }
                break;
        }
    }

    public void Awake()
    {
        base.Awake();
        Bootstrap.Instance.Map.Traverse((x, y, tile) =>
        {
            if (tile.Type == TileType.Thief)
            {
                this.CurrentPosition = new Point(x, y);
                this.CheckAdjacentTiles();
                return false;
            }

            return true;
        });

        ForceCurrentPosition();

    }

    private void ExecuteSkill(int id)
    {
        ActorSkill skill = this.Skills[id];

        if (this.CurrentActionPoints >= skill.ActionPointCost)
        {
            this.SpendActionPoints(skill.ActionPointCost);
            skill.Execute();
        }
    }
}
