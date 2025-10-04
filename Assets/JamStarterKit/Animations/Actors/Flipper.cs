using System;
using GameBase.Utils;
using UnityEngine;

namespace GameBase.Animations.Actors
{
    public class Flipper : MonoBehaviour
    {
        public float FlipTimeInSeconds = 0.2f;
        public AnimationCurve Curve;
        public Transform Visuals;

        private Vector3 originalScale;
        private bool isRunning = false;
        private float timeCounterInSeconds = 0.0f;
        private int flipDirection = 1;
        private int lastFlipDirection = 1;

        private void Awake()
        {
            originalScale = Visuals.localScale;
        }

        public void Execute(int direction)
        {
            // Skip flipping if we are already facing the right direction.
            // This avoids overriding the other animations of the scale on the x axis.
            if (lastFlipDirection == direction)
            {
                return;
            }
            isRunning = true;
            timeCounterInSeconds = 0;
            flipDirection = direction;
        }

        private void Update()
        {
            if (!isRunning)
            {
                return;
            }

            timeCounterInSeconds += Time.deltaTime;
            if (timeCounterInSeconds > FlipTimeInSeconds)
            {
                timeCounterInSeconds = FlipTimeInSeconds;
                isRunning = false;
                lastFlipDirection = flipDirection;
            }

            var ratio = timeCounterInSeconds / FlipTimeInSeconds;
            var step = Curve.Evaluate(ratio) * flipDirection;
            Visuals.localScale = Visuals.localScale.WhereX(originalScale.x * step);
        }
    }
}