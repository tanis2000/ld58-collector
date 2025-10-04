using System;
using System.Collections.Generic;
using UnityEngine;

namespace App
{
    public class CollectibleSpawnerItem
    {
        public float When;
        public Transform What;
    }

    public class CollectibleSpawner: MonoBehaviour
    {
        public float StartingCollectibleCount = 4;
        private Transform CollectiblesLibrary;
        private List<Transform> availableCollectibles = new List<Transform>();
        private List<Transform> inGameCollectibles = new List<Transform>();
        private Queue<CollectibleSpawnerItem> collectiblesQueue = new Queue<CollectibleSpawnerItem>();
        private Transform Level;
        private LevelBuilder levelBuilder;
        
        private void Start()
        {
            CollectiblesLibrary = GameObject.Find("Library").transform;
            var matchers = CollectiblesLibrary.GetComponentsInChildren<TileMatcher>();
            foreach (var matcher in matchers)
            {
                if (matcher.tileType == TileType.Collectible)
                {
                    availableCollectibles.Add(matcher.transform);
                }
            }
            Debug.Log("CollectiblesLibrary count: " + availableCollectibles.Count);
            Level = GameObject.Find("Level").transform;
            levelBuilder = gameObject.GetComponent<LevelBuilder>();

        }

        private void Update()
        {
            if (inGameCollectibles.Count + collectiblesQueue.Count < StartingCollectibleCount)
            {
                for (int i = 0; i < StartingCollectibleCount; i++)
                {
                    QueueSpawn();
                }
            }
            ProcessQueue();
        }

        private Transform RandomCollectible()
        {
            var item = levelBuilder.Rnd.Item(availableCollectibles);
            return item;
        }

        private void QueueSpawn()
        {
            var csi = new CollectibleSpawnerItem();
            csi.What = RandomCollectible();
            csi.When = Time.time + 0.2f;
            collectiblesQueue.Enqueue(csi);
        }

        private void ProcessQueue()
        {
            if (collectiblesQueue.Count == 0)
            {
                return;
            }

            var csi = collectiblesQueue.Peek();
            if (csi.When <= Time.time)
            {
                var inst = Instantiate(csi.What, Level, true);
                inGameCollectibles.Add(inst.transform);
                var c = inst.GetComponent<Collectible>();
                c.GridPosition = new Vector2(levelBuilder.Rnd.Range(0, 7), levelBuilder.Rnd.Range(0, 7));
                c.Spawn();
                collectiblesQueue.Dequeue();
            }
        }

        public void RemoveCollectibleFromList(Transform item)
        {
            inGameCollectibles.Remove(item);
        }
    }
}