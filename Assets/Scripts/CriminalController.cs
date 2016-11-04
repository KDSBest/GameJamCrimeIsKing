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

    public GuardController[] Guards;

    public override void MoveTo(Point currentPosition, int actionPointCost, Vector3[] waypoints)
    {
        this.currentMoveEndPoint = currentPosition;
        this.currentMoveActionCost = actionPointCost;
        this.currentMoveWaypoints = waypoints;

        this.Criminal.transform.DOPath(waypoints, waypoints.Length * 0.2f, PathType.CatmullRom, PathMode.Full3D, 5, Color.cyan).SetLookAt(0.1f);
        this.Invoke("UpdateWalkableTiles", waypoints.Length * 0.2f);
        Bootstrap.Instance.Map.Tiles[this.currentMoveEndPoint.X, this.currentMoveEndPoint.Y].Type = TileType.Thief;
    }

    protected override void UpdateVision()
    {
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

    public override void StartTurn()
    {
        base.StartTurn();
        this.mobaCam.settings.lockTargetTransform = this.transform;
        this.mobaCam.settings.cameraLocked = true;

        this.CheckAdjacentTiles();
    }

    public override TileType GetIgnoreType()
    {
        return TileType.Thief;
    }

    private void GiveTreasure()
    {
        this.Treasures++;
        this.TreasureText.text = this.Treasures.ToString();

        if (this.Treasures >= Bootstrap.TreasureWin)
        {
            this.HasWon = true;
        }
    }

    public override void ProcessAdjacentTile(Point position, Tile tile)
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
                    SpawnButton(false, positionCopyIntoClosure, tileCopyIntoClosure, () =>
                    {
                        tileCopyIntoClosure.Type = TileType.Door;
                        tileCopyIntoClosure.HP = 1;
                    }, "Close Door (" + tileCopyIntoClosure.HP + ")");
                }
                break;
            case TileType.Door:
                SpawnButton(false, positionCopyIntoClosure, tileCopyIntoClosure, () =>
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
                    SpawnButton(false, positionCopyIntoClosure, tileCopyIntoClosure, () =>
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

        var spawn = RandomHelper.RandomSelect(Bootstrap.Instance.Map.PossibleThiefSpawns);
        this.CurrentPosition = spawn;
        Bootstrap.Instance.Map.Tiles[spawn.X, spawn.Y].Type = TileType.Thief;
        this.CheckAdjacentTiles();
        
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
