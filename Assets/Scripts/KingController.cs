using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class KingController : BaseController
{
    public List<GuardController> Guards;

    public GuardController CurrentSelectedGuard;

    public ActorSkill[] Skills = new ActorSkill[4];

    public Moba_Camera mobaCam;

    private int currentGuardIndex = 0;

    public override void StartTurn()
    {
        base.StartTurn();

        this.canMove = false;
        this.Guards = this.Guards.OrderBy(x => x.gameObject.name).ToList();
        foreach (GuardController guardController in this.Guards)
        {
            guardController.KingController = this;
            guardController.StartTurn();
        }
        this.SelectGuard(this.Guards[this.currentGuardIndex]);
        this.mobaCam.settings.lockTargetTransform = this.Guards[this.currentGuardIndex].transform;
        this.mobaCam.settings.cameraLocked = true;
    }

    public void Update()
    {
        if (!this.HasTurnToken)
        {
            return;
        }

        if (this.CurrentSelectedGuard.CurrentActionPoints <= 0)
        {
            var guard = this.Guards.FirstOrDefault(x => x.CurrentActionPoints > 0);
            if (guard != null)
                this.SelectGuard(guard);
        }

        this.CurrentSelectedGuard.UpdateController();

        if (Input.GetKeyUp(KeyCode.A))
        {
            var guard = this.Guards.OrderBy(x => x.gameObject.name).First();

            this.SelectGuard(guard);
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            var guard = this.Guards.OrderByDescending(x => x.gameObject.name).First();

            this.SelectGuard(guard);
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
