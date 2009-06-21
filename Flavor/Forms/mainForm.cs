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
            gForm = new GraphForm();
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
        
        private TreeNode f1VoltageTextTreeNode;
        private TreeNode f1VoltageValueTreeNode;
        private TreeNode f2VoltageTextTreeNode;
        private TreeNode f2VoltageValueTreeNode;
        private TreeNode scanVoltageTextTreeNode;
        private TreeNode scanVoltageValueTreeNode;
        private TreeNode iVoltageTextTreeNode;
        private TreeNode iVoltageValueTreeNode;
        private TreeNode eCurrentTextTreeNode;
        private TreeNode eCurrentValueTreeNode;
        private TreeNode condPlusTextTreeNode;
        private TreeNode condPlusValueTreeNode;
        private TreeNode condMinusTextTreeNode;
        private TreeNode condMinusValueTreeNode;
        private TreeNode detectorVoltageTextTreeNode;
        private TreeNode detectorVoltageValueTreeNode;
        private TreeNode hCurrentTextTreeNode;
        private TreeNode hCurrentValueTreeNode;
        
        private TreeNode turboPumpNode;

        private TreeNode turboSpeedTextTreeNode;
        private TreeNode turboSpeedValueTreeNode;
        private TreeNode turboCurrentTextTreeNode;
        private TreeNode turboCurrentValueTreeNode;
        private TreeNode pumpTemperatureTextTreeNode;
        private TreeNode pumpTemperatureValueTreeNode;
        private TreeNode driveTemperatureTextTreeNode;
        private TreeNode driveTemperatureValueTreeNode;
        private TreeNode pwmTextTreeNode;
        private TreeNode pwmValueTreeNode;
        private TreeNode operationTimeTextTreeNode;
        private TreeNode operationTimeValueTreeNode;
        
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
            vacuumStateTextTreeNode.Text = "��������� �������";
            forPumpOnTextTreeNode.Text = "������������ �����";
            turboPumpOnTextTreeNode.Text = "����������������� �����";
            forVacuumTextTreeNode.Text = "������� ������� (���)";
            forVacuumValueTreeNode.ForeColor = Color.Green;
            highVacuumTextTreeNode.Text = "������� ������� (�����)";
            highVacuumValueTreeNode.ForeColor = Color.Green;
            hardwareBlockTextTreeNode.Text = "������� ����������";
            vGate1TextTreeNode.Text = "��������� ������� 1";
            vGate2TextTreeNode.Text = "��������� ������� 2";
            
            infoNode = new TreeNode("���������� � �������",
                new TreeNode[] { systemStateTextTreeNode, systemStateValueTreeNode, vacuumStateTextTreeNode, vacuumStateValueTreeNode,
                    forPumpOnTextTreeNode, forPumpOnValueTreeNode, turboPumpOnTextTreeNode, turboPumpOnValueTreeNode, forVacuumTextTreeNode,
                    forVacuumValueTreeNode, highVacuumTextTreeNode, highVacuumValueTreeNode, hardwareBlockTextTreeNode, hardwareBlockValueTreeNode,
                    vGate1TextTreeNode, vGate1ValueTreeNode, vGate2TextTreeNode, vGate2ValueTreeNode });
            infoNode.Name = "infoNode";
            infoNode.Text = "���������� � �������";

            f1VoltageTextTreeNode = new TreeNode();
            f1VoltageValueTreeNode = new TreeNode();
            f2VoltageTextTreeNode = new TreeNode();
            f2VoltageValueTreeNode = new TreeNode();
            scanVoltageTextTreeNode = new TreeNode();
            scanVoltageValueTreeNode = new TreeNode();
            iVoltageTextTreeNode = new TreeNode();
            iVoltageValueTreeNode = new TreeNode();
            eCurrentTextTreeNode = new TreeNode();
            eCurrentValueTreeNode = new TreeNode();
            condPlusTextTreeNode = new TreeNode();
            condPlusValueTreeNode = new TreeNode();
            condMinusTextTreeNode = new TreeNode();
            condMinusValueTreeNode = new TreeNode();
            detectorVoltageTextTreeNode = new TreeNode();
            detectorVoltageValueTreeNode = new TreeNode();
            hCurrentTextTreeNode = new TreeNode();
            hCurrentValueTreeNode = new TreeNode();

            f1VoltageTextTreeNode.Text = "������������ ����. (1) �";
            f1VoltageValueTreeNode.ForeColor = Color.Green;
            f2VoltageTextTreeNode.Text = "������������ ����. (2) �";
            f2VoltageValueTreeNode.ForeColor = Color.Green;
            scanVoltageTextTreeNode.Text = "���������� ��������� �";
            scanVoltageValueTreeNode.ForeColor = Color.Green;
            iVoltageTextTreeNode.Text = "���������� ���������, �";
            iVoltageValueTreeNode.ForeColor = Color.Green;
            eCurrentTextTreeNode.Text = "��� �������, ���";
            eCurrentValueTreeNode.ForeColor = Color.Green;
            condPlusTextTreeNode.Text = "���������� ������������ (+) (50-150 �)";
            condPlusValueTreeNode.ForeColor = Color.Green;
            condMinusTextTreeNode.Text = "���������� ������������ (-) (50-150 �)";
            condMinusValueTreeNode.ForeColor = Color.Green;
            detectorVoltageTextTreeNode.Text = "���������� �� ���������, �";
            detectorVoltageValueTreeNode.ForeColor = Color.Green;
            hCurrentTextTreeNode.Text = "��� �������, �";
            hCurrentValueTreeNode.ForeColor = Color.Green;
            
            extraNode = new TreeNode("�������������",
                new TreeNode[] { f1VoltageTextTreeNode, f1VoltageValueTreeNode, f2VoltageTextTreeNode, f2VoltageValueTreeNode,
                    scanVoltageTextTreeNode, scanVoltageValueTreeNode, iVoltageTextTreeNode, iVoltageValueTreeNode, eCurrentTextTreeNode,
                    eCurrentValueTreeNode, condPlusTextTreeNode, condPlusValueTreeNode, condMinusTextTreeNode, condMinusValueTreeNode,
                    detectorVoltageTextTreeNode, detectorVoltageValueTreeNode, hCurrentTextTreeNode, hCurrentValueTreeNode});
            extraNode.Name = "extraNode";
            extraNode.Text = "�������������";

            turboSpeedTextTreeNode = new TreeNode();
            turboSpeedValueTreeNode = new TreeNode();
            turboCurrentTextTreeNode = new TreeNode();
            turboCurrentValueTreeNode = new TreeNode();
            pumpTemperatureTextTreeNode = new TreeNode();
            pumpTemperatureValueTreeNode = new TreeNode();
            driveTemperatureTextTreeNode = new TreeNode();
            driveTemperatureValueTreeNode = new TreeNode();
            pwmTextTreeNode = new TreeNode();
            pwmValueTreeNode = new TreeNode();
            operationTimeTextTreeNode = new TreeNode();
            operationTimeValueTreeNode = new TreeNode();

            turboSpeedTextTreeNode.Text = "�������� ��������, ��./���.";
            turboSpeedValueTreeNode.ForeColor = System.Drawing.Color.Green;
            turboCurrentTextTreeNode.Text = "���, ��";
            turboCurrentValueTreeNode.ForeColor = System.Drawing.Color.Green;
            pumpTemperatureTextTreeNode.Text = "����������� ������";
            pumpTemperatureValueTreeNode.ForeColor = System.Drawing.Color.Green;
            driveTemperatureTextTreeNode.Text = "����������� �������";
            driveTemperatureValueTreeNode.ForeColor = System.Drawing.Color.Green;
            pwmTextTreeNode.Text = "pwm";
            pwmValueTreeNode.ForeColor = System.Drawing.Color.Green;
            operationTimeTextTreeNode.Text = "����� ������";
            operationTimeValueTreeNode.ForeColor = System.Drawing.Color.Green;

            turboPumpNode = new TreeNode("����������", 
                new TreeNode[] { turboSpeedTextTreeNode, turboSpeedValueTreeNode, turboCurrentTextTreeNode, turboCurrentValueTreeNode,
                    pumpTemperatureTextTreeNode, pumpTemperatureValueTreeNode, driveTemperatureTextTreeNode, driveTemperatureValueTreeNode,
                    pwmTextTreeNode, pwmValueTreeNode, operationTimeTextTreeNode, operationTimeValueTreeNode});
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

            startScanTextLabel.Visible = true;
            label18.Visible = true;
            firstStepLabel.Visible = true;
            lastStepLabel.Visible = true;
            label37.Visible = false;
            peakNumberLabel.Visible = false;

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
            gForm.setXScaleLimits();
            gForm.CreateGraph();
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

            startScanTextLabel.Visible = false;
            label18.Visible = false;
            firstStepLabel.Visible = false;
            lastStepLabel.Visible = false;
            label37.Visible = true;
            peakNumberLabel.Visible = true;

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
            gForm.setXScaleLimits(Config.PreciseData);
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
            measure_StatusLabel.Text = msg;
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
                    systemStateValueTreeNode.Text = "������";
                    systemStateValueTreeNode.ForeColor = Color.Red;
                    break;
                case (byte)Device.DeviceStates.Init:
                    systemStateValueTreeNode.Text = "�������������";
                    systemStateValueTreeNode.ForeColor = Color.Blue;
                    break;
                case (byte)Device.DeviceStates.VacuumInit:
                    systemStateValueTreeNode.Text = "������������� �������";
                    systemStateValueTreeNode.ForeColor = Color.Blue;
                    break;
                case (byte)Device.DeviceStates.WaitHighVoltage:
                    systemStateValueTreeNode.Text = "�������� �������� ����������";
                    systemStateValueTreeNode.ForeColor = Color.Green;
                    break;
                case (byte)Device.DeviceStates.Ready:
                    systemStateValueTreeNode.Text = "������ � ���������";
                    systemStateValueTreeNode.ForeColor = Color.Green;
                    break;
                case (byte)Device.DeviceStates.Measuring:
                    systemStateValueTreeNode.Text = "������������ ���������";
                    systemStateValueTreeNode.ForeColor = Color.Green;
                    break;
                case (byte)Device.DeviceStates.Measured:
                    systemStateValueTreeNode.Text = "��������� ���������";
                    systemStateValueTreeNode.ForeColor = Color.Green;
                    break;
                case (byte)Device.DeviceStates.Shutdowning:
                    systemStateValueTreeNode.Text = "���� ����������";
                    systemStateValueTreeNode.ForeColor = Color.Blue;
                    break;
                case (byte)Device.DeviceStates.Shutdown:
                    systemStateValueTreeNode.Text = "����������";
                    systemStateValueTreeNode.ForeColor = Color.Red;
                    break;
                case (byte)Device.DeviceStates.TurboPumpFailure:
                    systemStateValueTreeNode.Text = "����� �����������";
                    systemStateValueTreeNode.ForeColor = Color.Red;
                    break;
                case (byte)Device.DeviceStates.VacuumCrash:
                    systemStateValueTreeNode.Text = "������ �������";
                    systemStateValueTreeNode.ForeColor = Color.Red;
                    break;
                default:
                    systemStateValueTreeNode.Text = "����������";
                    systemStateValueTreeNode.ForeColor = Color.Red;
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
            turboSpeedValueTreeNode.Text = string.Format("{0:f0}", Device.TurboPump.Speed);
            turboCurrentValueTreeNode.Text = string.Format("{0:f0}", Device.TurboPump.Current);
            pwmValueTreeNode.Text = string.Format("{0:f3}", Device.TurboPump.pwm);
            pumpTemperatureValueTreeNode.Text = string.Format("{0:f0}", Device.TurboPump.PumpTemperature);
            driveTemperatureValueTreeNode.Text = string.Format("{0:f0}", Device.TurboPump.DriveTemperature);
            operationTimeValueTreeNode.Text = string.Format("{0:f0}", Device.TurboPump.OperationTime);
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
                forPumpOnValueTreeNode.ForeColor = Color.Green;
                forPumpOnValueTreeNode.Text = "�������";
            }
            else
            {
                forPumpOnValueTreeNode.ForeColor = Color.Red;
                forPumpOnValueTreeNode.Text = "��������";
            }
            if (Device.tPumpOn)
            {
                turboPumpOnValueTreeNode.ForeColor = Color.Green;
                turboPumpOnValueTreeNode.Text = "�������";
            }
            else
            {
                turboPumpOnValueTreeNode.ForeColor = Color.Red;
                turboPumpOnValueTreeNode.Text = "��������";
            }
            forVacuumValueTreeNode.Text = string.Format("{0:e3}", Device.fVacuumReal);
            highVacuumValueTreeNode.Text = string.Format("{0:e3}", Device.hVacuumReal);
            if (Device.highVoltageOn)
            {
                hardwareBlockValueTreeNode.ForeColor = Color.Green;
                hardwareBlockValueTreeNode.Text = "��������";
            }
            else
            {
                hardwareBlockValueTreeNode.ForeColor = Color.Red;
                hardwareBlockValueTreeNode.Text = "���������";
            }
            if (Device.highVacuumValve)
            {
                vGate1ValueTreeNode.ForeColor = Color.Green;
                vGate1ValueTreeNode.Text = "������";
            }
            else
            {
                vGate1ValueTreeNode.ForeColor = Color.Red;
                vGate1ValueTreeNode.Text = "������";
            }
            if (Device.probeValve)
            {
                vGate2ValueTreeNode.ForeColor = Color.Green;
                vGate2ValueTreeNode.Text = "������";
            }
            else
            {
                vGate2ValueTreeNode.ForeColor = Color.Red;
                vGate2ValueTreeNode.Text = "������";
            }
            f1VoltageValueTreeNode.Text = string.Format("{0:f2}", Device.fV1Real);
            f2VoltageValueTreeNode.Text = string.Format("{0:f2}", Device.fV2Real);
            iVoltageValueTreeNode.Text = string.Format("{0:f2}", Device.iVoltageReal);
            detectorVoltageValueTreeNode.Text = string.Format("{0:f1}", Device.dVoltageReal);
            condPlusValueTreeNode.Text = string.Format("{0:f2}", Device.cVPlusReal);
            condMinusValueTreeNode.Text = string.Format("{0:f2}", Device.cVMinReal);
            scanVoltageValueTreeNode.Text = string.Format("{0:f1}", Device.sVoltageReal);
            eCurrentValueTreeNode.Text = string.Format("{0:f3}", Device.eCurrentReal);
            hCurrentValueTreeNode.Text = string.Format("{0:f3}", Device.hCurrentReal);
            turboSpeedValueTreeNode.Text = string.Format("{0:f0}", Device.TurboPump.Speed);
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
                    vacuumStateValueTreeNode.Text = "�����������";
                    vacuumStateValueTreeNode.ForeColor = Color.Blue;
                    break;
                case (byte)Device.VacuumStates.Init:
                    vacuumStateValueTreeNode.Text = "�������������";
                    vacuumStateValueTreeNode.ForeColor = Color.Blue;
                    break;
                case (byte)Device.VacuumStates.StartingForvacuumPump:
                    vacuumStateValueTreeNode.Text = "��������� ���������";
                    vacuumStateValueTreeNode.ForeColor = Color.Blue;
                    break;
                case (byte)Device.VacuumStates.PumpingForvacuum:
                    vacuumStateValueTreeNode.Text = "������� ����������";
                    vacuumStateValueTreeNode.ForeColor = Color.Blue;
                    break;
                case (byte)Device.VacuumStates.DelayPumpingHighVacuumByForvac:
                    vacuumStateValueTreeNode.Text = "�������� �������� ������� ��-�� ���";
                    vacuumStateValueTreeNode.ForeColor = Color.Blue;
                    break;
                case (byte)Device.VacuumStates.PumpingHighVacuumByForvac:
                    vacuumStateValueTreeNode.Text = "������� �������� ������� ����������";
                    vacuumStateValueTreeNode.ForeColor = Color.Blue;
                    break;
                case (byte)Device.VacuumStates.PumpingHighVacuumByTurbo:
                    vacuumStateValueTreeNode.Text = "������� �����";
                    vacuumStateValueTreeNode.ForeColor = Color.Blue;
                    break;
                case (byte)Device.VacuumStates.Ready:
                    vacuumStateValueTreeNode.Text = "������";
                    vacuumStateValueTreeNode.ForeColor = Color.Green;
                    break;
                case (byte)Device.VacuumStates.ShutdownInit:
                    vacuumStateValueTreeNode.Text = "������������� ����������";
                    vacuumStateValueTreeNode.ForeColor = Color.Blue;
                    break;
                case (byte)Device.VacuumStates.ShutdownDelay:
                    vacuumStateValueTreeNode.Text = "�������� ����������";
                    vacuumStateValueTreeNode.ForeColor = Color.Blue;
                    break;
                case (byte)Device.VacuumStates.ShutdownPumpProbe:
                    vacuumStateValueTreeNode.Text = "���������� ������� ������";
                    vacuumStateValueTreeNode.ForeColor = Color.Blue;
                    break;
                case (byte)Device.VacuumStates.Shutdowned:
                    vacuumStateValueTreeNode.Text = "���������";
                    vacuumStateValueTreeNode.ForeColor = Color.Red;
                    break;
                case (byte)Device.VacuumStates.ShutdownTurboPump:
                    vacuumStateValueTreeNode.Text = "������� ��� ����������";
                    vacuumStateValueTreeNode.ForeColor = Color.Blue;
                    break;
                case (byte)Device.VacuumStates.BadHighVacuum:
                    vacuumStateValueTreeNode.Text = "������ ������� ������";
                    vacuumStateValueTreeNode.ForeColor = Color.Red;
                    break;
                case (byte)Device.VacuumStates.BadForvacuum:
                    vacuumStateValueTreeNode.Text = "������ ���������";
                    vacuumStateValueTreeNode.ForeColor = Color.Red;
                    break;
                case (byte)Device.VacuumStates.ForvacuumFailure:
                    vacuumStateValueTreeNode.Text = "����� ����������";
                    vacuumStateValueTreeNode.ForeColor = Color.Red;
                    break;
                case (byte)Device.VacuumStates.LargeLeak:
                    vacuumStateValueTreeNode.Text = "������� ����";
                    vacuumStateValueTreeNode.ForeColor = Color.Red;
                    break;
                case (byte)Device.VacuumStates.SmallLeak:
                    vacuumStateValueTreeNode.Text = "����� ����";
                    vacuumStateValueTreeNode.ForeColor = Color.Red;
                    break;
                case (byte)Device.VacuumStates.ThermoCoupleFailure:
                    vacuumStateValueTreeNode.Text = "����� ���������";
                    vacuumStateValueTreeNode.ForeColor = Color.Red;
                    break;
                case (byte)Device.VacuumStates.TurboPumpFailure:
                    vacuumStateValueTreeNode.Text = "����� �����������";
                    vacuumStateValueTreeNode.ForeColor = Color.Red;
                    break;
                case (byte)Device.VacuumStates.VacuumShutdownProbeLeak:
                    vacuumStateValueTreeNode.Text = "���������� ������� �������";
                    vacuumStateValueTreeNode.ForeColor = Color.Red;
                    break;
                default:
                    vacuumStateValueTreeNode.Text = "����������� ���������";
                    vacuumStateValueTreeNode.ForeColor = Color.Red;
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