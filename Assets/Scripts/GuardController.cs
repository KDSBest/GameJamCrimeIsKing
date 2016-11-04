using Assets.Scripts;

using UnityEngine;

public class GuardController : BaseController
{
    public GameObject Guard;

    public KingController KingController;

    public bool IsSelected = false;

    public int ActionPointsLastFrame;

    public override void StartTurn()
    {
        base.StartTurn();

        this.ActionPointsLastFrame = this.CurrentActionPoints;
    }

    public override void MoveTo(Point currentPosition, int actionPointCost, Vector3[] waypoints)
    {
        base.MoveTo(currentPosition, actionPointCost, waypoints);
        this.Guard.transform.position = new Vector3(this.CurrentPosition.X, this.Guard.transform.position.y, this.CurrentPosition.Y);
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
