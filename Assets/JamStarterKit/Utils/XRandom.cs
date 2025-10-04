using System;
using System.Collections.Generic;
using System.Linq;

namespace GameBase.Utils
{
    public delegate float ItemWeight<T>(T d);

    public class XRandom
    {
        private int seed;

        private Random random;

        public int Seed => seed;

        public float BottomHeavy
        {
            get
            {
                float num = Float();
                return num * num;
            }
        }

        public float TopHeavy
        {
            get
            {
                float num = Float();
                return 1f - num * num;
            }
        }

        public XRandom(int seed)
        {
            this.seed = seed;
            random = new Random(seed);
        }

        public float Float()
        {
            return (float)random.NextDouble();
        }

        public int Int()
        {
            return random.Next();
        }

        public float Range(float start, float end)
        {
            return (end - start) * Float() + start;
        }

        public int Range(int start, int end)
        {
            return start + random.Next() % (end - start);
        }

        public T Item<T>(IList<T> array, T def = default(T), Func<T, bool> filter = null)
        {
            if (array == null)
            {
                return def;
            }

            if (filter == null)
            {
                if (array.Count == 0)
                {
                    return def;
                }

                return array[random.Next() % array.Count];
            }

            List<T> list = array.Where(filter).ToList();
            if (list.Count == 0)
            {
                return def;
            }

            return list[random.Next() % list.Count];
        }

        public List<T> ItemTake<T>(IList<T> array, int count = 1, Func<T, bool> filter = null)
        {
            if (array == null)
            {
                return new List<T>();
            }

            IEnumerable<T> source = ((filter == null) ? array.ToArray() : array.Where(filter));
            return source.OrderBy((T c) => Float()).Take(count).ToList();
        }

        public T ItemWeighted<T>(IList<T> sortedList, ItemWeight<T> weight)
        {
            List<T> list = sortedList.ToList();
            float end = list.Sum((T x) => weight(x));
            float num = Range(0f, end);
            float num2 = 0f;
            foreach (T item in list)
            {
                num2 += weight(item);
                if (num < num2)
                {
                    return item;
                }
            }

            return default(T);
        }
    }
}