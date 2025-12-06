using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


// Packet structure: [255], [posX in mm], [posY in mm], [speed]

namespace MECH421Lab5Ex3
{
    public partial class Form1 : Form
    {
        // ===== GLOBAL VARS =====
        // --- Auto-Reconnect Vars ---
        Timer autoReconnectTimer = new Timer();
        bool userWantsConnection = false;
        Queue<byte> packetBytes = new Queue<byte>();

        // --- Motor Control Vars ---
        sbyte speed = 0; // 0 = 0% speed
        byte deltaX = 0; // relative X target
        byte deltaY = 0; // relative Y target


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

        // --- ConnectButton_Click ---
        // Triggers serial connection sequence
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

        private void AddSeqButton_Click(object sender, EventArgs e)
        {
            packetBytes.Enqueue(255);
            packetBytes.Enqueue(Convert.ToByte(xCoord.Text));
            packetBytes.Enqueue(Convert.ToByte(yCoord.Text));
            packetBytes.Enqueue(Convert.ToByte(speedPercentage.Text));

            string newPacketDisp = "(" + Convert.ToString(xCoord.Text) + ", " + Convert.ToString(yCoord.Text) + ", " + Convert.ToString(speedPercentage.Text) + ") ";

            sequenceTextInput.Text += newPacketDisp;
        }

        private void ClrSeqButton_Click(object sender, EventArgs e)
        {
            packetBytes.Clear();

            sequenceTextInput.Text = "";
        }

        private void executeSeqButton_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                for (int i = 0; i < packetBytes.Count; i++)
                {
                    byte txByte = packetBytes.Dequeue();
                    serialPort1.Write(txByte.ToString());
                }
            }
            else
            {
                MessageBox.Show("Serial port not open!");
            }
        }
    }
}
