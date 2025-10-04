using UnityEngine;

namespace GameBase.Physics
{
    public static class Calc
    {
        public static float Approach(float val, float target, float maxMove)
        {
            if (!(val > target))
            {
                return Mathf.Min(val + maxMove, target);
            }
            return Mathf.Max(val - maxMove, target);
        }
        
        public static Vector2 ClosestPointOnLine(Vector2 lineA, Vector2 lineB, Vector2 closestTo)
        {
            Vector2 v = lineB - lineA;
            float t = Vector2.Dot(closestTo - lineA, v) / Vector2.Dot(v, v);
            t = Mathf.Clamp(t, 0f, 1f);
            return lineA + v * t;
        }
    }
}