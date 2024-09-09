///////////////////////////////
//                           //
// Written by Edvin Öhrström //
//                           //
///////////////////////////////

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SteelMeet
{
    public class RoundPanel : Panel
    {
        public int radius { get; set; } = 10;

        public RoundPanel()
        {
            SetStyle( ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true );
        }

        public static void DrawRoundedRectangle( Graphics g, Rectangle rectangle, int cornerRadius, Color fillColor )
        {
            using ( GraphicsPath graphicsPath = CreateRoundRectanglePath( rectangle, cornerRadius ) )
            using ( Brush solidBrush = new SolidBrush( fillColor ) )
                g.FillPath( solidBrush, graphicsPath );
        }

        private static GraphicsPath CreateRoundRectanglePath( Rectangle rectangle, int cornerRadius )
        {
            int diameter = cornerRadius * 2;
            Size size = new Size( diameter, diameter );
            Rectangle arc = new Rectangle( rectangle.Location, size );

            GraphicsPath graphicsPath = new GraphicsPath();

            graphicsPath.AddArc( arc, 180, 90 );

            arc.X = rectangle.Right - diameter;
            graphicsPath.AddArc( arc, 270, 90 );

            arc.Y = rectangle.Bottom - diameter;
            graphicsPath.AddArc( arc, 0, 90 );

            arc.X = rectangle.Left;
            graphicsPath.AddArc( arc, 90, 90 );

            graphicsPath.CloseFigure();

            return graphicsPath;
        }
    }
}