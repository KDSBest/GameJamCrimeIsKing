using UnityEngine;
using System.Collections;
using System.Linq;

using UnityEditor;

namespace Assets.Scripts
{
    public class Bootstrap : MonoBehaviour
    {
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

            var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Map/map.txt");
            this.Map = new Grid(textAsset.text);

            this.Map.GeneratedMapVisibles(this.Floor, this.Wall, this.WallL, this.WallT, this.WallX, Bed);
        }
    }
}