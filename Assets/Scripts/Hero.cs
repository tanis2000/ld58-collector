using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace App
{
    public class Hero: MonoBehaviour
    {
        public Vector2 GridPosition;
        public float Speed = 5;
        public bool IsControlledByPlayer = true;
        private Vector2 DestinationGridPosition = Vector2.zero;
        private Vector3 DestinationPosition = Vector3.zero;
        private InputAction moveAction;
        private Vector2 movement = Vector2.zero;
        private Vector2 oldMovement = Vector2.zero;
        private bool inputPaused = false;
        private Vector2 cellSize = new Vector2(2, 2);
        private LevelBuilder levelBuilder;
        private Collectible carrying;
        private SubmitScore submitScore;
        private int counter;

        private void Start()
        {
            moveAction = InputSystem.actions.FindAction("Move");
            levelBuilder = FindFirstObjectByType<LevelBuilder>();
            submitScore = FindFirstObjectByType<SubmitScore>();
            DestinationGridPosition = GridPosition;
            SnapToGrid();
        }

        private void Update()
        {
            if (IsControlledByPlayer)
            {
                ProcessInput();
            }
            if (movement.magnitude != 0)
            {
                var possibleDestinationPosition = ComputeDestinationGridPosition(movement);
                var c = levelBuilder.CollectibleAtGridPosition(possibleDestinationPosition);
                if (c != null && c.CanPickUp && !carrying)
                {
                    Debug.Log("Trying to pick up");
                    // Pick up
                    MoveToGridPosition(movement);
                    ProcessPickup(possibleDestinationPosition);
                }
                else if (c != null && !c.CanPickUp && !carrying)
                {
                    Debug.Log("Trying to push");
                    // Push the pile
                    MoveToGridPosition(movement);
                    c.PushPile(movement);
                }
                else if (c != null && carrying)
                {
                    Debug.Log("Trying to drop");
                    // Drop
                    ProcessPickup(possibleDestinationPosition);
                }
                else if (c == null)
                {
                    // Just move to an empty cell
                    MoveToGridPosition(movement);
                }
            }

            ProcessMovement();
        }

        private void SnapToGrid()
        {
            transform.localPosition = new Vector3(GridPosition.x * cellSize.x, 1, GridPosition.y * cellSize.y);
        }

        private void ProcessInput()
        {
            oldMovement = movement;
            movement = Vector2.zero;
            if (inputPaused || !moveAction.WasPerformedThisFrame())
            {
                return;
            }
            var moveValue = moveAction.ReadValue<Vector2>();
            if (moveValue.x > 0 && oldMovement.x == 0)
            {
                movement.x = 1;
            } else if (moveValue.x < 0 && oldMovement.x == 0)
            {
                movement.x = -1;
            }

            if (moveValue.y > 0 && oldMovement.y == 0)
            {
                movement.y = 1;
            }
            else if (moveValue.y < 0 && oldMovement.y == 0)
            {
                movement.y = -1;
            }

        }

        public Vector2 ComputeDestinationGridPosition(Vector2 direction)
        {
            return GridPosition + direction;
        }
        
        private void MoveToGridPosition(Vector2 direction)
        {
            var possibleDest = ComputeDestinationGridPosition(direction);
            if (possibleDest.x < 0 || possibleDest.x > 7 || possibleDest.y < 0 || possibleDest.y > 7)
            {
                return;
            }
            DestinationGridPosition = possibleDest;
            DestinationPosition = new Vector3(DestinationGridPosition.x * cellSize.x, 1, DestinationGridPosition.y * cellSize.y);
        }

        private void ProcessMovement()
        {
            if (DestinationGridPosition == GridPosition)
            {
                inputPaused = false;
                return;
            }

            inputPaused = true;
            var step = Speed * Time.deltaTime;
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, DestinationPosition, step);
            if (Vector3.Distance(transform.localPosition, DestinationPosition) <= 0.001f)
            {
                GridPosition = DestinationGridPosition;
            }
        }

        private void ProcessPickup(Vector2 possibleDestinationPosition)
        {
            var c = levelBuilder.CollectibleAtGridPosition(possibleDestinationPosition);
            if (c != null)
            {
                if (!carrying && c.CanPickUp && !c.IsFalling)
                {
                    PickUp(c);
                }
                else if (carrying != c && !c.IsFalling)
                {
                    DropOn(c);
                }
            }
        }

        private void PickUp(Collectible c)
        {
            c.Carrier = this;
            carrying = c;
        }

        private void DropOn(Collectible c)
        {
            c.AddToPile(carrying);
            carrying.Carrier = null;
            carrying = null;
        }
    }
}