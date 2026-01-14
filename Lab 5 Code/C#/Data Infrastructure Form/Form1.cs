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
        

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _capture = new VideoCapture(1);
            _image = new Mat();
            _cameraThread = new Thread(new ThreadStart(CaptureCameraCallback));
            _cameraThread.Start();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            _cameraThread.Interrupt();
            _capture.Release();
            
        }

        private void CaptureCameraCallback()
        {
            while (true)
            {
                if (!_run) continue;
                var startTime = DateTime.Now;

                _capture.Read(_image);
                if (_image.Empty()) return;
                var imageRes = new Mat();
                var newImageGrey = new Mat();
                Cv2.Resize(_image, imageRes, new OpenCvSharp.Size(320, 240));
                var newImage = imageRes.Clone();
                if (_canny)
                    Cv2.Canny(imageRes, newImage, 50, 200);
                if (_hough)
                {
                    Cv2.MedianBlur(newImage, newImage, 5);
                    Cv2.CvtColor(newImage, newImageGrey, ColorConversionCodes.BGR2GRAY);
                    var circles = Cv2.HoughCircles(newImageGrey, HoughModes.Gradient, 1, 20);

                    foreach (var circle in circles)
                    {
                        Cv2.Circle(newImage, (int) circle.Center.X, (int) circle.Center.Y, (int) circle.Radius, Scalar.Green, 5);
                        Cv2.Circle(newImage, (int)circle.Center.X, (int)circle.Center.Y, 5, Scalar.Red, -1);
                    }

                }

                if (_fps)
                {
                    var diff = DateTime.Now - startTime;
                    var fpsInfo = $"FPS: Nan";
                    if (diff.Milliseconds > 0)
                    {
                        var fpsVal = 1.0 / diff.Milliseconds * 1000;
                        fpsInfo = $"FPS: {fpsVal:00}";
                    }
                    Cv2.PutText(imageRes, fpsInfo, new OpenCvSharp.Point(10, 20), HersheyFonts.HersheyComplexSmall, 1, Scalar.Black);
                }

                var bmpWebCam = BitmapConverter.ToBitmap(imageRes);
                var bmpEffect = BitmapConverter.ToBitmap(newImage);

                pictureBoxWebCam.Image = bmpWebCam;
                pictureBoxEffect.Image = bmpEffect;
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
    }
}
