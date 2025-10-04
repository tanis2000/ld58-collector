using System;
using UnityEngine;

namespace GameBase.Dev
{
    public class DevSpeedControl : MonoBehaviour
    {
        public float Fast = 10f;
        public float Slow = 0.1f;
        public bool IsToggle;
        public KeyCode FastKey = KeyCode.Tab;
        public KeyCode SlowKey = KeyCode.LeftShift;


        private bool state;

        private void Update()
        {
            if (!Application.isEditor) return;

            if (IsToggle)
            {
                if (state && Input.GetKey(FastKey) || Input.GetKey(SlowKey))
                {
                    Time.timeScale = 1f;
                    state = false;
                    return;
                }

                if (Input.GetKey(FastKey))
                {
                    Time.timeScale = Fast;
                    state = true;
                }

                if (Input.GetKey(SlowKey))
                {
                    Time.timeScale = Slow;
                    state = true;
                }

                return;
            }

            var speed = 1f;
            if (Input.GetKey(FastKey)) speed = Fast;
            if (Input.GetKey(SlowKey)) speed = Slow;
            Time.timeScale = speed;
        }
    }
}