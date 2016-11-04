using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

using Assets.Scripts;

using UnityEngine;
using UnityEngine.Events;
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
    public GameObject Button;

    public GameObject ButtonParent;

    [HideInInspector]
    public bool HasWon { get; set; }
    private List<GameObject> actionButtons = new List<GameObject>();
    private List<Point> actionButtonsPositions = new List<Point>();

    protected void TryToKillTile(Tile tile)
    {
        if (this.CurrentActionPoints >= tile.HP)
        {
            this.SpendActionPoints(tile.HP);
            tile.RemoveHP(tile.HP);
        }
        else
        {
            tile.RemoveHP(this.CurrentActionPoints);
            this.SpendActionPoints(this.CurrentActionPoints);
        }
    }

    protected void SpawnButton(Point position, Tile tile, UnityAction buttonAction, string text)
    {
        this.actionButtons.Add(GameObject.Instantiate(this.Button));
        this.actionButtonsPositions.Add(position);
        this.actionButtons[this.actionButtons.Count - 1].transform.SetParent(this.ButtonParent.transform);
        var button = this.actionButtons[this.actionButtons.Count - 1].GetComponent<Button>();
        button.onClick.AddListener(new UnityAction(() =>
        {
            TryToKillTile(tile);
            if (tile.HP == 0)
            {
                buttonAction();
            }
            this.UpdateUIElements();
        }));
        button.GetComponentInChildren<Text>().text = text;
    }

    public void UpdateButtonPositions()
    {
        for (int i = 0; i < this.actionButtonsPositions.Count; i++)
        {
            this.actionButtons[i].transform.position = Camera.main.WorldToScreenPoint(new Vector3(this.actionButtonsPositions[i].X, 0, this.actionButtonsPositions[i].Y));
        }
    }

    public bool IsValidTilePosition(int x, int y)
    {
        if (x < 0)
            return false;

        if (y < 0)
            return false;

        if (x >= Bootstrap.Instance.Map.Size.X)
            return false;

        if (y >= Bootstrap.Instance.Map.Size.Y)
            return false;

        return true;
    }

    public virtual void StartTurn()
    {
        Debug.Log("start turn: " + this.GetType().Name);

        this.GainActionPoints();
        this.HasTurnToken = true;

        this.SetActionPointsText();

        if (this.canMove)
        {
            this.SelectionGrid.CalculatePossibleTurns(this.CurrentPosition, this.CurrentActionPoints);
        }
    }

    public virtual void EndTurn()
    {
        Debug.Log("end turn: " + this.GetType().Name);
        this.HasTurnToken = false;
        this.RemoveUIButtonsForActions();
    }

    public void Awake()
    {
        this.SelectionGrid = GameObject.FindObjectOfType<SelectionGrid>();
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
        Bootstrap.Instance.Map.Tiles[this.CurrentPosition.X, this.CurrentPosition.Y].Type = TileType.Walkable;
        this.CurrentPosition = currentPosition;
        this.SpendActionPoints(actionPointCost);
        this.UpdateUIElements();
    }

    protected void UpdateUIElements()
    {
        this.CheckAdjacentTiles();
        this.UpdateButtonPositions();
        this.SelectionGrid.CalculatePossibleTurns(this.CurrentPosition, this.CurrentActionPoints);
    }

    protected virtual void CheckAdjacentTiles()
    {
        RemoveUIButtonsForActions();
    }

    public void LateUpdate()
    {
        UpdateButtonPositions();
    }

    protected void RemoveUIButtonsForActions()
    {
        foreach (var go in actionButtons)
            GameObject.Destroy(go);

        this.actionButtonsPositions.Clear();
        this.actionButtons.Clear();
    }

    private void SetActionPointsText()
    {
        if (this.ActionPointCounter != null)
        {
            this.ActionPointCounter.text = this.CurrentActionPoints.ToString();
        }
    }

    protected void ForceCurrentPosition()
    {
        this.transform.position = new Vector3(this.CurrentPosition.X, this.transform.position.y, this.CurrentPosition.Y);
    }
}
