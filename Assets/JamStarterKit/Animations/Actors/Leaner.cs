using System;
using UnityEngine;

namespace GameBase.Animations.Actors
{
    public class Leaner : MonoBehaviour
    {
        public float Angle = 50.0f;
        public float LeanTimeInSeconds = 0.2f;

        private Quaternion originalRotation;
        private bool isRunning = false;
        private float timeCounterInSeconds = 0.0f;
        private Quaternion finalRotation;

        private void Awake()
        {
            originalRotation = transform.rotation;
        }

        public void Execute(Vector3 movementDirection)
        {
            var direction = Vector3.Cross(Vector3.up, movementDirection);
            finalRotation = originalRotation * Quaternion.Euler(direction * Angle);
            isRunning = true;
            timeCounterInSeconds = 0;
        }

        private void Update()
        {
            if (!isRunning)
            {
                transform.rotation = originalRotation;
                return;
            }

            timeCounterInSeconds += Time.deltaTime;
            if (timeCounterInSeconds > LeanTimeInSeconds)
            {
                timeCounterInSeconds = LeanTimeInSeconds;
                isRunning = false;
            }

            var ratio = timeCounterInSeconds / LeanTimeInSeconds;
            var step = Quaternion.Lerp(originalRotation, finalRotation, ratio);
            transform.rotation = step;
        }
    }
}