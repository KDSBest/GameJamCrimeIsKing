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

        public void Start()
        {
            Instance = this;

            this.Map = new Grid(MapFile.text);

            this.Map.GeneratedMapVisibles(this.Floor, this.Wall, this.WallL, this.WallT, this.WallX, Bed);

            DontDestroyOnLoad(this.gameObject);

            SceneManager.LoadScene("test");
        }
    }
}
