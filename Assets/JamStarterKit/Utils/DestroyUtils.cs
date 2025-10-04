using System.Collections.Generic;
using UnityEngine;

namespace GameBase.Utils
{
    public static class DestroyUtils
    {
        public static void DestroyChildren(Transform transform)
        {
            List<GameObject> list = new List<GameObject>();
            foreach (Transform item in transform)
            {
                list.Add(item.gameObject);
            }

            list.ForEach(delegate(GameObject child)
            {
#if UNITY_EDITOR
                Object.DestroyImmediate(child.gameObject);
#else
            Object.Destroy(child.gameObject);
#endif
            });
        }
    }
}