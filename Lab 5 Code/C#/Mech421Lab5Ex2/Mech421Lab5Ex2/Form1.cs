using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mech421Lab5Ex2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        int speed = 0;
        private void CWOneStepButton_Click(object sender, EventArgs e)
        {
            //TD Lab 5 Exercise 2: Add code to make the stepper motor take one step clockwise
            //Should be implemented by just sending a letter to make the firmware do one step
            //No encoding on this side
        }

        private void CCWOneStepButton_Click(object sender, EventArgs e)
        {
            //TD Lab 5 Exercise 2: Add code to make the stepper motor take one step clockwise
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {

        }

        private void ControllerModeButton_Click(object sender, EventArgs e)
        {
            if(ControllerModeButton.Text == "Step")
            {
                //TD Lab 5 Exercise 2: Add code to enter controller mode
                //This will likely involve sending a character to the firmware
                //and changing the button text to "Exit Controller Mode"
                ControllerModeButton.Text = "Continuous Speed";
            }
            else
            {
                //TD Lab 5 Exercise 2: Add code to exit controller mode
                //This will likely involve sending a character to the firmware
                //and changing the button text to "Enter Controller Mode"
                ControllerModeButton.Text = "Step";
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void ControllerModeLabel_Click(object sender, EventArgs e)
        {

        }

        private void SpeedControl_Click(object sender, EventArgs e)
        {

        }

        private void ZeroedButton_Click(object sender, EventArgs e)
        {
            speed = 0;
            trackBar1.Value = speed;
        }

        private void trackBar1_Scroll_1(object sender, EventArgs e)
        {
            speed = trackBar1.Value;
            //TD Lab 5 Exercise 2: Add code to send the speed value to the firmware
        }

    }
}
