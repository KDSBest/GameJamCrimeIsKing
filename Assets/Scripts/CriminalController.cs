using Assets.Scripts;

using DG.Tweening;

using UnityEngine;
using UnityEngine.UI;

public class CriminalController : BaseController
{
    public GameObject Criminal;

    public ActorSkill[] Skills = new ActorSkill[4];

    public Moba_Camera mobaCam;

    public int Treasures = 0;

    public Text TreasureText;

    public Transform CriminalPivot;

    public KingController KingController;

    public ParticleSystem TreasureParticle;

    public GameObject[] CrownPieces;

    private Point currentMoveEndPoint;

    private int currentMoveActionCost;

    private Vector3[] currentMoveWaypoints;

    public override void MoveTo(Point currentPosition, int actionPointCost, Vector3[] waypoints)
    {
        this.currentMoveEndPoint = currentPosition;
        this.currentMoveActionCost = actionPointCost;
        this.currentMoveWaypoints = waypoints;

        this.Criminal.transform.DOPath(waypoints, waypoints.Length * 0.2f, PathType.CatmullRom, PathMode.Full3D, 5, Color.cyan).SetLookAt(0.01f).OnWaypointChange((wp) =>
                                                                                                                                                                  {
                                                                                                                                                                      var curPos = new Point((int)waypoints[wp].x, (int)waypoints[wp].z);
                                                                                                                                                                      this.SelectionGrid.CalculatePossibleTurns(curPos, this.CurrentActionPoints - wp - 1, this.GetIgnoreType(), this.Index);
                                                                                                                                                                      this.DoVision(curPos);
                                                                                                                                                                  });
        this.CriminalPivot.transform.DOPunchRotation(new Vector3(-20, 0, 0), waypoints.Length * 0.2f, 2, 0.5f);

        this.Invoke("UpdateWalkableTiles", waypoints.Length * 0.2f);
        Bootstrap.Instance.Map.Tiles[this.currentMoveEndPoint.X, this.currentMoveEndPoint.Y].Type = TileType.Thief;
    }

    public void Start()
    {
        this.TreasureParticle.gameObject.SetActive(false);

        foreach (GameObject piece in this.CrownPieces)
        {
            piece.SetActive(false);
        }
    }

    public void UpdateWalkableTiles()
    {
        base.MoveTo(this.currentMoveEndPoint, this.currentMoveActionCost, this.currentMoveWaypoints);
        DoVision(this.currentMoveEndPoint);
        this.CriminalPivot.transform.DOPunchRotation(new Vector3(20, 0, 0), .5f, 20, .5f);
    }

    public void Update()
    {
        if (!this.HasTurnToken)
        {
            return;
        }

        if (this.CanMove && this.CurrentActionPoints > 0)
        {
            this.SelectionGrid.Select(this);
        }

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

    public override void ProcessAdjacentTile(Point position, Tile tile)
    {
        Point positionCopyIntoClosure = position;
        Tile tileCopyIntoClosure = tile;
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
                    this.SpawnButton(false, positionCopyIntoClosure, tileCopyIntoClosure, () =>
                                                                                          {
                                                                                              tileCopyIntoClosure.Type = TileType.Door;
                                                                                              tileCopyIntoClosure.HP = 1;
                                                                                              tileCopyIntoClosure.OccupyingObject.GetComponent<DoorOpener>().Close();
                                                                                          }, "Close Door", tileCopyIntoClosure.HP);
                }
                break;
            case TileType.Door:
                this.SpawnButton(false, positionCopyIntoClosure, tileCopyIntoClosure, () =>
                                                                                      {
                                                                                          tileCopyIntoClosure.Type = TileType.Walkable;
                                                                                          tileCopyIntoClosure.HP = 1;
                                                                                          tileCopyIntoClosure.OccupyingObject.GetComponent<DoorOpener>().Open();
                                                                                      }, "Open Door", tileCopyIntoClosure.HP);
                break;
            case TileType.BedHead:
            case TileType.BedFoot:
            case TileType.Cupboard:
            case TileType.Tresor:
            case TileType.LockerHigh:
            case TileType.Crate:
            case TileType.Crate1:
            case TileType.Crate2:
            case TileType.Couch:
            case TileType.CouchCorner:
            case TileType.CouchTable:
            case TileType.Vase:
            case TileType.Seat:
                if (tileCopyIntoClosure.HP > 0 || tileCopyIntoClosure.HasTreasure)
                {
                    this.SpawnButton(false, positionCopyIntoClosure, tileCopyIntoClosure, () =>
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
                                                                                          }, "Search", tileCopyIntoClosure.HP);
                }
                break;
        }
    }

    public void Awake()
    {
        base.Awake();

        Point spawn = RandomHelper.RandomSelect(Bootstrap.Instance.Map.PossibleThiefSpawns);
        this.CurrentPosition = spawn;
        Bootstrap.Instance.Map.Tiles[spawn.X, spawn.Y].Type = TileType.Thief;
        this.CheckAdjacentTiles();

        this.ForceCurrentPosition();
    }

    private void GiveTreasure()
    {
        this.Treasures++;
        this.TreasureText.text = this.Treasures.ToString();
        this.DisplayTreasureGain();
        this.KingController.hasATreasueBeenTaken = true;

        if (this.Treasures >= Bootstrap.TreasureWin)
        {
            this.HasWon = true;
        }
    }

    private void DisplayTreasureGain()
    {
        this.TreasureParticle.gameObject.SetActive(false);
        this.TreasureParticle.gameObject.SetActive(true);

        this.CrownPieces[this.Treasures - 1].SetActive(true);
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
