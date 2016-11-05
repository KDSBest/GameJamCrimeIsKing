using System.Collections.Generic;
using System.Linq;

using Assets.Scripts;

using UnityEngine;

public class KingController : BaseController
{
    public List<GuardController> Guards;

    public GuardController CurrentSelectedGuard;

    public ActorSkill[] Skills = new ActorSkill[4];

    public Moba_Camera mobaCam;

    private int currentGuardIndex = 0;

    public override void EndTurn()
    {
        base.EndTurn();

        foreach (GuardController guardController in this.Guards)
        {
            guardController.EndTurn();
        }
    }

    public override void StartTurn()
    {
        base.StartTurn();

        this.currentGuardIndex = 0;
        this.canMove = false;
        this.Guards = this.Guards.OrderBy(x => x.gameObject.name).ToList();
        foreach (GuardController guardController in this.Guards)
        {
            guardController.KingController = this;
            guardController.hasArrived = false;
            guardController.StartTurn();
        }
        this.SelectGuard(this.Guards[this.currentGuardIndex]);
        this.mobaCam.settings.lockTargetTransform = this.Guards[this.currentGuardIndex].transform;
        this.mobaCam.settings.cameraLocked = true;
    }

    public void UpdateActionPointsFromGuard()
    {
        this.CurrentActionPoints = this.Guards.Sum(x => x.CurrentActionPoints);
        this.SetActionPointsText();
    }

    public override TileType GetIgnoreType()
    {
        return TileType.Guard;
    }

    public override void ProcessAdjacentTile(Point position, Tile tile)
    {

    }

    public void Update()
    {
        if (!this.HasTurnToken)
        {
            return;
        }

        this.HasWon = this.Guards.Any(x => x.HasWon);

        var orderedGuards = this.Guards.OrderBy(x => x.name).ToList();

        if ((this.CurrentSelectedGuard.CurrentActionPoints <= 0 && orderedGuards.Count > 0) && this.CurrentSelectedGuard.hasArrived)
        {
            this.CurrentSelectedGuard.hasArrived = false;
            var guard = orderedGuards.FirstOrDefault(x => x.CurrentActionPoints > 0);
            if (guard != null)
                this.SelectGuard(guard);
        }

        this.CurrentSelectedGuard.UpdateController();

        if (orderedGuards.Count > 0)
        {
            if (Input.GetKeyUp(KeyCode.A))
            {
                SelectPrevious();
            }
            else if (Input.GetKeyUp(KeyCode.D))
            {
                SelectNext();
            }
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

    public void SelectPrevious()
    {
        var orderedGuards = this.Guards.OrderBy(x => x.name).ToList();

        currentGuardIndex++;
        this.currentGuardIndex = currentGuardIndex % orderedGuards.Count;
        var guard = orderedGuards[this.currentGuardIndex];
        this.SelectGuard(guard);
    }

    public void SelectNext()
    {
        var orderedGuards = this.Guards.OrderBy(x => x.name).ToList();

        currentGuardIndex--;
        if (this.currentGuardIndex < 0)
            this.currentGuardIndex = orderedGuards.Count - 1;
        var guard = orderedGuards[this.currentGuardIndex];

        this.SelectGuard(guard);
    }

    private void SelectGuard(GuardController guardController)
    {
        this.CurrentSelectedGuard = guardController;
        foreach (GuardController controller in this.Guards)
        {
            controller.IsSelected = false;
        }

        this.CurrentSelectedGuard.IsSelected = true;
        this.CurrentSelectedGuard.ContinueTurn();
        this.mobaCam.settings.lockTargetTransform = this.CurrentSelectedGuard.transform;
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
