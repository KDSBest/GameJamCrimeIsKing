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

    public void Start()
    {
        this.CurrentActionPoints = this.ActionPointsMax;
    }

    public virtual void StartTurn()
    {
        Debug.Log("start turn: " + this.GetType().Name);

        this.GainActionPoints();
        this.HasTurnToken = true;

        this.ActionPointCounter.text = this.CurrentActionPoints.ToString();

        this.SelectionGrid.CalculatePossibleTurns(this);
    }

    public void GainActionPoints()
    {
        this.CurrentActionPoints += this.ActionPointsGainPerRound;
        this.CurrentActionPoints = Mathf.Clamp(this.CurrentActionPoints, 0, this.ActionPointsMax);
        this.ActionPointCounter.text = this.CurrentActionPoints.ToString();

    }

    public void SpendActionPoints(int amount)
    {
        this.CurrentActionPoints -= amount;
        this.CurrentActionPoints = Mathf.Clamp(this.CurrentActionPoints, 0, this.ActionPointsMax);
        this.ActionPointCounter.text = this.CurrentActionPoints.ToString();

    }

    public virtual void AddWaypoint(int posX, int posY)
    {
        throw new System.NotImplementedException();
    }

    public virtual void EndTurn()
    {
        Debug.Log("end turn: " + this.GetType().Name);
        this.HasTurnToken = false;
    }

    public virtual void MoveTo(Point currentPosition, int actionPointCost)
    {
        this.CurrentPosition = currentPosition;
        this.CurrentActionPoints -= actionPointCost;
        this.SelectionGrid.CalculatePossibleTurns(this);
    }
}
