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
            if (UpSig){
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
        // Sends a packet over UART with the form [255], [cmd], [speed]
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

            byte startByte = 255;

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
                serialPort1.Open();
                ConnectButton.Text = "Disconnect";
            }
            catch { }
        }

       
    }
}
