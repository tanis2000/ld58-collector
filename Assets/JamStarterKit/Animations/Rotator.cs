using UnityEngine;

namespace GameBase.Animations
{
    public class Rotator : MonoBehaviour
    {
        public float Speed = 1f;
        public float PulsingSpeed;
        public float PulsingMin;

        private float angle;

        private void Start()
        {
            angle = Random.value * 360f;
        }

        private void Update()
        {
            var mod = PulsingSpeed > 0 ? Mathf.Abs(Mathf.Sin(Time.time * PulsingSpeed)) + PulsingMin : 1f;
            angle -= Speed * Time.deltaTime * 60f * mod;
            transform.localRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }

        public void ChangeSpeed(float s)
        {
            Speed = s;
        }
    }
}