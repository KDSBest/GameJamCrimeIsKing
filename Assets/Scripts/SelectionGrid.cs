using UnityEngine;
using System.Collections;

public class SelectionGrid : MonoBehaviour
{
    public GameObject SelectionOverlay;

    public void OnMouseOver()
    {
        var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(mouseRay, out hit))
        {
            this.SelectionOverlay.transform.position = new Vector3(Mathf.Round(hit.point.x), this.SelectionOverlay.transform.position.y, Mathf.Round(hit.point.z));
        }
    }
}
