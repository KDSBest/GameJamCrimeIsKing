using Assets.Scripts;

using UnityEngine;
using UnityEngine.UI;

public class BaseController : MonoBehaviour, IController
{
    public int CurrentActionPoints;

    public int ActionPointsGainPerRound = 10;

    public int ActionPointsMax = 20;

    public Point CurrentPosition = new Point(0, 0);

    public bool HasTurnToken;

    public Text ActionPointCounter;

    public SelectionGrid SelectionGrid;

    public bool canMove = true;

    public virtual void StartTurn()
    {
        Debug.Log("start turn: " + this.GetType().Name);

        this.GainActionPoints();
        this.HasTurnToken = true;

        this.SetActionPointsText();

        if (this.canMove)
        {
            this.SelectionGrid.CalculatePossibleTurns(this);
        }
    }

    public virtual void EndTurn()
    {
        Debug.Log("end turn: " + this.GetType().Name);
        this.HasTurnToken = false;
    }

    public void Awake()
    {
        this.CurrentActionPoints = this.ActionPointsMax;
    }

    public void GainActionPoints()
    {
        this.CurrentActionPoints += this.ActionPointsGainPerRound;
        this.CurrentActionPoints = Mathf.Clamp(this.CurrentActionPoints, 0, this.ActionPointsMax);
        this.SetActionPointsText();
    }

    public void SpendActionPoints(int amount)
    {
        this.CurrentActionPoints -= amount;
        this.CurrentActionPoints = Mathf.Clamp(this.CurrentActionPoints, 0, this.ActionPointsMax);
        this.SetActionPointsText();
    }

    public virtual void MoveTo(Point currentPosition, int actionPointCost, Vector3[] waypoints)
    {
        this.CurrentPosition = currentPosition;
        this.SpendActionPoints(actionPointCost);
        this.SelectionGrid.CalculatePossibleTurns(this);
    }

    private void SetActionPointsText()
    {
        if (this.ActionPointCounter != null)
        {
            this.ActionPointCounter.text = this.CurrentActionPoints.ToString();
        }
    }
}
