using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows.Forms.DataVisualization.Charting;

namespace Lab6Ex2
{
    public partial class Form1 : Form
    {
        // ===== GLOBAL VARS =====
        // --- Auto-Reconnect Vars ---
        Timer autoReconnectTimer = new Timer();
        bool userWantsConnection = false;
        int SpeedCenter = 32768;
        int SpeedMax = 65536;
        int countsPerRev = 50;
        int timeBtwRefresh = 5; //ms
        List<byte> rxBuffer = new List<byte>();


        // --- Motor Control Vars ---
        int speed = 0;

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
            //RefreshCOMPorts();
        }
        private void ZeroedButton_Click(object sender, EventArgs e)
        {
            speed = SpeedCenter;
            trackBar1.Value = speed;

            SendPacket((byte)speed);

            SpeedLabel.Text = "0";
            Console.WriteLine("Speed set to: " + SpeedLabel.Text + " %");
        }

        private void TrackBar1_Scroll(object sender, EventArgs e)
        {
            speed = trackBar1.Value;
            if (speed <= 0)
            {
                speed = 1;
            }
            SendPacket(speed);
            SpeedLabel.Text = (((double)(speed - SpeedCenter) / SpeedCenter) * 100).ToString("F1");
            Console.WriteLine("Speed set to: " + SpeedLabel.Text + " %");
        }

        // ===== Encoder signal output processing =====
        private void ProcessEncoderSignal(bool UpSig, bool DownSig)
        {
            //TD
            //TA0ClK DWN is forward
            //TA1CLK UP is backward
            if (UpSig)
            {
                ForwardLabel.BackColor = System.Drawing.Color.Lime;
                BackwardLabel.BackColor = System.Drawing.SystemColors.ControlDark;
            }
            else if (DownSig)
            {
                ForwardLabel.BackColor = System.Drawing.SystemColors.ControlDark;
                BackwardLabel.BackColor = System.Drawing.Color.Lime;
            }

        }

        // ===== SERIAL COMMUNICATION METHODS =====

        // --- SendPacket ---
        // Sends a packet over UART with the form [255], [speedMSB], [speedLSB]
        private void SendPacket(int speedTotal)
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

            byte startByte = 0xAA;

            byte[] packet = new byte[3];
            packet[0] = startByte;
            packet[1] = (byte)(speedTotal >> 8);
            packet[2] = (byte)(speedTotal & 0xFF);

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
                BaudRateSetup();
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

        // --- RefreshCOMPorts --- //
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

        // --- SerialPort Baud Rate Setup ---
        private void BaudRateSetup()
        {
            serialPort1.BaudRate = 115200;
            serialPort1.DataBits = 8;
            serialPort1.Parity = Parity.None;
            serialPort1.StopBits = StopBits.One;
        }

        // --- AutoReconnectTimer_Tick ---
        // Attempts to reconnect serial port once
        private void AutoReconnectTimer_Tick(object sender, EventArgs e)
        {
            if (!userWantsConnection) return;
            if (serialPort1.IsOpen) return;

            //RefreshCOMPorts();

            try
            {
                serialPort1.PortName = comboBoxCOMPorts.Text;
                BaudRateSetup();
                serialPort1.Open();
                ConnectButton.Text = "Disconnect";
            }
            catch { }
        }



        // --- Chart2 Updating stuff ---
        private void AddDataPointToChart2(double position, double velocity)
        {
            // Add new data point
            chart2.Series[0].Points.AddY(position);
            chart2.Series[1].Points.AddY(velocity);
            // Remove old data points to maintain a fixed number of points
            int maxPoints = 100; // Set the maximum number of points to display
            while (chart2.Series[0].Points.Count > maxPoints)
            {
                chart2.Series[0].Points.RemoveAt(0);
                chart2.Series[1].Points.RemoveAt(0);
            }
            // Adjust X axis scale
            chart2.ChartAreas[0].RecalculateAxesScale();
        }

        // --- SerialPort1_DataReceived ---
        // --- Byte Package [255] , [Encoder Dir 0, 1 -> up, down] , [Encoder Counts]---
        private void SerialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int n = serialPort1.BytesToRead;
            byte[] temp = new byte[n];
            serialPort1.Read(temp, 0, n);

            rxBuffer.AddRange(temp);

            while (rxBuffer.Count >= 3)
            {
                // Search for start byte
                if (rxBuffer[0] != 255)
                {
                    rxBuffer.RemoveAt(0);
                    continue;
                }

                // Wait for full packet
                if (rxBuffer.Count < 3)
                    return;

                byte status = rxBuffer[1];
                byte counts = rxBuffer[2];

                rxBuffer.RemoveRange(0, 3);

                bool upSignal = (status & 0x01) != 0;
                bool downSignal = (status & 0x02) != 0;

                this.BeginInvoke(new Action(() =>
                {
                    ProcessEncoderSignal(upSignal, downSignal);
                    double position = counts;
                    double velocity = counts / (timeBtwRefresh / 1000.0);
                    AddDataPointToChart2(position, velocity);
                }));
            }
        }

    }
}
