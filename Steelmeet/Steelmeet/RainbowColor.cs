﻿using DocumentFormat.OpenXml.Math;
using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace SteelMeet
{
    internal class RainbowColor
    {
        public struct ColorRGB
        {
            public byte R;
            public byte G;
            public byte B;
            public ColorRGB( Color value )
            {
                this.R = value.R;
                this.G = value.G;
                this.B = value.B;
            }
            public static implicit operator Color( ColorRGB rgb )
            {
                Color c = Color.FromArgb(rgb.R,rgb.G,rgb.B);
                return c;
            }
            public static explicit operator ColorRGB( Color c )
            {
                return new ColorRGB( c );
            }
        }

        private List< Color > rainbowColors = new List<Color>();
        public List< Color > GetRainbowArray() { return rainbowColors; }

        public RainbowColor() 
        {
            SetRainbowColors();
        }

        private void SetRainbowColors() 
        {
            for( double i = 0 ; i < 1 ; i += 0.01 )
            {
                ColorRGB c = HSL2RGB(i, 0.5, 0.5);
                rainbowColors.Add( (Color)c );
                //do something with the color
            }
        }

      // Given H,S,L in range of 0-1
      // Returns a Color (RGB struct) in range of 0-255
      public static ColorRGB HSL2RGB( double h, double sl, double l )
        {
            double v;
            double r,g,b;

            r = l;   // default to gray
            g = l;
            b = l;
            v = ( l <= 0.5 ) ? ( l * ( 1.0 + sl ) ) : ( l + sl - l * sl );
            if( v > 0 )
            {
                double m;
                double sv;
                int sextant;
                double fract, vsf, mid1, mid2;

                m = l + l - v;
                sv = ( v - m ) / v;
                h *= 6.0;
                sextant = ( int )h;
                fract = h - sextant;
                vsf = v * sv * fract;
                mid1 = m + vsf;
                mid2 = v - vsf;
                switch( sextant )
                {
                    case 0:
                        r = v;
                        g = mid1;
                        b = m;
                        break;
                    case 1:
                        r = mid2;
                        g = v;
                        b = m;
                        break;
                    case 2:
                        r = m;
                        g = v;
                        b = mid1;
                        break;
                    case 3:
                        r = m;
                        g = mid2;
                        b = v;
                        break;
                    case 4:
                        r = mid1;
                        g = m;
                        b = v;
                        break;
                    case 5:
                        r = v;
                        g = m;
                        b = mid2;
                        break;
                }
            }
            ColorRGB rgb;
            rgb.R = Convert.ToByte( r * 255.0f );
            rgb.G = Convert.ToByte( g * 255.0f );
            rgb.B = Convert.ToByte( b * 255.0f );
            return rgb;
        }

        
    }
}
