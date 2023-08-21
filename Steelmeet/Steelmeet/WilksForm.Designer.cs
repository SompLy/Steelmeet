namespace Powermeet2
{
    partial class WilksForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            txtWeight = new TextBox();
            label2 = new Label();
            label3 = new Label();
            txtBodyWeight = new TextBox();
            btnMale = new Button();
            btnFemale = new Button();
            panel1 = new Panel();
            dataGridView = new DataGridView();
            rbtnFullMeet = new RadioButton();
            rbtnBench = new RadioButton();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(47, 91);
            label1.Name = "label1";
            label1.Size = new Size(83, 15);
            label1.TabIndex = 0;
            label1.Text = "Wieght Lifed : ";
            // 
            // txtWeight
            // 
            txtWeight.Location = new Point(134, 88);
            txtWeight.Name = "txtWeight";
            txtWeight.Size = new Size(51, 23);
            txtWeight.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(47, 125);
            label2.Name = "label2";
            label2.Size = new Size(79, 15);
            label2.TabIndex = 2;
            label2.Text = "Bodyweight : ";
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label3.AutoSize = true;
            label3.Location = new Point(72, 22);
            label3.Name = "label3";
            label3.Size = new Size(97, 15);
            label3.TabIndex = 3;
            label3.Text = "WILKS Calculator";
            // 
            // txtBodyWeight
            // 
            txtBodyWeight.Location = new Point(134, 122);
            txtBodyWeight.Name = "txtBodyWeight";
            txtBodyWeight.Size = new Size(51, 23);
            txtBodyWeight.TabIndex = 4;
            // 
            // btnMale
            // 
            btnMale.Location = new Point(61, 160);
            btnMale.Name = "btnMale";
            btnMale.Size = new Size(50, 23);
            btnMale.TabIndex = 5;
            btnMale.Text = "Male";
            btnMale.UseVisualStyleBackColor = true;
            btnMale.Click += btnMale_Click;
            // 
            // btnFemale
            // 
            btnFemale.Location = new Point(132, 160);
            btnFemale.Name = "btnFemale";
            btnFemale.Size = new Size(53, 23);
            btnFemale.TabIndex = 6;
            btnFemale.Text = "female";
            btnFemale.UseVisualStyleBackColor = true;
            btnFemale.Click += btnFemale_Click;
            // 
            // panel1
            // 
            panel1.Controls.Add(label3);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(579, 37);
            panel1.TabIndex = 7;
            // 
            // dataGridView
            // 
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Location = new Point(226, 33);
            dataGridView.Name = "dataGridView";
            dataGridView.RowTemplate.Height = 25;
            dataGridView.Size = new Size(325, 150);
            dataGridView.TabIndex = 8;
            // 
            // rbtnFullMeet
            // 
            rbtnFullMeet.AutoSize = true;
            rbtnFullMeet.Location = new Point(37, 54);
            rbtnFullMeet.Name = "rbtnFullMeet";
            rbtnFullMeet.Size = new Size(74, 19);
            rbtnFullMeet.TabIndex = 9;
            rbtnFullMeet.TabStop = true;
            rbtnFullMeet.Text = "Full Meet";
            rbtnFullMeet.UseVisualStyleBackColor = true;
            // 
            // rbtnBench
            // 
            rbtnBench.AutoSize = true;
            rbtnBench.Location = new Point(117, 54);
            rbtnBench.Name = "rbtnBench";
            rbtnBench.Size = new Size(86, 19);
            rbtnBench.TabIndex = 10;
            rbtnBench.TabStop = true;
            rbtnBench.Text = "Bench Only";
            rbtnBench.UseVisualStyleBackColor = true;
            // 
            // WilksForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(579, 200);
            Controls.Add(rbtnBench);
            Controls.Add(rbtnFullMeet);
            Controls.Add(dataGridView);
            Controls.Add(panel1);
            Controls.Add(btnFemale);
            Controls.Add(btnMale);
            Controls.Add(txtBodyWeight);
            Controls.Add(label2);
            Controls.Add(txtWeight);
            Controls.Add(label1);
            Name = "WilksForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Wilks";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox txtWeight;
        private Label label2;
        private Label label3;
        private TextBox txtBodyWeight;
        private Button btnMale;
        private Button btnFemale;
        private Panel panel1;
        private DataGridView dataGridView;
        private RadioButton rbtnFullMeet;
        private RadioButton rbtnBench;
    }
}