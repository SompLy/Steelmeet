using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteelMeet
{
    public class RainbowLabel : Label
    {
        SMKontrollpanel smk;
        public RainbowLabel( SMKontrollpanel _smk ) 
        {
            smk = _smk;
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 10;
            timer.Tick += ( sender, e ) => { Invalidate(); };
            timer.Start();
        }

        protected override void OnPaint( PaintEventArgs e )
        {
            base.OnPaint( e );

            Graphics g = e.Graphics;
            GraphicsPath p = new GraphicsPath();

            float x = ( Width + g.MeasureString( Text, Font ).Width ) / 4;
            float y = ( Height - g.MeasureString( Text, Font ).Height ) / 2;

            Brush rainbowBrush = new SolidBrush( smk.rainbowColor.GetRainbowArray()[ smk.millisecondsRecord ] );

            p.AddString(
                Text,
                Font.FontFamily,
                ( int )Font.Style,
                g.DpiY * Font.Size / 72,
                new PointF( x, y ),
                new StringFormat { Alignment = StringAlignment.Center } );          // set options here (e.g. center alignment)

            using( Pen outlinePen = new Pen( System.Drawing.Color.Black, 8 ) )
            {
                g.InterpolationMode = InterpolationMode.High;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                g.CompositingQuality = CompositingQuality.HighQuality;

                g.DrawPath( outlinePen, p );
                g.FillPath( rainbowBrush, p );
                //g.DrawString( Text, Font, Brushes.Black, x, y ); // Draw the text
            }
        }
    }
}
