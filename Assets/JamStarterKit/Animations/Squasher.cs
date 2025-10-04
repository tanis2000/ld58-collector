using System;
using UnityEngine;

namespace GameBase.Animations
{
    public class Squasher : MonoBehaviour
    {
        public Transform Visuals;
        public float Amount = 0.1f;
        public float Speed = 1f;
        
        private Vector3 originalScale;
        private float pos = -1f;
        
        private void OnEnable()
        {
            originalScale = Visuals.localScale;
        }

        void Update()
        {
            if(pos >= 0f)
            {
                pos = Mathf.MoveTowards(pos, 1f, Time.deltaTime * Speed);
                var stepped = Mathf.SmoothStep(0f, 1f, pos);
                var size = Mathf.Sin(Mathf.PI * stepped) * Amount + 1f;

                var height = stepped;
                
                var v = new Vector3(size * originalScale.x, height * originalScale.y, size * originalScale.z);
                Visuals.localScale = v;
            }
        }
        
        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log(collision.transform.name);
            Squash();
        }

        public void Squash()
        {
            pos = 0;
        }
    }
}