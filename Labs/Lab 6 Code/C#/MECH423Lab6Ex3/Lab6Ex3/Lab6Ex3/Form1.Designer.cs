using System.IO.Ports;

namespace Lab6Ex3
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
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.comboBoxCOMPorts = new System.Windows.Forms.ComboBox();
            this.ConnectButton = new System.Windows.Forms.Button();
            this.CWSpinLabel = new System.Windows.Forms.Label();
            this.CCWSpinLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.SetPWMButton = new System.Windows.Forms.Button();
            this.PWMSetTextBox = new System.Windows.Forms.TextBox();
            this.SpeedLabel = new System.Windows.Forms.Label();
            this.ForwardLabel = new System.Windows.Forms.Label();
            this.BackwardLabel = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.PositionLabel = new System.Windows.Forms.Label();
            this.VelocityLabel = new System.Windows.Forms.Label();
            this.PositionTextBox = new System.Windows.Forms.TextBox();
            this.VelocityTextBox = new System.Windows.Forms.TextBox();
            this.HzTextBox = new System.Windows.Forms.TextBox();
            this.HzLabel = new System.Windows.Forms.Label();
            this.SaveButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // serialPort1
            // 
            this.serialPort1.PortName = "COM7";
            this.serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.SerialPort1_DataReceived);
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
            this.groupBox4.Controls.Add(this.SetPWMButton);
            this.groupBox4.Controls.Add(this.PWMSetTextBox);
            this.groupBox4.Controls.Add(this.SpeedLabel);
            this.groupBox4.Controls.Add(this.CWSpinLabel);
            this.groupBox4.Controls.Add(this.CCWSpinLabel);
            this.groupBox4.Location = new System.Drawing.Point(268, 6);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox4.Size = new System.Drawing.Size(248, 97);
            this.groupBox4.TabIndex = 19;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Continuous Control";
            // 
            // SetPWMButton
            // 
            this.SetPWMButton.Location = new System.Drawing.Point(193, 37);
            this.SetPWMButton.Name = "SetPWMButton";
            this.SetPWMButton.Size = new System.Drawing.Size(50, 35);
            this.SetPWMButton.TabIndex = 16;
            this.SetPWMButton.Text = "Set \n PWM";
            this.SetPWMButton.UseVisualStyleBackColor = true;
            this.SetPWMButton.Click += new System.EventHandler(this.SetPWMButton_Click);
            // 
            // PWMSetTextBox
            // 
            this.PWMSetTextBox.Location = new System.Drawing.Point(193, 75);
            this.PWMSetTextBox.Name = "PWMSetTextBox";
            this.PWMSetTextBox.Size = new System.Drawing.Size(50, 20);
            this.PWMSetTextBox.TabIndex = 30;
            this.PWMSetTextBox.Text = "0";
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
            this.ForwardLabel.Location = new System.Drawing.Point(119, 90);
            this.ForwardLabel.Name = "ForwardLabel";
            this.ForwardLabel.Size = new System.Drawing.Size(55, 13);
            this.ForwardLabel.TabIndex = 20;
            this.ForwardLabel.Text = "Clockwise";
            // 
            // BackwardLabel
            // 
            this.BackwardLabel.AutoSize = true;
            this.BackwardLabel.BackColor = System.Drawing.SystemColors.ControlDark;
            this.BackwardLabel.Location = new System.Drawing.Point(50, 90);
            this.BackwardLabel.Name = "BackwardLabel";
            this.BackwardLabel.Size = new System.Drawing.Size(62, 13);
            this.BackwardLabel.TabIndex = 21;
            this.BackwardLabel.Text = "CounterCW";
            // 
            // PositionLabel
            // 
            this.PositionLabel.AutoSize = true;
            this.PositionLabel.Location = new System.Drawing.Point(521, 53);
            this.PositionLabel.Name = "PositionLabel";
            this.PositionLabel.Size = new System.Drawing.Size(67, 13);
            this.PositionLabel.TabIndex = 23;
            this.PositionLabel.Text = "Position [cm]";
            // 
            // VelocityLabel
            // 
            this.VelocityLabel.AutoSize = true;
            this.VelocityLabel.Location = new System.Drawing.Point(521, 81);
            this.VelocityLabel.Name = "VelocityLabel";
            this.VelocityLabel.Size = new System.Drawing.Size(77, 13);
            this.VelocityLabel.TabIndex = 24;
            this.VelocityLabel.Text = "Velocity [RPM]";
            // 
            // PositionTextBox
            // 
            this.PositionTextBox.Location = new System.Drawing.Point(599, 50);
            this.PositionTextBox.Name = "PositionTextBox";
            this.PositionTextBox.Size = new System.Drawing.Size(50, 20);
            this.PositionTextBox.TabIndex = 25;
            this.PositionTextBox.Text = "0";
            // 
            // VelocityTextBox
            // 
            this.VelocityTextBox.Location = new System.Drawing.Point(599, 76);
            this.VelocityTextBox.Name = "VelocityTextBox";
            this.VelocityTextBox.Size = new System.Drawing.Size(50, 20);
            this.VelocityTextBox.TabIndex = 26;
            this.VelocityTextBox.Text = "0";
            // 
            // HzTextBox
            // 
            this.HzTextBox.Location = new System.Drawing.Point(599, 24);
            this.HzTextBox.Name = "HzTextBox";
            this.HzTextBox.Size = new System.Drawing.Size(50, 20);
            this.HzTextBox.TabIndex = 28;
            this.HzTextBox.Text = "0";
            // 
            // HzLabel
            // 
            this.HzLabel.AutoSize = true;
            this.HzLabel.Location = new System.Drawing.Point(521, 27);
            this.HzLabel.Name = "HzLabel";
            this.HzLabel.Size = new System.Drawing.Size(79, 13);
            this.HzLabel.TabIndex = 27;
            this.HzLabel.Text = "Frequency [Hz]";
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(179, 79);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(75, 23);
            this.SaveButton.TabIndex = 16;
            this.SaveButton.Text = "Save?";
            this.SaveButton.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(656, 402);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.HzTextBox);
            this.Controls.Add(this.HzLabel);
            this.Controls.Add(this.VelocityTextBox);
            this.Controls.Add(this.PositionTextBox);
            this.Controls.Add(this.VelocityLabel);
            this.Controls.Add(this.PositionLabel);
            this.Controls.Add(this.BackwardLabel);
            this.Controls.Add(this.ForwardLabel);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label SpeedLabel;
        private System.Windows.Forms.Label CWSpinLabel;
        private System.Windows.Forms.Label CCWSpinLabel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox comboBoxCOMPorts;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button ConnectButton;
        private System.Windows.Forms.Label ForwardLabel;
        private System.Windows.Forms.Label BackwardLabel;
        //private System.Windows.Forms.DataVisualization.Charting.Chart chart2;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label PositionLabel;
        private System.Windows.Forms.Label VelocityLabel;
        private System.Windows.Forms.TextBox PositionTextBox;
        private System.Windows.Forms.TextBox VelocityTextBox;
        private System.Windows.Forms.TextBox HzTextBox;
        private System.Windows.Forms.Label HzLabel;
        private System.Windows.Forms.Button SetPWMButton;
        private System.Windows.Forms.TextBox PWMSetTextBox;
        private System.Windows.Forms.Button SaveButton;
    }
}

