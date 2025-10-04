using System;
using GameBase.Utils;
using UnityEngine;

namespace GameBase.Animations.Actors
{
    public class Stretcher : MonoBehaviour
    {
        public float StretchTimeInSeconds = 0.2f;
        public AnimationCurve HorizontalCurve;
        public AnimationCurve VerticalCurve;

        private Vector3 originalScale;
        private bool isRunning = false;
        private float timeCounterInSeconds = 0.0f;

        private void Awake()
        {
            originalScale = transform.localScale;
        }

        public void Execute()
        {
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
            if (timeCounterInSeconds > StretchTimeInSeconds)
            {
                timeCounterInSeconds = StretchTimeInSeconds;
                isRunning = false;
            }

            var ratio = timeCounterInSeconds / StretchTimeInSeconds;
            var scaleX = HorizontalCurve.Evaluate(ratio);
            var scaleY = VerticalCurve.Evaluate(ratio);
            transform.localScale = transform.localScale.WhereX(originalScale.x * scaleX).WhereY(originalScale.y * scaleY);
        }
    }
}