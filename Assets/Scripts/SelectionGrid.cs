using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    public class SelectionGrid : MonoBehaviour
    {
        Point currentPosition = new Point(0, 0);

        public GameObject SelectionOverlay;

        public GameObject WaypointOverlay;

        public void OnMouseOver()
        {
            var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(mouseRay, out hit))
            {
                this.SelectionOverlay.transform.position = new Vector3(Mathf.Round(hit.point.x), this.SelectionOverlay.transform.position.y, Mathf.Round(hit.point.z));

                if (Input.GetMouseButtonUp(0))
                {
                    var result = AStar.Search(this.currentPosition, new Point((int)this.SelectionOverlay.transform.position.x, (int)this.SelectionOverlay.transform.position.z), Bootstrap.Instance.Map.Walls, Bootstrap.Instance.Map.Size);

                    if (result != null)
                    {
                        GameObject go = GameObject.Instantiate(this.WaypointOverlay);
                        go.transform.position = this.SelectionOverlay.transform.position;
                    }
                }
            }
        }
    }
}