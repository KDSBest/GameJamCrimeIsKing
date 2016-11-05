using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts
{
    public class SelectionGrid : MonoBehaviour
    {
        public List<GameObject> WaypointsAllowed = new List<GameObject>();

        public List<GameObject> Waypoints = new List<GameObject>();

        public Point CurrentPosition = new Point(0, 0);

        public Point SelectedPoint = null;

        public GameObject SelectionOverlay;

        public GameObject WaypointOverlay;

        public GameObject WaypointPassOverlay;

        public GameObject WaypointAllowed;

        [HideInInspector]
        public int[,] AllowedMoves = null;

        [HideInInspector]
        public Point AllowedMovesOffest = null;

        [HideInInspector]
        public Point AllowedMovesSize = null;

        public void CalculatePossibleTurns(Point currentPosition, int actionPoints, TileType ignoreType, int ignoreGuardIndex)
        {
            this.CurrentPosition = currentPosition;
            Debug.Log("Calc Turns " + this.CurrentPosition.X + ", " + this.CurrentPosition.Y + " - " + actionPoints);
            AllowedMovesOffest = new Point(int.MaxValue, int.MaxValue);
            Point high = new Point(int.MinValue, int.MinValue);

            List<Tile> mapParts = new List<Tile>();

            for (int y = this.CurrentPosition.Y - actionPoints; y <= this.CurrentPosition.Y + actionPoints; y++)
            {
                for (int x = this.CurrentPosition.X - actionPoints; x <= this.CurrentPosition.X + actionPoints; x++)
                {
                    if (x < 0 || y < 0 || x >= Bootstrap.Instance.Map.Size.X || y >= Bootstrap.Instance.Map.Size.Y)
                    {
                        continue;
                    }

                    if (x < AllowedMovesOffest.X)
                    {
                        AllowedMovesOffest.X = x;
                    }

                    if (x > high.X)
                    {
                        high.X = x;
                    }

                    if (y < AllowedMovesOffest.Y)
                    {
                        AllowedMovesOffest.Y = y;
                    }

                    if (y > high.Y)
                    {
                        high.Y = y;
                    }

                    mapParts.Add(Bootstrap.Instance.Map.Tiles[x, y]);
                }
            }

            AllowedMovesSize = new Point(high.X - AllowedMovesOffest.X + 1, high.Y - AllowedMovesOffest.Y + 1);
            Tile[,] tiles = new Tile[AllowedMovesSize.X, AllowedMovesSize.Y];
            this.AllowedMoves = new int[AllowedMovesSize.X, AllowedMovesSize.Y];
            for (int y = 0; y < AllowedMovesSize.Y; y++)
            {
                for (int x = 0; x < AllowedMovesSize.X; x++)
                {
                    tiles[x, y] = mapParts[y * AllowedMovesSize.X + x];
                }
            }

            this.DeleteAllWaypointsAllowed();
            Grid g = new Grid(AllowedMovesSize, tiles);

            Point startPosition = new Point(this.CurrentPosition.X - AllowedMovesOffest.X, this.CurrentPosition.Y - AllowedMovesOffest.Y);
            var field = FlowField.Search(startPosition, g, ignoreType, ignoreGuardIndex);

            foreach (var fieldEntry in field)
            {
                if (fieldEntry.Cost <= actionPoints && (this.CurrentPosition.X != fieldEntry.Position.X || this.CurrentPosition.Y != fieldEntry.Position.Y))
                {
                    Point endPositionInMap = fieldEntry.Position + this.AllowedMovesOffest;
                    CreateWaypointAllowed(this.WaypointAllowed, endPositionInMap);
                    this.AllowedMoves[fieldEntry.Position.X, fieldEntry.Position.Y] = fieldEntry.Cost;
                }
                else
                {
                    this.AllowedMoves[fieldEntry.Position.X, fieldEntry.Position.Y] = -1;
                }
            }
        }

        public void Select(BaseController baseController)
        {
            this.CurrentPosition = baseController.CurrentPosition;

            var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(mouseRay, out hit))
            {
                this.SelectionOverlay.transform.position = new Vector3(Mathf.Round(hit.point.x), this.SelectionOverlay.transform.position.y, Mathf.Round(hit.point.z));

                if (Input.GetMouseButtonUp(0))
                {
                    var target = new Point((int)this.SelectionOverlay.transform.position.x, (int)this.SelectionOverlay.transform.position.z);
                    var offsetClick = target - this.AllowedMovesOffest;

                    if (offsetClick.X < 0 || offsetClick.Y < 0 || offsetClick.X >= this.AllowedMovesSize.X || offsetClick.Y >= this.AllowedMovesSize.Y || this.AllowedMoves[offsetClick.X, offsetClick.Y] < 0)
                        return;

                    if (this.SelectedPoint == null || this.SelectedPoint.X != target.X || this.SelectedPoint.Y != target.Y)
                    {
                        var result = AStar.Search(this.CurrentPosition, target, Bootstrap.Instance.Map);

                        if (result != null)
                        {
                            this.DeleteAllWaypoints();

                            this.CreateWaypoint(this.WaypointOverlay, result.Position);

                            var current = result.Parent;
                            this.SelectedPoint = result.Position;

                            while (current != null && current.Parent != null)
                            {
                                this.CreateWaypoint(this.WaypointPassOverlay, current.Position);
                                current = current.Parent;
                            }
                        }
                    }
                    else
                    {
                        this.CurrentPosition = this.SelectedPoint;
                        baseController.MoveTo(this.CurrentPosition, this.AllowedMoves[offsetClick.X, offsetClick.Y], this.Waypoints.Select(x => x.transform.position).Reverse().ToArray());
                        this.DeleteAllWaypoints();
                        this.CalculatePossibleTurns(baseController.CurrentPosition, baseController.CurrentActionPoints, baseController.GetIgnoreType(), baseController.Index);
                    }
                }
            }
        }

        private void CreateWaypointAllowed(GameObject toClone, Point point)
        {
            GameObject go = GameObject.Instantiate(toClone);
            go.transform.position = new Vector3(point.X, 0.05f, point.Y);
            go.transform.SetParent(this.transform);
            this.WaypointsAllowed.Add(go);
        }

        private void DeleteAllWaypointsAllowed()
        {
            foreach (var wp in this.WaypointsAllowed)
                GameObject.Destroy(wp.gameObject);

            this.WaypointsAllowed.Clear();
        }

        private void CreateWaypoint(GameObject toClone, Point point)
        {
            GameObject go = GameObject.Instantiate(toClone);
            go.transform.position = new Vector3(point.X, 0.5f, point.Y);
            go.transform.SetParent(this.transform);
            this.Waypoints.Add(go);
        }

        private void DeleteAllWaypoints()
        {
            foreach (var wp in this.Waypoints)
                GameObject.Destroy(wp.gameObject);

            this.Waypoints.Clear();
        }
    }
}
