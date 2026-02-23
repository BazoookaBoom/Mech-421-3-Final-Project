using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Dnn;
using OpenCvSharp.Extensions;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.AxHost;

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
            discCenter.X = (int)disc.Item0;
            discCenter.Y = (int)disc.Item1;

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

        // Calibrate the position measurements from the camera (stretch goal))
        //public void Calibrate()

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
        int _difficulty; // 0-3 where 0 is easiest
        readonly List<Vec2f> pegs = new List<Vec2f>()
        {
            new Vec2f(365.67f, 384.26f),
            new Vec2f(413.64f, 336.29f),
            new Vec2f(413.64f, 268.46f),
            new Vec2f(365.67f, 220.49f),
            new Vec2f(297.83f, 220.49f),
            new Vec2f(249.86f, 268.46f),
            new Vec2f(249.86f, 336.29f),
            new Vec2f(297.83f, 384.26f)

        };
        readonly float Cx = 331f; // Center X of 20-pt hole
        readonly float Cy = 302f; // Center Y of 20-pt hole
        readonly float R15 = 88.63f; // pixel radius of 15-pt circle
        readonly float R10 = 171.55f; // pixel radius of 10-pt circle
        readonly float R5 = 250.25f; // pixel radius of 5-pt circle
        readonly float RD = 15f; // pixel radius of a disc
        readonly float RP = 2f; // Estimated pixel radius of a peg

        public void SetDifficulty(int difficulty)
        {
            _difficulty = difficulty;
        }

        // Decide a strategy based on difficulty
        public int DetermineStrategy(List<Vec4f> discs)
        {
            // Check current board state
            CheckForOppDiscs(discs);


            Random random = new Random();

            double value = random.NextDouble();

            if (_difficulty == 0)
            {
                if (value < 0.01) { return 0; }
                else if (value < 0.4) { return 1; }
                else if (value < 0.7) { return 2; }
                else return 3;
            }
            else if (_difficulty == 1)
            {
                if (value < 0.1) { return 0; }
                else if (value < 0.3) { return 1; }
                else if (value < 0.7) { return 2; }
                else return 3;
            }
            else if (_difficulty == 2)
            {
                if (value < 0.2) { return 0; }
                else if (value < 0.5) { return 2; }
                else return 3;
            }
            else
            {
                return 0; // Shouldn't ever be reached unless debugging
            }

        }

        // ========================================================
        // Strategies follow the naming convention outlined below:
        // [Descriptive Name][Conditional Variant Suffix (NO)]
        // NO = "No Opponent" pieces on the board

        // Attempts to sink a shot in the center 20-point hole
        private Point2f SinkerStratNO(List<Vec4f> discs)
        {
            return new Point2f(Cx, Cy); // Set target to the center hole
        }

        // Defensive strategy attempts to place a shot in half of the 15-point zone closest to the opponent
        private Point2f DefenceStratNO(List<Vec4f> discs)
        {

            Random random = new Random();

            while(true)
            {
                // Generate coordinate
                float x = (float) random.NextDouble() * ((Cx - R15) - (Cx + R15));
                float y = (float) random.NextDouble() * ((Cy) - (Cy + R15));

                // If the coordinate lies within the half of the 15-pt circle closer to the opponent, proceed
                if( (Math.Pow(x - Cx, 2) + Math.Pow(y - Cy, 2)) < Math.Pow(R15, 2))
                {
                    return new Point2f(x, y);
                }
            }
        }

        // Spread strategy tries to place a shot in the 10 or 5-point zone to spread the opponent's attention
        private Point2f SpreadStratNO(List<Vec4f> discs)
        {
            Random random = new Random();

            while (true)
            {
                // Generate coordinate
                float x = (float)random.NextDouble() * ((Cx - R5) - (Cx + R5));
                float y = (float)random.NextDouble() * ((Cy - R5) - (Cy));

                // If the coordinate lies within the 5-pt circle but outside the 15-point circle...
                if (CheckPointInCircle(x, y, R5) && !CheckPointInCircle(x, y, R15))
                {
                    if (y < Cy)
                    {
                        return new Point2f(x, y);
                    }

                }
            }
        }

        // Offensive strategy attempts to knock opponent's pieces outwards
        private void AttackStrat(List<Vec4f> discs)
        {

        }

        // Calculate shot parameters
        // Avoids collisions with pins, own discs. Tries to hit opp discs
        public Vec3f CalculateShot(List<Vec4f> discs, Point2f target)
        {
            List<Vector2> validShots = new List<Vector2>(); 
            const float arcCenterAngle = 270f;
            float arcHalfSpan = 45f * ((float) Math.PI / 180f); // 45 degrees each side
            Random random = new Random();

            // Generate potential locations along the shooting arc

            int samples = 90; // one per degree, adjust as needed

            for (int i = 0; i <= samples; i++)
            {
                float t = i / (float)samples; // 0 to 1
                float angle = (arcCenterAngle - arcHalfSpan) + t * (arcHalfSpan * 2);

                Vector2 shooterPos = new Vector2(
                    (float) Math.Cos(angle) * R5 + Cx,
                    (float) Math.Sin(angle) * R5 + Cy
                );

                Vector2 vec2Target = new Vector2(target.X, target.Y);

                // direction from shooter toward target
                Vector2 direction = Vector2.Normalize(vec2Target - shooterPos);
                

                // check if path is clear of pegs
                if (PathClearOfPegs(shooterPos, vec2Target))
                    validShots.Add(shooterPos);
            }

            // Choose shooter position
            int selectedIndex = random.Next(validShots.Count());

            Vector2 selectedPos = new Vector2(
                validShots[selectedIndex].X,
                validShots[selectedIndex].Y);


            // Calculate slew angle
            Vector2 radialVec = new Vector2(Cx, Cy);
            Vector2 perpendicular = Vector2.Normalize(radialVec - selectedPos);
            Vector2 selVec2Target = new Vector2(target.X, target.Y);
            Vector2 selDirection = Vector2.Normalize(selVec2Target - selectedPos);
            float slewAngle = AngleBetween(perpendicular, selDirection) + 90;

            Vec3f output = new Vec3f(
                validShots[selectedIndex].X,
                validShots[selectedIndex].Y,
                slewAngle);

            return output;

        }
        // Calculate angle between 2 vectors
        float AngleBetween(Vector2 a, Vector2 b)
        {
            float dot = Vector2.Dot(a, b);
            float cross = a.X * b.Y - a.Y * b.X;
            return (float) (Math.Atan2(cross, dot) * Math.PI/180); // returns radians, negative = clockwise
        }

        // Check whether the path is clear of pegs [Add disc collision detection]
        bool PathClearOfPegs(Vector2 start, Vector2 end) =>
        !pegs.Any(peg => LineIntersectsCircle(start, end, new Vector2(peg[0], peg[1]), RP + RD));

        // Checks whether a line intersects a circle specified by its center and radius
        bool LineIntersectsCircle(Vector2 start, Vector2 end, Vector2 center, float radius)
        {
            Vector2 d = end - start;           // line direction vector
            Vector2 f = start - center;        // vector from circle center to line start

            float a = Vector2.Dot(d, d);
            float b = 2 * Vector2.Dot(f, d);
            float c = Vector2.Dot(f, f) - radius * radius;

            float discriminant = b * b - 4 * a * c;

            if (discriminant < 0)
                return false;  // no intersection

            float sqrtDisc = (float) Math.Sqrt(discriminant);
            float t1 = (-b - sqrtDisc) / (2 * a);
            float t2 = (-b + sqrtDisc) / (2 * a);

            // t represents how far along the segment the intersection is
            // it's only a hit if t is between 0 and 1 (within the segment)
            return (t1 >= 0 && t1 <= 1) || (t2 >= 0 && t2 <= 1);
        }

        // Checks whether any discs belonging to the player are on the board
        private bool CheckForOppDiscs(List<Vec4f> discs)
        {
            foreach (var discItem in discs)
            {
                if (discItem.Item3 < 0.1) return true; // if team field has a value of 0f, then that disc belongs to white (player)
            }
            return false;
        }

        // Checks whether the coordinate is within the circle of specified radius
        private bool CheckPointInCircle(float x, float y, float R)
        {
            if ((Math.Pow(x - Cx, 2) + Math.Pow(y - Cy, 2)) < Math.Pow(R, 2)) { return true; }
            else { return false; }
        }


    }

    internal class StateMachine
    {

        int state = 0; // "dumb" state variable that only tracks state. No state-related code is called from the Logic class

        // return current state
        public int GetState()
        {
            return state;
        }

        // Moves to next state in the state machine. For branching paths, path 0 is the default
        public int NextState(int stateChange = 0)
        {
            switch (state)
            {
                // Pre-game states
                case 0: state++; break;
                case 1: state++; break;
                case 2: state++; break;

                // In-round states
                case 3: state++; break;
                case 4: state++; break;
                case 5: state++; break;
                case 6: state++; break;
                case 7:
                    if (stateChange == 0) { state = 3; }
                    else { state = 8; } // Branch 1 goes to 
                    break;

                // Post-round states
                case 8: state++; break;
                case 9: state++; break;
                case 10:
                    if (stateChange == 0) { state = 3; } // New round
                    else { state = 11; }
                    break;

                // End game states
                case 11: state = 1; break;
                case 12: state = 1; break;
            }

            return state;
        }



    }


}
