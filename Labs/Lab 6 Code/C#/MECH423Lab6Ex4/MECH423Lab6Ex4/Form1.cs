using System;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;

namespace MECH423Lab6Ex4
{
    public partial class Form1 : Form
    {
        // ===== Logging =====
        StreamWriter logWriter;
        Stopwatch stopwatch = new Stopwatch();
        volatile bool logging = false;

        // ===== Encoder =====
        int lastCount = 0;
        double lastTime = 0;

        public Form1()
        {
            InitializeComponent();

            serialPort1.DataReceived += serialPort1_DataReceived;

            // Populate COM ports
            comboBoxComPort.Items.AddRange(SerialPort.GetPortNames());
        }

        // ================= CONNECT =================
        private void buttonConnect_Click(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                serialPort1.PortName = comboBoxComPort.Text;
                serialPort1.BaudRate = 115200;
                serialPort1.Open();
            }
        }

        // ================= SEND PWM STEP =================
        private void buttonSendPWM_Click(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen) return;

            int pwm = int.Parse(textBoxPWM.Text); // 0–65535
            byte msb = (byte)((pwm >> 8) & 0xFF);
            byte lsb = (byte)(pwm & 0xFF);

            byte[] packet = { 0xAA, msb, lsb };
            serialPort1.Write(packet, 0, packet.Length);
        }

        // ================= START LOGGING =================
        private void buttonStartLog_Click(object sender, EventArgs e)
        {
            logWriter = new StreamWriter("motor_log.csv");
            logWriter.WriteLine("Time_s,Position_counts,Velocity_counts_per_s");

            lastCount = 0;
            lastTime = 0;

            stopwatch.Restart();
            logging = true;
        }

        // ================= STOP LOGGING =================
        private void buttonStopLog_Click(object sender, EventArgs e)
        {
            logging = false;
            stopwatch.Stop();

            logWriter.Flush();
            logWriter.Close();
        }

        // ================= SERIAL RX =================
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            while (serialPort1.BytesToRead >= 4)
            {
                if (serialPort1.ReadByte() != 0xAA)
                    continue;

                int dir = serialPort1.ReadByte();
                int msb = serialPort1.ReadByte();
                int lsb = serialPort1.ReadByte();

                int count = (msb << 8) | lsb;
                if (dir == 0) count = -count;

                double t = stopwatch.Elapsed.TotalSeconds;

                double vel = 0;
                if (lastTime > 0)
                    vel = (count - lastCount) / (t - lastTime);

                lastCount = count;
                lastTime = t;

                if (logging)
                {
                    logWriter.WriteLine($"{t:F6},{count},{vel:F3}");
                }
            }
        }
    }
}
