///////////////////////////////
//                           //
// Written by Edvin Öhrström //
//                           //
///////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteelMeet
{
    public class BlendColor
    {
        public static Color BlendColorRGB( Color color, Color secondColor, float amount )
        {
            byte r = ( byte )( color.R * amount + secondColor.R * ( 1 - amount ) );
            byte g = ( byte )( color.G * amount + secondColor.G * ( 1 - amount ) );
            byte b = ( byte )( color.B * amount + secondColor.B * ( 1 - amount ) );
            return Color.FromArgb( r, g, b );
        }
    }
}
