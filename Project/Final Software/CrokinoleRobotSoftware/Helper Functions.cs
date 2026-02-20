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
        string _connectionStatus;

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
                _connectionStatus = "Connected";
            }

        }

        public void ReconnectMCU()
        {
            if (!_serialPort.IsOpen)
            {
                try
                {
                    _serialPort.Open();
                    _connectionStatus = "Connected";
                }
                catch 
                {
                    _connectionStatus = "Disconnected";
                }
            }
        }

        // Disconnect the MCU over UART
        public void DisconnectMCU()
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.Close();
            }
        }

        // Send byte packet to MCU
        public void SendPacket(ushort gantryPos, byte angle, byte power)
        {
            // Send Header [0xFF]
            SendByte(0xFF);

            // Send Gantry MSB [0x00-0xFF]
            byte MSB = (byte) ((gantryPos >> 8) & 0xFF);
            SendByte(MSB);

            // Send Gantry LSB [0x00-0xFF]
            byte LSB = (byte)(gantryPos & 0xFF);
            SendByte(LSB);

            // Send Angle [0-180 deg]
            SendByte(angle);
            
            // Send Solenoid Power [0x00-0xFF]
            SendByte(power);
        }

        // Send individual byte
        private void SendByte(byte txbyte)
        {
            _serialPort.Write(txbyte.ToString());
        }
    }

    internal class Vision
    {

        private VideoCapture _capture;
        private Mat _latestFrame = new Mat();
        private readonly object _frameLock = new object();
        private Thread _captureThread;
        private volatile bool _isCapturing = false;
        private List<Vec4f> discs = new List<Vec4f>();
        // Connect to the camera
        public void ConnectCamera(int cameraNum)
        {
            _capture = new VideoCapture(cameraNum);
            if (!_capture.IsOpened())
            { 
                throw new Exception($"Could not open camera with index: {cameraNum}"); 
            }

        }

        // Start backrgound capture thread
        public void StartCapture()
        {
            if (_isCapturing) return;
            _isCapturing = true;

            _captureThread = new Thread(() =>
            {
                while (_isCapturing)
                {
                    if (_capture == null || !_capture.IsOpened()) break;

                    var tempFrame = new Mat();
                    _capture.Read(tempFrame);

                    if (!tempFrame.Empty())
                    {
                        lock (_frameLock)
                        {
                            _latestFrame = tempFrame;
                        }
                    }
                }
            })

            {
                IsBackground = true, // Thread will die when the app closes
                Name = "CameraCaptureThread"
            };

            _captureThread.Start();
        }

        // Stop background capture thread
        public void StopCapture()
        {
            _isCapturing = false;
            _captureThread?.Join(500); // Wait up to 500ms for it to finish
        }

        // Analyse the latest frame from the buffer and update the PictureBox
        public List<Vec4f> AnalyseFrame(PictureBox picBoxObj)
        {
            Mat frameCopy;

            lock (_frameLock)
            {
                if (_latestFrame == null || _latestFrame.Empty())
                    return null;

                frameCopy = _latestFrame.Clone(); // Take a snapshot so the capture thread can keep running
            }

            var greyFrame = PrepFrame4Hough(frameCopy);
            var circles = Cv2.HoughCircles(greyFrame, HoughModes.GradientAlt, 1, 5, 300, 0.9, 5, 50);

            discs.Clear();
            foreach (var circle in circles)
            {
                Vec3b colour = frameCopy.At<Vec3b>((int)circle.Center.Y, (int)circle.Center.X);
                float team = AssignTeam(colour);
                discs.Add(new Vec4f(circle.Center.X, circle.Center.Y, circle.Radius, team));
            }

            DrawCVFrame(frameCopy, picBoxObj);

            return discs;
        }


        private void DrawCVFrame(Mat frame, PictureBox picBoxObj)
        {
            Mat drawnImage = frame.Clone();

            foreach (var disc in discs)
            {
                string team;
                if (disc.Item3 < 0.1)
                {
                    team = "White";
                }
                else
                {
                    team = "Black";
                }

                Cv2.Circle(drawnImage, (int)disc.Item0, (int)disc.Item1, (int)disc.Item2, Scalar.Green, 5);
                Cv2.Circle(drawnImage, (int)disc.Item0, (int)disc.Item1, 5, Scalar.Red, -1);
                Cv2.PutText(drawnImage, team, GetDiscCenter(disc), HersheyFonts.HersheyComplexSmall, 0.5, Scalar.Black);

            }

            picBoxObj.Image = BitmapConverter.ToBitmap(drawnImage); // Draw Image



        }

        // Extracts center coordinate of a disc vector (Vec4f) and converts it to a point
        private OpenCvSharp.Point GetDiscCenter( Vec4f disc)
        {
            OpenCvSharp.Point discCenter = new OpenCvSharp.Point();
            discCenter.X = (int)disc.Item1;
            discCenter.Y = (int)disc.Item0;

            return discCenter;
        }

        // Apply blur and greyscale to a BGR frame
        private Mat PrepFrame4Hough(Mat colourFrame)
        {
            var _greyFrame = new Mat();

            _greyFrame = colourFrame.Clone();
            Cv2.MedianBlur(_greyFrame, _greyFrame, 5);
            Cv2.CvtColor(_greyFrame, _greyFrame, ColorConversionCodes.BGR2GRAY);

            return _greyFrame;
        }

        // Determine team of disc. White = 0, Black = 1
        private int AssignTeam(Vec3b discColour)
        {

            if (CalculateLuminance(discColour) > 127.5)
            { return 0; }
            else { return 1; }
        }

        // Calculate luminance of an rgb vector
        private double CalculateLuminance(Vec3b colour)
        {
            byte blue = colour[0];
            byte green = colour[1];
            byte red = colour[2];

            double luminance = blue * 0.0722 + green * 0.7152 + red * 0.2172;

            return luminance;
        }

        // Calibrate the position measurements from the camera []TBD still
        public void Calibrate()
        {

        }

        // Release camera for use and dispose of related resources
        public void Dispose()
        {
            StopCapture();
            _capture?.Release();
            _capture?.Dispose();

        }


    }

    internal class Logic
    {


    }
}
