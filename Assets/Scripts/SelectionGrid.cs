using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    public class SelectionGrid : MonoBehaviour
    {
        Point currentPosition = new Point(0, 0);

        public GameObject SelectionOverlay;

        public GameObject WaypointOverlay;

        public GameObject WaypointPassOverlay;

        public void OnMouseOver()
        {
            var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(mouseRay, out hit))
            {
                this.SelectionOverlay.transform.position = new Vector3(Mathf.Round(hit.point.x), this.SelectionOverlay.transform.position.y, Mathf.Round(hit.point.z));

                if (Input.GetMouseButton(0))
                {
                    var result = AStar.Search(this.currentPosition, new Point((int)this.SelectionOverlay.transform.position.x, (int)this.SelectionOverlay.transform.position.z), Bootstrap.Instance.Map.Tiles, Bootstrap.Instance.Map.Size);

                    if (result != null)
                    {
                        Create(this.WaypointOverlay, result.Position);

                        var current = result.Parent;
                        this.currentPosition = result.Position;

                        while (current != null && current.Parent != null)
                        {
                            Create(this.WaypointPassOverlay, current.Position);
                            current = current.Parent;
                        }
                    }
                }
            }
        }

        private void Create(GameObject toClone, Point point)
        {
            GameObject go = GameObject.Instantiate(toClone);
            go.transform.position = new Vector3(point.X, 0.5f, point.Y);
        }
    }
}
