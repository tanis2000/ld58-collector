using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace App
{
    public class Grounded : MonoBehaviour
    {
        public delegate void OnHitGroundDelegate();

        public bool OnGround = false;
        public OnHitGroundDelegate OnHitGround;
        private BoxCollider groundCollider;
        private Vector3 startPoint = Vector3.zero;
        private RaycastHit[] hits = new RaycastHit[10];
        private Ray ray;
        private float distance;

        private void Start()
        {
            groundCollider = GetComponentInChildren<BoxCollider>();
        }

        private void Update()
        {
            startPoint = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            if (!OnGround && CollidesWithGround())
            {
                OnGround = true;
                transform.position = transform.position + Vector3.down * distance;
                if (OnHitGround != null)
                {
                    OnHitGround();
                }
            }
        }
        
        private bool CollidesWithGround()
        {
            ray = new Ray(startPoint, Vector3.down);
            Debug.DrawRay(ray.origin, ray.direction * 1f, Color.coral);
            var numHits = Physics.RaycastNonAlloc(ray, hits, 1f); 
            if (numHits > 0)
            {
                for (int i = 0 ; i < numHits; i++)
                {
                    RaycastHit hit = hits[i];
                    if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
                    {
                        //Debug.Log("Collided with " + hit.collider.gameObject.name);
                        distance = hit.distance;
                        return true;
                    }
                }
            }

            return false;
        }

    }
}