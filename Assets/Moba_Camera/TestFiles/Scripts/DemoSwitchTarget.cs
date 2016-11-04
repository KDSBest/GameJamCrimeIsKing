using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DemoSwitchTarget : MonoBehaviour
{
    public Moba_Camera moba_camera = null;

    public List<Transform> targets = new List<Transform>();

    private List<Transform> remainingTargets = new List<Transform>();
    public int currentTarget = 0;

    // Use this for initialization
    void Start()
    {
        if (!moba_camera) this.enabled = false;

        remainingTargets = this.targets;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < this.remainingTargets.Count; i++)
        {
            Transform target = this.remainingTargets[i];
            if (target.GetComponent<BaseController>().CurrentActionPoints <= 0)
            {
                this.remainingTargets.RemoveAt(i);
                this.remainingTargets = this.remainingTargets.Where(x => x != null).ToList();
            }

        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            changeTargetUp();
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            changeTargetDown();
        }

        moba_camera.SetTargetTransform(remainingTargets[currentTarget]);
    }

    public void changeTargetUp()
    {
        ++currentTarget;
        if (currentTarget >= remainingTargets.Count) currentTarget = 0;
    }

    public void changeTargetDown()
    {
        --currentTarget;
        if (currentTarget < 0) currentTarget = remainingTargets.Count - 1;
    }

    public void AddTarget(Transform target)
    {
        if (!remainingTargets.Contains(target))
        {
            remainingTargets.Add(target);
        }
    }
}
