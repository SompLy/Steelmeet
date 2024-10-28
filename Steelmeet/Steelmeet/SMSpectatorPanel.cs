///////////////////////////////
//                           //
// Written by Edvin Öhrström //
//                           //
///////////////////////////////

using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Font = System.Drawing.Font;

namespace SteelMeet
{
    public partial class SMSpectatorPanel: Form
    {
        public SMSpectatorPanel( Form _form )
        {
            smk = ( SMKontrollpanel )_form;
            InitializeComponent();
        }

        private void SMSpectatorPanel_Load( object sender, EventArgs e )
        {
            lbl_Record_Rainbow = new RainbowLabel( smk );
            lbl_Record_Rainbow.Parent = dataGridViewSpectatorPanel;
            lbl_Record_Rainbow.Location = new Point( 0, 0 ); // Maybe gay af
            lbl_Record_Rainbow.BackColor = System.Drawing.Color.Transparent;
            lbl_Record_Rainbow.AutoSize = true;
            dataGridViewSpectatorPanel.Controls.Add( lbl_Record_Rainbow );

            SetupDataGridView();
            UpdateAll();
        }

        public SMKontrollpanel smk;
        public RainbowLabel lbl_Record_Rainbow;
        Fullscreen fullscreen = new Fullscreen();
        bool isFullscreen = false;

        List<Label> LiftingOrderListLabels = new List<Label>();
        List<Label> GroupLiftingOrderListLabels = new List<Label>();

        void SetupDataGridView()
        {
            CloneColumns( smk.dataGridViewControlPanel.Columns );
        }
        public DataGridViewRow CloneRow( DataGridViewRow _row )
        {
            DataGridViewRow clonedRow = (DataGridViewRow)_row.Clone();
            clonedRow.Cells.RemoveAt( clonedRow.Cells.Count - 1 );
            clonedRow.Cells.RemoveAt( clonedRow.Cells.Count - 1 );
            clonedRow.Cells.RemoveAt( clonedRow.Cells.Count - 1 );
            clonedRow.Cells.RemoveAt( clonedRow.Cells.Count - 1 );
            clonedRow.Cells.RemoveAt( clonedRow.Cells.Count - 1 );
            clonedRow.Cells.RemoveAt( clonedRow.Cells.Count - 1 );
            clonedRow.Cells.RemoveAt( clonedRow.Cells.Count - 1 );
            clonedRow.Cells.RemoveAt( clonedRow.Cells.Count - 1 );
            clonedRow.Cells.RemoveAt( clonedRow.Cells.Count - 1 );
            clonedRow.Cells.RemoveAt( clonedRow.Cells.Count - 1 );
            clonedRow.Cells.RemoveAt( clonedRow.Cells.Count - 1 );
            clonedRow.Cells.RemoveAt( clonedRow.Cells.Count - 1 );

            int indexOffset = 0;
            for( Int32 index = 0 ; index < _row.Cells.Count ; index++ )
            {
                if( index != 7 && index != 8 && index != 9 &&
                    index != 10 && index != 11 && index != 12 &&
                    index != 16 && index != 17 && index != 18 &&
                    index != 4 && index != 19 && index != 20 ) // Does not clone heights
                {
                    clonedRow.Cells[ index - indexOffset ].Value = _row.Cells[ index ].Value;
                    clonedRow.Cells[ index - indexOffset ].Style = _row.Cells[ index ].Style;
                }
                else
                    indexOffset++;
            }
            return clonedRow;
        }
        private void CloneColumns( DataGridViewColumnCollection _columns )
        {
            foreach( DataGridViewColumn column in _columns )
            {
                if(
                    column.Index != 7 && column.Index != 8 && column.Index != 9 &&
                    column.Index != 10 && column.Index != 11 && column.Index != 12 &&
                    column.Index != 16 && column.Index != 17 && column.Index != 18 &&
                    column.Index != 4 && column.Index != 19 && column.Index != 20
                ) // Does not clone rack heights
                {
                    DataGridViewColumn clonedCloumn = (DataGridViewColumn)column.Clone();

                    if( clonedCloumn.DefaultCellStyle.Font == null )
                    {
                        clonedCloumn.DefaultCellStyle.Font = new Font( "DefaultFontFamily", 10 ); // Set your default font
                    }
                    else
                    {
                        // Clone the font to avoid modifying the original column's font
                        Font font = new Font(clonedCloumn.DefaultCellStyle.Font.FontFamily, clonedCloumn.DefaultCellStyle.Font.Size);
                        clonedCloumn.DefaultCellStyle.Font = font;
                    }

                    dataGridViewSpectatorPanel.Columns.Add( clonedCloumn );
                }
            }
        }
        protected override bool ProcessCmdKey( ref Message msg, Keys keyData ) // Handles all keyboard input
        {
            try
            {
                if( keyData == Keys.F )
                {
                    fullscreen.ToggleFullscreen( isFullscreen, this );
                    isFullscreen = !isFullscreen;
                    return true;
                }
            }
            catch( Exception ex )
            {
                MessageBox.Show( ex.Message );
            }
            return base.ProcessCmdKey( ref msg, keyData );
        }

        public void UpdateAll()
        {
            // DataGridView
            UpdateDataGriview();

            // DataGridView auto size
            if( smk.cb_dataGridViewAutoSize.Checked )
                UpdateDataGridviewFontAutoSize();

            // Lables update
            UpdateinfoPanel();
            UpdateLiftingOrderLables();

            // UpdateTimer(); Not updated here because it has to be updated at the same time, since it runs all the time
            UpdateNextGroup();

            // Redraw plates
            infopanel_Spectatorpanel.Invalidate();
            infopanel_Spectatorpanel2.Invalidate();
        }
        void UpdateNextGroup()
        {
            // Header text
            lbl_OpeningLift.Text = smk.lbl_OpeningLift.Text;

            // If the next group is complete hide Next group
            if( lbl_groupLiftOrder_control_1.Text[ 5 ] == '0' && lbl_groupLiftOrder_control_1.Text[ 6 ] == ' ' )
                p_NextGroupLifters.Visible = false;
            else
                p_NextGroupLifters.Visible = true;

            // Lifters text
            if( GroupLiftingOrderListLabels.Count < 1 )
                GroupLiftingOrderListLabels.AddRange( new System.Windows.Forms.Label[] { lbl_groupLiftOrder_control_1, lbl_groupLiftOrder_control_2, lbl_groupLiftOrder_control_3, lbl_groupLiftOrder_control_4,
                                                        lbl_groupLiftOrder_control_5, lbl_groupLiftOrder_control_6, lbl_groupLiftOrder_control_7, lbl_groupLiftOrder_control_8,
                                                        lbl_groupLiftOrder_control_9, lbl_groupLiftOrder_control_10, lbl_groupLiftOrder_control_11, lbl_groupLiftOrder_control_12,
                                                        lbl_groupLiftOrder_control_13, lbl_groupLiftOrder_control_14, lbl_groupLiftOrder_control_15, lbl_groupLiftOrder_control_16,
                                                        lbl_groupLiftOrder_control_17, lbl_groupLiftOrder_control_18, lbl_groupLiftOrder_control_19, lbl_groupLiftOrder_control_20} );
            for( int i = 0 ; i < smk.GroupLiftingOrderListLabels.Count ; i++ )
                GroupLiftingOrderListLabels[ i ].Text = smk.GroupLiftingOrderListLabels[ i ].Text;
        }
        void UpdateDataGridviewFontAutoSize()
        {
            // None of theese are corrent :(
            //int currentWidth = dataGridViewSpectatorPanel.Columns[ 0 ].Width * dataGridViewSpectatorPanel.ColumnCount;
            //int currentHeight = dataGridViewSpectatorPanel.Rows[ 0 ].Height * dataGridViewSpectatorPanel.RowCount;
            //int goalWidth = dataGridViewSpectatorPanel.Width;
            //int goalHeight = dataGridViewSpectatorPanel.Height;
            // Every font change adds 


            // DEBUG
            //lbl_gridWidth.Text = currentWidth.ToString() + "/" + goalWidth;
            //lbl_gridHeight.Text = currentHeight.ToString() + "/" + goalHeight;

        }
        public void UpdateDataGriview()
        {
            // Update values and colors
            dataGridViewSpectatorPanel.Rows.Clear();
            if( dataGridViewSpectatorPanel.ColumnCount > 0 ) // There must be columns to add more rows
            {
                for (int i = 0; i < smk.dataGridViewControlPanel.RowCount; i++)
                {
                    dataGridViewSpectatorPanel.Rows.Add( CloneRow( smk.dataGridViewControlPanel.Rows[ i ] ) );
                }

                // Select current lifter
                dataGridViewSpectatorPanel.CurrentCell = null; // Otherwise it will always select the first cell
                if( smk.dataGridViewControlPanel.RowCount > 1 && smk.LiftingOrderList.Count > 0 )
                    for( int columnIndex = 1 ; columnIndex <= 5 ; columnIndex++ )
                        dataGridViewSpectatorPanel.Rows[ smk.LiftingOrderList[ 0 ].index - smk.groupRowFixer ].Cells[ columnIndex ].Selected = true;
            }
        }
        public void UpdateDataGridviewFont( float _fontSize )
        {
            if( _fontSize > 0 )
            {
                Font newFont = new Font("Segoe UI", _fontSize);
                Font strikeoutFont = new Font("Segoe UI", _fontSize, FontStyle.Strikeout);

                // Set the default font for the entire DataGridView
                dataGridViewSpectatorPanel.DefaultCellStyle.Font = newFont;
                dataGridViewSpectatorPanel.ColumnHeadersDefaultCellStyle.Font = newFont;

                // Subscribe to the CellFormatting event
                dataGridViewSpectatorPanel.CellFormatting += ( sender, e ) =>
                {
                    if( e.RowIndex >= 0 && e.ColumnIndex >= 0 )
                    {
                        DataGridViewCell cell = dataGridViewSpectatorPanel.Rows[e.RowIndex].Cells[e.ColumnIndex];

                        if( cell.Style.BackColor == System.Drawing.Color.Red )
                        {
                            // If the cell has a red background, set the font to strikeoutFont
                            e.CellStyle.Font = strikeoutFont;
                        }
                        else
                        {
                            // Otherwise, set the font to newFont
                            e.CellStyle.Font = newFont;
                        }
                    }
                };
            }

            // Instantly resize gridView
            dataGridViewSpectatorPanel.AutoResizeColumns();
            dataGridViewSpectatorPanel.AutoResizeRows();
            UpdateDataGridviewFontAutoSize();
        }
        private void UpdateinfoPanel()
        {
            if( smk.LiftingOrderList.Count > 0 )
            {
                infopanel_Spectatorpanel.Visible = true;
                lbl_Name.Text = smk.lbl_Name.Text;
                lbl_currentWeight.Text = smk.lbl_currentWeight.Text;
                lbl_Avlyft.Text = smk.lbl_Avlyft.Text;
                lbl_Height.Text = smk.lbl_Height.Text;
                lbl_25x.Text = smk.lbl_25x.Text;
            }
            else
            {
                // If there is not next lifter just make it empty to not be confusing
                infopanel_Spectatorpanel.Visible = false;
            }

            if( smk.LiftingOrderList.Count > 1 )
            {
                infopanel_Spectatorpanel2.Visible = true;
                lbl_Name2.Text = smk.lbl_Name2.Text;
                lbl_currentWeight2.Text = smk.lbl_currentWeight2.Text;
                lbl_Avlyft2.Text = smk.lbl_Avlyft2.Text;
                lbl_Height2.Text = smk.lbl_Height2.Text;
                lbl_25x2.Text = smk.lbl_25x2.Text;
            }
            else
            {
                // If there is not next lifter just make it empty to not be confusing
                infopanel_Spectatorpanel2.Visible = false;
            }
        }
        private void UpdateLiftingOrderLables()
        {
            if( LiftingOrderListLabels.Count < 1 )
                LiftingOrderListLabels.AddRange( new System.Windows.Forms.Label[]
                    {
                    lbl_liftOrder_control_1, lbl_liftOrder_control_2, lbl_liftOrder_control_3, lbl_liftOrder_control_4,
                    lbl_liftOrder_control_5, lbl_liftOrder_control_6, lbl_liftOrder_control_7, lbl_liftOrder_control_8,
                    lbl_liftOrder_control_9, lbl_liftOrder_control_10, lbl_liftOrder_control_11, lbl_liftOrder_control_12,
                    lbl_liftOrder_control_13, lbl_liftOrder_control_14, lbl_liftOrder_control_15, lbl_liftOrder_control_16,
                    lbl_liftOrder_control_17, lbl_liftOrder_control_18, lbl_liftOrder_control_19, lbl_liftOrder_control_20
                    } );

            for( int i = 0 ; i < smk.LiftingOrderListLabels.Count ; i++ )
                LiftingOrderListLabels[ i ].Text = smk.LiftingOrderListLabels[ i ].Text;

            // Offset panel to make space for lifting order

            int headerPixels = 53;
            int stepPixels = 32; // Height of label plus space inbetween

            int nextGroupCount = 0;

            switch( smk.groupIndexCurrent )
            {
                case 0:
                    nextGroupCount = smk.group2Count;
                    break;
                case 1:
                    if( smk.groupIndexCount == 2 )
                        nextGroupCount = smk.group1Count;
                    else
                        nextGroupCount = smk.group3Count;
                    break;
                case 2:
                    nextGroupCount = smk.group1Count;
                    break;

                default:
                    break;
            }

            int offset = stepPixels * nextGroupCount;

            p_NextGroupLifters.Location = new Point( p_NextGroupLifters.Location.X, 1080 - offset - headerPixels );
        }

        // Drwaing shit

        private void DrawPlates( Graphics g, List<int> usedPlatesList, List<System.Drawing.Color> plateColorList, List<int> paintedPlatesList )
        {
            // x1 = Börja rita här
            // y1 = Börja rita här
            // x2 = 
            // y2 =

            int x1 = -7, y1 = 84, x2 = -7, y2 = 196;
            int offset = 28;

            for( int i = 0 ; i < 10 ; )
            {
                if( Enumerable.Any( usedPlatesList ) && usedPlatesList[ i ] > paintedPlatesList[ i ] )
                {
                    if( plateColorList[ i ] != System.Drawing.Color.Silver )
                    {
                        Brush gradientBrush = new LinearGradientBrush( new Point( -2, 0 ), new Point( 12, 0 ),
                            BlendColor.BlendColorRGB( System.Drawing.Color.Black, plateColorList[ i ], 0.3f ), plateColorList[ i ] );
                        Pen p1 = new Pen( gradientBrush, 22 );
                        g.DrawLine( p1, x1 + offset, y1, x2 + offset, y2 );
                    }
                    else
                    {
                        Brush gradientBrush = new LinearGradientBrush( new Point( 0, 0 ), new Point( 20, 10 ),
                            BlendColor.BlendColorRGB( System.Drawing.Color.Black, System.Drawing.Color.Silver, 0.3f ), System.Drawing.Color.Silver );
                        Pen p2 = new Pen( gradientBrush, 22 );
                        g.DrawLine( p2, x1 + offset, y1, x2 + offset, y2 );
                    }
                    offset += 28;

                    paintedPlatesList[ i ]++;
                }
                else { i++; }
            }

            Pen p = new Pen(System.Drawing.Color.Red, 22);
            p.Color = System.Drawing.Color.DarkGray;
            g.DrawLine( p, x1 + offset, 126, x2 + offset, 154 );
        }

        public void infopanel_SpectatorPanel_Paint( object sender, PaintEventArgs e )
        {
            Graphics g = e.Graphics;
            RoundPanel.DrawRoundedRectangle( g, infopanel_Spectatorpanel.ClientRectangle, 12, System.Drawing.Color.FromArgb( 27, 38, 44 ) );

            List<System.Drawing.Color> plateColorList = new List<System.Drawing.Color>
    {
        smk.plateInfo.col_plate50, smk.plateInfo.col_plate25, smk.plateInfo.col_plate20, smk.plateInfo.col_plate15, smk.plateInfo.col_plate10,
        smk.plateInfo.col_plate5, smk.plateInfo.col_plate25small, smk.plateInfo.col_plate125, smk.plateInfo.col_plate05, smk.plateInfo.col_plate025
    };

            List<int> paintedPlatesList = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            DrawPlates( g, smk.usedPlatesList, plateColorList, paintedPlatesList );
        }

        private void infopanel_SpectatorPanel2_Paint( object sender, PaintEventArgs e )
        {
            Graphics g = e.Graphics;
            RoundPanel.DrawRoundedRectangle( g, infopanel_Spectatorpanel2.ClientRectangle, 12, System.Drawing.Color.FromArgb( 27, 38, 44 ) );

            List<System.Drawing.Color> plateColorList = new List<System.Drawing.Color>
    {
        smk.plateInfo.col_plate50, smk.plateInfo.col_plate25, smk.plateInfo.col_plate20, smk.plateInfo.col_plate15, smk.plateInfo.col_plate10,
        smk.plateInfo.col_plate5, smk.plateInfo.col_plate25small, smk.plateInfo.col_plate125, smk.plateInfo.col_plate05, smk.plateInfo.col_plate025
    };

            List<int> paintedPlatesList = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            if( smk.LiftingOrderList.Count > 1 )
            {
                DrawPlates( g, smk.usedPlatesList2, plateColorList, paintedPlatesList );
                infopanel_Spectatorpanel2.Visible = true;
            }
            else
            {
                g.Clear( infopanel_Spectatorpanel2.BackColor );
                //infopanel_Spectatorpanel2.Visible = false;
            }
        }

        // Lifting order
        private void panel10_Paint( object sender, PaintEventArgs e )
        {
            Graphics g = e.Graphics;
            RoundPanel.DrawRoundedRectangle( g, panel10.ClientRectangle, 12, System.Drawing.Color.FromArgb( 27, 38, 44 ) );
        }

        // Next group
        private void panel11_Paint( object sender, PaintEventArgs e )
        {
            Graphics g = e.Graphics;
            RoundPanel.DrawRoundedRectangle( g, p_NextGroupLifters.ClientRectangle, 12, System.Drawing.Color.FromArgb( 27, 38, 44 ) );
        }

        // Timer
        private void panel5_Paint( object sender, PaintEventArgs e )
        {
            Graphics g = e.Graphics;
            RoundPanel.DrawRoundedRectangle( g, panel5.ClientRectangle, 12, System.Drawing.Color.FromArgb( 27, 38, 44 ) );
        }

    }
}