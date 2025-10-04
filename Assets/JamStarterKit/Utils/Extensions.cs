using UnityEngine;

namespace GameBase.Utils
{
    public static class Extensions
    {
        public static Vector3 WithZ(this Vector2 v, float z)
        {
            return new Vector3(v.x, v.y, z);
        }
    }
}
