namespace Data_Infrastructure_Form
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pictureBoxWebCam = new System.Windows.Forms.PictureBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnCanny = new System.Windows.Forms.Button();
            this.btnFPS = new System.Windows.Forms.Button();
            this.pictureBoxEffect = new System.Windows.Forms.PictureBox();
            this.btnHough = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.dpdnConnect = new System.Windows.Forms.ComboBox();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtCOut = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtYOut = new System.Windows.Forms.TextBox();
            this.txtROut = new System.Windows.Forms.TextBox();
            this.txtXOut = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWebCam)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxEffect)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBoxWebCam
            // 
            this.pictureBoxWebCam.Location = new System.Drawing.Point(96, 325);
            this.pictureBoxWebCam.Name = "pictureBoxWebCam";
            this.pictureBoxWebCam.Size = new System.Drawing.Size(1464, 1005);
            this.pictureBoxWebCam.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxWebCam.TabIndex = 0;
            this.pictureBoxWebCam.TabStop = false;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(21, 30);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(219, 46);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(246, 30);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(219, 46);
            this.btnStop.TabIndex = 2;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnCanny
            // 
            this.btnCanny.Location = new System.Drawing.Point(780, 30);
            this.btnCanny.Name = "btnCanny";
            this.btnCanny.Size = new System.Drawing.Size(219, 46);
            this.btnCanny.TabIndex = 3;
            this.btnCanny.Text = "Canny";
            this.btnCanny.UseVisualStyleBackColor = true;
            this.btnCanny.Click += new System.EventHandler(this.btnCanny_Click);
            // 
            // btnFPS
            // 
            this.btnFPS.Location = new System.Drawing.Point(555, 30);
            this.btnFPS.Name = "btnFPS";
            this.btnFPS.Size = new System.Drawing.Size(219, 46);
            this.btnFPS.TabIndex = 4;
            this.btnFPS.Text = "FPS";
            this.btnFPS.UseVisualStyleBackColor = true;
            this.btnFPS.Click += new System.EventHandler(this.btnFPS_Click);
            // 
            // pictureBoxEffect
            // 
            this.pictureBoxEffect.Location = new System.Drawing.Point(1584, 325);
            this.pictureBoxEffect.Name = "pictureBoxEffect";
            this.pictureBoxEffect.Size = new System.Drawing.Size(1398, 1005);
            this.pictureBoxEffect.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxEffect.TabIndex = 5;
            this.pictureBoxEffect.TabStop = false;
            // 
            // btnHough
            // 
            this.btnHough.Location = new System.Drawing.Point(1005, 30);
            this.btnHough.Name = "btnHough";
            this.btnHough.Size = new System.Drawing.Size(219, 46);
            this.btnHough.TabIndex = 6;
            this.btnHough.Text = "Hough Circle";
            this.btnHough.UseVisualStyleBackColor = true;
            this.btnHough.Click += new System.EventHandler(this.btnHough_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnStart);
            this.groupBox1.Controls.Add(this.btnHough);
            this.groupBox1.Controls.Add(this.btnStop);
            this.groupBox1.Controls.Add(this.btnCanny);
            this.groupBox1.Controls.Add(this.btnFPS);
            this.groupBox1.Location = new System.Drawing.Point(96, 206);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(2250, 100);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Controls";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnConnect);
            this.groupBox2.Controls.Add(this.dpdnConnect);
            this.groupBox2.Location = new System.Drawing.Point(96, 46);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(2250, 142);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Setup";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(246, 41);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(219, 44);
            this.btnConnect.TabIndex = 10;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // dpdnConnect
            // 
            this.dpdnConnect.FormattingEnabled = true;
            this.dpdnConnect.Location = new System.Drawing.Point(32, 48);
            this.dpdnConnect.Name = "dpdnConnect";
            this.dpdnConnect.Size = new System.Drawing.Size(208, 33);
            this.dpdnConnect.TabIndex = 9;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.txtCOut);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.txtYOut);
            this.groupBox3.Controls.Add(this.txtROut);
            this.groupBox3.Controls.Add(this.txtXOut);
            this.groupBox3.Location = new System.Drawing.Point(96, 1336);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(661, 237);
            this.groupBox3.TabIndex = 11;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Outputs";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(27, 156);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 25);
            this.label4.TabIndex = 18;
            this.label4.Text = "Colour";
            // 
            // txtCOut
            // 
            this.txtCOut.Location = new System.Drawing.Point(140, 150);
            this.txtCOut.Name = "txtCOut";
            this.txtCOut.Size = new System.Drawing.Size(100, 31);
            this.txtCOut.TabIndex = 17;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(27, 119);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 25);
            this.label3.TabIndex = 16;
            this.label3.Text = "Radius";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 82);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 25);
            this.label2.TabIndex = 15;
            this.label2.Text = "YCoord";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 25);
            this.label1.TabIndex = 14;
            this.label1.Text = "XCoord";
            // 
            // txtYOut
            // 
            this.txtYOut.Location = new System.Drawing.Point(140, 76);
            this.txtYOut.Name = "txtYOut";
            this.txtYOut.Size = new System.Drawing.Size(100, 31);
            this.txtYOut.TabIndex = 13;
            // 
            // txtROut
            // 
            this.txtROut.Location = new System.Drawing.Point(140, 113);
            this.txtROut.Name = "txtROut";
            this.txtROut.Size = new System.Drawing.Size(100, 31);
            this.txtROut.TabIndex = 12;
            // 
            // txtXOut
            // 
            this.txtXOut.Location = new System.Drawing.Point(140, 39);
            this.txtXOut.Name = "txtXOut";
            this.txtXOut.Size = new System.Drawing.Size(100, 31);
            this.txtXOut.TabIndex = 11;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(3020, 1612);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pictureBoxEffect);
            this.Controls.Add(this.pictureBoxWebCam);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWebCam)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxEffect)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxWebCam;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnCanny;
        private System.Windows.Forms.Button btnFPS;
        private System.Windows.Forms.PictureBox pictureBoxEffect;
        private System.Windows.Forms.Button btnHough;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.ComboBox dpdnConnect;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtYOut;
        private System.Windows.Forms.TextBox txtROut;
        private System.Windows.Forms.TextBox txtXOut;
        private System.Windows.Forms.TextBox txtCOut;
        private System.Windows.Forms.Label label4;
    }
}

