using UnityEngine;
using System.Collections;

using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class Bootstrap : MonoBehaviour
    {
        public static Bootstrap Instance;

        public Grid Map;

        public const int MapX = 100;
        public const int MapY = 100;

        public void Start()
        {
            Instance = this;

            this.Map = new Grid(new Point(MapX, MapY));

            DontDestroyOnLoad(this.gameObject);

            SceneManager.LoadScene("test");
        }
    }
}
