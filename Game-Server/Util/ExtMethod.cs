using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Game_Server.Util
{
    public static class ExtMethod
    {
        private static Random rng = new Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static Queue<T> ToQueue<T>(this IList<T> list)
        {
            Queue<T> queue = new Queue<T>();
            foreach(var element in list)
            {
                queue.Enqueue(element);
            }
            return queue;
        }

        public static bool Exists<T>(this Hashtable table) where T : class
        {
            return table.Contains(typeof(T));
        }

        public static bool Put<T>(this Hashtable table, DbSet<T> data) where T : class
        {
            if(table.Contains(typeof(T)))
            {
                return false;
            }
            table.Add(typeof(T), data);
            return true;
        }

        public static DbSet<T> Get<T>(this Hashtable table) where T : class
        {
            return (DbSet<T>)table[typeof(T)];
        }

        public static double StdDev(this IEnumerable<int> values)
        {
            double ret = 0;
            int count = values.Count();
            if (count > 1)
            {
                //Compute the Average
                double avg = values.Average();

                //Perform the Sum of (value-avg)^2
                double sum = values.Sum(d => (d - avg) * (d - avg));

                //Put it all together
                ret = Math.Sqrt(sum / count);
            }
            return ret;
        }

        public static double StdDev(this IEnumerable<double> values)
        {
            double ret = 0;
            int count = values.Count();
            if (count > 1)
            {
                //Compute the Average
                double avg = values.Average();

                //Perform the Sum of (value-avg)^2
                double sum = values.Sum(d => (d - avg) * (d - avg));

                //Put it all together
                ret = Math.Sqrt(sum / count);
            }
            return ret;
        }
    }
}
