using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;

namespace Lab6Ex3
{
    public partial class Form1 : Form
    {
        // ================= TIMERS =================
        Timer sendSpeedTimer = new Timer();

        int lastSpeedToSend = -1;

        // ================= ENCODER =================
        double countsPerRev = 979.62;
        double count2VelocityFactor = 1 / 979.62 / 0.05 * 60.0;   // RPM
        double count2PositionFactor = 1.0 / 979.62 * 20.0 / 5.0 * 4; // cm

        double position = 0;
        double velocity = 0;

        int SpeedCenter = 32768;

        // ================= SERIAL =================
        List<byte> rxBuffer = new List<byte>();

        // ================= LOGGING =================
        Stopwatch logTimer = new Stopwatch();
        StreamWriter logWriter = null;
        bool isLogging = false;

        List<string> logBuffer = new List<string>();
        const int LOG_FLUSH_SIZE = 20;   // flush every 20 samples (~500 ms)

        string filename =
            "C:\\Users\\Centr\\Documents\\GitHub\\Mech-421-3-Final-Project\\Labs\\Lab 6 Code\\C#\\MECH423Lab6Ex3\\test.csv";

        int currentSpeedCommand = 32768;

        // ================= CONSTRUCTOR =================
        public Form1()
        {
            InitializeComponent();

            sendSpeedTimer.Interval = 100;
            sendSpeedTimer.Tick += SendSpeedTimer_Tick;
            sendSpeedTimer.Start();

            timerSave.Interval = 25;
            timerSave.Tick += TimerSave_Tick;
            timerSave.Start();

            serialPort1.DataReceived += SerialPort1_DataReceived;
        }

        // ================= UI BUTTONS =================
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
            }
            catch
            {
                MessageBox.Show("Failed to open COM port");
            }
        }

        private void SetPWMButton_Click(object sender, EventArgs e)
        {
            currentSpeedCommand =
                SpeedCenter * (100 + Convert.ToInt32(PWMSetTextBox.Text)) / 100;

            lastSpeedToSend = currentSpeedCommand;
            SpeedLabel.Text = PWMSetTextBox.Text;
        }

        // ================= SPEED TX =================
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

        // ================= SERIAL RX =================
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
                    ushort counts =
                        (ushort)((rxBuffer[2] << 8) | rxBuffer[3]);

                    rxBuffer.RemoveRange(0, 4);

                    int signedCounts = (dir == 0x01) ? counts : -counts;

                    position += signedCounts * count2PositionFactor;
                    velocity = signedCounts * count2VelocityFactor;
                }
            }
        }

        // ================= LOGGING =================
        private void StartLogging(string filename)
        {
            logWriter = new StreamWriter(filename);
            logWriter.WriteLine("Time_ms,Position_cm,Velocity_RPM");
            logBuffer.Clear();
            logTimer.Restart();
            isLogging = true;
        }

        private void StopLogging()
        {
            isLogging = false;
            logTimer.Stop();

            if (logWriter != null)
            {
                // Final flush
                foreach (var line in logBuffer)
                    logWriter.WriteLine(line);

                logBuffer.Clear();

                logWriter.Flush();
                logWriter.Close();
                logWriter = null;
            }
        }

        private void TimerSave_Tick(object sender, EventArgs e)
        {
            if (!isLogging || logWriter == null) return;

            int timeMs = (int)logTimer.ElapsedMilliseconds;

            logBuffer.Add($"{timeMs},{position:F4},{velocity:F4}");

            if (logBuffer.Count >= LOG_FLUSH_SIZE)
            {
                foreach (var line in logBuffer)
                    logWriter.WriteLine(line);

                logBuffer.Clear();
            }
        }
    }
}
