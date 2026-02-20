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


        public Form1()
        {
            InitializeComponent();

            // Initialize helper classes
            _TxHelper = new Transmissions();
            _visionHelper = new Vision();
            _logicHelper = new Logic();
        }

        
        private void Form1_Load(object sender, EventArgs e)
        {

            Heartbeat.Start();
            Task.Run(() => InitializeHardware());

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


        }
    }
}
