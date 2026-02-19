using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using static System.Net.Mime.MediaTypeNames;

namespace CrokinoleRobotSoftware
{
    
    

    public class Transmissions
    {

        SerialPort _serialPort;

        // Connect to the MCU over UART serial communications
        public void ConnectMCU(SerialPort serialPort, string portName)
        {
            
            if (portName == null)
            {
                throw new ArgumentNullException(nameof(portName)); // throw null argument exception if portName is empty
            }

            _serialPort = serialPort;
            _serialPort.PortName = portName;
            if (!_serialPort.IsOpen)
            {
                _serialPort.Open();
            }

        }

        private void ReconnectMCU()
        {
            if (!_serialPort.IsOpen)
            {
                try
                {
                    _serialPort.Open();
                }
                catch { }
            }
        }

        // Disconnect the MCU over UART
        private void DisconnectMCU(SerialPort serialPort)
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }
        }

        // Send byte packet to MCU
        private void SendPacket()
        {
            // Send Header [0xFF]
            // Send Gantry MSB [0x00-0xFF]
            // Send Gantry LSB [0x00-0xFF]
            // Send Angle [0-180 deg]
            // Send Solenoid Power [0x00-0xFF]
        }

        // Send individual byte
        private void SendByte(byte txbyte)
        {
            
        }
    }

    internal class Vision
    {

        private VideoCapture _capture;
        private Mat _frame = new Mat();

        // Connect to the camera
        public void ConnectCamera(int cameraNum)
        {
            _capture = new VideoCapture(cameraNum);

            if (!_capture.IsOpened())
            { 
                throw new Exception($"Could not open camera with index: {cameraNum}"); 
            }

        }


        // Read camera capture
        public void CaptureFrame()
        {
            // InvalidOperation if camera isn't connected yet
            if (_capture == null || !_capture.IsOpened())
            {
                throw new InvalidOperationException("Camera is not connected.");
            }

            _capture.Read(_frame); // Read the current capture into the frame object

            // if empty frame
            if (_frame.Empty())
            {
                // For now do nothing [To be changed]
            }

        }

        // Calibrate the position measurements from the camera
        public void Calibrate()
        {

        }

        // Release camera for use
        // Dispose of related resources
        public void Dispose()
        {
            _capture?.Release();
            _capture?.Dispose();

        }


    }

    internal class Logic
    {


    }
}
