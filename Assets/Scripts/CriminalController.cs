using System.Collections.Generic;

using Assets.Scripts;

using UnityEngine;

public class CriminalController : BaseController
{
    public GameObject Criminal;

    public ActorSkill[] Skills = new ActorSkill[4];

    public override void MoveTo(Point currentPosition, int actionPointCost)
    {
        base.MoveTo(currentPosition, actionPointCost);
        this.Criminal.transform.position = new Vector3(this.CurrentPosition.X, this.Criminal.transform.position.y, this.CurrentPosition.Y);
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

    private void ExecuteSkill(int id)
    {
        ActorSkill skill = this.Skills[id];

        if (this.CurrentActionPoints >= skill.ActionPointCost)
        {
            base.SpendActionPoints(skill.ActionPointCost);
            skill.Execute();
        }
    }
}
