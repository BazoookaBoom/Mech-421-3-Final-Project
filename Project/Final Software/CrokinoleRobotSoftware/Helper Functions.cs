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
        public Point2f DetermineStrategy(List<Vec4f> discs)
        {
            Random random = new Random();
            double randVal;

            // Check current board state
            if (CheckForOppDiscs(discs))
            {
                // Opponent's discs are on board
                while (_difficulty == 0) {
                    randVal = random.NextDouble();

                    if (randVal < 0.01) { return 0; }
                    else if (randVal < 0.4) { return 1; }
                    else if (randVal < 0.7) { return 2; }
                    else return 3;
                }
                while (_difficulty == 1) { }
                while (_difficulty == 2) { }
                while (_difficulty == 3) { }
            }
            else
            {
                // No opponent discs
                while (_difficulty == 0) {
                    randVal = random.NextDouble();

                    if (randVal < 0.05) { return SinkerStratNO(discs); } // 5% chance to target 20-pt
                    else { return DefenceStratNO(discs); }
                }
                while (_difficulty == 1) {
                    randVal = random.NextDouble();

                    if (randVal < 0.10) { return SinkerStratNO(discs); } // 10% chance to target 20-pt
                    else { return DefenceStratNO(discs); }
                }
                while (_difficulty == 2) {
                    randVal = random.NextDouble();

                    if (randVal < 0.20) { return SinkerStratNO(discs); } // 20% chance to target 20-pt
                    else { return DefenceStratNO(discs); }
                }
                while (_difficulty == 3) {
                    randVal = random.NextDouble();

                    if (randVal < 0.80) { return SinkerStratNO(discs); } // 80% chance to target 20-pt
                    else { return DefenceStratNO(discs); }
                }
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
        private Point2f SpreadStrat(List<Vec4f> discs)
        {
            Random random = new Random();

            while (true)
            {
                // Pick a disc in the 10 or 5-pt zone and target it's coordinates
                random.Next(discs.Count());


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

        // HitAny strategy attempts to hit an opponents pieces
        private void HitAny(List<Vec4f> discs)
        {
            Random random = new Random();

            while (true)
            {
                int index = random.Next(discs.Count);

                float x = discs[index].Item0;
                float y = discs[index].Item1;


            }
        }

        // Calculate shot parameters
        // Avoids collisions with pins, own discs. Tries to hit opp discs
        public Vec3f CalculateShot(List<Vec4f> discs, Point2f target)
        {
            List<Vector2> validShots = new List<Vector2>(); 
            List<int> validAngles = new List<int>();
            const float arcCenterAngle = 270f * ((float)Math.PI / 180f); // 270 degrees on the unit circle is the center of the arc
            float arcHalfSpan = 45f * ((float) Math.PI / 180f); // 45 degrees each side
            Random random = new Random();



            // Generate potential locations along the shooting arc
            int samples = 90; // one per degree, adjust as needed. 0 corresponds with 225 on the units circle, 90 corresponds with 315

            for (int i = 0; i <= samples; i++)
            {
                float t = i / (float)samples; // 0 to 1
                float angle = (arcCenterAngle - arcHalfSpan) + t * (arcHalfSpan * 2);

                Vector2 shooterPos = new Vector2(
                    (float) Math.Cos(angle) * R5 + Cx,
                    (float) Math.Sin(angle) * R5 + Cy
                );

                Vector2 vec2Target = new Vector2(target.X, target.Y);

                // check if path is clear of pegs
                if (PathClearOfPegs(shooterPos, vec2Target))
                {
                    validShots.Add(shooterPos);
                    validAngles.Add(i);
                }
                    
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
            byte slewAngle = (byte) (AngleBetween(perpendicular, selDirection) + 90);

            // Convert selected position to shooter circular position
            int gantryPos = 0xFFFF - validAngles[selectedIndex] / 90 * 0xFFFF;


            Vec3f output = new Vec3f(
                gantryPos, // actual format should be gantry pos, slew angle, power
                slewAngle,
                (byte) 0xFF);

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

        bool DiscCollision(Vector2 start, Vector2 end, List<Vec4f> discs) =>
        !discs.Any(disc => LineIntersectsCircle(start, end, new Vector2(disc[0], disc[1]), disc[2] + RD));

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
        private bool whiteStartedRound = true;
        private int shotsLeft = 8;
        private bool pendingRoundEnd = false;



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
                case 0: state++; break; // Load game
                case 1: state++; break; // Difficulty selection received

                case 2: 

                    if(pendingRoundEnd) { pendingRoundEnd = false; state = 5; } //Jump to round end now that player has finished their turn, ending the round
                    else { state++;  }
                    break; 

                case 3: state++; break; // Calculating and sending instructions to MCU
                case 4:

                    if (shotsLeft-- > 0) { 
                        if(whiteStartedRound == true)
                        {
                            state = 5;
                        }
                        else
                        {
                            state = 2;
                            pendingRoundEnd = true;
                        }

                            state = 2; 
                    } // If shots remaining, go to next player turn
                    else { state = 5; } // Else go to round end
                    
                    break;
                case 5: 
                    if(whiteStartedRound == true) { 
                        state = 3; // If white started this round, black starts the next round
                        whiteStartedRound = false;
                        shotsLeft = 8; // Reset shot count
                    }
                    else { state = 2; whiteStartedRound = true; }


                        ; break;
                case 6:
                    if (stateChange == 0) { state = 2; }
                    else { state = 7; } // Branch 1 goes to 
                    break;
                case 7: state++; break;
            }

            return state;
        }



    }


}
