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
            this.pictureBoxWebCam = new System.Windows.Forms.PictureBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnCanny = new System.Windows.Forms.Button();
            this.btnFPS = new System.Windows.Forms.Button();
            this.pictureBoxEffect = new System.Windows.Forms.PictureBox();
            this.btnHough = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWebCam)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxEffect)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxWebCam
            // 
            this.pictureBoxWebCam.Location = new System.Drawing.Point(97, 163);
            this.pictureBoxWebCam.Name = "pictureBoxWebCam";
            this.pictureBoxWebCam.Size = new System.Drawing.Size(661, 575);
            this.pictureBoxWebCam.TabIndex = 0;
            this.pictureBoxWebCam.TabStop = false;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(178, 12);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(219, 46);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(403, 12);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(219, 46);
            this.btnStop.TabIndex = 2;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnCanny
            // 
            this.btnCanny.Location = new System.Drawing.Point(937, 12);
            this.btnCanny.Name = "btnCanny";
            this.btnCanny.Size = new System.Drawing.Size(219, 46);
            this.btnCanny.TabIndex = 3;
            this.btnCanny.Text = "Canny";
            this.btnCanny.UseVisualStyleBackColor = true;
            this.btnCanny.Click += new System.EventHandler(this.btnCanny_Click);
            // 
            // btnFPS
            // 
            this.btnFPS.Location = new System.Drawing.Point(712, 12);
            this.btnFPS.Name = "btnFPS";
            this.btnFPS.Size = new System.Drawing.Size(219, 46);
            this.btnFPS.TabIndex = 4;
            this.btnFPS.Text = "FPS";
            this.btnFPS.UseVisualStyleBackColor = true;
            this.btnFPS.Click += new System.EventHandler(this.btnFPS_Click);
            // 
            // pictureBoxEffect
            // 
            this.pictureBoxEffect.Location = new System.Drawing.Point(866, 163);
            this.pictureBoxEffect.Name = "pictureBoxEffect";
            this.pictureBoxEffect.Size = new System.Drawing.Size(661, 575);
            this.pictureBoxEffect.TabIndex = 5;
            this.pictureBoxEffect.TabStop = false;
            // 
            // btnHough
            // 
            this.btnHough.Location = new System.Drawing.Point(1162, 12);
            this.btnHough.Name = "btnHough";
            this.btnHough.Size = new System.Drawing.Size(219, 46);
            this.btnHough.TabIndex = 6;
            this.btnHough.Text = "Hough Circle";
            this.btnHough.UseVisualStyleBackColor = true;
            this.btnHough.Click += new System.EventHandler(this.btnHough_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1691, 825);
            this.Controls.Add(this.btnHough);
            this.Controls.Add(this.pictureBoxEffect);
            this.Controls.Add(this.btnFPS);
            this.Controls.Add(this.btnCanny);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.pictureBoxWebCam);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWebCam)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxEffect)).EndInit();
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
    }
}

