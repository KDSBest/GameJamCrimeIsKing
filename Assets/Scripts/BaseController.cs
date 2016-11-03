using Assets.Scripts;

using UnityEngine;

public class BaseController : MonoBehaviour, IController
{
    public Point CurrentPosition;

    public void StartTurn()
    {
        Debug.Log("start turn: " + this.GetType().Name);
    }

    public void AddWaypoint(int posX, int posY)
    {
        throw new System.NotImplementedException();
    }

    public void EndTurn()
    {
        Debug.Log("end turn: " + this.GetType().Name);
    }
}
