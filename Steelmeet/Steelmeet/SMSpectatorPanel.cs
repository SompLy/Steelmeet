using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SteelMeet
{
    public partial class SMSpectatorPanel : Form
    {
        public SMSpectatorPanel( Form _form )
        {
            smk = ( SMKontrollpanel )_form;

            InitializeComponent();
        }

        private void SMSpectatorPanel_Load( object sender, EventArgs e )
        {
            SetupDataGridView();

        }

        SMKontrollpanel smk;
        Fullscreen fullscreen = new Fullscreen();
        bool isFullscreen = false;

        List<Label>LiftingOrderListLabels = new List<Label>();
        List<Label>GroupLiftingOrderListLabels = new List<Label>();

        void SetupDataGridView()
        {
            CloneColumns( smk.dataGridViewControlPanel.Columns );
        }
        public DataGridViewRow CloneRow( DataGridViewRow _row )
        {
            DataGridViewRow clonedRow = ( DataGridViewRow )_row.Clone();
            clonedRow.Cells.RemoveAt( clonedRow.Cells.Count - 1 );
            clonedRow.Cells.RemoveAt( clonedRow.Cells.Count - 1 );
            clonedRow.Cells.RemoveAt( clonedRow.Cells.Count - 1 );
            int indexOffset = 0;
            for ( Int32 index = 0 ; index < _row.Cells.Count ; index++ )
            {
                if ( index != 7 && index != 8 && index != 9 ) // Klonar inte höjder
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
            foreach ( DataGridViewColumn column in _columns )
            {
                if ( column.Index != 7 && column.Index != 8 && column.Index != 9 ) // Klonar inte höjder
                    dataGridViewSpectatorPanel.Columns.Add( ( DataGridViewColumn )column.Clone() );
            }
        }
        protected override bool ProcessCmdKey( ref Message msg, Keys keyData ) //Hanterar all input från tagentbord
        {
            try
            {
                if ( keyData == Keys.F )
                {
                    fullscreen.ToggleFullscreen( isFullscreen, this );
                    isFullscreen = !isFullscreen;
                    return true;
                }
            }
            catch ( Exception ex )
            {
                MessageBox.Show( ex.Message );
            }
            return base.ProcessCmdKey( ref msg, keyData );
        }

        public void UpdateAll()
        {
            // DataGridView
            UpdateDataGriview();
            // Lables update
            UpdateinfoPanel();
            UpdateLiftingOrderLables();
            UpdateTimer();
            UpdateNextGroup();
            // Redraw plates
            infopanel_Controlpanel.Invalidate();
            infopanel_Controlpanel2.Invalidate();
        }
        void UpdateNextGroup() 
        {
            if ( GroupLiftingOrderListLabels.Count < 1 )
            GroupLiftingOrderListLabels.AddRange( new System.Windows.Forms.Label[] { lbl_groupLiftOrder_control_1, lbl_groupLiftOrder_control_2, lbl_groupLiftOrder_control_3, lbl_groupLiftOrder_control_4,
                                                        lbl_groupLiftOrder_control_5, lbl_groupLiftOrder_control_6, lbl_groupLiftOrder_control_7, lbl_groupLiftOrder_control_8,
                                                        lbl_groupLiftOrder_control_9, lbl_groupLiftOrder_control_10, lbl_groupLiftOrder_control_11, lbl_groupLiftOrder_control_12,
                                                        lbl_groupLiftOrder_control_13, lbl_groupLiftOrder_control_14, lbl_groupLiftOrder_control_15, lbl_groupLiftOrder_control_16,
                                                        lbl_groupLiftOrder_control_17, lbl_groupLiftOrder_control_18, lbl_groupLiftOrder_control_19, lbl_groupLiftOrder_control_20} );
            for ( int i = 0 ; i < smk.GroupLiftingOrderListLabels.Count ; i++ )
                GroupLiftingOrderListLabels[ i ].Text = smk.GroupLiftingOrderListLabels[ i ].Text;
        }
        void UpdateTimer() 
        {
            lbl_timerLyft.Text = smk.lbl_timerLyft.Text;
            lbl_timerLapp.Text = smk.lbl_timerLapp.Text;
        }
        private void UpdateDataGriview()
        {
            // Uppdatera values och färg
            dataGridViewSpectatorPanel.Rows.Clear();
            for ( int i = 0 ; i < smk.dataGridViewControlPanel.RowCount ; i++ )
                dataGridViewSpectatorPanel.Rows.Add( CloneRow( smk.dataGridViewControlPanel.Rows[ i ] ) );
            // Markera nuvarande lyftare
            dataGridViewSpectatorPanel.CurrentCell = null; // Annars markerar den alltid första cellen
            for ( int columnIndex = 1 ; columnIndex <= 5 ; columnIndex++ )
                dataGridViewSpectatorPanel.Rows[ smk.LiftingOrderList[ 0 ].index - smk.groupRowFixer ].Cells[ columnIndex ].Selected = true;
        }
        private void UpdateinfoPanel()
        {
            lbl_Name.Text = smk.lbl_Name.Text;
            lbl_currentWeight.Text = smk.lbl_currentWeight.Text;
            lbl_Avlyft.Text = smk.lbl_Avlyft.Text;
            lbl_Height.Text = smk.lbl_Height.Text;
            lbl_25x.Text = smk.lbl_25x.Text;
            if ( smk.LiftingOrderList.Count > 1 )
            {
                lbl_Name2.Text = smk.lbl_Name2.Text;
                lbl_currentWeight2.Text = smk.lbl_currentWeight2.Text;
                lbl_Avlyft2.Text = smk.lbl_Avlyft2.Text;
                lbl_Height2.Text = smk.lbl_Height2.Text;
                lbl_25x2.Text = smk.lbl_25x2.Text;
            }
        }
        private void UpdateLiftingOrderLables()
        {
            if ( LiftingOrderListLabels.Count < 1 )
                LiftingOrderListLabels.AddRange( new System.Windows.Forms.Label[]
                    {
                    lbl_liftOrder_control_1, lbl_liftOrder_control_2, lbl_liftOrder_control_3, lbl_liftOrder_control_4,
                    lbl_liftOrder_control_5, lbl_liftOrder_control_6, lbl_liftOrder_control_7, lbl_liftOrder_control_8,
                    lbl_liftOrder_control_9, lbl_liftOrder_control_10
                    } );

            for ( int i = 0 ; i < smk.LiftingOrderListLabels.Count ; i++ )
                LiftingOrderListLabels[ i ].Text = smk.LiftingOrderListLabels[ i ].Text;
        }

        public void infopanel_Controlpanel_Paint( object sender, PaintEventArgs e )
        {
            Graphics g = e.Graphics;

            List<System.Drawing.Color> plateColorList = new List<System.Drawing.Color>
    {
        smk.plateInfo.col_plate50, smk.plateInfo.col_plate25, smk.plateInfo.col_plate20, smk.plateInfo.col_plate15, smk.plateInfo.col_plate10,
        smk.plateInfo.col_plate5, smk.plateInfo.col_plate25small, smk.plateInfo.col_plate125, smk.plateInfo.col_plate05, smk.plateInfo.col_plate025
    };

            List<int> paintedPlatesList = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            DrawPlates( g, smk.usedPlatesList, plateColorList, paintedPlatesList );
        }

        private void infopanel_Controlpanel2_Paint( object sender, PaintEventArgs e )
        {
            Graphics g = e.Graphics;

            List<System.Drawing.Color> plateColorList = new List<System.Drawing.Color>
    {
        smk.plateInfo.col_plate50, smk.plateInfo.col_plate25, smk.plateInfo.col_plate20, smk.plateInfo.col_plate15, smk.plateInfo.col_plate10,
        smk.plateInfo.col_plate5, smk.plateInfo.col_plate25small, smk.plateInfo.col_plate125, smk.plateInfo.col_plate05, smk.plateInfo.col_plate025
    };

            List<int> paintedPlatesList = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            DrawPlates( g, smk.usedPlatesList2, plateColorList, paintedPlatesList );
        }
        private void DrawPlates( Graphics g, List<int> usedPlatesList, List<System.Drawing.Color> plateColorList, List<int> paintedPlatesList )
        {
            // x1 = Börja rita här
            // y1 = Börja rita här
            // x2 = 
            // y2 =

            int x1 = -7, y1 = 84, x2 = -5, y2 = 196;
            Pen p = new Pen( System.Drawing.Color.Red, 22 );
            int offset = 28;

            for ( int i = 0 ; i < 10 ; )
            {
                if ( Enumerable.Any( usedPlatesList ) && usedPlatesList[ i ] > paintedPlatesList[ i ] )
                {
                    p.Color = plateColorList[ i ];

                    g.DrawLine( p, x1 + offset, y1, x2 + offset, y2 );
                    offset += 28;

                    paintedPlatesList[ i ]++;
                }
                else { i++; }
            }

            p.Color = System.Drawing.Color.DarkGray;
            g.DrawLine( p, x1 + offset, 126, x2 + offset, 154 );
        }
    }
}