using System;
using System.Collections.Generic;
using GameBase.Utils;
using UnityEngine;

namespace App
{
    public class LevelBuilder: MonoBehaviour
    {
        public XRandom Rnd = new XRandom(42);
        public Vector2 CellSize = new Vector2(2, 2);
        private Transform FloorsLibrary;
        private List<Transform> Floors = new List<Transform>();
        private Transform Level;

        private void Start()
        {
            FloorsLibrary = GameObject.Find("Library").transform;
            var matchers = FloorsLibrary.GetComponentsInChildren<TileMatcher>();
            foreach (var matcher in matchers)
            {
                if (matcher.tileType == TileType.Floor)
                {
                    Floors.Add(matcher.transform);
                }
            }
            Debug.Log("FloorsLibrary count: " + Floors.Count);
            Level = GameObject.Find("Level").transform;
            Build();
        }

        private Transform RandomFloor()
        {
            var floor = Rnd.Item(Floors);
            return floor;
        }
        public void Build()
        {
            for (var x = 0; x < 8; x++)
            {
                for (var y = 0; y < 8; y++)
                {
                    var floor = RandomFloor();
                    var inst = GameObject.Instantiate(floor, Level, true);
                    inst.transform.localPosition = new Vector3(x * CellSize.x, Rnd.Range(-0.1f, 0.1f), y * CellSize.y);
                }
            }
        }
    }
}