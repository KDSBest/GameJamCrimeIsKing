using System;

using UnityEngine;

[Serializable]
public class ActorSkill
{
    public int ActionPointCost = 1;

    public string Name = "";

    public virtual void Execute()
    {
        Debug.Log("Execute: " + this.Name);
    }
}
