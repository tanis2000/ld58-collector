using System;
using GameBase.Utils;
using UnityEngine;

namespace GameBase.Animations.Actors
{
    public class Mover : MonoBehaviour
    {
        public float MoveTimeInSeconds = 0.2f;

        private Vector3 originalPosition;
        private bool isRunning = false;
        private float timeCounterInSeconds = 0.0f;
        private Vector3 finalPosition;

        private void Awake()
        {
        }

        public void Execute(Vector3 position)
        {
            originalPosition = transform.localPosition;
            finalPosition = position;
            isRunning = true;
            timeCounterInSeconds = 0;
        }

        private void Update()
        {
            if (!isRunning)
            {
                return;
            }

            timeCounterInSeconds += Time.deltaTime;
            if (timeCounterInSeconds > MoveTimeInSeconds)
            {
                timeCounterInSeconds = MoveTimeInSeconds;
                isRunning = false;
            }

            var ratio = timeCounterInSeconds / MoveTimeInSeconds;
            var step = Vector3.Lerp(originalPosition, finalPosition, ratio);
            transform.localPosition = step;//.WhereY(transform.localPosition.y);
        }
    }
}