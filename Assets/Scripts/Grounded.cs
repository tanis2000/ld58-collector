using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace App
{
    public class Grounded : MonoBehaviour
    {
        public bool OnGround = false;
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
            if (CollidesWithGround())
            {
                OnGround = true;
                transform.position = transform.position + Vector3.down * distance;
            }
        }
        
        private bool CollidesWithGround()
        {
            ray = new Ray(startPoint, Vector3.down);
            Debug.DrawRay(ray.origin, ray.direction * 1f, Color.coral);
            if (Physics.RaycastNonAlloc(ray, hits, 1f) > 0)
            {
                foreach (var hit in hits)
                {
                    Debug.Log("Collided with " + hit.collider.gameObject.name);
                    if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
                    {
                        distance = hit.distance;
                        return true;
                    }
                }
            }

            return false;
        }

    }
}