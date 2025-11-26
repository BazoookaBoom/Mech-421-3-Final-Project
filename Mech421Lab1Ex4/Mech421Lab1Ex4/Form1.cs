using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mech421Lab1Ex4
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string serialDataString; //Holds incoming serial data
        ConcurrentQueue<Int32> dataQueue = new ConcurrentQueue<Int32>();

        private void SerialBytesReadTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void TempStrLengthTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void ItemsInQueueTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void ConnectDisconnectSerialButton_Click(object sender, EventArgs e)
        {
            //Should connect and disconnect depending on serial port status
            if(serialPort1.PortName == "COM3")
            {
                string transmitClose = "z";
                byte[] TxByte = Encoding.Unicode.GetBytes(transmitClose);
                serialPort1.Write(TxByte, 0, 1);
                serialPort1.Close();
                serialPort1.PortName = "COM1";
                Console.WriteLine("Serial Port Disconnected, set to: " + serialPort1.PortName);
                ConnectDisconnectSerialButton.Text = "Connect";
                SerialPortComboBox.Text = "Select Serial Port";
            }
            else if(SerialPortComboBox.Text == "Select Serial Port")
            {
                MessageBox.Show("Select Serial Port");

            }
            else
            {
                serialPort1.PortName = SerialPortComboBox.Text;
                serialPort1.Open();
                ConnectDisconnectSerialButton.Text = "Disconnect";
                Console.WriteLine("Connected to " + serialPort1.PortName);
                string transmit = "a";
                byte[] TxByte = Encoding.Unicode.GetBytes(transmit);
                serialPort1.Write(TxByte, 0, 1);
                Console.WriteLine("Sent: " + transmit);
            }


        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                SerialBytesReadTextBox.Text = serialPort1.BytesToRead.ToString();
                TempStrLengthTextBox.Text = serialDataString.Length.ToString();
                ItemsInQueueTextBox.Text = dataQueue.Count.ToString();
                serialDataString = "";
                foreach (Int32 item in dataQueue)
                {
                    int localValue;
                    while (dataQueue.TryDequeue(out localValue) == false) ;
                    SerialDataStreamTextBox.AppendText(localValue.ToString() + ", ");
                }
            }
            else
            { 
                SerialBytesReadTextBox.Text = "0";
                TempStrLengthTextBox.Text = "0";
                ItemsInQueueTextBox.Text = "0";
            }
        }
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int newByte = 0;
            int bytesToRead;
            bytesToRead = serialPort1.BytesToRead;
            while (bytesToRead != 0)
            {
                newByte = serialPort1.ReadByte();
                serialDataString = serialDataString + newByte.ToString() + ", ";
                dataQueue.Enqueue(newByte);
                bytesToRead = serialPort1.BytesToRead;
                Console.WriteLine("New Byte: " + newByte.ToString());
            }
        }


    }
}
