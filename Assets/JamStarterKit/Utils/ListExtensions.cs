using System.Collections.Generic;
using System.Linq;

namespace GameBase.Utils
{
    public static class ListExtensions
    {
        public static T Random<T>(this IList<T> list)
        {
            return list.Any() ? list[UnityEngine.Random.Range(0, list.Count)] : default;
        }
    }
}