namespace MECH423Lab6Ex4
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.comboBoxCOMPorts = new System.Windows.Forms.ComboBox();
            this.ConnectButton = new System.Windows.Forms.Button();
            this.PWMSetTextBox = new System.Windows.Forms.TextBox();
            this.SetPWMButton = new System.Windows.Forms.Button();
            this.PositionTextBox = new System.Windows.Forms.TextBox();
            this.VelocityTextBox = new System.Windows.Forms.TextBox();
            this.HzTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();

            // 
            // serialPort1
            // 
            this.serialPort1.DataReceived +=
                new System.IO.Ports.SerialDataReceivedEventHandler(this.SerialPort1_DataReceived);

            // 
            // comboBoxCOMPorts
            // 
            this.comboBoxCOMPorts.FormattingEnabled = true;
            this.comboBoxCOMPorts.Items.AddRange(new object[] {
            "COM3",
            "COM7"});
            this.comboBoxCOMPorts.Location = new System.Drawing.Point(12, 12);
            this.comboBoxCOMPorts.Name = "comboBoxCOMPorts";
            this.comboBoxCOMPorts.Size = new System.Drawing.Size(121, 21);

            // 
            // ConnectButton
            // 
            this.ConnectButton.Location = new System.Drawing.Point(150, 12);
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new System.Drawing.Size(75, 23);
            this.ConnectButton.Text = "Connect";
            this.ConnectButton.UseVisualStyleBackColor = true;
            this.ConnectButton.Click +=
                new System.EventHandler(this.ConnectButton_Click);

            // 
            // PWMSetTextBox
            // 
            this.PWMSetTextBox.Location = new System.Drawing.Point(12, 55);
            this.PWMSetTextBox.Name = "PWMSetTextBox";
            this.PWMSetTextBox.Size = new System.Drawing.Size(75, 20);
            this.PWMSetTextBox.Text = "0";

            // 
            // SetPWMButton
            // 
            this.SetPWMButton.Location = new System.Drawing.Point(100, 53);
            this.SetPWMButton.Name = "SetPWMButton";
            this.SetPWMButton.Size = new System.Drawing.Size(75, 23);
            this.SetPWMButton.Text = "Set PWM";
            this.SetPWMButton.UseVisualStyleBackColor = true;
            this.SetPWMButton.Click +=
                new System.EventHandler(this.SetPWMButton_Click);

            // 
            // PositionTextBox
            // 
            this.PositionTextBox.Location = new System.Drawing.Point(12, 100);
            this.PositionTextBox.Name = "PositionTextBox";
            this.PositionTextBox.Size = new System.Drawing.Size(75, 20);
            this.PositionTextBox.Text = "0";

            // 
            // VelocityTextBox
            // 
            this.VelocityTextBox.Location = new System.Drawing.Point(100, 100);
            this.VelocityTextBox.Name = "VelocityTextBox";
            this.VelocityTextBox.Size = new System.Drawing.Size(75, 20);
            this.VelocityTextBox.Text = "0";

            // 
            // HzTextBox
            // 
            this.HzTextBox.Location = new System.Drawing.Point(188, 100);
            this.HzTextBox.Name = "HzTextBox";
            this.HzTextBox.Size = new System.Drawing.Size(75, 20);
            this.HzTextBox.Text = "0";

            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(280, 150);
            this.Controls.Add(this.comboBoxCOMPorts);
            this.Controls.Add(this.ConnectButton);
            this.Controls.Add(this.PWMSetTextBox);
            this.Controls.Add(this.SetPWMButton);
            this.Controls.Add(this.PositionTextBox);
            this.Controls.Add(this.VelocityTextBox);
            this.Controls.Add(this.HzTextBox);
            this.Name = "Form1";
            this.Text = "Lab6Ex3 - Logger";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.ComboBox comboBoxCOMPorts;
        private System.Windows.Forms.Button ConnectButton;
        private System.Windows.Forms.TextBox PWMSetTextBox;
        private System.Windows.Forms.Button SetPWMButton;
        private System.Windows.Forms.TextBox PositionTextBox;
        private System.Windows.Forms.TextBox VelocityTextBox;
        private System.Windows.Forms.TextBox HzTextBox;
    }
}
