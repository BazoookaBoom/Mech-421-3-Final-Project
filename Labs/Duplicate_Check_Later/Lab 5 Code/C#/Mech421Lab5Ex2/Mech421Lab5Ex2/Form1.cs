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

        // ===== GLOBAL VARS =====
        // --- Auto-Reconnect Vars ---
        Timer autoReconnectTimer = new Timer();
        bool userWantsConnection = false;

        // --- Motor Control Vars ---
        int speed = 0;
        byte forwardStep = 1; // command for forward one step
        byte backwardStep = 2; // command for backward one step
        byte speedMode = 3; // command for speed mode
        byte stopMotor = 4; // command to stop motor
        private bool inContinuousMode = false;

        public Form1()
        {
            InitializeComponent();

            // Auto-reconnect setup
            autoReconnectTimer.Interval = 1000;
            autoReconnectTimer.Tick += AutoReconnectTimer_Tick;
            autoReconnectTimer.Start();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            RefreshCOMPorts();
        }

        // ===== MOTOR CONTROL METHODS =====
        
        // --- CWOneStepButton_Click ---
        // On call, makes and sends packet to MCU with instructions to step once CW
        private void CWOneStepButton_Click(object sender, EventArgs e)
        {
            if (!inContinuousMode)
                SendPacket(forwardStep, 127);   // command: forward one step
        }

        private void CCWOneStepButton_Click(object sender, EventArgs e)
        {
            if (!inContinuousMode)
                SendPacket(backwardStep, 127);   // command: forward one step
        }

        // --- ControllerModeButton_Click ---
        // Toggles the control mode (continuous vs single-step)
        private void ControllerModeButton_Click(object sender, EventArgs e)
        {
            inContinuousMode = !inContinuousMode; // On function call, switch modes

            if (inContinuousMode)
            {
                ControllerModeButton.Text = "Continuous"; // Update UI element to indicate continuous control
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
        
        private void ZeroedButton_Click(object sender, EventArgs e)
        {
            speed = 127;
            trackBar1.Value = speed;

            if (inContinuousMode)
                SendPacket(3, 127);

            SpeedLabel.Text = "0";
        }
        private void TrackBar1_Scroll(object sender, EventArgs e)
        {
            speed = trackBar1.Value;

            if (inContinuousMode)
                SendPacket(3, (byte)speed);  // command = speed mode

            SpeedLabel.Text = ((speed - 127)/1.27).ToString();
        }

        // ===== SERIAL COMMUNICATION METHODS =====

        // --- SendPacket ---
        // Sends a packet over UART with the form [255], [cmd], [speed]
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

        // ===== SERIAL CONNECTION METHODS =====

        // --- ConnectButton_Click ---
        // Opens selected COM port for serial communication if closed
        // Closes COM port if currently open
        private void ConnectButton_Click(object sender, EventArgs e)
        {
            // Close serial port and reset button text to reflect new state
            if (serialPort1.IsOpen)
            {
                userWantsConnection = false;
                serialPort1.Close();
                ConnectButton.Text = "Connect";
                return;
            }

            // If connection attempted, but no COM ports are available to connect to, show error message
            if (comboBoxCOMPorts.Text == "No COM ports!")
            {
                MessageBox.Show("No COM ports detected!");
                return;
            }

            // If program has made it to this point, then:
            // - serial port is currently closed
            // - AND there must be a valid COM port to connect to

            // Take dropdown-selected COM port as the connection target
            serialPort1.PortName = comboBoxCOMPorts.Text;

            // Attempt to connect to drop-down selected port. If connection failed, attempt auto-reconnects
            try
            {
                serialPort1.Open();
                ConnectButton.Text = "Disconnect";
                userWantsConnection = true;
            }
            catch
            {
                MessageBox.Show("Failed to open port. Will auto-retry.");
                userWantsConnection = true;
            }
        }

        // --- RefreshCOMPorts ---
        // Populates combobox with all available COM ports
        private void RefreshCOMPorts()
        {
            var ports = System.IO.Ports.SerialPort.GetPortNames();
            var selected = comboBoxCOMPorts.Text;

            comboBoxCOMPorts.Items.Clear();
            comboBoxCOMPorts.Items.AddRange(ports);

            if (ports.Contains(selected))
                comboBoxCOMPorts.Text = selected;
            else if (ports.Length > 0)
                comboBoxCOMPorts.SelectedIndex = 0;
            else
                comboBoxCOMPorts.Text = "No COM ports!";
        }

        // --- AutoReconnectTimer_Tick ---
        // Attempts to reconnect serial port once
        private void AutoReconnectTimer_Tick(object sender, EventArgs e)
        {
            if (!userWantsConnection) return;
            if (serialPort1.IsOpen) return;

            RefreshCOMPorts();

            try
            {
                serialPort1.PortName = comboBoxCOMPorts.Text;
                serialPort1.Open();
                ConnectButton.Text = "Disconnect";
            }
            catch { }
        }

    }
}
