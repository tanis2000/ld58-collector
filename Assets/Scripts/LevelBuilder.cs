using System;
using System.Collections.Generic;
using System.Linq;
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
        private List<Transform> inGameCollectibles = new List<Transform>();

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

        public void AddInGameCollectible(Transform inGameCollectible)
        {
            inGameCollectibles.Add(inGameCollectible);
        }

        public void RemoveInGameCollectible(Transform inGameCollectible)
        {
            inGameCollectibles.Remove(inGameCollectible);
        }

        public int InGameCollectiblesCount()
        {
            return inGameCollectibles.Count;
        }

        public int PickUpInGameCollectiblesCount()
        {
            int count = 0;
            foreach (var inGameCollectible in inGameCollectibles)
            {
                var c = inGameCollectible.GetComponent<Collectible>();
                if (c.CanPickUp)
                {
                    count++;
                }
            }

            return count;
        }

        public Collectible CollectibleAtGridPosition(Vector2 pos)
        {
            var list = new List<Collectible>();
            foreach (var inGameCollectible in inGameCollectibles)
            {
                var c = inGameCollectible.GetComponent<Collectible>();
                if (c.GridPosition == pos)
                {
                    list.Add(c);
                }
            }

            foreach (var c in list)
            {
                // Look for the one that is the root of the pile
                if (c.LevelOnPile == 0)
                {
                    return c;
                }
            }

            if (list.Count > 0)
            {
                // Return any, it does not matter as it should be just one
                return list[0];
            }

            return null;
        }
    }
}