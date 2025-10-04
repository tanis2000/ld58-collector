using System;
using UnityEngine;
using UnityEngine.Events;

namespace GameBase.Dev
{
    public class OnDevKey : MonoBehaviour
    {
        public KeyCode Key;
        public UnityEvent Action;

        private void Update()
        {
            if (DevKey.Down(Key))
            {
                Action?.Invoke();
            }
        }
    }
}