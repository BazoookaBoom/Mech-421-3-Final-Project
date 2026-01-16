using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using static System.Net.Mime.MediaTypeNames;

namespace Data_Infrastructure_Form
{
    public partial class Form1 : Form
    {
        private bool _run = false;
        private bool _canny = false;
        private bool _hough = false;
        private VideoCapture _capture;
        private Mat _image;
        private Thread _cameraThread;
        private bool _fps = false;
        private int cameraNum = 0; // Choose which camera to use
        private volatile bool _closing = false;
        private byte txbyte;

        System.Windows.Forms.Timer autoReconnectTimer = new System.Windows.Forms.Timer();
        bool userWantsConnection = false;

        public Form1()
        {
            InitializeComponent();
            RefreshCOMPorts();

            autoReconnectTimer.Interval = 1000;
            autoReconnectTimer.Tick += AutoReconnectTimer_Tick;
            autoReconnectTimer.Start();
        }

        private void RefreshCOMPorts()
        {
            var ports = System.IO.Ports.SerialPort.GetPortNames();
            var selected = dpdnConnect.Text;

            dpdnConnect.Items.Clear();
            dpdnConnect.Items.AddRange(ports);

            if (ports.Contains(selected))
                dpdnConnect.Text = selected;
            else if (ports.Length > 0)
                dpdnConnect.SelectedIndex = 0;
            else
                dpdnConnect.Text = "No COM ports!";
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
                serialPort1.PortName = dpdnConnect.Text;
                serialPort1.Open();
                btnConnect.Text = "Disconnect";
            }
            catch { }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _capture = new VideoCapture(cameraNum);
            _image = new Mat();
            _cameraThread = new Thread(new ThreadStart(CaptureCameraCallback));
            _cameraThread.Start();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            _closing = true;
            _run = false;

            try { _cameraThread?.Join(500); } catch { }
            try { _capture?.Release(); } catch { }

        }

        private void UI(Action action)
        {
            if (IsDisposed) return;

            if (InvokeRequired)
                BeginInvoke(action);
            else
                action();
        }

        private void CaptureCameraCallback()
        {
            while (!_closing)
            {
                // Don’t spin at 100% CPU when paused
                if (!_run)
                {
                    Thread.Sleep(10);
                    continue;
                }

                var startTime = DateTime.Now;

                _capture.Read(_image);
                if (_image.Empty())
                {
                    Thread.Sleep(10);
                    continue;
                }

                var imageRes = new Mat();
                var newImageGrey = new Mat();

                Cv2.Resize(_image, imageRes, new OpenCvSharp.Size(320, 240));
                var newImage = imageRes.Clone();

                if (_canny)
                    Cv2.Canny(imageRes, newImage, 50, 200);

                string xText = "NaN", yText = "NaN", rText = "NaN";
                txbyte = 0; // 0 encodes as no data read

                if (_hough)
                {
                    Cv2.MedianBlur(newImage, newImage, 5);
                    Cv2.CvtColor(newImage, newImageGrey, ColorConversionCodes.BGR2GRAY);
                    var circles = Cv2.HoughCircles(newImageGrey, HoughModes.Gradient, 1, 20);

                    foreach (var circle in circles)
                    {
                        Cv2.Circle(newImage, (int)circle.Center.X, (int)circle.Center.Y, (int)circle.Radius, Scalar.Green, 5);
                        Cv2.Circle(newImage, (int)circle.Center.X, (int)circle.Center.Y, 5, Scalar.Red, -1);
                    }

                    if (circles.Length > 0)
                    {
                        xText = circles[0].Center.X.ToString("0.##");
                        yText = circles[0].Center.Y.ToString("0.##");
                        rText = circles[0].Radius.ToString("0.##");

                        txbyte = Convert.ToByte(circles[0].Center.X/320*254 + 1); // Convert to a byte (1 to 255)
                    }
                }

                if (_fps)
                {
                    var diff = DateTime.Now - startTime;
                    var fpsInfo = "FPS: NaN";
                    if (diff.TotalMilliseconds > 0)
                    {
                        var fpsVal = 1000.0 / diff.TotalMilliseconds;
                        fpsInfo = $"FPS: {fpsVal:00}";
                    }
                    Cv2.PutText(imageRes, fpsInfo, new OpenCvSharp.Point(10, 20),
                        HersheyFonts.HersheyComplexSmall, 1, Scalar.Black);
                }

                // Create bitmaps on worker thread
                var bmpWebCam = BitmapConverter.ToBitmap(imageRes);
                var bmpEffect = BitmapConverter.ToBitmap(newImage);

                // Assign on UI thread (and dispose old images to avoid leaking memory)
                UI(() =>
                {
                    // If form is closing/disposed, don’t touch controls
                    if (IsDisposed) { bmpWebCam.Dispose(); bmpEffect.Dispose(); return; }

                    txtXOut.Text = xText;
                    txtYOut.Text = yText;
                    txtROut.Text = rText;

                    var old1 = pictureBoxWebCam.Image;
                    pictureBoxWebCam.Image = bmpWebCam;
                    old1?.Dispose();

                    var old2 = pictureBoxEffect.Image;
                    pictureBoxEffect.Image = bmpEffect;
                    old2?.Dispose();
                });
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            _run = true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _run = false;
        }

        private void btnCanny_Click(object sender, EventArgs e)
        {
            _hough = false;
            _canny = !_canny;

        }

        private void btnFPS_Click(object sender, EventArgs e)
        {
            _fps = !_fps;
        }

        private void btnHough_Click(object sender, EventArgs e)
        {
            _canny = false;
            _hough = !_hough;
            
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            // Close serial port and reset button text to reflect new state
            if (serialPort1.IsOpen)
            {
                userWantsConnection = false;
                serialPort1.Close();
                btnConnect.Text = "Connect";
                return;
            }

            // If connection attempted, but no COM ports are available to connect to, show error message
            if (dpdnConnect.Text == "No COM ports!")
            {
                MessageBox.Show("No COM ports detected!");
                return;
            }

            // If program has made it to this point, then:
            // - serial port is currently closed
            // - AND there must be a valid COM port to connect to

            // Take dropdown-selected COM port as the connection target
            serialPort1.PortName = dpdnConnect.Text;

            // Attempt to connect to drop-down selected port. If connection failed, attempt auto-reconnects
            try
            {
                serialPort1.Open();
                btnConnect.Text = "Disconnect";
                userWantsConnection = true;
            }
            catch
            {
                MessageBox.Show("Failed to open port. Will auto-retry.");
                userWantsConnection = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            serialPort1.Write(txbyte.ToString());
        }
    }
}
