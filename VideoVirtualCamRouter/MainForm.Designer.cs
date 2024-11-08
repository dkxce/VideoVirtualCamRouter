namespace VideoVirtualCamRouter
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.bgImage = new System.Windows.Forms.PictureBox();
            this.bgFile = new System.Windows.Forms.TextBox();
            this.chbBG = new System.Windows.Forms.CheckBox();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.label4 = new System.Windows.Forms.Label();
            this.textColor = new System.Windows.Forms.TextBox();
            this.pColBG = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.chromaBox = new System.Windows.Forms.ComboBox();
            this.chbCK = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.velBar = new System.Windows.Forms.TrackBar();
            this.label7 = new System.Windows.Forms.Label();
            this.trsBar = new System.Windows.Forms.TrackBar();
            this.velVal = new System.Windows.Forms.Label();
            this.trsVal = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button3 = new System.Windows.Forms.Button();
            this.fpsEdit = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.heEdit = new System.Windows.Forms.NumericUpDown();
            this.wiEdit = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.destCamBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.sourceCamsBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ssBtn = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.chbOvl = new System.Windows.Forms.CheckBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.cSize = new System.Windows.Forms.ToolStripStatusLabel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.previewBox = new System.Windows.Forms.PictureBox();
            this.label12 = new System.Windows.Forms.Label();
            this.chbPreview = new System.Windows.Forms.CheckBox();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.chbMJpeg = new System.Windows.Forms.CheckBox();
            this.label13 = new System.Windows.Forms.Label();
            this.mjPort = new System.Windows.Forms.NumericUpDown();
            this.button4 = new System.Windows.Forms.Button();
            this.chbUVC = new System.Windows.Forms.CheckBox();
            this.label14 = new System.Windows.Forms.Label();
            this.methBox = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button5 = new System.Windows.Forms.Button();
            this.label16 = new System.Windows.Forms.Label();
            this.delayEdit = new System.Windows.Forms.NumericUpDown();
            this.label17 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.bgImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.velBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trsBar)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fpsEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.heEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.wiEdit)).BeginInit();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.previewBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mjPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.delayEdit)).BeginInit();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 126);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "Background:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(540, 124);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "Load ...";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // bgImage
            // 
            this.bgImage.BackColor = System.Drawing.Color.Black;
            this.bgImage.Location = new System.Drawing.Point(624, 126);
            this.bgImage.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.bgImage.Name = "bgImage";
            this.bgImage.Size = new System.Drawing.Size(157, 128);
            this.bgImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.bgImage.TabIndex = 6;
            this.bgImage.TabStop = false;
            this.bgImage.Click += new System.EventHandler(this.bgImage_Click);
            // 
            // bgFile
            // 
            this.bgFile.Location = new System.Drawing.Point(113, 126);
            this.bgFile.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.bgFile.Name = "bgFile";
            this.bgFile.Size = new System.Drawing.Size(399, 22);
            this.bgFile.TabIndex = 7;
            this.bgFile.Text = "C:\\Disk2\\Images\\il_1588xN.4792548473_b52j_640x480.png";
            this.bgFile.TextChanged += new System.EventHandler(this.bgFile_TextChanged);
            // 
            // chbBG
            // 
            this.chbBG.AutoSize = true;
            this.chbBG.Location = new System.Drawing.Point(517, 128);
            this.chbBG.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chbBG.Name = "chbBG";
            this.chbBG.Size = new System.Drawing.Size(18, 17);
            this.chbBG.TabIndex = 8;
            this.chbBG.UseVisualStyleBackColor = true;
            this.chbBG.CheckedChanged += new System.EventHandler(this.chbBG_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(21, 160);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 16);
            this.label4.TabIndex = 9;
            this.label4.Text = "BG Color:";
            // 
            // textColor
            // 
            this.textColor.Location = new System.Drawing.Point(113, 158);
            this.textColor.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textColor.Name = "textColor";
            this.textColor.ReadOnly = true;
            this.textColor.Size = new System.Drawing.Size(327, 22);
            this.textColor.TabIndex = 10;
            this.textColor.Text = "Black";
            // 
            // pColBG
            // 
            this.pColBG.BackColor = System.Drawing.Color.Black;
            this.pColBG.Location = new System.Drawing.Point(445, 158);
            this.pColBG.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pColBG.Name = "pColBG";
            this.pColBG.Size = new System.Drawing.Size(91, 23);
            this.pColBG.TabIndex = 11;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(540, 158);
            this.button2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 12;
            this.button2.Text = "Select ...";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(21, 231);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(79, 16);
            this.label5.TabIndex = 13;
            this.label5.Text = "Chromakey:";
            // 
            // chromaBox
            // 
            this.chromaBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.chromaBox.FormattingEnabled = true;
            this.chromaBox.Items.AddRange(new object[] {
            "Red",
            "Green",
            "Blue"});
            this.chromaBox.Location = new System.Drawing.Point(113, 229);
            this.chromaBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chromaBox.Name = "chromaBox";
            this.chromaBox.Size = new System.Drawing.Size(131, 24);
            this.chromaBox.TabIndex = 14;
            this.chromaBox.SelectedIndexChanged += new System.EventHandler(this.chromaBox_SelectedIndexChanged);
            // 
            // chbCK
            // 
            this.chbCK.AutoSize = true;
            this.chbCK.Location = new System.Drawing.Point(251, 233);
            this.chbCK.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chbCK.Name = "chbCK";
            this.chbCK.Size = new System.Drawing.Size(18, 17);
            this.chbCK.TabIndex = 15;
            this.chbCK.UseVisualStyleBackColor = true;
            this.chbCK.CheckedChanged += new System.EventHandler(this.chbCK_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(23, 267);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(82, 16);
            this.label6.TabIndex = 16;
            this.label6.Text = "Min treshold:";
            // 
            // velBar
            // 
            this.velBar.Location = new System.Drawing.Point(107, 258);
            this.velBar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.velBar.Maximum = 255;
            this.velBar.Name = "velBar";
            this.velBar.Size = new System.Drawing.Size(675, 56);
            this.velBar.TabIndex = 17;
            this.velBar.Value = 8;
            this.velBar.Scroll += new System.EventHandler(this.velBar_Scroll);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(23, 318);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(86, 16);
            this.label7.TabIndex = 18;
            this.label7.Text = "Max treshold:";
            // 
            // trsBar
            // 
            this.trsBar.Location = new System.Drawing.Point(107, 309);
            this.trsBar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.trsBar.Maximum = 255;
            this.trsBar.Name = "trsBar";
            this.trsBar.Size = new System.Drawing.Size(675, 56);
            this.trsBar.TabIndex = 19;
            this.trsBar.Value = 60;
            this.trsBar.Scroll += new System.EventHandler(this.trsBar_Scroll);
            // 
            // velVal
            // 
            this.velVal.AutoSize = true;
            this.velVal.Location = new System.Drawing.Point(47, 286);
            this.velVal.Name = "velVal";
            this.velVal.Size = new System.Drawing.Size(14, 16);
            this.velVal.TabIndex = 20;
            this.velVal.Text = "8";
            // 
            // trsVal
            // 
            this.trsVal.AutoSize = true;
            this.trsVal.Location = new System.Drawing.Point(47, 335);
            this.trsVal.Name = "trsVal";
            this.trsVal.Size = new System.Drawing.Size(21, 16);
            this.trsVal.TabIndex = 21;
            this.trsVal.Text = "60";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.fpsEdit);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.heEdit);
            this.panel1.Controls.Add(this.wiEdit);
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.destCamBox);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.sourceCamsBox);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(11, 9);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(783, 110);
            this.panel1.TabIndex = 24;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(695, 41);
            this.button3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 38;
            this.button3.Text = "Setup";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // fpsEdit
            // 
            this.fpsEdit.Location = new System.Drawing.Point(695, 76);
            this.fpsEdit.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.fpsEdit.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.fpsEdit.Minimum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.fpsEdit.Name = "fpsEdit";
            this.fpsEdit.Size = new System.Drawing.Size(76, 22);
            this.fpsEdit.TabIndex = 37;
            this.fpsEdit.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(653, 78);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(36, 16);
            this.label8.TabIndex = 36;
            this.label8.Text = "FPS:";
            // 
            // heEdit
            // 
            this.heEdit.Location = new System.Drawing.Point(297, 75);
            this.heEdit.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.heEdit.Maximum = new decimal(new int[] {
            1200,
            0,
            0,
            0});
            this.heEdit.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.heEdit.Name = "heEdit";
            this.heEdit.Size = new System.Drawing.Size(75, 22);
            this.heEdit.TabIndex = 35;
            this.heEdit.Value = new decimal(new int[] {
            480,
            0,
            0,
            0});
            // 
            // wiEdit
            // 
            this.wiEdit.Location = new System.Drawing.Point(103, 76);
            this.wiEdit.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.wiEdit.Maximum = new decimal(new int[] {
            1200,
            0,
            0,
            0});
            this.wiEdit.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.wiEdit.Name = "wiEdit";
            this.wiEdit.Size = new System.Drawing.Size(79, 22);
            this.wiEdit.TabIndex = 34;
            this.wiEdit.Value = new decimal(new int[] {
            720,
            0,
            0,
            0});
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(197, 78);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(91, 16);
            this.label10.TabIndex = 33;
            this.label10.Text = "Default height:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 78);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(85, 16);
            this.label9.TabIndex = 32;
            this.label9.Text = "Default width:";
            // 
            // destCamBox
            // 
            this.destCamBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.destCamBox.FormattingEnabled = true;
            this.destCamBox.Location = new System.Drawing.Point(103, 39);
            this.destCamBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.destCamBox.Name = "destCamBox";
            this.destCamBox.Size = new System.Drawing.Size(585, 24);
            this.destCamBox.TabIndex = 31;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 16);
            this.label2.TabIndex = 30;
            this.label2.Text = "Destination:";
            // 
            // sourceCamsBox
            // 
            this.sourceCamsBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sourceCamsBox.FormattingEnabled = true;
            this.sourceCamsBox.Location = new System.Drawing.Point(103, 2);
            this.sourceCamsBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.sourceCamsBox.Name = "sourceCamsBox";
            this.sourceCamsBox.Size = new System.Drawing.Size(668, 24);
            this.sourceCamsBox.TabIndex = 29;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 16);
            this.label1.TabIndex = 28;
            this.label1.Text = "Source:";
            // 
            // ssBtn
            // 
            this.ssBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.6F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ssBtn.Location = new System.Drawing.Point(668, 352);
            this.ssBtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ssBtn.Name = "ssBtn";
            this.ssBtn.Size = new System.Drawing.Size(113, 38);
            this.ssBtn.TabIndex = 25;
            this.ssBtn.Text = "Start";
            this.ssBtn.UseVisualStyleBackColor = true;
            this.ssBtn.Click += new System.EventHandler(this.ssBtn_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(21, 193);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(53, 16);
            this.label11.TabIndex = 26;
            this.label11.Text = "Ovelay:";
            // 
            // chbOvl
            // 
            this.chbOvl.AutoSize = true;
            this.chbOvl.Location = new System.Drawing.Point(113, 194);
            this.chbOvl.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chbOvl.Name = "chbOvl";
            this.chbOvl.Size = new System.Drawing.Size(18, 17);
            this.chbOvl.TabIndex = 27;
            this.chbOvl.UseVisualStyleBackColor = true;
            this.chbOvl.CheckedChanged += new System.EventHandler(this.chbOvl_CheckedChanged);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.cSize});
            this.statusStrip1.Location = new System.Drawing.Point(0, 399);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 13, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1261, 26);
            this.statusStrip1.TabIndex = 28;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(89, 20);
            this.toolStripStatusLabel1.Text = "Current size:";
            // 
            // cSize
            // 
            this.cSize.Name = "cSize";
            this.cSize.Size = new System.Drawing.Size(48, 20);
            this.cSize.Text = "-- x --";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 2000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // previewBox
            // 
            this.previewBox.BackColor = System.Drawing.Color.Black;
            this.previewBox.Location = new System.Drawing.Point(811, 52);
            this.previewBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.previewBox.Name = "previewBox";
            this.previewBox.Size = new System.Drawing.Size(436, 326);
            this.previewBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.previewBox.TabIndex = 29;
            this.previewBox.TabStop = false;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(835, 17);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(58, 16);
            this.label12.TabIndex = 30;
            this.label12.Text = "Preview:";
            // 
            // chbPreview
            // 
            this.chbPreview.AutoSize = true;
            this.chbPreview.Location = new System.Drawing.Point(811, 17);
            this.chbPreview.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chbPreview.Name = "chbPreview";
            this.chbPreview.Size = new System.Drawing.Size(18, 17);
            this.chbPreview.TabIndex = 31;
            this.chbPreview.UseVisualStyleBackColor = true;
            this.chbPreview.CheckedChanged += new System.EventHandler(this.chbPreview_CheckedChanged);
            // 
            // timer2
            // 
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // chbMJpeg
            // 
            this.chbMJpeg.AutoSize = true;
            this.chbMJpeg.Enabled = false;
            this.chbMJpeg.Location = new System.Drawing.Point(113, 366);
            this.chbMJpeg.Margin = new System.Windows.Forms.Padding(4);
            this.chbMJpeg.Name = "chbMJpeg";
            this.chbMJpeg.Size = new System.Drawing.Size(18, 17);
            this.chbMJpeg.TabIndex = 32;
            this.chbMJpeg.UseVisualStyleBackColor = true;
            this.chbMJpeg.CheckedChanged += new System.EventHandler(this.chbMJpeg_CheckedChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(23, 364);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(56, 16);
            this.label13.TabIndex = 33;
            this.label13.Text = "MJPEG:";
            // 
            // mjPort
            // 
            this.mjPort.Location = new System.Drawing.Point(144, 364);
            this.mjPort.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.mjPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.mjPort.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.mjPort.Name = "mjPort";
            this.mjPort.Size = new System.Drawing.Size(79, 22);
            this.mjPort.TabIndex = 35;
            this.mjPort.Value = new decimal(new int[] {
            7999,
            0,
            0,
            0});
            // 
            // button4
            // 
            this.button4.Enabled = false;
            this.button4.Location = new System.Drawing.Point(228, 363);
            this.button4.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 25);
            this.button4.TabIndex = 36;
            this.button4.Text = "open";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // chbUVC
            // 
            this.chbUVC.AutoSize = true;
            this.chbUVC.Checked = true;
            this.chbUVC.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbUVC.Location = new System.Drawing.Point(1065, 17);
            this.chbUVC.Margin = new System.Windows.Forms.Padding(4);
            this.chbUVC.Name = "chbUVC";
            this.chbUVC.Size = new System.Drawing.Size(145, 20);
            this.chbUVC.TabIndex = 37;
            this.chbUVC.Text = "Pass to Virtual Cam";
            this.chbUVC.UseVisualStyleBackColor = true;
            this.chbUVC.CheckedChanged += new System.EventHandler(this.chbUVC_CheckedChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(310, 367);
            this.label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(205, 16);
            this.label14.TabIndex = 38;
            this.label14.Text = "(Danger: It takes a lot of memory!)";
            // 
            // methBox
            // 
            this.methBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.methBox.FormattingEnabled = true;
            this.methBox.Items.AddRange(new object[] {
            "RGB",
            "YCbCr",
            "RGB 3D",
            "Grayscale",
            "Colour metric",
            "ΔE (CIE 1976)",
            "ΔE (CIE 2000)",
            "ΔE (CMC)",
            "HSV Hue",
            "HSV Full",
            "HSV Hue (R)",
            "HSV Full (R)"});
            this.methBox.Location = new System.Drawing.Point(347, 228);
            this.methBox.Name = "methBox";
            this.methBox.Size = new System.Drawing.Size(104, 24);
            this.methBox.TabIndex = 39;
            this.methBox.SelectedIndexChanged += new System.EventHandler(this.methBox_SelectedIndexChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(286, 233);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(55, 16);
            this.label15.TabIndex = 40;
            this.label15.Text = "Method:";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(116)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.panel2.Location = new System.Drawing.Point(541, 228);
            this.panel2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(30, 23);
            this.panel2.TabIndex = 41;
            this.panel2.Visible = false;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(577, 227);
            this.button5.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(41, 24);
            this.button5.TabIndex = 42;
            this.button5.Text = "...";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Visible = false;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(459, 232);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(78, 16);
            this.label16.TabIndex = 43;
            this.label16.Text = "Mask Color:";
            this.label16.Visible = false;
            // 
            // delayEdit
            // 
            this.delayEdit.Location = new System.Drawing.Point(536, 191);
            this.delayEdit.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.delayEdit.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.delayEdit.Name = "delayEdit";
            this.delayEdit.Size = new System.Drawing.Size(79, 22);
            this.delayEdit.TabIndex = 44;
            this.delayEdit.ValueChanged += new System.EventHandler(this.delayEdit_ValueChanged);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(442, 193);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(75, 16);
            this.label17.TabIndex = 45;
            this.label17.Text = "Delay (ms):";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1261, 425);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.delayEdit);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.methBox);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.chbUVC);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.mjPort);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.chbMJpeg);
            this.Controls.Add(this.chbPreview);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.previewBox);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.chbOvl);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.ssBtn);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.trsVal);
            this.Controls.Add(this.velVal);
            this.Controls.Add(this.trsBar);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.velBar);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.chbCK);
            this.Controls.Add(this.chromaBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.pColBG);
            this.Controls.Add(this.textColor);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.chbBG);
            this.Controls.Add(this.bgFile);
            this.Controls.Add(this.bgImage);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Virtual Video Cam Router by dkxce";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.bgImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.velBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trsBar)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fpsEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.heEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.wiEdit)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.previewBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mjPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.delayEdit)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox bgImage;
        private System.Windows.Forms.TextBox bgFile;
        private System.Windows.Forms.CheckBox chbBG;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textColor;
        private System.Windows.Forms.Panel pColBG;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox chromaBox;
        private System.Windows.Forms.CheckBox chbCK;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TrackBar velBar;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TrackBar trsBar;
        private System.Windows.Forms.Label velVal;
        private System.Windows.Forms.Label trsVal;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.NumericUpDown heEdit;
        private System.Windows.Forms.NumericUpDown wiEdit;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox destCamBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox sourceCamsBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ssBtn;
        private System.Windows.Forms.NumericUpDown fpsEdit;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox chbOvl;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel cSize;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.PictureBox previewBox;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.CheckBox chbPreview;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.CheckBox chbMJpeg;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.NumericUpDown mjPort;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.CheckBox chbUVC;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.ComboBox methBox;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.NumericUpDown delayEdit;
        private System.Windows.Forms.Label label17;
    }
}

