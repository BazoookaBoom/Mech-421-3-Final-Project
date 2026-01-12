namespace Lab6Ex2
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.ZeroedButton = new System.Windows.Forms.Button();
            this.comboBoxCOMPorts = new System.Windows.Forms.ComboBox();
            this.ConnectButton = new System.Windows.Forms.Button();
            this.CWSpinLabel = new System.Windows.Forms.Label();
            this.CCWSpinLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.SpeedLabel = new System.Windows.Forms.Label();
            this.ForwardLabel = new System.Windows.Forms.Label();
            this.BackwardLabel = new System.Windows.Forms.Label();
            this.chart2 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).BeginInit();
            this.SuspendLayout();
            // 
            // serialPort1
            // 
            this.serialPort1.PortName = "COM7";
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(43, 20);
            this.trackBar1.Maximum = 65535;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(153, 45);
            this.trackBar1.TabIndex = 2;
            this.trackBar1.Value = 32768;
            this.trackBar1.Scroll += new System.EventHandler(this.TrackBar1_Scroll);
            // 
            // ZeroedButton
            // 
            this.ZeroedButton.Location = new System.Drawing.Point(77, 49);
            this.ZeroedButton.Name = "ZeroedButton";
            this.ZeroedButton.Size = new System.Drawing.Size(80, 23);
            this.ZeroedButton.TabIndex = 6;
            this.ZeroedButton.Text = "Zero Speed";
            this.ZeroedButton.UseVisualStyleBackColor = true;
            this.ZeroedButton.Click += new System.EventHandler(this.ZeroedButton_Click);
            // 
            // comboBoxCOMPorts
            // 
            this.comboBoxCOMPorts.FormattingEnabled = true;
            this.comboBoxCOMPorts.Items.AddRange(new object[] {
            "COM3",
            "COM7"});
            this.comboBoxCOMPorts.Location = new System.Drawing.Point(9, 34);
            this.comboBoxCOMPorts.Name = "comboBoxCOMPorts";
            this.comboBoxCOMPorts.Size = new System.Drawing.Size(121, 21);
            this.comboBoxCOMPorts.TabIndex = 10;
            // 
            // ConnectButton
            // 
            this.ConnectButton.Location = new System.Drawing.Point(142, 34);
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new System.Drawing.Size(75, 23);
            this.ConnectButton.TabIndex = 11;
            this.ConnectButton.Text = "Connect";
            this.ConnectButton.UseVisualStyleBackColor = true;
            this.ConnectButton.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // CWSpinLabel
            // 
            this.CWSpinLabel.AutoSize = true;
            this.CWSpinLabel.Location = new System.Drawing.Point(202, 21);
            this.CWSpinLabel.Name = "CWSpinLabel";
            this.CWSpinLabel.Size = new System.Drawing.Size(25, 13);
            this.CWSpinLabel.TabIndex = 12;
            this.CWSpinLabel.Text = "CW";
            // 
            // CCWSpinLabel
            // 
            this.CCWSpinLabel.AutoSize = true;
            this.CCWSpinLabel.Location = new System.Drawing.Point(5, 21);
            this.CCWSpinLabel.Name = "CCWSpinLabel";
            this.CCWSpinLabel.Size = new System.Drawing.Size(32, 13);
            this.CCWSpinLabel.TabIndex = 13;
            this.CCWSpinLabel.Text = "CCW";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "COM Port Selection";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBoxCOMPorts);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.ConnectButton);
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(248, 66);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Serial Connection Control";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.SpeedLabel);
            this.groupBox4.Controls.Add(this.ZeroedButton);
            this.groupBox4.Controls.Add(this.trackBar1);
            this.groupBox4.Controls.Add(this.CWSpinLabel);
            this.groupBox4.Controls.Add(this.CCWSpinLabel);
            this.groupBox4.Location = new System.Drawing.Point(6, 90);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox4.Size = new System.Drawing.Size(248, 97);
            this.groupBox4.TabIndex = 19;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Continuous Control";
            // 
            // SpeedLabel
            // 
            this.SpeedLabel.AutoSize = true;
            this.SpeedLabel.Location = new System.Drawing.Point(103, 75);
            this.SpeedLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.SpeedLabel.Name = "SpeedLabel";
            this.SpeedLabel.Size = new System.Drawing.Size(13, 13);
            this.SpeedLabel.TabIndex = 20;
            this.SpeedLabel.Text = "0";
            // 
            // ForwardLabel
            // 
            this.ForwardLabel.AutoSize = true;
            this.ForwardLabel.BackColor = System.Drawing.Color.Lime;
            this.ForwardLabel.Location = new System.Drawing.Point(415, 9);
            this.ForwardLabel.Name = "ForwardLabel";
            this.ForwardLabel.Size = new System.Drawing.Size(45, 13);
            this.ForwardLabel.TabIndex = 20;
            this.ForwardLabel.Text = "Forward";
            // 
            // BackwardLabel
            // 
            this.BackwardLabel.AutoSize = true;
            this.BackwardLabel.BackColor = System.Drawing.SystemColors.ControlDark;
            this.BackwardLabel.Location = new System.Drawing.Point(354, 9);
            this.BackwardLabel.Name = "BackwardLabel";
            this.BackwardLabel.Size = new System.Drawing.Size(55, 13);
            this.BackwardLabel.TabIndex = 21;
            this.BackwardLabel.Text = "Backward";
            // 
            // chart2
            // 
            chartArea1.Name = "ChartArea1";
            this.chart2.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chart2.Legends.Add(legend1);
            this.chart2.Location = new System.Drawing.Point(269, 40);
            this.chart2.Name = "chart2";
            this.chart2.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.None;
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chart2.Series.Add(series1);
            this.chart2.Size = new System.Drawing.Size(260, 284);
            this.chart2.TabIndex = 22;
            this.chart2.Text = "chart2";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(541, 336);
            this.Controls.Add(this.chart2);
            this.Controls.Add(this.BackwardLabel);
            this.Controls.Add(this.ForwardLabel);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label SpeedLabel;
        private System.Windows.Forms.Button ZeroedButton;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Label CWSpinLabel;
        private System.Windows.Forms.Label CCWSpinLabel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox comboBoxCOMPorts;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button ConnectButton;
        private System.Windows.Forms.Label ForwardLabel;
        private System.Windows.Forms.Label BackwardLabel; 
        internal System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart2;
    }
}

