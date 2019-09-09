using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlatRedBall;

namespace Shiprekt.Utilities
{
    public static class Extensions
    {
        public static float ToDegrees(this float value)
        {
            return ToDegrees(value);
        }

        public static float ToDegrees(this double value)
        {
            return (float)(value * (180 / Math.PI));
        }

        public static T Random<T>(this IEnumerable<T> enumerable, Random rand = null)
        {
            rand = rand ?? FlatRedBallServices.Random;

            T o;
            var c = enumerable.Count();
            if (c > 0)
            {
                o = enumerable.ElementAt(rand.Next(0, c));
            }
            else
            {
                o = default(T);
            }

            return o;
        }

    }
}
