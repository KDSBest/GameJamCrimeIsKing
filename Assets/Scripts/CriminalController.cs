using System.Collections.Generic;

using Assets.Scripts;

using UnityEngine;

public class CriminalController : BaseController
{
    public GameObject Criminal;

    public ActorSkill[] Skills = new ActorSkill[4];

    private SelectionGrid selection;

    public void Start()
    {
        this.selection = FindObjectOfType<SelectionGrid>();
    }

    public void Update()
    {
        if (!this.HasTurnToken)
        {
            return;
        }

        if (Input.GetMouseButtonUp(0))
        {
            this.selection.Select();
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
