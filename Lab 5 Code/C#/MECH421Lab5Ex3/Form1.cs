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

namespace MECH421Lab5Ex3
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
