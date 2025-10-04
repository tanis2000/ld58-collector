using UnityEngine;

namespace GameBase.Physics
{
    public struct CollisionData
    {
        public Vector2 Direction;

        public Vector2 Moved;

        public Vector2 TargetPosition;

        public Platform Hit;

        public Solid Pusher;

        public static readonly CollisionData Empty;
    }
}