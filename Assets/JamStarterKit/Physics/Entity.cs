using UnityEngine;

namespace GameBase.Physics
{
    public class Entity: MonoBehaviour
    {
        [SerializeField]
        new Collider collider;
        
        [SerializeField]
        private bool collidable = true;
        
        public Vector2 Position
        {
            get => transform.position;
            set => transform.position = value;
        }

        public Collider Collider
        {
            get => collider;
            set => collider = value;
        }

        public bool Collidable
        {
            get => collidable;
            set => collidable = value;
        }
        
        public float X
        {
            get
            {
                return Position.x;
            }
            set
            {
                Position = new Vector2(value, transform.position.y);
            }
        }

        public float Y
        {
            get
            {
                return Position.y;
            }
            set
            {
                Position = new Vector2(transform.position.x, value);
            }
        }
        
        public void RemoveSelf()
        {
           Destroy(gameObject);
        }
        
        public bool CollideCheck(Entity other)
        {
            return Collide.Check(this, other);
        }

        public bool CollideCheck(Entity other, Vector2 at)
        {
            return Collide.Check(this, other, at);
        }
        
        public bool CollideCheck<T>() where T : Entity
        {
            return Collide.Check(this, FindObjectsOfType<T>());
        }
        
        public bool CollideCheck<T>(Vector2 at) where T : Entity
        {
            return Collide.Check(this, FindObjectsOfType<T>(), at);
        }
        
        public bool CollideCheckOutside<T>(Vector2 at) where T : Entity
        {
            foreach (Entity entity in FindObjectsOfType<T>())
            {
                if (!Collide.Check(this, entity) && Collide.Check(this, entity, at))
                {
                    return true;
                }
            }
            return false;
        }
        
        public T CollideFirst<T>(Vector2 at) where T : Entity
        {
            var res = FindObjectsOfType<T>();
            return Collide.First(this, FindObjectsOfType<T>(), at) as T;
        }
        
        public T CollideFirstOutside<T>(Vector2 at) where T : Entity
        {
            foreach (Entity entity in FindObjectsOfType<T>())
            {
                if (!Collide.Check(this, entity) && Collide.Check(this, entity, at))
                {
                    return entity as T;
                }
            }
            return null;
        }
    }
}