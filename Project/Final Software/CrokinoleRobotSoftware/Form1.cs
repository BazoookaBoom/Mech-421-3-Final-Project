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


namespace CrokinoleRobotSoftware
{
    public partial class Form1 : Form
    {
        // Constants
        const int CAMERANUM = 0;
        const string COMPORT = "COM5";

        // Declare helper classes
        private Transmissions _TxHelper;
        private Vision _visionHelper;
        private Logic _logicHelper;
        private StateMachine _SM;

        // Track Scores
        int blackScore = 0;
        int whiteScore = 0;


        public Form1()
        {
            InitializeComponent();

            // Initialize helper classes
            _TxHelper = new Transmissions();
            _visionHelper = new Vision();
            _logicHelper = new Logic();
            _SM = new StateMachine();
        }


        private void Form1_Load(object sender, EventArgs e)
        {

            Heartbeat.Start();
            Task.Run(() => InitializeHardware());

            _SM.NextState(); // Startup & Load complete

        }
        private void InitializeHardware()
        {
            try
            {
                _TxHelper.ConnectMCU(serialPort1, COMPORT);
                _visionHelper.ConnectCamera(CAMERANUM);
                _visionHelper.StartCapture();
            }
            catch (Exception ex)
            {
                this.Invoke((Action)(() =>
                    MessageBox.Show($"Hardware init failed: {ex.Message}")));
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            _visionHelper.Dispose();
            _TxHelper.DisconnectMCU();
        }

        private void Heartbeat_Tick(object sender, EventArgs e)
        {
            _TxHelper.ReconnectMCU();

            _visionHelper.AnalyseFrame(camFrame);

            ActionMgr();

        }

        private void ButtDiffSel_Click(object sender, EventArgs e)
        {
            if(_SM.GetState() == 2)
            {
                bool proceedFlag = false;

                switch (ComboBxDiff.Text)
                {
                    case "Easy": _logicHelper.SetDifficulty(0); proceedFlag = true; break;
                    case "Medium": _logicHelper.SetDifficulty(1); proceedFlag = true; break;
                    case "Hard": _logicHelper.SetDifficulty(2); proceedFlag = true; break;
                    case "Just Plain Mean": _logicHelper.SetDifficulty(3); proceedFlag = true; break;
                }

                if (proceedFlag) {
                    _SM.NextState(); // Move to next state
                    tabControl1.SelectedIndex = 1; // Load the start screen
                }
                
            }
        }

        public void ActionMgr()
        {
            switch (_SM.GetState())
            {
                case 0:
                    // Actions done in Form1_load and Form1 class initialization
                    break;
                case 1: // Wait for difficulty selection
                    tabControl1.SelectedIndex = 0; // Load the start screen
                    ButtDiffSel.Enabled = true; // Enable difficulty selection button
                    break;

                // In-Game
                case 2: 
                    // Move to state 3 when player turn completed
                    break;
                case 3: // Calculate target and instructions, send to MCU
                    List<Vec4f> discs = _visionHelper.AnalyseFrame(camFrame);
                    Point2f target = _logicHelper.DetermineStrategy(discs);
                    Vec3f instructions = _logicHelper.CalculateShot(discs, target);
                    _TxHelper.SendPacket((ushort) instructions.Item0, (byte) instructions.Item1, (byte) instructions.Item2);

                    _SM.NextState();
                    break;
                case 4:
                    // Proceed to next state only after acknowledgement of shot. See serialPort1_DataReceived
                    break;
                case 5: // End of round, load round end screen
                    tabControl1.SelectedIndex = 3;
                    WhtGameScore.Text = whiteScore.ToString();
                    BlkGameScore.Text = blackScore.ToString();

                    break;
                case 6: // 

                // End-Game
                    break;
                case 7: 
                    break;



            }
        }


        private void EndTurnButt_Click(object sender, EventArgs e)
        {
            _SM.NextState(); // Proceeed to state 4:
        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {

        }

        private void RoundEndProceedButt_Click(object sender, EventArgs e)
        {


            if (int.TryParse(WhtRoundScore.Text.Trim(), out int valueWht))
            {
                whiteScore += valueWht;
            }
            if (int.TryParse(BlkRoundScore.Text.Trim(), out int valueBlk))
            {
                blackScore += valueBlk;
            }

            if (whiteScore >= 100 || blackScore >= 100)
            {
                _SM.NextState(1);
            }
            else
            {
                _SM.NextState();
            }
            

        }
    }
}
