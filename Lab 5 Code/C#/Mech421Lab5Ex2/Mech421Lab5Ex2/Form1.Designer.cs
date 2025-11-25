namespace Mech421Lab5Ex2
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
            this.CWOneStepButton = new System.Windows.Forms.Button();
            this.CCWOneStepButton = new System.Windows.Forms.Button();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.ControllerModeButton = new System.Windows.Forms.Button();
            this.ControllerModeLabel = new System.Windows.Forms.Label();
            this.SpeedControl = new System.Windows.Forms.Label();
            this.ZeroedButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // CWOneStepButton
            // 
            this.CWOneStepButton.Location = new System.Drawing.Point(37, 132);
            this.CWOneStepButton.Name = "CWOneStepButton";
            this.CWOneStepButton.Size = new System.Drawing.Size(108, 27);
            this.CWOneStepButton.TabIndex = 0;
            this.CWOneStepButton.Text = "CWOneStepButton";
            this.CWOneStepButton.UseVisualStyleBackColor = true;
            this.CWOneStepButton.Click += new System.EventHandler(this.CWOneStepButton_Click);
            // 
            // CCWOneStepButton
            // 
            this.CCWOneStepButton.Location = new System.Drawing.Point(161, 132);
            this.CCWOneStepButton.Name = "CCWOneStepButton";
            this.CCWOneStepButton.Size = new System.Drawing.Size(115, 27);
            this.CCWOneStepButton.TabIndex = 1;
            this.CCWOneStepButton.Text = "CCWOneStepButton";
            this.CCWOneStepButton.UseVisualStyleBackColor = true;
            // 
            // serialPort1
            // 
            this.serialPort1.PortName = "COM4";
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(37, 214);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(132, 45);
            this.trackBar1.TabIndex = 2;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // ControllerModeButton
            // 
            this.ControllerModeButton.Location = new System.Drawing.Point(168, 77);
            this.ControllerModeButton.Name = "ControllerModeButton";
            this.ControllerModeButton.Size = new System.Drawing.Size(108, 28);
            this.ControllerModeButton.TabIndex = 3;
            this.ControllerModeButton.Text = "Step";
            this.ControllerModeButton.UseVisualStyleBackColor = true;
            this.ControllerModeButton.Click += new System.EventHandler(this.ControllerModeButton_Click);
            // 
            // ControllerModeLabel
            // 
            this.ControllerModeLabel.AutoSize = true;
            this.ControllerModeLabel.Location = new System.Drawing.Point(169, 52);
            this.ControllerModeLabel.Name = "ControllerModeLabel";
            this.ControllerModeLabel.Size = new System.Drawing.Size(104, 13);
            this.ControllerModeLabel.TabIndex = 4;
            this.ControllerModeLabel.Text = "ControllerModeLabel";
            this.ControllerModeLabel.Click += new System.EventHandler(this.ControllerModeLabel_Click);
            // 
            // SpeedControl
            // 
            this.SpeedControl.AutoSize = true;
            this.SpeedControl.Location = new System.Drawing.Point(68, 198);
            this.SpeedControl.Name = "SpeedControl";
            this.SpeedControl.Size = new System.Drawing.Size(74, 13);
            this.SpeedControl.TabIndex = 5;
            this.SpeedControl.Text = "Speed Control";
            this.SpeedControl.Click += new System.EventHandler(this.SpeedControl_Click);
            // 
            // ZeroedButton
            // 
            this.ZeroedButton.Location = new System.Drawing.Point(97, 251);
            this.ZeroedButton.Name = "ZeroedButton";
            this.ZeroedButton.Size = new System.Drawing.Size(16, 25);
            this.ZeroedButton.TabIndex = 6;
            this.ZeroedButton.Text = "Zeroed";
            this.ZeroedButton.UseVisualStyleBackColor = true;
            this.ZeroedButton.Click += new System.EventHandler(this.ZeroedButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(382, 452);
            this.Controls.Add(this.ZeroedButton);
            this.Controls.Add(this.SpeedControl);
            this.Controls.Add(this.ControllerModeLabel);
            this.Controls.Add(this.ControllerModeButton);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.CCWOneStepButton);
            this.Controls.Add(this.CWOneStepButton);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button CWOneStepButton;
        private System.Windows.Forms.Button CCWOneStepButton;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Button ControllerModeButton;
        private System.Windows.Forms.Label ControllerModeLabel;
        private System.Windows.Forms.Label SpeedControl;
        private System.Windows.Forms.Button ZeroedButton;
    }
}

