using System;
using System.IO.Ports;
using System.Windows.Forms;

namespace PositionSender
{
    public partial class Form1 : Form
    {
        SerialPort serialPort;

        public Form1()
        {
            InitializeComponent();

            comboPort.Items.AddRange(SerialPort.GetPortNames());
            if (comboPort.Items.Count > 0)
                comboPort.SelectedIndex = 0;

            positionTextBox.Text = "0";
        }

        private void comPortConnectButton_Click(object sender, EventArgs e)
        {
            if (serialPort == null || !serialPort.IsOpen)
            {
                try
                {
                    serialPort = new SerialPort(
                        comboPort.SelectedItem.ToString(),
                        9600,
                        Parity.None,
                        8,
                        StopBits.One);

                    serialPort.Open();
                    comPortConnectButton.Text = "Disconnect Com";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Serial error: " + ex.Message);
                    return;
                }
            }
            else
            {
                serialPort.Close();
                comPortConnectButton.Text = "Connect Com";
            }
        }

        private void positionSendButton_Click(object sender, EventArgs e)
        {
            // Read user input as double (allows decimals like 1.2 cm)
            double positionCm;

            if (!double.TryParse(positionTextBox.Text, out positionCm))
            {
                MessageBox.Show("Invalid position value");
                return;
            }

            // Clamp physical limits
            if (positionCm < 0.0) positionCm = 0.0;
            if (positionCm > 25.5) positionCm = 25.5;

            // Scale: 1.0 cm -> 10 counts
            int scaledValue = (int)Math.Round(positionCm * 10.0);

            // Clamp to byte range (safety)
            if (scaledValue < 0) scaledValue = 0;
            if (scaledValue > 255) scaledValue = 255;

            byte positionByte = (byte)scaledValue;

            byte[] packet = new byte[2];
            packet[0] = 0xAA;
            packet[1] = positionByte;

            serialPort.Write(packet, 0, packet.Length);
        }

    }
}
