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
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // CWOneStepButton
            // 
            this.CWOneStepButton.Location = new System.Drawing.Point(30, 25);
            this.CWOneStepButton.Name = "CWOneStepButton";
            this.CWOneStepButton.Size = new System.Drawing.Size(108, 27);
            this.CWOneStepButton.TabIndex = 0;
            this.CWOneStepButton.Text = "CWOneStepButton";
            this.CWOneStepButton.UseVisualStyleBackColor = true;
            this.CWOneStepButton.Click += new System.EventHandler(this.CWOneStepButton_Click);
            // 
            // CCWOneStepButton
            // 
            this.CCWOneStepButton.Location = new System.Drawing.Point(161, 25);
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
            this.trackBar1.Location = new System.Drawing.Point(30, 129);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(132, 45);
            this.trackBar1.TabIndex = 2;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(382, 452);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.CCWOneStepButton);
            this.Controls.Add(this.CWOneStepButton);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button CWOneStepButton;
        private System.Windows.Forms.Button CCWOneStepButton;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.TrackBar trackBar1;
    }
}

