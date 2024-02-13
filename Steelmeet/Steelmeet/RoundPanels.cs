using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SteelMeet
{
    public class RoundPanel : Panel
    {
        public int CornerRadius { get; set; } = 10;

        public RoundPanel()
        {
            // Disable default background drawing
            SetStyle( ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true );
        }

        protected override void OnPaint( PaintEventArgs e )
        {
            base.OnPaint( e );
            DrawRoundedRectangle( e.Graphics, ClientRectangle, CornerRadius, BackColor );
        }

        public static void DrawRoundedRectangle( Graphics g, Rectangle rectangle, int cornerRadius, Color fillColor )
        {
            // Draw the rounded rectangle
            using ( GraphicsPath path = CreateRoundRectanglePath( rectangle, cornerRadius ) )
            using ( Brush brush = new SolidBrush( fillColor ) )
            {
                g.FillPath( brush, path );
            }
        }

        private static GraphicsPath CreateRoundRectanglePath( Rectangle rectangle, int cornerRadius )
        {
            int diameter = cornerRadius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(rectangle.Location, size);

            GraphicsPath path = new GraphicsPath();

            // Top-left corner
            path.AddArc( arc, 180, 90 );

            // Top-right corner
            arc.X = rectangle.Right - diameter;
            path.AddArc( arc, 270, 90 );

            // Bot-right corner
            arc.Y = rectangle.Bottom - diameter;
            path.AddArc( arc, 0, 90 );

            // Bot-left corner
            arc.X = rectangle.Left;
            path.AddArc( arc, 90, 90 );

            path.CloseFigure();

            return path;
        }
    }
}