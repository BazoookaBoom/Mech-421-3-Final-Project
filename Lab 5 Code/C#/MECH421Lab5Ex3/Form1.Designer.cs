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
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ControllerModeButton = new System.Windows.Forms.Button();
            this.ControllerModeLabel = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ConnectButton = new System.Windows.Forms.Button();
            this.comboBoxCOMPorts = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.targetYVal = new System.Windows.Forms.TextBox();
            this.targetXVal = new System.Windows.Forms.TextBox();
            this.MoveToTarget = new System.Windows.Forms.Button();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox5.SuspendLayout();
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
            this.groupBox4.Text = "Speed Control";
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
            this.ZeroedButton.Click += new System.EventHandler(this.ZeroedButton_Click);
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(85, 37);
            this.trackBar1.Margin = new System.Windows.Forms.Padding(6);
            this.trackBar1.Maximum = 254;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(306, 90);
            this.trackBar1.TabIndex = 2;
            // 
            // CWSpinLabel
            // 
            this.CWSpinLabel.AutoSize = true;
            this.CWSpinLabel.Location = new System.Drawing.Point(403, 40);
            this.CWSpinLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.CWSpinLabel.Name = "CWSpinLabel";
            this.CWSpinLabel.Size = new System.Drawing.Size(67, 25);
            this.CWSpinLabel.TabIndex = 12;
            this.CWSpinLabel.Text = "100%";
            // 
            // CCWSpinLabel
            // 
            this.CCWSpinLabel.AutoSize = true;
            this.CCWSpinLabel.Location = new System.Drawing.Point(11, 40);
            this.CCWSpinLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.CCWSpinLabel.Name = "CCWSpinLabel";
            this.CCWSpinLabel.Size = new System.Drawing.Size(43, 25);
            this.CCWSpinLabel.TabIndex = 13;
            this.CCWSpinLabel.Text = "0%";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.button3);
            this.groupBox3.Controls.Add(this.button2);
            this.groupBox3.Controls.Add(this.button1);
            this.groupBox3.Controls.Add(this.button4);
            this.groupBox3.Location = new System.Drawing.Point(18, 183);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(497, 450);
            this.groupBox3.TabIndex = 22;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Manual Control";
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.button3.Location = new System.Drawing.Point(183, 290);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(127, 127);
            this.button3.TabIndex = 30;
            this.button3.Text = "-Y";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.button2.Location = new System.Drawing.Point(183, 29);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(127, 127);
            this.button2.TabIndex = 29;
            this.button2.Text = "+Y";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.button1.Location = new System.Drawing.Point(316, 160);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(127, 127);
            this.button1.TabIndex = 28;
            this.button1.Text = "+X";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.button4.Location = new System.Drawing.Point(50, 160);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(127, 127);
            this.button4.TabIndex = 27;
            this.button4.Text = "-X";
            this.button4.UseVisualStyleBackColor = true;
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
            this.ControllerModeButton.Text = "Manual";
            this.ControllerModeButton.UseVisualStyleBackColor = true;
            this.ControllerModeButton.Click += new System.EventHandler(this.ControllerModeButton_Click);
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
            this.groupBox1.Controls.Add(this.ConnectButton);
            this.groupBox1.Controls.Add(this.comboBoxCOMPorts);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(18, 19);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(497, 126);
            this.groupBox1.TabIndex = 20;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Serial Connection Control";
            // 
            // ConnectButton
            // 
            this.ConnectButton.Location = new System.Drawing.Point(292, 62);
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new System.Drawing.Size(165, 42);
            this.ConnectButton.TabIndex = 17;
            this.ConnectButton.Text = "Connect";
            this.ConnectButton.UseVisualStyleBackColor = true;
            this.ConnectButton.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // comboBoxCOMPorts
            // 
            this.comboBoxCOMPorts.FormattingEnabled = true;
            this.comboBoxCOMPorts.Location = new System.Drawing.Point(38, 62);
            this.comboBoxCOMPorts.Name = "comboBoxCOMPorts";
            this.comboBoxCOMPorts.Size = new System.Drawing.Size(196, 33);
            this.comboBoxCOMPorts.TabIndex = 16;
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
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label4);
            this.groupBox5.Controls.Add(this.label5);
            this.groupBox5.Controls.Add(this.targetYVal);
            this.groupBox5.Controls.Add(this.targetXVal);
            this.groupBox5.Controls.Add(this.MoveToTarget);
            this.groupBox5.Location = new System.Drawing.Point(558, 370);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(497, 263);
            this.groupBox5.TabIndex = 31;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Relative Coordinate Control";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 84);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 25);
            this.label4.TabIndex = 8;
            this.label4.Text = "Delta Y";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 45);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(82, 25);
            this.label5.TabIndex = 7;
            this.label5.Text = "Delta X";
            // 
            // targetYVal
            // 
            this.targetYVal.Location = new System.Drawing.Point(124, 81);
            this.targetYVal.Name = "targetYVal";
            this.targetYVal.Size = new System.Drawing.Size(138, 31);
            this.targetYVal.TabIndex = 6;
            // 
            // targetXVal
            // 
            this.targetXVal.Location = new System.Drawing.Point(124, 42);
            this.targetXVal.Name = "targetXVal";
            this.targetXVal.Size = new System.Drawing.Size(138, 31);
            this.targetXVal.TabIndex = 5;
            // 
            // MoveToTarget
            // 
            this.MoveToTarget.Location = new System.Drawing.Point(313, 42);
            this.MoveToTarget.Name = "MoveToTarget";
            this.MoveToTarget.Size = new System.Drawing.Size(154, 76);
            this.MoveToTarget.TabIndex = 4;
            this.MoveToTarget.Text = "Send Command";
            this.MoveToTarget.UseVisualStyleBackColor = true;
            this.MoveToTarget.Click += new System.EventHandler(this.MoveToTarget_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1081, 645);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button ZeroedButton;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Label CWSpinLabel;
        private System.Windows.Forms.Label CCWSpinLabel;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button ControllerModeButton;
        private System.Windows.Forms.Label ControllerModeLabel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.Button ConnectButton;
        private System.Windows.Forms.ComboBox comboBoxCOMPorts;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox targetYVal;
        private System.Windows.Forms.TextBox targetXVal;
        private System.Windows.Forms.Button MoveToTarget;
    }
}

