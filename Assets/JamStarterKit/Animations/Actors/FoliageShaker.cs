using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameBase.Animations.Actors
{
    public class FoliageShaker : MonoBehaviour
    {
        public SpriteRenderer Visuals;
        public float FalloutTimeInSeconds = 2.0f;
        public AnimationCurve FalloutCurve;

        private float frequency = 0;
        private static readonly int Frequency = Shader.PropertyToID("_Frequency");
        private bool isRunning = false;
        private float timeCounterInSeconds = 0.0f;

        private void Awake()
        {
            Visuals.material = Visuals.sharedMaterial;
            Visuals.material.SetFloat(Frequency, 0);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            frequency = Random.Range(0.1f, 0.7f);
            Visuals.material.SetFloat(Frequency, frequency);
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
            if (timeCounterInSeconds > FalloutTimeInSeconds)
            {
                timeCounterInSeconds = FalloutTimeInSeconds;
                isRunning = false;
            }
            
            var ratio = timeCounterInSeconds / FalloutTimeInSeconds;
            var step = FalloutCurve.Evaluate(ratio) * frequency;
            Visuals.material.SetFloat(Frequency, step);
            
        }
    }
}