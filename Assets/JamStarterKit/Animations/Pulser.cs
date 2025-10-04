using UnityEngine;

namespace GameBase.Animations
{
    public class Pulser : MonoBehaviour
    {
        public float Amount = 0.1f;
        public float Speed = 1f;

        private float pos = -1f;

        void Update()
        {
            if (pos >= 0f)
            {
                pos = Mathf.MoveTowards(pos, 1f, Time.deltaTime * Speed);
                var stepped = Mathf.SmoothStep(0f, 1f, pos);
                var size = Mathf.Sin(Mathf.PI * stepped) * Amount + 1f;
                transform.localScale = size * Vector3.one;
            }
        }

        public void Pulsate()
        {
            pos = 0f;
        }
    }
}