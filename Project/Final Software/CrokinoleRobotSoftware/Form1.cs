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
            // Connect to MCU
            _TxHelper.ConnectMCU(serialPort1, COMPORT);
            // Connect to Camera
            _visionHelper.ConnectCamera(CAMERANUM);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            _visionHelper.Dispose();
        }

        private void Heartbeat_Tick(object sender, EventArgs e)
        {
            
        }
    }
}
