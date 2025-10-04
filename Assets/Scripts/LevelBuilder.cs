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
        public Transform Level;
        private Transform FloorsLibrary;
        private List<Transform> Floors = new List<Transform>();
        private List<Transform> heroesLibrary = new List<Transform>();
        private List<Transform> inGameCollectibles = new List<Transform>();
        private List<Hero> inGameHeroes = new List<Hero>();

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

            var hl = GameObject.Find("Heroes");
            var hm = hl.GetComponentsInChildren<Hero>();
            foreach (var h in hm)
            {
                heroesLibrary.Add(h.transform);
            }
            
            Build();
            SpawnCharacter(true, 0);
            SpawnCharacter(false, 1);
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
        
        private bool MatchesCollectibleAtGridPosition(Vector2 gridPosition)
        {
            foreach (var c in inGameCollectibles)
            {
                var col =  c.GetComponent<Collectible>();
                if (col.GridPosition == gridPosition)
                {
                    return true;
                }
            }
            return false;
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

        public Vector2 FindGridPositionForHero()
        {
            var pos = Vector2.zero;
            do
            {
                pos = new Vector2(Rnd.Range(0, 7), Rnd.Range(0, 7));
            } while (MatchesHeroesGridPosition(pos) || MatchesCollectibleAtGridPosition(pos));

            return pos;
        }
        
        private void SpawnCharacter(bool isPlayer, int idx = -1)
        {
            Transform model;
            if (idx == -1)
            {
                model = Rnd.Item(heroesLibrary);
            }
            else
            {
                model = heroesLibrary[idx];
            }
            var inst = Instantiate(model, Level, true);
            //inst.localPosition = new Vector3(Rnd.Range(0, 7) * CellSize.x, 50, Rnd.Range(0, 7) * CellSize.y);
            var h = inst.GetComponent<Hero>();
            h.IsControlledByPlayer = isPlayer;
            do
            {
                h.GridPosition = new Vector2(Rnd.Range(0, 7), Rnd.Range(0, 7));
            } while (MatchesHeroesGridPosition(h.GridPosition));
            inGameHeroes.Add(h);
        }

        private bool MatchesHeroesGridPosition(Vector2 gridPosition)
        {
            foreach (var hero in inGameHeroes)
            {
                if (hero.GridPosition == gridPosition)
                {
                    return true;
                }
            }
            return false;
        }

        public Hero HeroAtGridPosition(Vector2 gridPosition)
        {
            foreach (var hero in inGameHeroes)
            {
                if (hero.GridPosition == gridPosition)
                {
                    return hero;
                }
            }
            return null;
        }
    }
}