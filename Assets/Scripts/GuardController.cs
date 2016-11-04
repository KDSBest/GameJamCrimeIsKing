using Assets.Scripts;

using DG.Tweening;

using UnityEngine;

public class GuardController : BaseController
{
    public GameObject Guard;

    public KingController KingController;

    public bool IsSelected = false;

    public int ActionPointsLastFrame;

    public int Index = 0;

    public bool hasArrived;

    private Point currentMoveEndPoint;

    private int currentMoveActionCost;

    private Vector3[] currentMoveWaypoints;

    public override void StartTurn()
    {
        base.StartTurn();

        this.ActionPointsLastFrame = this.CurrentActionPoints;
    }

    public void Awake()
    {
        base.Awake();
        int index = 0;
        Bootstrap.Instance.Map.Traverse((x, y, tile) =>
        {
            if (tile.Type == TileType.Guard)
            {
                if (index == this.Index)
                {
                    this.CurrentPosition = new Point(x, y);
                    this.CheckAdjacentTiles();
                    return false;
                }
                index++;
            }

            return true;
        });

        ForceCurrentPosition();
    }

    public override void MoveTo(Point currentPosition, int actionPointCost, Vector3[] waypoints)
    {
        this.currentMoveEndPoint = currentPosition;
        this.currentMoveActionCost = actionPointCost;
        this.currentMoveWaypoints = waypoints;

        this.hasArrived = false;
        base.MoveTo(currentPosition, actionPointCost, waypoints);
        this.Guard.transform.DOPath(waypoints, waypoints.Length * 0.2f, PathType.CatmullRom, PathMode.TopDown2D, 5, Color.cyan);
        this.Invoke("UpdateWalkableTiles", waypoints.Length * 0.2f + 0.5f);
        Bootstrap.Instance.Map.Tiles[this.currentMoveEndPoint.X, this.currentMoveEndPoint.Y].Type = TileType.Thief;
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
            this.SelectionGrid.CalculatePossibleTurns(this);
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
