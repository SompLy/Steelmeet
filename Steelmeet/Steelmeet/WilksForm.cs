using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Powermeet2
{
    public partial class WilksForm : Form
    {

        DataTable dt = new DataTable();
        double WilksCoef;
        bool a = true;

        public WilksForm()
        {
            InitializeComponent();
        }

        private void btnMale_Click(object sender, EventArgs e)
        {
            double[] McoefWilks = new double[]
            {
                -216.0475144
            ,   16.2606339
            ,   -0.002388645
            ,   -0.00113732
            ,   7.01863E-6
            ,   -1.291E-8 };

            double bw = double.Parse(txtBodyWeight.Text);
            double Weight = double.Parse(txtWeight.Text);
            WilksCoef = Weight * 500 / (McoefWilks[0] + McoefWilks[1] * bw + McoefWilks[2] * Math.Pow(bw, 2) + McoefWilks[3] * Math.Pow(bw, 3) + McoefWilks[4] * Math.Pow(bw, 4) + McoefWilks[5] * Math.Pow(bw, 5));

            Display();
        }


        private void btnFemale_Click(object sender, EventArgs e)
        {
            double[] FcoefWilks = new double[]
            {
                594.31747775582
            ,   -27.23842536447
            ,   0.82112226871
            ,   -0.00930733913
            ,   4.731582E-5
            ,   -9.054E-8 };

            double bw = double.Parse(txtBodyWeight.Text);
            double Weight = double.Parse(txtWeight.Text);

            WilksCoef = Weight * 500 / (FcoefWilks[0] + FcoefWilks[1] * bw + FcoefWilks[2] * Math.Pow(bw, 2) + FcoefWilks[3] * Math.Pow(bw, 3) + FcoefWilks[4] * Math.Pow(bw, 4) + FcoefWilks[5] * Math.Pow(bw, 5));

            Display();
        }
        public void Display()
        {
            if (a)
            {
                dt.Columns.Add(" Vikt ");
                dt.Columns.Add(" Kroppsvikt ");
                dt.Columns.Add(" Wilks ");
                dt.Columns.Add(" GL ");
                a = false;
            }
            DataRow dr = dt.NewRow();
            dr[0] = double.Parse(txtBodyWeight.Text);
            dr[1] = double.Parse(txtWeight.Text);
            dr[2] = Math.Round(WilksCoef, 2).ToString(); ;


            dt.Rows.Add(dr);
            dataGridView.DataSource = dt;
        }
    }
}
