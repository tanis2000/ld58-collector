using UnityEngine;
using Random = UnityEngine.Random;

namespace GameBase.Animations
{
    public class Shaker : MonoBehaviour
    {
        public float Amount = 0.1f;
        public float RotationAmount = 1f;
        public float Duration = 0.1f;
        public bool Decreasing;

        private Vector3 startPos;
        private float durationLeft;
        private float startAngle;
        private Transform t;

        public void Shake()
        {
            t = transform;
            durationLeft = Duration;
            startPos = t.position;
            startAngle = t.rotation.eulerAngles.z;
        }

        private void Update()
        {
            if (!(durationLeft > 0)) return;

            durationLeft -= Time.deltaTime;
            transform.position = durationLeft > 0 ? startPos + GetOffset(AdjustedAmount()) : startPos;
            var angle = durationLeft > 0 ? startAngle + AdjustedAngleAmount() : startAngle;
            transform.localRotation = Quaternion.Euler(0, 0, angle);
        }

        private float AdjustedAmount()
        {
            return Decreasing ? Mathf.Lerp(0, Amount, durationLeft / Duration) : Amount;
        }

        private float AdjustedAngleAmount()
        {
            return Decreasing ? Mathf.Lerp(0, RotationAmount, durationLeft / Duration) : RotationAmount;
        }

        private static Vector3 GetOffset(float max)
        {
            return new Vector3(Random.Range(-max, max), Random.Range(-max, max), 0);
        }
    }
}