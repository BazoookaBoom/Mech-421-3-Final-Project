namespace PositionSender
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
            this.comboPort = new System.Windows.Forms.ComboBox();
            this.comPortConnectButton = new System.Windows.Forms.Button();
            this.positionSendButton = new System.Windows.Forms.Button();
            this.positionTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // comboPort
            // 
            this.comboPort.FormattingEnabled = true;
            this.comboPort.Location = new System.Drawing.Point(26, 33);
            this.comboPort.Name = "comboPort";
            this.comboPort.Size = new System.Drawing.Size(121, 21);
            this.comboPort.TabIndex = 0;
            // 
            // comPortConnectButton
            // 
            this.comPortConnectButton.Location = new System.Drawing.Point(167, 33);
            this.comPortConnectButton.Name = "comPortConnectButton";
            this.comPortConnectButton.Size = new System.Drawing.Size(62, 20);
            this.comPortConnectButton.TabIndex = 1;
            this.comPortConnectButton.Text = "Connect Com";
            this.comPortConnectButton.UseVisualStyleBackColor = true;
            this.comPortConnectButton.Click += new System.EventHandler(this.comPortConnectButton_Click);
            // 
            // positionSendButton
            // 
            this.positionSendButton.Location = new System.Drawing.Point(167, 92);
            this.positionSendButton.Name = "positionSendButton";
            this.positionSendButton.Size = new System.Drawing.Size(62, 34);
            this.positionSendButton.TabIndex = 2;
            this.positionSendButton.Text = "Send Position";
            this.positionSendButton.UseVisualStyleBackColor = true;
            this.positionSendButton.Click += new System.EventHandler(this.positionSendButton_Click);
            // 
            // positionTextBox
            // 
            this.positionTextBox.Location = new System.Drawing.Point(42, 100);
            this.positionTextBox.Name = "positionTextBox";
            this.positionTextBox.Size = new System.Drawing.Size(91, 20);
            this.positionTextBox.TabIndex = 3;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.positionTextBox);
            this.Controls.Add(this.positionSendButton);
            this.Controls.Add(this.comPortConnectButton);
            this.Controls.Add(this.comboPort);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.ComboBox comboPort;
        private System.Windows.Forms.Button comPortConnectButton;
        private System.Windows.Forms.Button positionSendButton;
        private System.Windows.Forms.TextBox positionTextBox;
    }
}

