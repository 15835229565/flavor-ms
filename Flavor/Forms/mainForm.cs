using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Timers;

namespace Flavor
{
    public partial class mainForm : Form
    {
        private GraphForm gForm;

        public mainForm()
        {
            this.gForm = new GraphForm();
            InitializeComponent();
            populateStatusTreeView();
            gForm.MdiParent = this;
            gForm.Visible = GraphWindowToolStripMenuItem.Checked;
            gForm.WindowState = FormWindowState.Maximized;
            this.LayoutMdi(MdiLayout.Cascade);
            gForm.Show();

            Config.getInitialDirectory();

            Device.OnDeviceStateChanged += new DeviceEventHandler(InvokeRefreshDeviceState);
            Device.OnDeviceStatusChanged += new DeviceEventHandler(InvokeRefreshDeviceStatus);
            Device.OnVacuumStateChanged += new DeviceEventHandler(InvokeRefreshVacuumState);
            Device.OnTurboPumpStatusChanged += new DeviceEventHandler(InvokeRefreshTurboPumpStatus);
            Device.OnTurboPumpAlert += new TurboPumpAlertEventHandler(InvokeProcessTurboPumpAlert);
            Device.Init();

            Graph.OnNewGraphData += new Graph.GraphEventHandler(InvokeRefreshGraph);
            
            Commander.OnProgramStateChanged += new Commander.ProgramEventHandler(InvokeRefreshButtons);
            Commander.pState = Commander.programStates.Start;
            Commander.pStatePrev = Commander.pState;
        }
        #region Status TreeView population
        private TreeNode rootNode;

        private TreeNode infoNode;

        private TreeNode systemStateTextTreeNode;
        private TreeNode systemStateValueTreeNode;
        private TreeNode vacuumStateTextTreeNode;
        private TreeNode vacuumStateValueTreeNode;
        private TreeNode forPumpOnTextTreeNode;
        private TreeNode forPumpOnValueTreeNode;
        private TreeNode turboPumpOnTextTreeNode;
        private TreeNode turboPumpOnValueTreeNode;
        private TreeNode forVacuumTextTreeNode;
        private TreeNode forVacuumValueTreeNode;
        private TreeNode highVacuumTextTreeNode;
        private TreeNode highVacuumValueTreeNode;
        private TreeNode hardwareBlockTextTreeNode;
        private TreeNode hardwareBlockValueTreeNode;
        private TreeNode vGate1TextTreeNode;
        private TreeNode vGate1ValueTreeNode;
        private TreeNode vGate2TextTreeNode;
        private TreeNode vGate2ValueTreeNode;

        private TreeNode extraNode;
        private TreeNode turboPumpNode;

        private void populateStatusTreeView()
        {
            systemStateTextTreeNode = new TreeNode();
            systemStateValueTreeNode = new TreeNode();
            vacuumStateTextTreeNode = new TreeNode();
            vacuumStateValueTreeNode = new TreeNode();
            forPumpOnTextTreeNode = new TreeNode();
            forPumpOnValueTreeNode = new TreeNode();
            turboPumpOnTextTreeNode = new TreeNode();
            turboPumpOnValueTreeNode = new TreeNode();
            forVacuumTextTreeNode = new TreeNode();
            forVacuumValueTreeNode = new TreeNode();
            highVacuumTextTreeNode = new TreeNode();
            highVacuumValueTreeNode = new TreeNode();
            hardwareBlockTextTreeNode = new TreeNode();
            hardwareBlockValueTreeNode = new TreeNode();
            vGate1TextTreeNode = new TreeNode();
            vGate1ValueTreeNode = new TreeNode();
            vGate2TextTreeNode = new TreeNode();
            vGate2ValueTreeNode = new TreeNode();

            systemStateTextTreeNode.Text = "��������� �������";
            // 
            systemStateValueTreeNode.ForeColor = Color.Green;
            systemStateValueTreeNode.Text = "*";
            // 
            vacuumStateTextTreeNode.Text = "��������� �������";
            // 
            vacuumStateValueTreeNode.ForeColor = Color.Green;
            vacuumStateValueTreeNode.Text = "*";
            // 
            forPumpOnTextTreeNode.Text = "������������ �����";
            // 
            forPumpOnValueTreeNode.ForeColor = Color.Green;
            forPumpOnValueTreeNode.Text = "*";
            // 
            turboPumpOnTextTreeNode.Text = "����������������� �����";
            // 
            turboPumpOnValueTreeNode.ForeColor = Color.Green;
            turboPumpOnValueTreeNode.Text = "*";
            // 
            forVacuumTextTreeNode.Text = "������� ������� (���)";
            // 
            forVacuumValueTreeNode.ForeColor = Color.Green;
            forVacuumValueTreeNode.Text = "*";
            // 
            highVacuumTextTreeNode.Text = "������� ������� (�����)";
            // 
            highVacuumValueTreeNode.ForeColor = Color.Green;
            highVacuumValueTreeNode.Text = "*";
            // 
            hardwareBlockTextTreeNode.Text = "������� ����������";
            // 
            hardwareBlockValueTreeNode.ForeColor = Color.Green;
            hardwareBlockValueTreeNode.Text = "*";
            // 
            vGate1TextTreeNode.Text = "��������� ������� 1";
            // 
            vGate1ValueTreeNode.ForeColor = Color.Green;
            vGate1ValueTreeNode.Text = "*";
            // 
            vGate2TextTreeNode.Text = "��������� ������� 2";
            // 
            vGate2ValueTreeNode.ForeColor = Color.Green;
            vGate2ValueTreeNode.Text = "*";
            
            infoNode = new TreeNode("���������� � �������",
                new TreeNode[] { systemStateTextTreeNode, systemStateValueTreeNode, vacuumStateTextTreeNode, vacuumStateValueTreeNode,
                    forPumpOnTextTreeNode, forPumpOnValueTreeNode, turboPumpOnTextTreeNode, turboPumpOnValueTreeNode, forVacuumTextTreeNode,
                    forVacuumValueTreeNode, highVacuumTextTreeNode, highVacuumValueTreeNode, hardwareBlockTextTreeNode, hardwareBlockValueTreeNode,
                    vGate1TextTreeNode, vGate1ValueTreeNode, vGate2TextTreeNode, vGate2ValueTreeNode });
            infoNode.Name = "infoNode";
            infoNode.Text = "���������� � �������";

            extraNode = new TreeNode("�������������");
            extraNode.Name = "extraNode";
            extraNode.Text = "�������������";

            turboPumpNode = new TreeNode("����������");
            turboPumpNode.Name = "turboPumpNode";
            turboPumpNode.Text = "����������";

            rootNode = new TreeNode("������",
                new TreeNode[] { infoNode, extraNode, turboPumpNode });
            rootNode.Name = "rootNode";
            rootNode.Text = "��������� �������";
            rootNode.ExpandAll();

            statusTreeView.Nodes.AddRange(new TreeNode[] {rootNode});
        }
        #endregion
        private void mainForm_Load(object sender, EventArgs e)
        {
            openConfigFileToolStripMenuItem_Click(sender, e);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainForm.ActiveForm.Close();
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConnectOptionsForm cForm = new ConnectOptionsForm();
            cForm.ShowDialog();
        }

        private void overviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ScanOptionsForm sForm = new ScanOptionsForm();
            sForm.ShowDialog();
        }

        private void senseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PreciseOptionsForm pForm = PreciseOptionsForm.getInstance();
            pForm.UpLevel = this;
            pForm.Show();
        }

        private void initSys_butt_Click(object sender, EventArgs e)
        {
            initSys_butt.Enabled = false;
            Commander.Init();
        }
        private void shutSys_butt_Click(object sender, EventArgs e)
        {
            shutSys_butt.Enabled = false;
            Commander.Shutdown();
        }
        private void unblock_butt_Click(object sender, EventArgs e)
        {
            unblock_butt.Enabled = false;
            Commander.Unblock();
        }
        private void overview_button_Click(object sender, EventArgs e)
        {
            overview_button.Enabled = false;
            sensmeasure_button.Enabled = false;

            //Elements are not visible until first real information is ready
            peakNumberLabel.Visible = false;
            label39.Visible = false;
            peakCenterLabel.Visible = false;
            label41.Visible = false;
            peakWidthLabel.Visible = false;
            detector1CountsLabel.Visible = false;
            label15.Visible = false;
            detector2CountsLabel.Visible = false;
            label16.Visible = false;

            measurePanelToolStripMenuItem.Enabled = true;
            measurePanelToolStripMenuItem.Checked = true;

            this.startScanTextLabel.Visible = true;
            this.label18.Visible = true;
            this.firstStepLabel.Visible = true;
            this.lastStepLabel.Visible = true;
            this.label37.Visible = false;
            this.peakNumberLabel.Visible = false;

            firstStepLabel.Text = Config.sPoint.ToString();
            lastStepLabel.Text = Config.ePoint.ToString();
            etime_label.Text = Config.eTimeReal.ToString();
            itime_label.Text = Config.iTimeReal.ToString();
            iVolt_label.Text = string.Format("{0:f3}", Config.iVoltageReal);
            cp_label.Text = string.Format("{0:f3}", Config.CPReal);
            emCurLabel.Text = string.Format("{0:f3}", Config.eCurrentReal);
            heatCurLabel.Text = string.Format("{0:f3}", Config.hCurrentReal);
            f1_label.Text = string.Format("{0:f3}", Config.fV1Real);
            f2_label.Text = string.Format("{0:f3}", Config.fV2Real);
            scanProgressBar.Value = 0;
            scanProgressBar.Maximum = Config.ePoint - Config.sPoint;
            scanProgressBar.Step = 1;
            cancelScanButton.Enabled = true;
            cancelScanButton.Visible = true;

            Graph.ResetLoadedPointLists();
            this.gForm.setXScaleLimits();
            this.gForm.CreateGraph();
            Commander.OnScanCancelled += new Commander.ProgramEventHandler(InvokeCancelScan);
            Commander.Scan();
        }
        private void sensmeasure_button_Click(object sender, EventArgs e)
        {
            overview_button.Enabled = false;
            sensmeasure_button.Enabled = false;

            //Elements are not visible until first real information is ready
            peakNumberLabel.Visible = false;
            label39.Visible = false;
            peakCenterLabel.Visible = false;
            label41.Visible = false;
            peakWidthLabel.Visible = false;
            detector1CountsLabel.Visible = false;
            label15.Visible = false;
            detector2CountsLabel.Visible = false;
            label16.Visible = false;

            measurePanelToolStripMenuItem.Enabled = true;
            measurePanelToolStripMenuItem.Checked = true;

            this.startScanTextLabel.Visible = false;
            this.label18.Visible = false;
            this.firstStepLabel.Visible = false;
            this.lastStepLabel.Visible = false;
            this.label37.Visible = true;
            this.peakNumberLabel.Visible = true;

            etime_label.Text = Config.eTimeReal.ToString();
            itime_label.Text = Config.iTimeReal.ToString();
            iVolt_label.Text = string.Format("{0:f3}", Config.iVoltageReal);
            cp_label.Text = string.Format("{0:f3}", Config.CPReal);
            emCurLabel.Text = string.Format("{0:f3}", Config.eCurrentReal);
            heatCurLabel.Text = string.Format("{0:f3}", Config.hCurrentReal);
            f1_label.Text = string.Format("{0:f3}", Config.fV1Real);
            f2_label.Text = string.Format("{0:f3}", Config.fV2Real);
            scanProgressBar.Value = 0;
            scanProgressBar.Maximum = 0;
            foreach (Utility.PreciseEditorData ped in Config.PreciseData)
            {
                if (ped.Use)
                    scanProgressBar.Maximum += (2 * ped.Width + 1) * ped.Iterations;
            }
            scanProgressBar.Step = 1;
            cancelScanButton.Enabled = true;
            cancelScanButton.Visible = true;

            Graph.ResetLoadedPointLists();
            this.gForm.setXScaleLimits(Config.PreciseData);
            Commander.OnScanCancelled += new Commander.ProgramEventHandler(InvokeCancelScan);
            Commander.Sense();
        }

        private void InvokeProcessTurboPumpAlert(bool isFault, byte bits)
        {
            string msg = "����������: ";
            msg += isFault ? "����� (" : "�������������� (";
            msg += bits.ToString("{N}");
            msg += ")";
            InvokeRefreshUserMessage(msg);
            Config.logTurboPumpAlert(msg);
        }

        private void InvokeRefreshUserMessage(string msg)
        {
            if (this.InvokeRequired)
            {
                Commander.AsyncReplyHandler InvokeDelegate = new Commander.AsyncReplyHandler(RefreshUserMessage);
                this.Invoke(InvokeDelegate, msg);
            }
            else
            {
                RefreshUserMessage(msg);
            }
        }
        private void RefreshUserMessage(string msg)
        {
            this.measure_StatusLabel.Text = msg;
        }

        private void InvokeRefreshGraph(Graph.Displaying displayMode, bool recreate)
        {
            if (this.InvokeRequired)
            {
                Graph.GraphEventHandler InvokeDelegate = new Graph.GraphEventHandler(RefreshGraph);
                this.Invoke(InvokeDelegate, displayMode, recreate);
            }
            else
            {
                RefreshGraph(displayMode, recreate);
            }
        }
        private void RefreshGraph(Graph.Displaying displayMode, bool recreate)
        {
            switch (displayMode)
            {
                case Graph.Displaying.Loaded:
                    if (recreate)
                        gForm.DisplayLoadedSpectrum();
                    else
                        gForm.RefreshGraph();
                    break;
                case Graph.Displaying.Measured:
                    if (recreate)
                        gForm.CreateGraph();
                    else
                    {
                        gForm.RefreshGraph();
                        scanProgressBar.PerformStep();
                        stepNumberLabel.Text = Graph.LastPoint.ToString();
                        scanRealTimeLabel.Text = string.Format("{0:f1}", Config.scanVoltageReal(Graph.LastPoint));
                        detector1CountsLabel.Text = Device.Detector1.ToString();
                        detector2CountsLabel.Text = Device.Detector2.ToString();
                        if (Commander.isSenseMeasure)
                        {
                            peakNumberLabel.Text = (Graph.CurrentPeak.pNumber + 1).ToString();
                            peakNumberLabel.Visible = true;
                            label39.Visible = true;
                            peakCenterLabel.Text = Graph.CurrentPeak.Step.ToString();
                            peakCenterLabel.Visible = true;
                            label41.Visible = true;
                            peakWidthLabel.Text = Graph.CurrentPeak.Width.ToString();
                            peakWidthLabel.Visible = true;
                            if (Graph.CurrentPeak.Collector == 1)
                            {
                                detector1CountsLabel.Visible = true;
                                label15.Visible = true;
                                detector2CountsLabel.Visible = false;
                                label16.Visible = false;
                            }
                            else
                            {
                                detector1CountsLabel.Visible = false;
                                label15.Visible = false;
                                detector2CountsLabel.Visible = true;
                                label16.Visible = true;
                            }
                        }
                        else
                        {
                            gForm.yAxisChange();
                            gForm.specterSavingEnabled = true;
                            
                            detector1CountsLabel.Visible = true;
                            label15.Visible = true;
                            detector2CountsLabel.Visible = true;
                            label16.Visible = true;
                            peakNumberLabel.Visible = false;
                            label39.Visible = false;
                            peakCenterLabel.Visible = false;
                            label41.Visible = false;
                            peakWidthLabel.Visible = false;
                        }
                    }
                    break;
                case Graph.Displaying.Diff:
                    if (recreate)
                        gForm.DisplayDiff();
                    else
                        gForm.RefreshGraph();
                    break;
            }
        }

        private void InvokeRefreshDeviceState()
        {
            if (this.InvokeRequired)
            {
                DeviceEventHandler InvokeDelegate = new DeviceEventHandler(RefreshDeviceState);
                this.Invoke(InvokeDelegate);
            }
            else
            {
                RefreshDeviceState();
            }
        }
        private void RefreshDeviceState()
        {
            switch (Device.sysState)
            {
                case (byte)Device.DeviceStates.Start:
                    this.systemStateValueTreeNode.Text = "������";
                    this.systemStateValueTreeNode.ForeColor = Color.Red;
                    break;
                case (byte)Device.DeviceStates.Init:
                    this.systemStateValueTreeNode.Text = "�������������";
                    this.systemStateValueTreeNode.ForeColor = Color.Blue;
                    break;
                case (byte)Device.DeviceStates.VacuumInit:
                    this.systemStateValueTreeNode.Text = "������������� �������";
                    this.systemStateValueTreeNode.ForeColor = Color.Blue;
                    break;
                case (byte)Device.DeviceStates.WaitHighVoltage:
                    this.systemStateValueTreeNode.Text = "�������� �������� ����������";
                    this.systemStateValueTreeNode.ForeColor = Color.Green;
                    break;
                case (byte)Device.DeviceStates.Ready:
                    this.systemStateValueTreeNode.Text = "������ � ���������";
                    this.systemStateValueTreeNode.ForeColor = Color.Green;
                    break;
                case (byte)Device.DeviceStates.Measuring:
                    this.systemStateValueTreeNode.Text = "������������ ���������";
                    this.systemStateValueTreeNode.ForeColor = Color.Green;
                    break;
                case (byte)Device.DeviceStates.Measured:
                    this.systemStateValueTreeNode.Text = "��������� ���������";
                    this.systemStateValueTreeNode.ForeColor = Color.Green;
                    break;
                case (byte)Device.DeviceStates.Shutdowning:
                    this.systemStateValueTreeNode.Text = "���� ����������";
                    this.systemStateValueTreeNode.ForeColor = Color.Blue;
                    break;
                case (byte)Device.DeviceStates.Shutdown:
                    this.systemStateValueTreeNode.Text = "����������";
                    this.systemStateValueTreeNode.ForeColor = Color.Red;
                    break;
                case (byte)Device.DeviceStates.TurboPumpFailure:
                    this.systemStateValueTreeNode.Text = "����� �����������";
                    this.systemStateValueTreeNode.ForeColor = Color.Red;
                    break;
                case (byte)Device.DeviceStates.VacuumCrash:
                    this.systemStateValueTreeNode.Text = "������ �������";
                    this.systemStateValueTreeNode.ForeColor = Color.Red;
                    break;
                default:
                    this.systemStateValueTreeNode.Text = "����������";
                    this.systemStateValueTreeNode.ForeColor = Color.Red;
                    break;
            }
        }

        private void InvokeRefreshTurboPumpStatus() 
        {
            if (this.InvokeRequired)
            {
                DeviceEventHandler InvokeDelegate = new DeviceEventHandler(RefreshTurboPumpStatus);
                this.Invoke(InvokeDelegate);
            }
            else
            {
                RefreshTurboPumpStatus();
            }
        }
        private void RefreshTurboPumpStatus()
        {
            this.turboSpeedLabel.Text = string.Format("{0:f0}", Device.TurboPump.Speed);
            this.turboCurrentLabel.Text = string.Format("{0:f0}", Device.TurboPump.Current);
            this.pwmLabel.Text = string.Format("{0:f3}", Device.TurboPump.pwm);
            this.pumpTemperatureLabel.Text = string.Format("{0:f0}", Device.TurboPump.PumpTemperature);
            this.driveTemperatureLabel.Text = string.Format("{0:f0}", Device.TurboPump.DriveTemperature);
            this.operationTimeLabel.Text = string.Format("{0:f0}", Device.TurboPump.OperationTime);
        }

        private void InvokeRefreshDeviceStatus()
        {
            if (this.InvokeRequired)
            {
                DeviceEventHandler InvokeDelegate = new DeviceEventHandler(RefreshDeviceStatus);
                this.Invoke(InvokeDelegate);
            }
            else
            {
                RefreshDeviceStatus();
            }
        }
        private void RefreshDeviceStatus()
        {
            if (Device.fPumpOn)
            {
                this.forPumpOnValueTreeNode.ForeColor = Color.Green;
                this.forPumpOnValueTreeNode.Text = "�������";
            }
            else
            {
                this.forPumpOnValueTreeNode.ForeColor = Color.Red;
                this.forPumpOnValueTreeNode.Text = "��������";
            }
            if (Device.tPumpOn)
            {
                this.turboPumpOnValueTreeNode.ForeColor = Color.Green;
                this.turboPumpOnValueTreeNode.Text = "�������";
            }
            else
            {
                this.turboPumpOnValueTreeNode.ForeColor = Color.Red;
                this.turboPumpOnValueTreeNode.Text = "��������";
            }
            this.forVacuumValueTreeNode.Text = string.Format("{0:e3}", Device.fVacuumReal);
            this.highVacuumValueTreeNode.Text = string.Format("{0:e3}", Device.hVacuumReal);
            if (Device.highVoltageOn)
            {
                this.hardwareBlockValueTreeNode.ForeColor = Color.Green;
                this.hardwareBlockValueTreeNode.Text = "��������";
            }
            else
            {
                this.hardwareBlockValueTreeNode.ForeColor = Color.Red;
                this.hardwareBlockValueTreeNode.Text = "���������";
            }
            if (Device.highVacuumValve)
            {
                this.vGate1ValueTreeNode.ForeColor = Color.Green;
                this.vGate1ValueTreeNode.Text = "������";
            }
            else
            {
                this.vGate1ValueTreeNode.ForeColor = Color.Red;
                this.vGate1ValueTreeNode.Text = "������";
            }
            if (Device.probeValve)
            {
                this.vGate2ValueTreeNode.ForeColor = Color.Green;
                this.vGate2ValueTreeNode.Text = "������";
            }
            else
            {
                this.vGate2ValueTreeNode.ForeColor = Color.Red;
                this.vGate2ValueTreeNode.Text = "������";
            }
            this.f1Voltage_label.Text = string.Format("{0:f2}", Device.fV1Real);
            this.f2Voltage_label.Text = string.Format("{0:f2}", Device.fV2Real);
            this.iVoltage_label.Text = string.Format("{0:f2}", Device.iVoltageReal);
            this.detectorVoltage_label.Text = string.Format("{0:f1}", Device.dVoltageReal);
            this.CondPlus_label.Text = string.Format("{0:f2}", Device.cVPlusReal);
            this.CondMin_label.Text = string.Format("{0:f2}", Device.cVMinReal);
            this.scanVoltageLabel.Text = string.Format("{0:f1}", Device.sVoltageReal);
            this.eCurrent_label.Text = string.Format("{0:f3}", Device.eCurrentReal);
            this.hCurrent_label.Text = string.Format("{0:f3}", Device.hCurrentReal);
            this.turboSpeedLabel.Text = string.Format("{0:f0}", Device.TurboPump.Speed);
        }

        private void InvokeRefreshVacuumState()
        {
            if (this.InvokeRequired)
            {
                DeviceEventHandler InvokeDelegate = new DeviceEventHandler(RefreshVacuumState);
                this.Invoke(InvokeDelegate);
            }
            else
            {
                RefreshVacuumState();
            }
        }
        private void RefreshVacuumState()
        {
            switch (Device.vacState) 
            {
                case (byte)Device.VacuumStates.Idle:
                    this.vacuumStateValueTreeNode.Text = "�����������";
                    this.vacuumStateValueTreeNode.ForeColor = Color.Blue;
                    break;
                case (byte)Device.VacuumStates.Init:
                    this.vacuumStateValueTreeNode.Text = "�������������";
                    this.vacuumStateValueTreeNode.ForeColor = Color.Blue;
                    break;
                case (byte)Device.VacuumStates.StartingForvacuumPump:
                    this.vacuumStateValueTreeNode.Text = "��������� ���������";
                    this.vacuumStateValueTreeNode.ForeColor = Color.Blue;
                    break;
                case (byte)Device.VacuumStates.PumpingForvacuum:
                    this.vacuumStateValueTreeNode.Text = "������� ����������";
                    this.vacuumStateValueTreeNode.ForeColor = Color.Blue;
                    break;
                case (byte)Device.VacuumStates.DelayPumpingHighVacuumByForvac:
                    this.vacuumStateValueTreeNode.Text = "�������� �������� ������� ��-�� ���";
                    this.vacuumStateValueTreeNode.ForeColor = Color.Blue;
                    break;
                case (byte)Device.VacuumStates.PumpingHighVacuumByForvac:
                    this.vacuumStateValueTreeNode.Text = "������� �������� ������� ����������";
                    this.vacuumStateValueTreeNode.ForeColor = Color.Blue;
                    break;
                case (byte)Device.VacuumStates.PumpingHighVacuumByTurbo:
                    this.vacuumStateValueTreeNode.Text = "������� �����";
                    this.vacuumStateValueTreeNode.ForeColor = Color.Blue;
                    break;
                case (byte)Device.VacuumStates.Ready:
                    this.vacuumStateValueTreeNode.Text = "������";
                    this.vacuumStateValueTreeNode.ForeColor = Color.Green;
                    break;
                case (byte)Device.VacuumStates.ShutdownInit:
                    this.vacuumStateValueTreeNode.Text = "������������� ����������";
                    this.vacuumStateValueTreeNode.ForeColor = Color.Blue;
                    break;
                case (byte)Device.VacuumStates.ShutdownDelay:
                    this.vacuumStateValueTreeNode.Text = "�������� ����������";
                    this.vacuumStateValueTreeNode.ForeColor = Color.Blue;
                    break;
                case (byte)Device.VacuumStates.ShutdownPumpProbe:
                    this.vacuumStateValueTreeNode.Text = "���������� ������� ������";
                    this.vacuumStateValueTreeNode.ForeColor = Color.Blue;
                    break;
                case (byte)Device.VacuumStates.Shutdowned:
                    this.vacuumStateValueTreeNode.Text = "���������";
                    this.vacuumStateValueTreeNode.ForeColor = Color.Red;
                    break;
                case (byte)Device.VacuumStates.ShutdownTurboPump:
                    this.vacuumStateValueTreeNode.Text = "������� ��� ����������";
                    this.vacuumStateValueTreeNode.ForeColor = Color.Blue;
                    break;
                case (byte)Device.VacuumStates.BadHighVacuum:
                    this.vacuumStateValueTreeNode.Text = "������ ������� ������";
                    this.vacuumStateValueTreeNode.ForeColor = Color.Red;
                    break;
                case (byte)Device.VacuumStates.BadForvacuum:
                    this.vacuumStateValueTreeNode.Text = "������ ���������";
                    this.vacuumStateValueTreeNode.ForeColor = Color.Red;
                    break;
                case (byte)Device.VacuumStates.ForvacuumFailure:
                    this.vacuumStateValueTreeNode.Text = "����� ����������";
                    this.vacuumStateValueTreeNode.ForeColor = Color.Red;
                    break;
                case (byte)Device.VacuumStates.LargeLeak:
                    this.vacuumStateValueTreeNode.Text = "������� ����";
                    this.vacuumStateValueTreeNode.ForeColor = Color.Red;
                    break;
                case (byte)Device.VacuumStates.SmallLeak:
                    this.vacuumStateValueTreeNode.Text = "����� ����";
                    this.vacuumStateValueTreeNode.ForeColor = Color.Red;
                    break;
                case (byte)Device.VacuumStates.ThermoCoupleFailure:
                    this.vacuumStateValueTreeNode.Text = "����� ���������";
                    this.vacuumStateValueTreeNode.ForeColor = Color.Red;
                    break;
                case (byte)Device.VacuumStates.TurboPumpFailure:
                    this.vacuumStateValueTreeNode.Text = "����� �����������";
                    this.vacuumStateValueTreeNode.ForeColor = Color.Red;
                    break;
                case (byte)Device.VacuumStates.VacuumShutdownProbeLeak:
                    this.vacuumStateValueTreeNode.Text = "���������� ������� �������";
                    this.vacuumStateValueTreeNode.ForeColor = Color.Red;
                    break;
                default:
                    this.vacuumStateValueTreeNode.Text = "����������� ���������";
                    this.vacuumStateValueTreeNode.ForeColor = Color.Red;
                    break;
            }
        }

        internal void InvokeRefreshButtons() 
        {
            if (this.InvokeRequired)
            {
                Commander.ProgramEventHandler InvokeDelegate = new Commander.ProgramEventHandler(RefreshButtons);
                this.Invoke(InvokeDelegate);
            }
            else 
            {
                RefreshButtons(); 
            }
        }
        private void RefreshButtons()
        {
            //bool precPointsExist = (Config.PreciseData.Count != 0);
            bool precPointsExist = Commander.somePointsUsed();
            switch (Commander.hBlock) 
            {
                case true:
                    unblock_butt.Text = "����� ����������";
                    unblock_butt.ForeColor = Color.Green;
                    break;
                case false:
                    unblock_butt.Text = "�������� ����������";
                    unblock_butt.ForeColor = Color.Red;
                    break;
            }
            switch (Commander.pState)
            {
                case Commander.programStates.Start:
                    connectToolStripButton.Enabled = true;
                    if (Commander.deviceIsConnected)
                    {
                        connectToolStripButton.Text = "�����������";
                        connectToolStripButton.ForeColor = Color.Red;
                    }
                    else
                    {
                        connectToolStripButton.Text = "���������";
                        connectToolStripButton.ForeColor = Color.Green;
                    }

                    initSys_butt.Enabled = Commander.deviceIsConnected;//true;
                    shutSys_butt.Enabled = Commander.deviceIsConnected;//false;
                    unblock_butt.Enabled = Commander.deviceIsConnected && !Commander.hBlock;//��������� ��� ��������� ����������
                    overview_button.Enabled = false;
                    sensmeasure_button.Enabled = false;
                    
                    connectToolStripMenuItem.Enabled = true;
                    measureToolStripMenuItem.Enabled = true;

                    measurePanelToolStripMenuItem.Checked = false;
                    measurePanelToolStripMenuItem.Enabled = false;
                    
                    gForm.specterOpeningEnabled = true;
                    
                    break;
                case Commander.programStates.WaitInit:
                    connectToolStripButton.Enabled = false;

                    initSys_butt.Enabled = false;
                    shutSys_butt.Enabled = false;
                    unblock_butt.Enabled = false;
                    overview_button.Enabled = false;
                    sensmeasure_button.Enabled = false;

                    connectToolStripMenuItem.Enabled = false;
                    measureToolStripMenuItem.Enabled = true;

                    measurePanelToolStripMenuItem.Checked = false;
                    measurePanelToolStripMenuItem.Enabled = false;

                    gForm.specterOpeningEnabled = true;
                    break;
                case Commander.programStates.Init:
                    connectToolStripButton.Enabled = false;

                    initSys_butt.Enabled = false;
                    shutSys_butt.Enabled = true;
                    unblock_butt.Enabled = false;
                    overview_button.Enabled = false;
                    sensmeasure_button.Enabled = false;

                    connectToolStripMenuItem.Enabled = false;
                    measureToolStripMenuItem.Enabled = true;

                    measurePanelToolStripMenuItem.Checked = false;
                    measurePanelToolStripMenuItem.Enabled = false;

                    gForm.specterOpeningEnabled = true;
                    break;
                case Commander.programStates.WaitHighVoltage:
                    connectToolStripButton.Enabled = false;

                    initSys_butt.Enabled = false;
                    shutSys_butt.Enabled = true;
                    unblock_butt.Enabled = true;
                    overview_button.Enabled = false;
                    sensmeasure_button.Enabled = false;

                    connectToolStripMenuItem.Enabled = false;
                    measureToolStripMenuItem.Enabled = true;

                    measurePanelToolStripMenuItem.Checked = false;
                    measurePanelToolStripMenuItem.Enabled = false;

                    gForm.specterOpeningEnabled = true;
                    break;
                case Commander.programStates.Ready:
                    connectToolStripButton.Enabled = false;

                    initSys_butt.Enabled = false;
                    shutSys_butt.Enabled = true;
                    unblock_butt.Enabled = true;
                    overview_button.Enabled = true & !Commander.hBlock;
                    sensmeasure_button.Enabled = true & !Commander.hBlock & precPointsExist;

                    connectToolStripMenuItem.Enabled = false;
                    measureToolStripMenuItem.Enabled = true;

                    measurePanelToolStripMenuItem.Checked = false;
                    measurePanelToolStripMenuItem.Enabled = false;

                    gForm.specterOpeningEnabled = true;
                    break;
                case Commander.programStates.Measure:
                    connectToolStripButton.Enabled = false;

                    initSys_butt.Enabled = false;
                    shutSys_butt.Enabled = true;
                    unblock_butt.Enabled = true;
                    overview_button.Enabled = false;
                    sensmeasure_button.Enabled = false;

                    connectToolStripMenuItem.Enabled = false;
                    measureToolStripMenuItem.Enabled = false;

                    measurePanelToolStripMenuItem.Enabled = true;

                    gForm.specterOpeningEnabled = false;
                    break;
                case Commander.programStates.WaitShutdown:
                    connectToolStripButton.Enabled = false;

                    initSys_butt.Enabled = false;
                    shutSys_butt.Enabled = false;
                    unblock_butt.Enabled = false;
                    overview_button.Enabled = false;
                    sensmeasure_button.Enabled = false;

                    connectToolStripMenuItem.Enabled = false;
                    measureToolStripMenuItem.Enabled = true;

                    measurePanelToolStripMenuItem.Checked = false;
                    measurePanelToolStripMenuItem.Enabled = false;

                    gForm.specterOpeningEnabled = true;
                    break;
                case Commander.programStates.Shutdown:
                    connectToolStripButton.Enabled = false;

                    initSys_butt.Enabled = false;
                    shutSys_butt.Enabled = false;
                    unblock_butt.Enabled = false;
                    overview_button.Enabled = false;
                    sensmeasure_button.Enabled = false;

                    connectToolStripMenuItem.Enabled = false;
                    measureToolStripMenuItem.Enabled = true;

                    measurePanelToolStripMenuItem.Checked = false;
                    measurePanelToolStripMenuItem.Enabled = false;

                    gForm.specterOpeningEnabled = true;
                    break;
            }
        }

        private void InvokeCancelScan()
        {
            if (this.InvokeRequired)
            {
                Commander.ProgramEventHandler InvokeDelegate = new Commander.ProgramEventHandler(CancelScan);
                this.Invoke(InvokeDelegate);
            }
            else
            {
                CancelScan();
            }
        }
        private void CancelScan()
        {
            Commander.OnScanCancelled -= new Commander.ProgramEventHandler(InvokeCancelScan);
            cancelScanButton.Enabled = false;
            cancelScanButton.Visible = false;
        }
        
        private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ControlToolStrip.Visible = toolBarToolStripMenuItem.Checked;
        }

        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip.Visible = statusBarToolStripMenuItem.Checked;
        }

        private void GraphWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gForm.Visible = GraphWindowToolStripMenuItem.Checked;
        }

        private void ParameterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ParameterPanel.Visible = ParameterToolStripMenuItem.Checked;
        }

        private void measurePanelToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            measurePanel.Visible = measurePanelToolStripMenuItem.Checked;
        }

        private void mainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Commander.pState != Commander.programStates.Start)
            {
                if (new ClosureDialog().ShowDialog() != DialogResult.OK)
                    e.Cancel = true;
            }
            else 
            {
                if (Commander.deviceIsConnected) Commander.Disconnect();
            }
        }

        private void openConfigFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Config.LoadConfig();
            }
            catch (Config.ConfigLoadException cle)
            {
                cle.visualise();
            }
        }

        private void saveConfigFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Config.SaveAll();
        }

        private void cancelScanButton_Click(object sender, EventArgs e)
        {
            cancelScanButton.Enabled = false;
            Commander.measureCancelRequested = true;
            gForm.specterOpeningEnabled = true;
            //!!!
        }

        private void connectToolStripButton_Click(object sender, EventArgs e)
        {
            if (Commander.deviceIsConnected)
            {
                Commander.Disconnect();
                Commander.OnAsyncReply -= new Commander.AsyncReplyHandler(InvokeRefreshUserMessage);
            }
            else
            {
                Commander.OnAsyncReply += new Commander.AsyncReplyHandler(InvokeRefreshUserMessage);
                Commander.Connect();
            }
        }

        private void delaysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DelaysOptionsForm dForm = new DelaysOptionsForm();
            dForm.ShowDialog();
        }
    }
}