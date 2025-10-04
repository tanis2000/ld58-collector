using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace App
{
    public class Hero: MonoBehaviour
    {
        public Vector2 GridPosition;
        public float Speed = 5;
        private Vector2 DestinationGridPosition = Vector2.zero;
        private Vector3 DestinationPosition = Vector3.zero;
        private InputAction moveAction;
        private Vector2 movement = Vector2.zero;
        private bool inputPaused = false;
        private Vector2 cellSize = new Vector2(2, 2);

        private void Start()
        {
            moveAction = InputSystem.actions.FindAction("Move");
            DestinationGridPosition = GridPosition;
            SnapToGrid();
        }

        private void Update()
        {
            
            ProcessInput();
            if (movement.magnitude != 0)
            {
                MoveToGridPosition(movement);
            }

            ProcessMovement();
        }

        private void SnapToGrid()
        {
            transform.localPosition = new Vector3(GridPosition.x * cellSize.x, 1, GridPosition.y * cellSize.y);
        }

        private void ProcessInput()
        {
            movement = Vector2.zero;
            if (inputPaused)
            {
                return;
            }
            var moveValue = moveAction.ReadValue<Vector2>();
            if (moveValue.x > 0)
            {
                movement.x = 1;
            } else if (moveValue.x < 0)
            {
                movement.x = -1;
            }

            if (moveValue.y > 0)
            {
                movement.y = 1;
            }
            else if (moveValue.y < 0)
            {
                movement.y = -1;
            }

        }

        private void MoveToGridPosition(Vector2 direction)
        {
            DestinationGridPosition = GridPosition + direction;
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
    }
}