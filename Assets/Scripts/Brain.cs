using System;
using System.Collections;
using UnityEngine;

namespace App
{
    public enum BrainState
    {
        Idle = 0,
        PickUp = 1,
        Drop = 2,
        Push = 3,
    }
    
    public class Brain: MonoBehaviour
    {
        public bool IsRunning = false;
        private Hero hero;
        private BrainState state;
        private LevelBuilder levelBuilder;
        private Vector2 targetGridPosition;

        private void Start()
        {
            hero = GetComponent<Hero>();
            state = BrainState.Idle;
            levelBuilder = FindFirstObjectByType<LevelBuilder>();
            if (!IsRunning)
            {
                return;
            }
            StartCoroutine(RunStateMachine());
        }
        
        private IEnumerator RunStateMachine()
        {
            yield return StartCoroutine(state.ToString());
        }

        private IEnumerator Idle()
        {
            Debug.Log("Idle entered");
            while (state == BrainState.Idle)
            {
                Debug.Log("Waiting 1 second");
                yield return new WaitForSeconds(1.0f);
                var c = levelBuilder.FindClosestCollectible(hero.GridPosition, true);
                if (c != null)
                {
                    targetGridPosition = c.GridPosition;
                    state = BrainState.PickUp;
                }
                yield return null;
            }
            Debug.Log("Idle exited");
            StartCoroutine(state.ToString());
        }

        private IEnumerator PickUp()
        {
            Debug.Log("PickUp entered");
            while (state == BrainState.PickUp)
            {
                Debug.Log("Waiting 1 second");
                yield return new WaitForSeconds(1.0f);
                var direction = Vector2.zero;
                if (targetGridPosition.x > hero.GridPosition.x)
                {
                    direction.x = 1;
                } else if (targetGridPosition.x < hero.GridPosition.x)
                {
                    direction.x = -1;
                } else if (targetGridPosition.y > hero.GridPosition.y)
                {
                    direction.y = 1;
                } else if (targetGridPosition.y < hero.GridPosition.y)
                {
                    direction.y = -1;
                }
                hero.SetMovement(direction);
                yield return new WaitForSeconds(0.2f);
                hero.SetMovement(Vector2.zero);
                yield return new WaitForSeconds(0.2f);
                if (hero.GridPosition == targetGridPosition)
                {
                    state = BrainState.Idle;
                }
                yield return null;
            }
            Debug.Log("PickUp exited");
            StartCoroutine(state.ToString());
        }

    }
}