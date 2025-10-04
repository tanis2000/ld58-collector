using System;
using UnityEngine;

namespace App
{
    public class Collectible: MonoBehaviour
    {
        public Vector2 GridPosition;
        public float Speed = 10;
        private CollectibleSpawner collectibleSpawner;
        private Grounded grounded;

        private void Start()
        {
            collectibleSpawner = FindFirstObjectByType<CollectibleSpawner>();
            grounded = GetComponent<Grounded>();
        }

        private void Update()
        {
            if (transform.position.y > 0 && !grounded.OnGround)
            {
                transform.localPosition += Vector3.down * (Speed * Time.deltaTime);
            }
        }

        public void Spawn()
        {
            transform.localPosition = new Vector3(GridPosition.x * 2, 50, GridPosition.y * 2);
        }

        public void Destroy()
        {
            collectibleSpawner.RemoveCollectibleFromList(transform);
            Destroy(gameObject);
        }
    }
}