using System.Collections.Generic;
using UnityEngine;

namespace GameBase.Utils
{
    public static class Utils
    {
        public static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp;
            temp = lhs;
            lhs = rhs;
            rhs = temp;
        }
        
        public static float EaseCubic( float ratio )
        {
            ratio = Mathf.Clamp01(ratio);
            return (-2.0f*ratio*ratio*ratio + 3.0f*ratio*ratio);
        }
        
        public static bool ApproximatelyZero( float a, float epsilon )
        {	
            return ( a > 0 ) ? (a < epsilon) : a > -epsilon;
        }

        public static bool ApproximatelyZero( float a )
        {	
            return ( a > 0 ) ? (a < Mathf.Epsilon) : a > -Mathf.Epsilon;
        }
        
        public static bool IsIndexValid<T>( this List<T> list, int index ) { return ( index >= 0 && index < list.Count ); }
	
        public static bool IsIndexValid<T>( this T[] list, int index ) { return ( index >= 0 && index < list.Length ); }	

        public static Vector3 Damp(Vector3 source, float factor, float deltaTime) {
            return source * Mathf.Pow(factor, deltaTime);
        }
    }
}