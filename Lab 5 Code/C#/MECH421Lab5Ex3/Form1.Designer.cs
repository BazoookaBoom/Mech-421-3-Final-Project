namespace MECH421Lab5Ex3
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
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.ZeroedButton = new System.Windows.Forms.Button();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.CWSpinLabel = new System.Windows.Forms.Label();
            this.CCWSpinLabel = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.CCWOneStepButton = new System.Windows.Forms.Button();
            this.CWOneStepButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ControllerModeButton = new System.Windows.Forms.Button();
            this.ControllerModeLabel = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBoxCOMPorts = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ConnectButton = new System.Windows.Forms.Button();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.ZeroedButton);
            this.groupBox4.Controls.Add(this.trackBar1);
            this.groupBox4.Controls.Add(this.CWSpinLabel);
            this.groupBox4.Controls.Add(this.CCWSpinLabel);
            this.groupBox4.Location = new System.Drawing.Point(558, 183);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(497, 169);
            this.groupBox4.TabIndex = 23;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Continuous Control";
            // 
            // ZeroedButton
            // 
            this.ZeroedButton.Location = new System.Drawing.Point(154, 94);
            this.ZeroedButton.Margin = new System.Windows.Forms.Padding(6);
            this.ZeroedButton.Name = "ZeroedButton";
            this.ZeroedButton.Size = new System.Drawing.Size(161, 44);
            this.ZeroedButton.TabIndex = 6;
            this.ZeroedButton.Text = "Zero Speed";
            this.ZeroedButton.UseVisualStyleBackColor = true;
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(85, 37);
            this.trackBar1.Margin = new System.Windows.Forms.Padding(6);
            this.trackBar1.Maximum = 255;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(306, 90);
            this.trackBar1.TabIndex = 2;
            this.trackBar1.Value = 127;
            // 
            // CWSpinLabel
            // 
            this.CWSpinLabel.AutoSize = true;
            this.CWSpinLabel.Location = new System.Drawing.Point(403, 40);
            this.CWSpinLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.CWSpinLabel.Name = "CWSpinLabel";
            this.CWSpinLabel.Size = new System.Drawing.Size(47, 25);
            this.CWSpinLabel.TabIndex = 12;
            this.CWSpinLabel.Text = "CW";
            // 
            // CCWSpinLabel
            // 
            this.CCWSpinLabel.AutoSize = true;
            this.CCWSpinLabel.Location = new System.Drawing.Point(11, 40);
            this.CCWSpinLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.CCWSpinLabel.Name = "CCWSpinLabel";
            this.CCWSpinLabel.Size = new System.Drawing.Size(62, 25);
            this.CCWSpinLabel.TabIndex = 13;
            this.CCWSpinLabel.Text = "CCW";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.CCWOneStepButton);
            this.groupBox3.Controls.Add(this.CWOneStepButton);
            this.groupBox3.Location = new System.Drawing.Point(18, 183);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(497, 169);
            this.groupBox3.TabIndex = 22;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Single Step Control";
            // 
            // CCWOneStepButton
            // 
            this.CCWOneStepButton.Location = new System.Drawing.Point(258, 65);
            this.CCWOneStepButton.Margin = new System.Windows.Forms.Padding(6);
            this.CCWOneStepButton.Name = "CCWOneStepButton";
            this.CCWOneStepButton.Size = new System.Drawing.Size(230, 52);
            this.CCWOneStepButton.TabIndex = 1;
            this.CCWOneStepButton.Text = "CCW One Step";
            this.CCWOneStepButton.UseVisualStyleBackColor = true;
            // 
            // CWOneStepButton
            // 
            this.CWOneStepButton.Location = new System.Drawing.Point(10, 65);
            this.CWOneStepButton.Margin = new System.Windows.Forms.Padding(6);
            this.CWOneStepButton.Name = "CWOneStepButton";
            this.CWOneStepButton.Size = new System.Drawing.Size(216, 52);
            this.CWOneStepButton.TabIndex = 0;
            this.CWOneStepButton.Text = "CW One Step";
            this.CWOneStepButton.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.ControllerModeButton);
            this.groupBox2.Controls.Add(this.ControllerModeLabel);
            this.groupBox2.Location = new System.Drawing.Point(558, 19);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(497, 126);
            this.groupBox2.TabIndex = 21;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Controller Mode Control";
            // 
            // ControllerModeButton
            // 
            this.ControllerModeButton.Location = new System.Drawing.Point(281, 47);
            this.ControllerModeButton.Margin = new System.Windows.Forms.Padding(6);
            this.ControllerModeButton.Name = "ControllerModeButton";
            this.ControllerModeButton.Size = new System.Drawing.Size(207, 40);
            this.ControllerModeButton.TabIndex = 3;
            this.ControllerModeButton.Text = "Step";
            this.ControllerModeButton.UseVisualStyleBackColor = true;
            // 
            // ControllerModeLabel
            // 
            this.ControllerModeLabel.AutoSize = true;
            this.ControllerModeLabel.Location = new System.Drawing.Point(14, 55);
            this.ControllerModeLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.ControllerModeLabel.Name = "ControllerModeLabel";
            this.ControllerModeLabel.Size = new System.Drawing.Size(237, 25);
            this.ControllerModeLabel.TabIndex = 4;
            this.ControllerModeLabel.Text = "Controller Mode Toggle";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBoxCOMPorts);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.ConnectButton);
            this.groupBox1.Location = new System.Drawing.Point(18, 19);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(497, 126);
            this.groupBox1.TabIndex = 20;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Serial Connection Control";
            // 
            // comboBoxCOMPorts
            // 
            this.comboBoxCOMPorts.FormattingEnabled = true;
            this.comboBoxCOMPorts.Location = new System.Drawing.Point(18, 65);
            this.comboBoxCOMPorts.Margin = new System.Windows.Forms.Padding(6);
            this.comboBoxCOMPorts.Name = "comboBoxCOMPorts";
            this.comboBoxCOMPorts.Size = new System.Drawing.Size(238, 33);
            this.comboBoxCOMPorts.TabIndex = 10;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(33, 34);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(201, 25);
            this.label2.TabIndex = 15;
            this.label2.Text = "COM Port Selection";
            // 
            // ConnectButton
            // 
            this.ConnectButton.Location = new System.Drawing.Point(284, 65);
            this.ConnectButton.Margin = new System.Windows.Forms.Padding(6);
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new System.Drawing.Size(150, 44);
            this.ConnectButton.TabIndex = 11;
            this.ConnectButton.Text = "Connect";
            this.ConnectButton.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1081, 450);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button ZeroedButton;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Label CWSpinLabel;
        private System.Windows.Forms.Label CCWSpinLabel;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button CCWOneStepButton;
        private System.Windows.Forms.Button CWOneStepButton;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button ControllerModeButton;
        private System.Windows.Forms.Label ControllerModeLabel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox comboBoxCOMPorts;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button ConnectButton;
        private System.IO.Ports.SerialPort serialPort1;
    }
}

