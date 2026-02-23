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

            _logicHelper.NextState(); // Startup & Load complete

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
            switch (ComboBxDiff.Text)
            {
                case "Easy": _logicHelper.SetDifficulty(0); break;
                case "Medium": _logicHelper.SetDifficulty(1); break;
                case "Hard": _logicHelper.SetDifficulty(2); break;
                case "Just Plain Mean": _logicHelper.SetDifficulty(3); break;
            }

            _SM.NextState();


        }

        public void ActionMgr()
        {
            switch (_SM.GetState())
            {
                case 0:
                    // Actions done in Form1_load and Form1 class initialization
                    break;
                case 1:
                    // Load the start screen
                    tabControl1.SelectedIndex = 0;
                    break;
                case 2: // Wait for player to input difficulty
                    ButtDiffSel.Enabled = true;
                    break;


                case 3: // Wait for user to complete turn
                    break;
                case 4: // Determine which strategy to use. Capture board state
                    break;
                case 5: // Calculate the desired shot
                    break;
                case 6: // Calculate MCU instructions
                    break;
                case 7: // Transmit Instructions
                    break;


                case 8:
                    break;
                case 9:
                    break;
                case 10:
                    break;


                case 11:
                    break;
                case 12:
                    break;



            }
        }


        private void EndTurnButt_Click(object sender, EventArgs e)
        {
            _SM.NextState(); // Proceeed to state 4:
        }

    }
}
