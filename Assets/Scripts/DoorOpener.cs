using UnityEngine;
using System.Collections;

using DG.Tweening;

public class DoorOpener : MonoBehaviour
{
    public GameObject Door;

    public void Open()
    {
        this.Door.transform.DOMoveY(-2, 0.5f);
    }

    public void Close()
    {
        this.Door.transform.DOMoveY(1, 0.5f);
    }
}
