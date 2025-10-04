using System;
using System.Collections.Generic;
using GameBase.Audio;
using GameBase.Effects;
using UnityEngine;

namespace App
{
    public class Collectible: MonoBehaviour
    {
        public Vector2 GridPosition;
        public float Speed = 10;
        public int LevelOnPile;
        public bool CanPickUp = true;
        public List<Collectible> Pile = new List<Collectible>();
        public Hero Carrier;
        public bool IsFalling = true;
        private CollectibleSpawner collectibleSpawner;
        private Grounded grounded;
        private Vector2 DestinationGridPosition;
        private Vector3 DestinationPosition;
        private bool IsMoving;
        private LevelBuilder levelBuilder;
        private SubmitScore submitScore;
        private EnemyScore enemyScore;

        private void Start()
        {
            collectibleSpawner = FindFirstObjectByType<CollectibleSpawner>();
            grounded = GetComponent<Grounded>();
            grounded.OnHitGround += () =>
            {
                IsFalling = false;
                AudioSystem.Instance().Play("SoundFall");
            };
            levelBuilder = FindFirstObjectByType<LevelBuilder>();
            submitScore = FindFirstObjectByType<SubmitScore>();
            enemyScore = FindFirstObjectByType<EnemyScore>();
        }

        private void Update()
        {
            if (transform.position.y > 0 && !grounded.OnGround)
            {
                transform.localPosition += Vector3.down * (Speed * Time.deltaTime);
            }

            if (Carrier != null)
            {
                GridPosition = Carrier.GridPosition;
                transform.position =  Carrier.transform.position + Vector3.up * 1.5f;
            }

            if (IsMoving)
            {
                ProcessMovement();
            }

            ProcessMaxHeight();
        }

        public void Spawn()
        {
            transform.localPosition = new Vector3(GridPosition.x * 2, 50, GridPosition.y * 2);
        }

        public void Destroy()
        {
            collectibleSpawner.RemoveCollectibleFromList(transform);
            Destroy(gameObject);
        }

        public void AddToPile(Collectible c, Hero hero)
        {
            c.OnAddedToPile(this, Pile.Count);
            Pile.Add(c);
            CanPickUp = false;
            if (hero.IsControlledByPlayer)
            {
                submitScore.IncrementScore(1);
            }
            else
            {
                enemyScore.IncrementScore(1);
            }
            AudioSystem.Instance().Play("SoundPile");
        }

        public void OnAddedToPile(Collectible c, int pileHeight)
        {
            LevelOnPile = pileHeight + 1;
            CanPickUp = false;
            GridPosition = c.GridPosition;
            transform.position = c.transform.position + Vector3.up * (0.8f * (LevelOnPile + 1));
        }

        public void PushPile(Vector2 direction, Hero hero)
        {
            if (!IsMoving)
            {
                var tmpGridPosition = ComputeDestinationGridPosition(direction);
                if (tmpGridPosition.x < 0 || tmpGridPosition.x > 7 || tmpGridPosition.y < 0 || tmpGridPosition.y > 7)
                {
                    direction = direction * -1;
                    tmpGridPosition = ComputeDestinationGridPosition(direction);
                }
                var col = levelBuilder.CollectibleAtGridPosition(tmpGridPosition);
                if (col != null)
                {
                    if (col.CanPickUp)
                    {
                        // Add to pile
                        AddToPile(col, hero);
                    } else if (!col.CanPickUp)
                    {
                        // Add the whole pile to this one
                        MoveToThisPile(col, hero);
                    }
                }
                
                MoveToGridPosition(direction);
                foreach (Collectible c in Pile)
                {
                    c.PushPile(direction, hero);
                }
            }
        }
        
        private void MoveToGridPosition(Vector2 direction)
        {
            DestinationGridPosition = GridPosition + direction;
            DestinationPosition = new Vector3(DestinationGridPosition.x * 2, LevelOnPile+1, DestinationGridPosition.y * 2);
            IsMoving = true;
        }

        private void ProcessMovement()
        {
            if (DestinationGridPosition == GridPosition)
            {
                IsMoving = false;
                return;
            }

            var step = Speed * Time.deltaTime;
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, DestinationPosition, step);
            if (Vector3.Distance(transform.localPosition, DestinationPosition) <= 0.001f)
            {
                GridPosition = DestinationGridPosition;
            }
        }

        public Vector2 ComputeDestinationGridPosition(Vector2 direction)
        {
            return GridPosition + direction;
        }

        private void MoveToThisPile(Collectible c, Hero hero)
        {
            foreach (var p in c.Pile)
            {
                AddToPile(p, hero);
            }
            c.Pile.Clear();
            AddToPile(c, hero);
        }

        private void ProcessMaxHeight()
        {
            if (Pile.Count >= 6)
            {
                Dismantle();
            }
        }

        private void Dismantle()
        {
            foreach (var p in Pile)
            {
                EffectsSystem.AddEffect(1, p.transform.position);
                levelBuilder.RemoveInGameCollectible(p.transform);
                Destroy(p.gameObject);
            }
            EffectsSystem.AddEffect(1, transform.position);
            Pile.Clear();
            levelBuilder.RemoveInGameCollectible(transform);
            Destroy(gameObject);
        }
    }
}