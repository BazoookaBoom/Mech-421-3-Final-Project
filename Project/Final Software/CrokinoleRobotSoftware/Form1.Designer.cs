namespace CrokinoleRobotSoftware
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
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.Heartbeat = new System.Windows.Forms.Timer(this.components);
            this.camFrame = new System.Windows.Forms.PictureBox();
            this.EndTurnButt = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.Start = new System.Windows.Forms.TabPage();
            this.ButtDiffSel = new System.Windows.Forms.Button();
            this.ComboBxDiff = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.Run = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.RoundEnd = new System.Windows.Forms.TabPage();
            this.GameOver = new System.Windows.Forms.TabPage();
            this.RoundEndProceedButt = new System.Windows.Forms.Button();
            this.WhtRoundScore = new System.Windows.Forms.TextBox();
            this.BlkRoundScore = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.BlkGameScore = new System.Windows.Forms.TextBox();
            this.WhtGameScore = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.camFrame)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.Start.SuspendLayout();
            this.Run.SuspendLayout();
            this.RoundEnd.SuspendLayout();
            this.SuspendLayout();
            // 
            // serialPort1
            // 
            this.serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
            // 
            // Heartbeat
            // 
            this.Heartbeat.Interval = 200;
            this.Heartbeat.Tick += new System.EventHandler(this.Heartbeat_Tick);
            // 
            // camFrame
            // 
            this.camFrame.Location = new System.Drawing.Point(59, 178);
            this.camFrame.Name = "camFrame";
            this.camFrame.Size = new System.Drawing.Size(1280, 720);
            this.camFrame.TabIndex = 0;
            this.camFrame.TabStop = false;
            // 
            // EndTurnButt
            // 
            this.EndTurnButt.Location = new System.Drawing.Point(59, 36);
            this.EndTurnButt.Name = "EndTurnButt";
            this.EndTurnButt.Size = new System.Drawing.Size(304, 113);
            this.EndTurnButt.TabIndex = 1;
            this.EndTurnButt.Text = "End Turn";
            this.EndTurnButt.UseVisualStyleBackColor = true;
            this.EndTurnButt.Click += new System.EventHandler(this.EndTurnButt_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.Start);
            this.tabControl1.Controls.Add(this.Run);
            this.tabControl1.Controls.Add(this.RoundEnd);
            this.tabControl1.Controls.Add(this.GameOver);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1790, 1225);
            this.tabControl1.TabIndex = 2;
            // 
            // Start
            // 
            this.Start.Controls.Add(this.ButtDiffSel);
            this.Start.Controls.Add(this.ComboBxDiff);
            this.Start.Controls.Add(this.label3);
            this.Start.Location = new System.Drawing.Point(8, 39);
            this.Start.Name = "Start";
            this.Start.Padding = new System.Windows.Forms.Padding(3);
            this.Start.Size = new System.Drawing.Size(1774, 1178);
            this.Start.TabIndex = 0;
            this.Start.Text = "Start";
            this.Start.UseVisualStyleBackColor = true;
            // 
            // ButtDiffSel
            // 
            this.ButtDiffSel.Enabled = false;
            this.ButtDiffSel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ButtDiffSel.Location = new System.Drawing.Point(921, 565);
            this.ButtDiffSel.Name = "ButtDiffSel";
            this.ButtDiffSel.Size = new System.Drawing.Size(183, 86);
            this.ButtDiffSel.TabIndex = 2;
            this.ButtDiffSel.Text = "Select Difficulty";
            this.ButtDiffSel.UseVisualStyleBackColor = true;
            this.ButtDiffSel.Click += new System.EventHandler(this.ButtDiffSel_Click);
            // 
            // ComboBxDiff
            // 
            this.ComboBxDiff.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ComboBxDiff.FormattingEnabled = true;
            this.ComboBxDiff.Items.AddRange(new object[] {
            "Easy",
            "Medium",
            "Hard",
            "Just Plain Mean"});
            this.ComboBxDiff.Location = new System.Drawing.Point(658, 587);
            this.ComboBxDiff.Name = "ComboBxDiff";
            this.ComboBxDiff.Size = new System.Drawing.Size(228, 45);
            this.ComboBxDiff.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe Print", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(682, 34);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(507, 112);
            this.label3.TabIndex = 0;
            this.label3.Text = "Crokinolinator";
            // 
            // Run
            // 
            this.Run.Controls.Add(this.label2);
            this.Run.Controls.Add(this.label1);
            this.Run.Controls.Add(this.EndTurnButt);
            this.Run.Controls.Add(this.camFrame);
            this.Run.Location = new System.Drawing.Point(8, 39);
            this.Run.Name = "Run";
            this.Run.Padding = new System.Windows.Forms.Padding(3);
            this.Run.Size = new System.Drawing.Size(1774, 1178);
            this.Run.TabIndex = 1;
            this.Run.Text = "Run";
            this.Run.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(734, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(122, 55);
            this.label2.TabIndex = 3;
            this.label2.Text = "0 : 0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(534, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(122, 55);
            this.label1.TabIndex = 2;
            this.label1.Text = "0 : 0";
            // 
            // RoundEnd
            // 
            this.RoundEnd.Controls.Add(this.BlkGameScore);
            this.RoundEnd.Controls.Add(this.WhtGameScore);
            this.RoundEnd.Controls.Add(this.label7);
            this.RoundEnd.Controls.Add(this.label6);
            this.RoundEnd.Controls.Add(this.label5);
            this.RoundEnd.Controls.Add(this.label4);
            this.RoundEnd.Controls.Add(this.BlkRoundScore);
            this.RoundEnd.Controls.Add(this.WhtRoundScore);
            this.RoundEnd.Controls.Add(this.RoundEndProceedButt);
            this.RoundEnd.Location = new System.Drawing.Point(8, 39);
            this.RoundEnd.Name = "RoundEnd";
            this.RoundEnd.Padding = new System.Windows.Forms.Padding(3);
            this.RoundEnd.Size = new System.Drawing.Size(1774, 1178);
            this.RoundEnd.TabIndex = 3;
            this.RoundEnd.Text = "RoundEnd";
            this.RoundEnd.UseVisualStyleBackColor = true;
            // 
            // GameOver
            // 
            this.GameOver.Location = new System.Drawing.Point(8, 39);
            this.GameOver.Name = "GameOver";
            this.GameOver.Size = new System.Drawing.Size(1774, 1178);
            this.GameOver.TabIndex = 2;
            this.GameOver.Text = "GameOver";
            this.GameOver.UseVisualStyleBackColor = true;
            // 
            // RoundEndProceedButt
            // 
            this.RoundEndProceedButt.Location = new System.Drawing.Point(637, 275);
            this.RoundEndProceedButt.Name = "RoundEndProceedButt";
            this.RoundEndProceedButt.Size = new System.Drawing.Size(188, 125);
            this.RoundEndProceedButt.TabIndex = 3;
            this.RoundEndProceedButt.Text = "Next Round";
            this.RoundEndProceedButt.UseVisualStyleBackColor = true;
            this.RoundEndProceedButt.Click += new System.EventHandler(this.RoundEndProceedButt_Click);
            // 
            // WhtRoundScore
            // 
            this.WhtRoundScore.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WhtRoundScore.Location = new System.Drawing.Point(502, 168);
            this.WhtRoundScore.Name = "WhtRoundScore";
            this.WhtRoundScore.Size = new System.Drawing.Size(130, 44);
            this.WhtRoundScore.TabIndex = 4;
            // 
            // BlkRoundScore
            // 
            this.BlkRoundScore.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BlkRoundScore.Location = new System.Drawing.Point(1093, 172);
            this.BlkRoundScore.Name = "BlkRoundScore";
            this.BlkRoundScore.Size = new System.Drawing.Size(117, 44);
            this.BlkRoundScore.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(202, 175);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(294, 37);
            this.label4.TabIndex = 6;
            this.label4.Text = "White Round Score";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(774, 175);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(290, 37);
            this.label5.TabIndex = 7;
            this.label5.Text = "Black Round Score";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(202, 101);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(287, 37);
            this.label6.TabIndex = 8;
            this.label6.Text = "White Game Score";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(774, 101);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(283, 37);
            this.label7.TabIndex = 9;
            this.label7.Text = "Black Game Score";
            // 
            // BlkGameScore
            // 
            this.BlkGameScore.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BlkGameScore.Location = new System.Drawing.Point(1093, 102);
            this.BlkGameScore.Name = "BlkGameScore";
            this.BlkGameScore.Size = new System.Drawing.Size(117, 44);
            this.BlkGameScore.TabIndex = 11;
            // 
            // WhtGameScore
            // 
            this.WhtGameScore.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WhtGameScore.Location = new System.Drawing.Point(502, 98);
            this.WhtGameScore.Name = "WhtGameScore";
            this.WhtGameScore.Size = new System.Drawing.Size(130, 44);
            this.WhtGameScore.TabIndex = 10;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1807, 1249);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.camFrame)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.Start.ResumeLayout(false);
            this.Start.PerformLayout();
            this.Run.ResumeLayout(false);
            this.Run.PerformLayout();
            this.RoundEnd.ResumeLayout(false);
            this.RoundEnd.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.Timer Heartbeat;
        private System.Windows.Forms.PictureBox camFrame;
        private System.Windows.Forms.Button EndTurnButt;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage Start;
        private System.Windows.Forms.TabPage Run;
        private System.Windows.Forms.TabPage GameOver;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabPage RoundEnd;
        private System.Windows.Forms.Button ButtDiffSel;
        private System.Windows.Forms.ComboBox ComboBxDiff;
        private System.Windows.Forms.Button RoundEndProceedButt;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox BlkRoundScore;
        private System.Windows.Forms.TextBox WhtRoundScore;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox BlkGameScore;
        private System.Windows.Forms.TextBox WhtGameScore;
        private System.Windows.Forms.Label label7;
    }
}

