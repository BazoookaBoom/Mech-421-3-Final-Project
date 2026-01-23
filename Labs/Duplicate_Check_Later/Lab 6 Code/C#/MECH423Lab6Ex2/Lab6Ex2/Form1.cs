using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Diagnostics;

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
        double countsPerRev = 979.62;
        int timeBtwRefresh = 50; //ms
        double count2VelocityFactor = 1/ 979.62 / (50.0/1000.0) * 60.0; // 1 / 979.62 counts per revolution = revolutions completed / 50ms = velocity in Rev per ms * 1000 * 60 to get RPM 
        double Hz = 0;
        double position = 0;
        double velocity = 0;
        double previousPosition = 0;
        // 5 teeth per cm on belt, 20 teeth per revolution
        double count2PositionFactor = 1.0 / 979.62 * 20.0 / 5.0 * 4; // counts to revolutions 

        List<byte> rxBuffer = new List<byte>();

        // ===== DATA LOGGING VARS =====
        StreamWriter logWriter = null;
        int currentSpeedCommand = 32768;
        bool isLogging = false;
        string filename = "C:\\Users\\Centr\\Documents\\GitHub\\Mech-421-3-Final-Project\\Labs\\Duplicate_Check_Later\\Lab 6 Code\\C#\\MECH423Lab6Ex3" +
            "\\step50.csv";

        Stopwatch logTimer = new Stopwatch();


        // --- Speed update Timer Vars ---
        Timer sendSpeedTimer = new Timer();
        int lastSpeedToSend = -1;

        // --- Charting Vars ---
        int chartIndex = 1;

        // --- Motor Control Vars ---
        int speed = 0;

        public Form1()
        {
            InitializeComponent();
            InitializeChart2();
            // Auto-reconnect setup
            autoReconnectTimer.Interval = 1000;
            autoReconnectTimer.Tick += AutoReconnectTimer_Tick;
            autoReconnectTimer.Start();

            // Setup speed sending timer (every 100ms, same as encoder)
            sendSpeedTimer.Interval = 100;
            sendSpeedTimer.Tick += SendSpeedTimer_Tick;
            sendSpeedTimer.Start();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeChart2();
        }
        private void ZeroedButton_Click(object sender, EventArgs e)
        {
            currentSpeedCommand = SpeedCenter;
            lastSpeedToSend = SpeedCenter;

            trackBar1.Value = SpeedCenter;
            SpeedLabel.Text = "0";
        }


        private void TrackBar1_Scroll(object sender, EventArgs e)
        {
            speed = trackBar1.Value;
            if (speed <= 0) speed = 1;

            currentSpeedCommand = speed;   // ✅ persistent state
            lastSpeedToSend = speed;       // UART transmit buffer

            SpeedLabel.Text =
                (((double)(speed - SpeedCenter) / SpeedCenter) * 100).ToString("F1");
        }


        private void SetPWMButton_Click(object sender, EventArgs e)
        {
            currentSpeedCommand = SpeedCenter * (100 + Convert.ToInt32(PWMSetTextBox.Text)) / 100;

            lastSpeedToSend = currentSpeedCommand;

            trackBar1.Value = Math.Min(currentSpeedCommand, SpeedMax - 1);
            SpeedLabel.Text = PWMSetTextBox.Text;
        }


        private void SendSpeedTimer_Tick(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen && lastSpeedToSend != -1)
            {
                SendPacket(lastSpeedToSend);
                lastSpeedToSend = -1;
            }
        }

        // ===== Encoder signal output processing =====
        private void ProcessEncoderSignal(int direction)
        {
            //TA0ClK DWN is forward
            //TA1CLK UP is backward
            switch (direction)
            {
                case 1:
                    ForwardLabel.BackColor = System.Drawing.Color.Lime;
                    BackwardLabel.BackColor = System.Drawing.SystemColors.ControlDark;
                    break;
                case 2:
                    ForwardLabel.BackColor = System.Drawing.SystemColors.ControlDark;
                    BackwardLabel.BackColor = System.Drawing.Color.Lime;
                    break;
                case 3:
                    ForwardLabel.BackColor = System.Drawing.SystemColors.ControlDark;
                    BackwardLabel.BackColor = System.Drawing.SystemColors.ControlDark;
                    break;
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
                StopLogging();
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
            StartLogging(filename);
        }
        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!isLogging)
            {
                StartLogging(filename);
                SaveButton.Text = "Writing";
            }
            else
            {
                StopLogging();
                SaveButton.Text = "Save?";
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
            serialPort1.BaudRate = 9600;
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

        // === Position and Velocity Charting ===


        // ===== CHARTING HELPER =====
        private void InitializeChart2()
        {
            chart2.Series.Clear();

            Series posSeries = new Series
            {
                Name = "Position",
                Color = Color.Blue,
                ChartType = SeriesChartType.Line
            };
            Series velSeries = new Series
            {
                Name = "Velocity",
                Color = Color.Red,
                ChartType = SeriesChartType.Line,
                YAxisType = AxisType.Secondary
            };

            chart2.Series.Add(posSeries);


            //chart2.Series.Add(posSeries);
            //chart2.Series.Add(velSeries);

            var area = chart2.ChartAreas[0];
            area.AxisX.Title = "Time (ms)";
            area.AxisY.Title = "Position";
            area.AxisX.Minimum = 0;
            area.AxisX.Maximum = 5000; // 5000ms window

            area.AxisY2.Title = "Velocity (RPM)";
            area.AxisY2.Enabled = AxisEnabled.True;

            chart2.Series.Add(velSeries);
        }


        // --- Chart2 Updating stuff ---
        private void AddDataPointToChart2(double pos, double vel)
        {
            // Add points
            chart2.Series[0].Points.AddXY(chartIndex, pos);
            chart2.Series[1].Points.AddXY(chartIndex, vel);
            chartIndex += timeBtwRefresh;

            // Keep last N points
            int maxPoints = 5000;
            while (chart2.Series[0].Points.Count > maxPoints)
            {
                chart2.Series[0].Points.RemoveAt(0);
                chart2.Series[1].Points.RemoveAt(0);
            }

            // Scroll X-axis
            var area = chart2.ChartAreas[0];
            area.AxisX.Minimum = chartIndex - maxPoints > 0 ? chartIndex - maxPoints : 0;
            area.AxisX.Maximum = chartIndex;

            chart2.Invalidate(); // redraw chart
        }
        private void StartLogging(string filename)
        {
            logWriter = new StreamWriter(filename);
            logWriter.WriteLine("Time_ms,DutyCycle_percent,Position,Speed");

            logTimer.Reset();
            logTimer.Start();

            isLogging = true;
        }


        private void StopLogging()
        {
            if (logWriter != null)
            {
                logTimer.Stop();

                isLogging = false;
                logWriter.Flush();
                logWriter.Close();
                logWriter = null;
            }
        }




        // ===== SERIAL DATA RECEIVED =====
        // Packet format: [0xAA], [Dir 0/1], [Count MSB], [Count LSB]
        
        
        /*private void SerialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int bytesToRead = serialPort1.BytesToRead;
            if (bytesToRead == 0) return;

            byte[] tempBuffer = new byte[bytesToRead];
            serialPort1.Read(tempBuffer, 0, bytesToRead);

            lock (rxBuffer)
            {
                rxBuffer.AddRange(tempBuffer);

                // Process all complete 4-byte packets
                while (rxBuffer.Count >= 4)
                {
                    int startIndex = rxBuffer.IndexOf(0xAA);
                    if (startIndex == -1)
                    {
                        // no start byte, clear garbage
                        rxBuffer.Clear();
                        break;
                    }
                    if (startIndex > 0)
                    {
                        rxBuffer.RemoveRange(0, startIndex);
                    }
                    if (rxBuffer.Count < 4) break;

                    byte direction = rxBuffer[1];
                    ushort counts = (ushort)((rxBuffer[2] << 8) | rxBuffer[3]);

                    rxBuffer.RemoveRange(0, 4);

                    int signedCounts = (direction == 0x01) ? counts : -(int)counts;
                    if(signedCounts == 0)
                    {
                        direction = 3;
                    }

                    Hz = (double)counts / countsPerRev / (timeBtwRefresh / 1000.0); // counts to revolutions per second
                    position += signedCounts * count2PositionFactor;
                    velocity = signedCounts * count2VelocityFactor;


                    // Update UI safely
                    //this.BeginInvoke(new Action(() =>
                    //{
                    //    ProcessEncoderSignal(direction);  // highlight CW/CCW
                    //    AddDataPointToChart2(position, velocity);

                    //    HzTextBox.Text = Hz.ToString("F2");
                    //    PositionTextBox.Text = position.ToString("F2");
                    //    VelocityTextBox.Text = velocity.ToString("F2");


                    //}));

                    // Data logging

                    if (isLogging && logWriter != null)
                    {
                        int timeMs = (int)logTimer.ElapsedMilliseconds;

                        double dutyCyclePercent = ((double)(currentSpeedCommand - SpeedCenter)) * 100.0 / SpeedCenter;


                        logWriter.WriteLine($"{timeMs},{dutyCyclePercent:F2},{position:F4},{velocity:F4}");
                    }
                }
            }
        }*/

        

    }
}
