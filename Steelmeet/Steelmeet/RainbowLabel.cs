///////////////////////////////
//                           //
// Written by Edvin Öhrström //
//                           //
///////////////////////////////

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
        public int colorIndex = 0;
        public RainbowLabel( SMKontrollpanel _smk ) 
        {
            smk = _smk;

            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            AutoSize = true;
            Font = new Font( "Segoe UI", 80.25F, FontStyle.Bold, GraphicsUnit.Point );
            ForeColor = Color.White;
            Location = new Point( 266, 237 );
            Margin = new Padding( 0 );
            Name = "lbl_Record";
            Size = new Size( 1032, 568 );
            TabIndex = 24;
            Text = "Klubb Rekord !!!\r\nÖrebro KK\r\nJunior\r\nKlassiskt Bänkpress\r\n";
            TextAlign = ContentAlignment.MiddleCenter;
            Visible = false;

            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 10;
            timer.Tick += ( sender, e ) => 
            {
                if ( colorIndex >= smk.rainbowColor.GetRainbowArray().Count - 1 )
                    colorIndex = 0;
                Invalidate();
                colorIndex++;
            };
            timer.Start();
            
        }

        protected override void OnPaint( PaintEventArgs e )
        {
            Graphics g = e.Graphics;
            GraphicsPath p = new GraphicsPath();

            float x = ( Width + g.MeasureString( Text, Font ).Width ) / 4;
            float y = ( Height - g.MeasureString( Text, Font ).Height ) / 2;

            Brush rainbowBrush = new SolidBrush( smk.rainbowColor.GetRainbowArray()[ colorIndex ] );

            p.AddString(
                Text,
                Font.FontFamily,
                ( int )Font.Style,
                g.DpiY * Font.Size / 72,
                new PointF( x, y ),
                new StringFormat { Alignment = StringAlignment.Center } );          // set options here (e.g. center alignment)

            using( Pen outlinePen = new Pen( System.Drawing.Color.Black, 22 ) )
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
