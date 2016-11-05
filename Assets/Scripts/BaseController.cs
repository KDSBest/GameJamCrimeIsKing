using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

using Assets.Scripts;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class BaseController : MonoBehaviour, IController
{

    protected List<TileType> Blocking = new List<TileType>()
                                      {
        TileType.Wall, TileType.Door, TileType.DoorFrame, TileType.Cupboard,
        TileType.LockerHigh
                                      };

    public int CurrentActionPoints;

    public int ActionPointsGainPerRound = 10;

    public int ActionPointsMax = 20;

    public int Vision = 20;

    public Point CurrentPosition = new Point(0, 0);

    public bool HasTurnToken;

    public Text ActionPointCounter;

    public SelectionGrid SelectionGrid;

    public bool canMove = true;
    public GameObject Button;

    public GameObject ButtonParent;

    [HideInInspector]
    public bool HasWon { get; set; }

    public int Index = 0;

    private List<GameObject> actionButtons = new List<GameObject>();
    private List<Point> actionButtonsPositions = new List<Point>();

    private List<GameObject> visionBlockersDeactivated = new List<GameObject>();

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

    protected void SpawnButton(bool IsFreeAction, Point position, Tile tile, UnityAction buttonAction, string text)
    {
        this.actionButtons.Add(GameObject.Instantiate(this.Button));
        this.actionButtonsPositions.Add(position);
        this.actionButtons[this.actionButtons.Count - 1].transform.SetParent(this.ButtonParent.transform);
        var button = this.actionButtons[this.actionButtons.Count - 1].GetComponent<Button>();
        button.onClick.AddListener(new UnityAction(() =>
        {
            if (IsFreeAction)
            {
                buttonAction();
            }
            else
            {
                TryToKillTile(tile);
                if (tile.HP == 0)
                {
                    buttonAction();
                }
            }
            this.UpdateUIElements();
        }));
        button.GetComponentInChildren<Text>().text = text;
    }

    public void UpdateButtonPositions()
    {
        for (int i = 0; i < this.actionButtonsPositions.Count; i++)
        {
            this.actionButtons[i].transform.position = Camera.main.WorldToScreenPoint(new Vector3(this.actionButtonsPositions[i].X, -1.5f, this.actionButtonsPositions[i].Y));
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
            this.UpdateUIElements();
        }
    }

    public abstract TileType GetIgnoreType();

    public virtual void EndTurn()
    {
        Debug.Log("end turn: " + this.GetType().Name);
        this.HasTurnToken = false;
        this.RemoveUIButtonsForActions();
        this.ShowDeactivatedVisionBlockers();
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

        if (this.HasTurnToken)
            this.UpdateUIElements();
    }

    public List<Point> LineToGrid(Point p0, Point p1)
    {
        int dx = p1.X - p0.X;
        int dy = p1.Y - p0.Y;
        int nx = Math.Abs(dx);
        int ny = Math.Abs(dy);
        int sign_x = dx > 0 ? 1 : -1, sign_y = dy > 0 ? 1 : -1;

        var p = new Point(p0.X, p0.Y);
        var points = new List<Point>() { new Point(p.X, p.Y) };
        for (int ix = 0, iy = 0; ix < nx || iy < ny;)
        {
            if ((0.5 + ix) / nx < (0.5 + iy) / ny)
            {
                // next step is horizontal
                p.X += sign_x;
                ix++;
            }
            else
            {
                // next step is vertical
                p.Y += sign_y;
                iy++;
            }
            points.Add(new Point(p.X, p.Y));
        }
        return points;
    }

    protected void UpdateUIElements()
    {
        this.CheckAdjacentTiles();
        this.UpdateButtonPositions();
        this.SelectionGrid.CalculatePossibleTurns(this.CurrentPosition, this.CurrentActionPoints, this.GetIgnoreType(), this.Index);

        this.UpdateVision();
    }

    private void ShowDeactivatedVisionBlockers()
    {
        foreach (GameObject o in this.visionBlockersDeactivated)
        {
            o.SetActive(true);
        }

        this.visionBlockersDeactivated.Clear();
    }

    protected void UpdateVision()
    {
        if (!this.HasTurnToken || !this.canMove)
            return;

        this.ShowDeactivatedVisionBlockers();

        for (float i = 0; i < 360; i += 0.5f)
        {
            Vector3 dir = Vector3.up;
            dir = Quaternion.AngleAxis(i, Vector3.forward) * dir;
            var points = LineToGrid(this.CurrentPosition, this.CurrentPosition + new Point((int)Mathf.Round(dir.x * this.Vision), (int)Mathf.Round(dir.y * this.Vision)));

            foreach (Point point in points)
            {
                if (!this.IsValidTilePosition(point.X, point.Y))
                    continue;

                if (Bootstrap.Instance.Map.Tiles[point.X, point.Y].VisionBlocker != null)
                {
                    var vB = Bootstrap.Instance.Map.Tiles[point.X, point.Y].VisionBlocker;
                    this.visionBlockersDeactivated.Add(vB);
                    vB.SetActive(false);
                }

                if (this.Blocking.Contains(Bootstrap.Instance.Map.Tiles[point.X, point.Y].Type))
                {
                    break;
                }
            }
        }
    }

    protected virtual void CheckAdjacentTiles()
    {
        RemoveUIButtonsForActions();

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

    public abstract void ProcessAdjacentTile(Point position, Tile tile);

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
