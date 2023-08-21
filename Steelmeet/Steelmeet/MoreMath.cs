using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteelMeet
{
    internal class MoreMath
    {
        public static float Max(float x, float y)
        {
            return Math.Max(x, y);
        }

        public static float Max(float x, float y, float z)
        {
            return Math.Max(x, Math.Max(y, z));
        }

        public static float Max(float w, float x, float y, float z)
        {
            return Math.Max(w, Math.Max(x, Math.Max(y, z)));
        }

        public static float Max(params float[] values)
        {
            return Enumerable.Max(values);
        }
    }
}
