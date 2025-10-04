using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace App
{
    public class Grounded : MonoBehaviour
    {
        public bool OnGround = false;
        private BoxCollider groundCollider;

        private void Start()
        {
            groundCollider = GetComponentInChildren<BoxCollider>();
        }
        
        private void OnCollisionEnter(Collision other)
        {
            Debug.Log("OnCollisionEnter " + other.gameObject.name);
            OnGround = true;
        }

    }
}