using Assets.Scripts;

using DG.Tweening;

using UnityEngine;

public class GuardController : BaseController
{
    public GameObject Guard;

    public KingController KingController;

    public bool IsSelected = false;

    public int ActionPointsLastFrame;

    public bool hasArrived;

    private Point currentMoveEndPoint;

    private int currentMoveActionCost;

    private Vector3[] currentMoveWaypoints;

    public Transform GuardPivot;

    public override void StartTurn()
    {
        base.StartTurn();

        this.ActionPointsLastFrame = this.CurrentActionPoints;
    }

    public override TileType GetIgnoreType()
    {
        return TileType.Guard;
    }

    public void Awake()
    {
        base.Awake();

        var spawn = RandomHelper.RandomSelect(Bootstrap.Instance.Map.PossibleGuardSpawns);

        Debug.Log("Guard: " + spawn.X + " " + spawn.Y);
        this.CurrentPosition = spawn;
        Bootstrap.Instance.Map.Tiles[spawn.X, spawn.Y].Type = TileType.Guard;
        Bootstrap.Instance.Map.PossibleGuardSpawns.Remove(spawn);
        this.CheckAdjacentTiles();

        ForceCurrentPosition();
    }

    public override void MoveTo(Point currentPosition, int actionPointCost, Vector3[] waypoints)
    {
        this.currentMoveEndPoint = currentPosition;
        this.currentMoveActionCost = actionPointCost;
        this.currentMoveWaypoints = waypoints;

        this.hasArrived = false;
        this.Guard.transform.DOPath(waypoints, waypoints.Length * 0.2f, PathType.CatmullRom, PathMode.Full3D, 5, Color.cyan).SetLookAt(0.01f);
        this.GuardPivot.transform.DOPunchRotation(new Vector3(-20, 0, 0), waypoints.Length * 0.2f, 2, 0.5f);

        this.Invoke("UpdateWalkableTiles", waypoints.Length * 0.2f + 0.5f);
        Bootstrap.Instance.Map.Tiles[this.currentMoveEndPoint.X, this.currentMoveEndPoint.Y].Type = TileType.Guard;
        Bootstrap.Instance.Map.Tiles[this.currentMoveEndPoint.X, this.currentMoveEndPoint.Y].GuardIndex = this.Index;
    }

    public override void ProcessAdjacentTile(Point position, Tile tile)
    {
        Point positionCopyIntoClosure = position;
        var tileCopyIntoClosure = tile;
        switch (tile.Type)
        {
            case TileType.Wall:
            case TileType.DoorFrame:
            case TileType.BedHead:
            case TileType.BedFoot:
            case TileType.Cupboard:
            case TileType.Guard:
                break;
            case TileType.Walkable:

                if (tileCopyIntoClosure.WasDoor)
                {
                    SpawnButton(true, positionCopyIntoClosure, tileCopyIntoClosure, () =>
                    {
                        tileCopyIntoClosure.Type = TileType.Door;
                        tileCopyIntoClosure.HP = 1;
                    }, "Close Door");
                }
                break;
            case TileType.Door:
                SpawnButton(true, positionCopyIntoClosure, tileCopyIntoClosure, () =>
                {
                    tileCopyIntoClosure.Type = TileType.Walkable;
                    tileCopyIntoClosure.HP = 1;
                }, "Open Door");
                break;
            case TileType.Thief:
                SpawnButton(true, positionCopyIntoClosure, tileCopyIntoClosure, () =>
                {
                    this.HasWon = true;
                }, "Catch");
                break;
        }
    }

    public void UpdateWalkableTiles()
    {
        base.MoveTo(this.currentMoveEndPoint, this.currentMoveActionCost, this.currentMoveWaypoints);
        this.hasArrived = true;
    }

    public void ContinueTurn()
    {
        if (this.canMove && this.IsSelected)
        {
            this.SelectionGrid.CalculatePossibleTurns(this.CurrentPosition, this.CurrentActionPoints, this.GetIgnoreType(), this.Index);
        }
    }

    public void UpdateController()
    {
        if (this.IsSelected)
        {
            this.SelectionGrid.Select(this);
            if (this.ActionPointsLastFrame != this.CurrentActionPoints)
            {
                this.KingController.SpendActionPoints(this.CurrentActionPoints - this.ActionPointsLastFrame);
            }
        }

        this.ActionPointsLastFrame = this.CurrentActionPoints;
    }
}
