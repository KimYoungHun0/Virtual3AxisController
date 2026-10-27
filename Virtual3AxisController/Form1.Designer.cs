namespace Virtual3AxisController
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            txtIp = new TextBox();
            txtPort = new TextBox();
            btnConnect = new Button();
            lblStatus = new Label();
            lblMachineState = new Label();
            btnReset = new Button();
            btnHome = new Button();
            btnAutoStart = new Button();
            btnAutoStop = new Button();
            btnEMO = new Button();
            lblDoorSensor = new Label();
            grpJog = new GroupBox();
            lblPosX = new Label();
            lblPosY = new Label();
            lblPosZ = new Label();
            btnJogXPlus = new Button();
            btnJogXMinus = new Button();
            btnJogYPlus = new Button();
            btnJogYMinus = new Button();
            btnJogZPlus = new Button();
            btnJogZMinus = new Button();
            grpDashboard = new GroupBox();
            lblTotal = new Label();
            lblOk = new Label();
            lblNg = new Label();
            lblYield = new Label();
            dgvAlarmLog = new DataGridView();
            colTime = new DataGridViewTextBoxColumn();
            colCode = new DataGridViewTextBoxColumn();
            colType = new DataGridViewTextBoxColumn();
            colMessage = new DataGridViewTextBoxColumn();
            grpVision = new GroupBox();
            picWebcam = new PictureBox();
            btnCameraOn = new Button();
            btnCameraOff = new Button();
            grpJog.SuspendLayout();
            grpDashboard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvAlarmLog).BeginInit();
            grpVision.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picWebcam).BeginInit();
            SuspendLayout();
            // 
            // txtIp
            // 
            txtIp.Location = new Point(20, 20);
            txtIp.Name = "txtIp";
            txtIp.Size = new Size(120, 27);
            txtIp.TabIndex = 0;
            txtIp.Text = "127.0.0.1";
            // 
            // txtPort
            // 
            txtPort.Location = new Point(150, 20);
            txtPort.Name = "txtPort";
            txtPort.Size = new Size(60, 27);
            txtPort.TabIndex = 1;
            txtPort.Text = "502";
            // 
            // btnConnect
            // 
            btnConnect.Location = new Point(220, 19);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(90, 25);
            btnConnect.TabIndex = 2;
            btnConnect.Text = "통신 연결";
            btnConnect.UseVisualStyleBackColor = true;
            btnConnect.Click += btnConnect_Click;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Font = new Font("Malgun Gothic", 9.75F, FontStyle.Bold);
            lblStatus.ForeColor = Color.Gray;
            lblStatus.Location = new Point(20, 99);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(151, 23);
            lblStatus.TabIndex = 3;
            lblStatus.Text = "상태: 연결 대기 중";
            // 
            // lblMachineState
            // 
            lblMachineState.AutoSize = true;
            lblMachineState.Font = new Font("Malgun Gothic", 14F, FontStyle.Bold);
            lblMachineState.ForeColor = Color.Blue;
            lblMachineState.Location = new Point(20, 55);
            lblMachineState.Name = "lblMachineState";
            lblMachineState.Size = new Size(126, 32);
            lblMachineState.TabIndex = 4;
            lblMachineState.Text = "상태: IDLE";
            // 
            // btnReset
            // 
            btnReset.BackColor = Color.Orange;
            btnReset.Font = new Font("Malgun Gothic", 10F, FontStyle.Bold);
            btnReset.Location = new Point(509, 56);
            btnReset.Name = "btnReset";
            btnReset.Size = new Size(100, 35);
            btnReset.TabIndex = 11;
            btnReset.Text = "초기화(RESET)";
            btnReset.UseVisualStyleBackColor = false;
            btnReset.Click += btnReset_Click;
            // 
            // btnHome
            // 
            btnHome.BackColor = Color.LightYellow;
            btnHome.Font = new Font("Malgun Gothic", 10F, FontStyle.Bold);
            btnHome.Location = new Point(180, 56);
            btnHome.Name = "btnHome";
            btnHome.Size = new Size(100, 35);
            btnHome.TabIndex = 5;
            btnHome.Text = "원점 (HOME)";
            btnHome.UseVisualStyleBackColor = false;
            btnHome.Click += btnHome_Click;
            // 
            // btnAutoStart
            // 
            btnAutoStart.BackColor = Color.PaleGreen;
            btnAutoStart.Font = new Font("Malgun Gothic", 10F, FontStyle.Bold);
            btnAutoStart.Location = new Point(286, 56);
            btnAutoStart.Name = "btnAutoStart";
            btnAutoStart.Size = new Size(115, 35);
            btnAutoStart.TabIndex = 6;
            btnAutoStart.Text = "자동운전(START)";
            btnAutoStart.UseVisualStyleBackColor = false;
            btnAutoStart.Click += btnAutoStart_Click;
            // 
            // btnAutoStop
            // 
            btnAutoStop.BackColor = Color.Salmon;
            btnAutoStop.Font = new Font("Malgun Gothic", 10F, FontStyle.Bold);
            btnAutoStop.Location = new Point(408, 56);
            btnAutoStop.Name = "btnAutoStop";
            btnAutoStop.Size = new Size(95, 35);
            btnAutoStop.TabIndex = 7;
            btnAutoStop.Text = "정지(STOP)";
            btnAutoStop.UseVisualStyleBackColor = false;
            btnAutoStop.Click += btnAutoStop_Click;
            // 
            // btnEMO
            // 
            btnEMO.BackColor = Color.Red;
            btnEMO.Font = new Font("Malgun Gothic", 13F, FontStyle.Bold);
            btnEMO.ForeColor = Color.White;
            btnEMO.Location = new Point(640, 21);
            btnEMO.Name = "btnEMO";
            btnEMO.Size = new Size(124, 70);
            btnEMO.TabIndex = 9;
            btnEMO.Text = "EMO\r\n비상정지";
            btnEMO.UseVisualStyleBackColor = false;
            btnEMO.Click += btnEMO_Click;
            // 
            // lblDoorSensor
            // 
            lblDoorSensor.BackColor = Color.Lime;
            lblDoorSensor.BorderStyle = BorderStyle.FixedSingle;
            lblDoorSensor.Font = new Font("Malgun Gothic", 11F, FontStyle.Bold);
            lblDoorSensor.Location = new Point(331, 13);
            lblDoorSensor.Name = "lblDoorSensor";
            lblDoorSensor.Size = new Size(105, 35);
            lblDoorSensor.TabIndex = 10;
            lblDoorSensor.Text = "도어 닫힘";
            lblDoorSensor.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // grpJog
            // 
            grpJog.Controls.Add(lblPosX);
            grpJog.Controls.Add(lblPosY);
            grpJog.Controls.Add(lblPosZ);
            grpJog.Controls.Add(btnJogXPlus);
            grpJog.Controls.Add(btnJogXMinus);
            grpJog.Controls.Add(btnJogYPlus);
            grpJog.Controls.Add(btnJogYMinus);
            grpJog.Controls.Add(btnJogZPlus);
            grpJog.Controls.Add(btnJogZMinus);
            grpJog.Location = new Point(20, 150);
            grpJog.Name = "grpJog";
            grpJog.Size = new Size(400, 185);
            grpJog.TabIndex = 8;
            grpJog.TabStop = false;
            grpJog.Text = "3축 수동 제어 (JOG)";
            // 
            // lblPosX
            // 
            lblPosX.AutoSize = true;
            lblPosX.Font = new Font("Malgun Gothic", 11F, FontStyle.Bold);
            lblPosX.Location = new Point(20, 35);
            lblPosX.Name = "lblPosX";
            lblPosX.Size = new Size(109, 25);
            lblPosX.TabIndex = 0;
            lblPosX.Text = "X축: 0 mm";
            // 
            // lblPosY
            // 
            lblPosY.AutoSize = true;
            lblPosY.Font = new Font("Malgun Gothic", 11F, FontStyle.Bold);
            lblPosY.Location = new Point(20, 80);
            lblPosY.Name = "lblPosY";
            lblPosY.Size = new Size(109, 25);
            lblPosY.TabIndex = 3;
            lblPosY.Text = "Y축: 0 mm";
            // 
            // lblPosZ
            // 
            lblPosZ.AutoSize = true;
            lblPosZ.Font = new Font("Malgun Gothic", 11F, FontStyle.Bold);
            lblPosZ.Location = new Point(20, 125);
            lblPosZ.Name = "lblPosZ";
            lblPosZ.Size = new Size(109, 25);
            lblPosZ.TabIndex = 6;
            lblPosZ.Text = "Z축: 0 mm";
            // 
            // btnJogXPlus
            // 
            btnJogXPlus.Location = new Point(180, 30);
            btnJogXPlus.Name = "btnJogXPlus";
            btnJogXPlus.Size = new Size(80, 35);
            btnJogXPlus.TabIndex = 1;
            btnJogXPlus.Text = "X +";
            // 
            // btnJogXMinus
            // 
            btnJogXMinus.Location = new Point(280, 30);
            btnJogXMinus.Name = "btnJogXMinus";
            btnJogXMinus.Size = new Size(80, 35);
            btnJogXMinus.TabIndex = 2;
            btnJogXMinus.Text = "X -";
            // 
            // btnJogYPlus
            // 
            btnJogYPlus.Location = new Point(180, 75);
            btnJogYPlus.Name = "btnJogYPlus";
            btnJogYPlus.Size = new Size(80, 35);
            btnJogYPlus.TabIndex = 4;
            btnJogYPlus.Text = "Y +";
            // 
            // btnJogYMinus
            // 
            btnJogYMinus.Location = new Point(280, 75);
            btnJogYMinus.Name = "btnJogYMinus";
            btnJogYMinus.Size = new Size(80, 35);
            btnJogYMinus.TabIndex = 5;
            btnJogYMinus.Text = "Y -";
            // 
            // btnJogZPlus
            // 
            btnJogZPlus.Location = new Point(180, 120);
            btnJogZPlus.Name = "btnJogZPlus";
            btnJogZPlus.Size = new Size(80, 35);
            btnJogZPlus.TabIndex = 7;
            btnJogZPlus.Text = "Z +";
            // 
            // btnJogZMinus
            // 
            btnJogZMinus.Location = new Point(280, 120);
            btnJogZMinus.Name = "btnJogZMinus";
            btnJogZMinus.Size = new Size(80, 35);
            btnJogZMinus.TabIndex = 8;
            btnJogZMinus.Text = "Z -";
            // 
            // grpDashboard
            // 
            grpDashboard.Controls.Add(lblTotal);
            grpDashboard.Controls.Add(lblOk);
            grpDashboard.Controls.Add(lblNg);
            grpDashboard.Controls.Add(lblYield);
            grpDashboard.Location = new Point(440, 150);
            grpDashboard.Name = "grpDashboard";
            grpDashboard.Size = new Size(180, 185);
            grpDashboard.TabIndex = 13;
            grpDashboard.TabStop = false;
            grpDashboard.Text = "당일 생산 현황 (MES)";
            // 
            // lblTotal
            // 
            lblTotal.AutoSize = true;
            lblTotal.Font = new Font("Malgun Gothic", 11F, FontStyle.Bold);
            lblTotal.Location = new Point(15, 35);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new Size(125, 25);
            lblTotal.TabIndex = 0;
            lblTotal.Text = "총 생산: 0 개";
            // 
            // lblOk
            // 
            lblOk.AutoSize = true;
            lblOk.Font = new Font("Malgun Gothic", 11F, FontStyle.Bold);
            lblOk.ForeColor = Color.Green;
            lblOk.Location = new Point(15, 70);
            lblOk.Name = "lblOk";
            lblOk.Size = new Size(99, 25);
            lblOk.TabIndex = 1;
            lblOk.Text = "양품: 0 개";
            // 
            // lblNg
            // 
            lblNg.AutoSize = true;
            lblNg.Font = new Font("Malgun Gothic", 11F, FontStyle.Bold);
            lblNg.ForeColor = Color.Red;
            lblNg.Location = new Point(15, 105);
            lblNg.Name = "lblNg";
            lblNg.Size = new Size(99, 25);
            lblNg.TabIndex = 2;
            lblNg.Text = "불량: 0 개";
            // 
            // lblYield
            // 
            lblYield.AutoSize = true;
            lblYield.Font = new Font("Malgun Gothic", 12F, FontStyle.Bold);
            lblYield.ForeColor = Color.Blue;
            lblYield.Location = new Point(15, 140);
            lblYield.Name = "lblYield";
            lblYield.Size = new Size(118, 28);
            lblYield.TabIndex = 3;
            lblYield.Text = "수율: 0.0 %";
            // 
            // dgvAlarmLog
            // 
            dgvAlarmLog.AllowUserToAddRows = false;
            dgvAlarmLog.AllowUserToDeleteRows = false;
            dgvAlarmLog.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvAlarmLog.Columns.AddRange(new DataGridViewColumn[] { colTime, colCode, colType, colMessage });
            dgvAlarmLog.Location = new Point(20, 350);
            dgvAlarmLog.MultiSelect = false;
            dgvAlarmLog.Name = "dgvAlarmLog";
            dgvAlarmLog.ReadOnly = true;
            dgvAlarmLog.RowHeadersVisible = false;
            dgvAlarmLog.RowHeadersWidth = 51;
            dgvAlarmLog.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAlarmLog.Size = new Size(600, 110);
            dgvAlarmLog.TabIndex = 12;
            // 
            // colTime
            // 
            colTime.HeaderText = "발생 일시";
            colTime.MinimumWidth = 6;
            colTime.Name = "colTime";
            colTime.ReadOnly = true;
            colTime.Width = 140;
            // 
            // colCode
            // 
            colCode.HeaderText = "코드";
            colCode.MinimumWidth = 6;
            colCode.Name = "colCode";
            colCode.ReadOnly = true;
            colCode.Width = 70;
            // 
            // colType
            // 
            colType.HeaderText = "구분";
            colType.MinimumWidth = 6;
            colType.Name = "colType";
            colType.ReadOnly = true;
            colType.Width = 80;
            // 
            // colMessage
            // 
            colMessage.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colMessage.HeaderText = "내용 및 원인";
            colMessage.MinimumWidth = 6;
            colMessage.Name = "colMessage";
            colMessage.ReadOnly = true;
            // 
            // grpVision
            // 
            grpVision.Controls.Add(picWebcam);
            grpVision.Controls.Add(btnCameraOn);
            grpVision.Controls.Add(btnCameraOff);
            grpVision.Location = new Point(640, 150);
            grpVision.Name = "grpVision";
            grpVision.Size = new Size(340, 310);
            grpVision.TabIndex = 14;
            grpVision.TabStop = false;
            grpVision.Text = "머신 비전 (Webcam Alignment)";
            // 
            // picWebcam
            // 
            picWebcam.BackColor = Color.Black;
            picWebcam.BorderStyle = BorderStyle.FixedSingle;
            picWebcam.Location = new Point(10, 25);
            picWebcam.Name = "picWebcam";
            picWebcam.Size = new Size(320, 240);
            picWebcam.SizeMode = PictureBoxSizeMode.Zoom;
            picWebcam.TabIndex = 0;
            picWebcam.TabStop = false;
            // 
            // btnCameraOn
            // 
            btnCameraOn.Location = new Point(10, 275);
            btnCameraOn.Name = "btnCameraOn";
            btnCameraOn.Size = new Size(100, 25);
            btnCameraOn.TabIndex = 1;
            btnCameraOn.Text = "카메라 켜기";
            btnCameraOn.UseVisualStyleBackColor = true;
            btnCameraOn.Click += btnCameraOn_Click;
            // 
            // btnCameraOff
            // 
            btnCameraOff.Location = new Point(120, 275);
            btnCameraOff.Name = "btnCameraOff";
            btnCameraOff.Size = new Size(100, 25);
            btnCameraOff.TabIndex = 2;
            btnCameraOff.Text = "카메라 끄기";
            btnCameraOff.UseVisualStyleBackColor = true;
            btnCameraOff.Click += btnCameraOff_Click;
            // 
            // Form1
            // 
            ClientSize = new Size(1005, 500);
            Controls.Add(grpVision);
            Controls.Add(grpDashboard);
            Controls.Add(dgvAlarmLog);
            Controls.Add(lblDoorSensor);
            Controls.Add(btnEMO);
            Controls.Add(btnAutoStop);
            Controls.Add(btnAutoStart);
            Controls.Add(btnHome);
            Controls.Add(btnReset);
            Controls.Add(lblMachineState);
            Controls.Add(grpJog);
            Controls.Add(lblStatus);
            Controls.Add(btnConnect);
            Controls.Add(txtPort);
            Controls.Add(txtIp);
            Name = "Form1";
            Text = "가상 3축 제어기 (Virtual 3-Axis Controller)";
            grpJog.ResumeLayout(false);
            grpJog.PerformLayout();
            grpDashboard.ResumeLayout(false);
            grpDashboard.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvAlarmLog).EndInit();
            grpVision.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picWebcam).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox txtIp;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblMachineState;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnHome;
        private System.Windows.Forms.Button btnAutoStart;
        private System.Windows.Forms.Button btnAutoStop;
        private System.Windows.Forms.Button btnEMO;
        private System.Windows.Forms.Label lblDoorSensor;
        private System.Windows.Forms.GroupBox grpJog;
        private System.Windows.Forms.Label lblPosX;
        private System.Windows.Forms.Label lblPosY;
        private System.Windows.Forms.Label lblPosZ;
        private System.Windows.Forms.Button btnJogXPlus;
        private System.Windows.Forms.Button btnJogXMinus;
        private System.Windows.Forms.Button btnJogYPlus;
        private System.Windows.Forms.Button btnJogYMinus;
        private System.Windows.Forms.Button btnJogZPlus;
        private System.Windows.Forms.Button btnJogZMinus;
        private System.Windows.Forms.GroupBox grpDashboard;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Label lblOk;
        private System.Windows.Forms.Label lblNg;
        private System.Windows.Forms.Label lblYield;
        private System.Windows.Forms.DataGridView dgvAlarmLog;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn colType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMessage;

        // 웹캠 컨트롤
        private System.Windows.Forms.GroupBox grpVision;
        private System.Windows.Forms.PictureBox picWebcam;
        private System.Windows.Forms.Button btnCameraOn;
        private System.Windows.Forms.Button btnCameraOff;
    }
}