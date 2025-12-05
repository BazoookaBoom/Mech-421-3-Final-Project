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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ConnectButton = new System.Windows.Forms.Button();
            this.comboBoxCOMPorts = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.ClrSeqButton = new System.Windows.Forms.Button();
            this.AddSeqButton = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.speedPercentage = new System.Windows.Forms.MaskedTextBox();
            this.yCoord = new System.Windows.Forms.MaskedTextBox();
            this.xCoord = new System.Windows.Forms.MaskedTextBox();
            this.executeSeqButton = new System.Windows.Forms.Button();
            this.sequenceTextInput = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
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
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.ClrSeqButton);
            this.groupBox3.Controls.Add(this.AddSeqButton);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.speedPercentage);
            this.groupBox3.Controls.Add(this.yCoord);
            this.groupBox3.Controls.Add(this.xCoord);
            this.groupBox3.Controls.Add(this.executeSeqButton);
            this.groupBox3.Controls.Add(this.sequenceTextInput);
            this.groupBox3.Location = new System.Drawing.Point(18, 183);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(497, 450);
            this.groupBox3.TabIndex = 18;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Sequence Control";
            // 
            // ClrSeqButton
            // 
            this.ClrSeqButton.Location = new System.Drawing.Point(27, 257);
            this.ClrSeqButton.Name = "ClrSeqButton";
            this.ClrSeqButton.Size = new System.Drawing.Size(236, 39);
            this.ClrSeqButton.TabIndex = 41;
            this.ClrSeqButton.Text = "Clear Sequence";
            this.ClrSeqButton.UseVisualStyleBackColor = true;
            this.ClrSeqButton.Click += new System.EventHandler(this.ClrSeqButton_Click);
            // 
            // AddSeqButton
            // 
            this.AddSeqButton.Location = new System.Drawing.Point(27, 200);
            this.AddSeqButton.Name = "AddSeqButton";
            this.AddSeqButton.Size = new System.Drawing.Size(236, 39);
            this.AddSeqButton.TabIndex = 40;
            this.AddSeqButton.Text = "Add to Sequence";
            this.AddSeqButton.UseVisualStyleBackColor = true;
            this.AddSeqButton.Click += new System.EventHandler(this.AddSeqButton_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(22, 156);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(111, 25);
            this.label6.TabIndex = 39;
            this.label6.Text = "Speed [%]";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 97);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(143, 25);
            this.label3.TabIndex = 38;
            this.label3.Text = "Y Coord [mm]";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(142, 25);
            this.label1.TabIndex = 37;
            this.label1.Text = "X Coord [mm]";
            // 
            // speedPercentage
            // 
            this.speedPercentage.Location = new System.Drawing.Point(163, 153);
            this.speedPercentage.Mask = "000";
            this.speedPercentage.Name = "speedPercentage";
            this.speedPercentage.Size = new System.Drawing.Size(100, 31);
            this.speedPercentage.TabIndex = 36;
            // 
            // yCoord
            // 
            this.yCoord.Location = new System.Drawing.Point(163, 94);
            this.yCoord.Mask = "000";
            this.yCoord.Name = "yCoord";
            this.yCoord.Size = new System.Drawing.Size(100, 31);
            this.yCoord.TabIndex = 35;
            // 
            // xCoord
            // 
            this.xCoord.Location = new System.Drawing.Point(163, 40);
            this.xCoord.Mask = "000";
            this.xCoord.Name = "xCoord";
            this.xCoord.Size = new System.Drawing.Size(100, 31);
            this.xCoord.TabIndex = 34;
            this.xCoord.ValidatingType = typeof(System.DateTime);
            // 
            // executeSeqButton
            // 
            this.executeSeqButton.Location = new System.Drawing.Point(27, 393);
            this.executeSeqButton.Name = "executeSeqButton";
            this.executeSeqButton.Size = new System.Drawing.Size(430, 39);
            this.executeSeqButton.TabIndex = 33;
            this.executeSeqButton.Text = "Execute Sequence";
            this.executeSeqButton.UseVisualStyleBackColor = true;
            this.executeSeqButton.Click += new System.EventHandler(this.executeSeqButton_Click);
            // 
            // sequenceTextInput
            // 
            this.sequenceTextInput.BackColor = System.Drawing.SystemColors.ControlLight;
            this.sequenceTextInput.Location = new System.Drawing.Point(269, 40);
            this.sequenceTextInput.Multiline = true;
            this.sequenceTextInput.Name = "sequenceTextInput";
            this.sequenceTextInput.ReadOnly = true;
            this.sequenceTextInput.Size = new System.Drawing.Size(188, 337);
            this.sequenceTextInput.TabIndex = 32;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(562, 645);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.Button ConnectButton;
        private System.Windows.Forms.ComboBox comboBoxCOMPorts;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox sequenceTextInput;
        private System.Windows.Forms.Button executeSeqButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.MaskedTextBox speedPercentage;
        private System.Windows.Forms.MaskedTextBox yCoord;
        private System.Windows.Forms.MaskedTextBox xCoord;
        private System.Windows.Forms.Button ClrSeqButton;
        private System.Windows.Forms.Button AddSeqButton;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label3;
    }
}

