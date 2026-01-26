using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Lab6Ex3
{
    public partial class Form1 : Form
    {
        // Using the serialPort1 from designer
        Timer sendSpeedTimer = new Timer();
        int lastSpeedToSend = -1;
        int currentSpeedCommand = 32768; // center default

        // Encoder variables
        double countsPerRev = 979.62;
        double count2VelocityFactor = 1 / 979.62 / 0.05 * 60.0; // RPM
        double count2PositionFactor = 1.0 / 979.62 * 20.0 / 5.0 * 4; // counts -> cm
        double position = 0;
        double velocity = 0;
        int SpeedCenter = 32768;

        List<byte> rxBuffer = new List<byte>();
        StreamWriter logWriter = null;
        Stopwatch logTimer = new Stopwatch();

        public Form1()
        {
            InitializeComponent();

            sendSpeedTimer.Interval = 10; // 10 ms for fast sending
            sendSpeedTimer.Tick += SendSpeedTimer_Tick;
            sendSpeedTimer.Start();

            serialPort1.DataReceived += SerialPort1_DataReceived;
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
                ConnectButton.Text = "Connect";
                StopLogging();
                return;
            }

            serialPort1.BaudRate = 9600;
            serialPort1.DataBits = 8;
            serialPort1.Parity = Parity.None;
            serialPort1.StopBits = StopBits.One;

            serialPort1.PortName = comboBoxCOMPorts.Text;

            try
            {
                serialPort1.Open();
                ConnectButton.Text = "Disconnect";
                StartLogging("log.csv");
            }
            catch
            {
                MessageBox.Show("Failed to open COM port");
            }
        }

        // Called when user presses the "Set PWM" button
        private void SetPWMButton_Click(object sender, EventArgs e)
        {
            currentSpeedCommand = SpeedCenter * (100 + Convert.ToInt32(PWMSetTextBox.Text)) / 100;

            lastSpeedToSend = currentSpeedCommand;

            SpeedLabel.Text = PWMSetTextBox.Text;
        }

        private void SendSpeedTimer_Tick(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen && lastSpeedToSend != -1)
            {
                byte[] packet = new byte[3];
                packet[0] = 0xAA;
                packet[1] = (byte)(lastSpeedToSend >> 8);
                packet[2] = (byte)(lastSpeedToSend & 0xFF);

                serialPort1.Write(packet, 0, 3);
                lastSpeedToSend = -1;
            }
        }

        private void SerialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int bytesToRead = serialPort1.BytesToRead;
            if (bytesToRead == 0) return;

            byte[] tempBuffer = new byte[bytesToRead];
            serialPort1.Read(tempBuffer, 0, bytesToRead);

            lock (rxBuffer)
            {
                rxBuffer.AddRange(tempBuffer);

                while (rxBuffer.Count >= 4)
                {
                    int startIndex = rxBuffer.IndexOf(0xAA);
                    if (startIndex == -1)
                    {
                        rxBuffer.Clear();
                        break;
                    }
                    if (startIndex > 0)
                        rxBuffer.RemoveRange(0, startIndex);

                    if (rxBuffer.Count < 4) break;

                    byte dir = rxBuffer[1];
                    ushort counts = (ushort)((rxBuffer[2] << 8) | rxBuffer[3]);
                    rxBuffer.RemoveRange(0, 4);

                    int signedCounts = (dir == 0x01) ? counts : -counts;

                    position += signedCounts * count2PositionFactor;
                    velocity = signedCounts * count2VelocityFactor;

                    this.BeginInvoke(new Action(() =>
                    {
                        PositionTextBox.Text = position.ToString("F4");
                        VelocityTextBox.Text = velocity.ToString("F4");
                        HzTextBox.Text = (velocity / 60.0).ToString("F4");
                    }));

                    if (logWriter != null)
                    {
                        int timeMs = (int)logTimer.ElapsedMilliseconds;
                        logWriter.WriteLine($"{timeMs},{position:F4},{velocity:F4}");
                    }
                }
            }
        }

        private void StartLogging(string filename)
        {
            logWriter = new StreamWriter(filename);
            logWriter.WriteLine("Time_ms,Position,Velocity");
            logTimer.Restart();
        }

        private void StopLogging()
        {
            logTimer.Stop();
            if (logWriter != null)
            {
                logWriter.Flush();
                logWriter.Close();
                logWriter = null;
            }
        }
    }
}
