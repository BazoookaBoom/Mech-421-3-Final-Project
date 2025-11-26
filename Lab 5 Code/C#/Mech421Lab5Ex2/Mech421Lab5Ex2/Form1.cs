using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mech421Lab5Ex2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        int speed = 0;
        byte forwardStep = 1; // command for forward one step
        byte backwardStep = 2; // command for backward one step
        byte speedMode = 3; // command for speed mode
        byte stopMotor = 4; // command to stop motor

        private bool inContinuousMode = false;
        private void CWOneStepButton_Click(object sender, EventArgs e)
        {
            if (!inContinuousMode)
                SendPacket(forwardStep, 127);   // command: forward one step
        }

        private void CCWOneStepButton_Click(object sender, EventArgs e)
        {
            if (!inContinuousMode)
                SendPacket(backwardStep, 127);   // command: backward one step
        }
        private void ControllerModeButton_Click(object sender, EventArgs e)
        {
            inContinuousMode = !inContinuousMode;

            if (inContinuousMode)
            {
                ControllerModeButton.Text = "Continuous";
                speed = 127;
                SendPacket(speedMode, ((byte)speed)); // Enter speed mode, 0 speed at start
                trackBar1.Value = speed;
            }
            else
            {
                ControllerModeButton.Text = "Step";
                SendPacket(stopMotor, 127); // Stop motor on exit
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            RefreshCOMButton_Click(null, null);
        }
        private void ZeroedButton_Click(object sender, EventArgs e)
        {
            speed = 127;
            trackBar1.Value = speed;

            if (inContinuousMode)
                SendPacket(3, 127);
        }
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            speed = trackBar1.Value;

            if (inContinuousMode)
                SendPacket(3, (byte)speed);  // command = speed mode
        }

        private void SendPacket(byte command, byte speedByte)
        {
            if (!serialPort1.IsOpen)
            {
                //try
                //{
                //    serialPort1.PortName = comboBoxCOM.SelectedItem.ToString();
                //    serialPort1.Open();
                //}
                //catch
                //{
                //    MessageBox.Show("Unable to open COM port!");
                    return;
                //}
            }

            byte startByte = 255;

            byte[] packet = new byte[3];
            packet[0] = startByte;
            packet[1] = command;
            packet[2] = speedByte;

            serialPort1.Write(packet, 0, 3);
        }
        private void RefreshCOMButton_Click(object sender, EventArgs e)
        {
            comboBoxCOM.Items.Clear();
            comboBoxCOM.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());

            if (comboBoxCOM.Items.Count > 0)
                comboBoxCOM.SelectedIndex = 0;
        }

        
    }
}
