namespace Mech421Lab1Ex4
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
            this.SerialPortComboBox = new System.Windows.Forms.ComboBox();
            this.ConnectDisconnectSerialButton = new System.Windows.Forms.Button();
            this.SerialBytesReadTextBox = new System.Windows.Forms.TextBox();
            this.TempStrLengthTextBox = new System.Windows.Forms.TextBox();
            this.ItemsInQueueTextBox = new System.Windows.Forms.TextBox();
            this.SerialDataStreamTextBox = new System.Windows.Forms.TextBox();
            this.SerialBytesReadLabel = new System.Windows.Forms.Label();
            this.TempStrLengthLabel = new System.Windows.Forms.Label();
            this.ItemsInQueueLabel = new System.Windows.Forms.Label();
            this.SerialDataStreamLabel = new System.Windows.Forms.Label();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // SerialPortComboBox
            // 
            this.SerialPortComboBox.FormattingEnabled = true;
            this.SerialPortComboBox.Items.AddRange(new object[] {
            "COM3"});
            this.SerialPortComboBox.Location = new System.Drawing.Point(12, 21);
            this.SerialPortComboBox.Name = "SerialPortComboBox";
            this.SerialPortComboBox.Size = new System.Drawing.Size(127, 21);
            this.SerialPortComboBox.TabIndex = 0;
            this.SerialPortComboBox.Text = "Select Serial Port";
            // 
            // ConnectDisconnectSerialButton
            // 
            this.ConnectDisconnectSerialButton.Location = new System.Drawing.Point(145, 21);
            this.ConnectDisconnectSerialButton.Name = "ConnectDisconnectSerialButton";
            this.ConnectDisconnectSerialButton.Size = new System.Drawing.Size(139, 21);
            this.ConnectDisconnectSerialButton.TabIndex = 1;
            this.ConnectDisconnectSerialButton.Text = "Connect";
            this.ConnectDisconnectSerialButton.UseVisualStyleBackColor = true;
            this.ConnectDisconnectSerialButton.Click += new System.EventHandler(this.ConnectDisconnectSerialButton_Click);
            // 
            // SerialBytesReadTextBox
            // 
            this.SerialBytesReadTextBox.Location = new System.Drawing.Point(155, 59);
            this.SerialBytesReadTextBox.Name = "SerialBytesReadTextBox";
            this.SerialBytesReadTextBox.Size = new System.Drawing.Size(129, 20);
            this.SerialBytesReadTextBox.TabIndex = 2;
            this.SerialBytesReadTextBox.TextChanged += new System.EventHandler(this.SerialBytesReadTextBox_TextChanged);
            // 
            // TempStrLengthTextBox
            // 
            this.TempStrLengthTextBox.Location = new System.Drawing.Point(156, 85);
            this.TempStrLengthTextBox.Name = "TempStrLengthTextBox";
            this.TempStrLengthTextBox.Size = new System.Drawing.Size(129, 20);
            this.TempStrLengthTextBox.TabIndex = 3;
            this.TempStrLengthTextBox.TextChanged += new System.EventHandler(this.TempStrLengthTextBox_TextChanged);
            // 
            // ItemsInQueueTextBox
            // 
            this.ItemsInQueueTextBox.Location = new System.Drawing.Point(156, 111);
            this.ItemsInQueueTextBox.Name = "ItemsInQueueTextBox";
            this.ItemsInQueueTextBox.Size = new System.Drawing.Size(129, 20);
            this.ItemsInQueueTextBox.TabIndex = 4;
            this.ItemsInQueueTextBox.TextChanged += new System.EventHandler(this.ItemsInQueueTextBox_TextChanged);
            // 
            // SerialDataStreamTextBox
            // 
            this.SerialDataStreamTextBox.Location = new System.Drawing.Point(12, 182);
            this.SerialDataStreamTextBox.Multiline = true;
            this.SerialDataStreamTextBox.Name = "SerialDataStreamTextBox";
            this.SerialDataStreamTextBox.Size = new System.Drawing.Size(281, 247);
            this.SerialDataStreamTextBox.TabIndex = 5;
            // 
            // SerialBytesReadLabel
            // 
            this.SerialBytesReadLabel.AutoSize = true;
            this.SerialBytesReadLabel.Location = new System.Drawing.Point(24, 59);
            this.SerialBytesReadLabel.Name = "SerialBytesReadLabel";
            this.SerialBytesReadLabel.Size = new System.Drawing.Size(103, 13);
            this.SerialBytesReadLabel.TabIndex = 6;
            this.SerialBytesReadLabel.Text = "Serial Bytes to Read";
            // 
            // TempStrLengthLabel
            // 
            this.TempStrLengthLabel.AutoSize = true;
            this.TempStrLengthLabel.Location = new System.Drawing.Point(24, 85);
            this.TempStrLengthLabel.Name = "TempStrLengthLabel";
            this.TempStrLengthLabel.Size = new System.Drawing.Size(100, 13);
            this.TempStrLengthLabel.TabIndex = 7;
            this.TempStrLengthLabel.Text = "Temp String Length";
            // 
            // ItemsInQueueLabel
            // 
            this.ItemsInQueueLabel.AutoSize = true;
            this.ItemsInQueueLabel.Location = new System.Drawing.Point(24, 111);
            this.ItemsInQueueLabel.Name = "ItemsInQueueLabel";
            this.ItemsInQueueLabel.Size = new System.Drawing.Size(79, 13);
            this.ItemsInQueueLabel.TabIndex = 8;
            this.ItemsInQueueLabel.Text = "Items In Queue";
            // 
            // SerialDataStreamLabel
            // 
            this.SerialDataStreamLabel.AutoSize = true;
            this.SerialDataStreamLabel.Location = new System.Drawing.Point(24, 166);
            this.SerialDataStreamLabel.Name = "SerialDataStreamLabel";
            this.SerialDataStreamLabel.Size = new System.Drawing.Size(95, 13);
            this.SerialDataStreamLabel.TabIndex = 9;
            this.SerialDataStreamLabel.Text = "Serial Data Stream";
            // 
            // serialPort1
            // 
            this.serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(296, 450);
            this.Controls.Add(this.SerialDataStreamLabel);
            this.Controls.Add(this.ItemsInQueueLabel);
            this.Controls.Add(this.TempStrLengthLabel);
            this.Controls.Add(this.SerialBytesReadLabel);
            this.Controls.Add(this.SerialDataStreamTextBox);
            this.Controls.Add(this.ItemsInQueueTextBox);
            this.Controls.Add(this.TempStrLengthTextBox);
            this.Controls.Add(this.SerialBytesReadTextBox);
            this.Controls.Add(this.ConnectDisconnectSerialButton);
            this.Controls.Add(this.SerialPortComboBox);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox SerialPortComboBox;
        private System.Windows.Forms.Button ConnectDisconnectSerialButton;
        private System.Windows.Forms.TextBox SerialBytesReadTextBox;
        private System.Windows.Forms.TextBox TempStrLengthTextBox;
        private System.Windows.Forms.TextBox ItemsInQueueTextBox;
        private System.Windows.Forms.TextBox SerialDataStreamTextBox;
        private System.Windows.Forms.Label SerialBytesReadLabel;
        private System.Windows.Forms.Label TempStrLengthLabel;
        private System.Windows.Forms.Label ItemsInQueueLabel;
        private System.Windows.Forms.Label SerialDataStreamLabel;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.Timer timer1;
    }
}

