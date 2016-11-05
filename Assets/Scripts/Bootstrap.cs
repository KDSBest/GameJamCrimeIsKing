using System;

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

        public GameObject LockerHigh;

        public GameObject VisionBlocker;

        public GameObject CouchTable;

        public GameObject Chair;

        public GameObject Couch;

        public GameObject Crate;

        public GameObject Crate1;

        public GameObject Crate2;

        public GameObject Desk;

        public GameObject Cupboard;

        public GameObject Seat;

        public GameObject Tresor;

        public GameObject Vase;

        public const int TreasureCount = 10;

        public const int TreasureWin = 4;

        public const int WeightLockerHigh = 10;

        public const int WeightBed = 1;

        public const int WeightOther = 0;

        private int GetWeight(TileType type)
        {
            switch (type)
            {
                case TileType.BedHead:
                    return WeightBed;
                case TileType.Cupboard:
                    return WeightLockerHigh;
                default:
                    return WeightOther;
            }
        }

        public void Start()
        {
            Instance = this;

            this.Map = new Grid(MapFile.text, VisionBlocker, Parent);

            var weights = this.Map.PossibleTreasureTiles.Select(x => this.GetWeight(x.Type)).ToList();

            int currentTreasures = 0;

            while (currentTreasures < TreasureCount)
            {
                var possibleTreasure = RandomHelper.RandomSelectGewichtung(this.Map.PossibleTreasureTiles, weights);

                if (!possibleTreasure.HasTreasure)
                {
                    possibleTreasure.SetTreasure(true);
                    currentTreasures++;
                }
            }

            FloorGrid.transform.localScale = new Vector3(this.Map.Size.X, this.Map.Size.Y, 1);
            this.FloorGrid.transform.position = new Vector3(this.Map.Size.X / 2, 0, this.Map.Size.Y / 2);

            this.Map.GeneratedMapVisibles(Parent, this.Wall, this.WallL, this.WallT, this.WallX, Bed, Door, this.LockerHigh, this.Couch, this.CouchTable, this.Chair, this.Crate, this.Crate1, this.Crate2, this.Desk, this.Cupboard, this.Seat, this.Tresor, this.Vase);

            DontDestroyOnLoad(this.gameObject);

            SceneManager.LoadScene("test");
        }
    }
}
