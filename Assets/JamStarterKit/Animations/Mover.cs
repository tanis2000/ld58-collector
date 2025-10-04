using UnityEngine;

namespace GameBase.Animations
{
    public class Mover : MonoBehaviour
    {
        public float Speed = 1f;
        public float Offset;
        public bool NoNegatives;
        public Vector3 Direction = Vector3.up;
        public bool RandomizeOffset;

        private Vector3 originalPosition;

        private void Start()
        {
            originalPosition = transform.localPosition;

            if (RandomizeOffset)
            {
                Offset = Random.value * 100f;
            }
        }

        private void Update()
        {
            var sinVal = Mathf.Sin(Time.time * Speed + Offset * Mathf.PI);
            sinVal = NoNegatives ? Mathf.Abs(sinVal) : sinVal;
            transform.localPosition = originalPosition + Direction * sinVal;
        }
    }
}