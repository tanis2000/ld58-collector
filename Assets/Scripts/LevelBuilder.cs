using System;
using System.Collections.Generic;
using GameBase.Utils;
using UnityEngine;

namespace App
{
    public class LevelBuilder: MonoBehaviour
    {
        private Transform FloorsLibrary;
        private List<Transform> Floors = new List<Transform>();
        private XRandom rnd = new XRandom(42);
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
            var floor = rnd.Item(Floors);
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
                    inst.transform.localPosition = new Vector3(x, rnd.Range(-0.2f, 0.2f), y);
                }
            }
        }
    }
}