using UnityEngine;

namespace GameBase.Utils
{
    public static class Vector3Extensions
    {
        public static Vector3 FlatX(this Vector3 v)
        {
            return new Vector3(0, v.y, v.z);
        }

        public static Vector3 FlatY(this Vector3 v)
        {
            return new Vector3(v.x, 0, v.z);
        }

        public static Vector3 FlatZ(this Vector3 v)
        {
            return new Vector3(v.x, v.y, 0);
        }

        public static Vector3 WhereX(this Vector3 v, float x)
        {
            return new Vector3(x, v.y, v.z);
        }

        public static Vector3 WhereY(this Vector3 v, float y)
        {
            return new Vector3(v.x, y, v.z);
        }

        public static Vector3 WhereZ(this Vector3 v, float z)
        {
            return new Vector3(v.x, v.y, z);
        }

        public static Vector3 AsVector3(this Vector2Int v)
        {
            return new Vector3(v.x, 0, v.y);
        }
    }
}