using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiprekt.Utilities
{
    public static class MathExtensions
    {
        public static float ToDegrees(this float value)
        {
            return ToDegrees(value);
        }

        public static float ToDegrees(this double value)
        {
            return (float)(value * (180 / Math.PI));
        }

    }
}
