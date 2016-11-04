using UnityEngine;
using System.Collections;
using System.Linq;

using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class Bootstrap : MonoBehaviour
    {
        public TextAsset MapFile;
        public static Bootstrap Instance;

        public Grid Map;

        public GameObject Floor;

        public GameObject WallX;

        public GameObject WallT;

        public GameObject WallL;

        public GameObject Wall;

        public GameObject Bed;

        public GameObject Door;

        public GameObject Parent;

        public GameObject FloorGrid;

        public GameObject Cupboard;

        public void Start()
        {
            Instance = this;

            this.Map = new Grid(MapFile.text);

            this.Map.GeneratedMapVisibles(Parent, this.Floor, this.Wall, this.WallL, this.WallT, this.WallX, Bed, Door, Cupboard);

            FloorGrid.transform.localScale = new Vector3(this.Map.Size.X, 1, this.Map.Size.Y);
            this.FloorGrid.transform.position = new Vector3(this.Map.Size.X / 2, 0, this.Map.Size.Y / 2);

            DontDestroyOnLoad(this.gameObject);

            SceneManager.LoadScene("test");
        }
    }
}
