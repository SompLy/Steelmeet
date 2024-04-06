using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteelMeet
{
    public class RoundedLine: Control
    {
        public Color LineColor { get; set; } = Color.Black;
        public int LineWidth { get; set; } = 1;
        public int CornerRadius { get; set; } = 10;

        protected override void OnPaint( PaintEventArgs e )
        {
            base.OnPaint( e );

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            using( Pen pen = new Pen( LineColor, LineWidth ) )
            {
                // Draw rounded line
                g.DrawLine( pen, CornerRadius, Height / 2, Width - CornerRadius, Height / 2 );
                g.DrawArc( pen, 0, Height / 2 - CornerRadius, CornerRadius * 2, CornerRadius * 2, 90, 180 );
                g.DrawArc( pen, Width - CornerRadius * 2, Height / 2 - CornerRadius, CornerRadius * 2, CornerRadius * 2, 270, 180 );
            }
        }

        protected override void OnResize( EventArgs e )
        {
            base.OnResize( e );
            Invalidate();
        }
    }
}
