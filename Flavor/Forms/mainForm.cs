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
        private TreeNodePlus rootNode;

        private TreeNodeLeaf systemStateValueTreeNode;
        private TreeNodeLeaf vacuumStateValueTreeNode;
        private TreeNodeLeaf forPumpOnValueTreeNode;
        private TreeNodeLeaf turboPumpOnValueTreeNode;
        private TreeNodeLeaf forVacuumValueTreeNode;
        private TreeNodeLeaf highVacuumValueTreeNode;
        private TreeNodeLeaf hardwareBlockValueTreeNode;
        private TreeNodeLeaf vGate1ValueTreeNode;
        private TreeNodeLeaf vGate2ValueTreeNode;

        private TreeNodeLeaf f1VoltageValueTreeNode;
        private TreeNodeLeaf f2VoltageValueTreeNode;
        private TreeNodeLeaf scanVoltageValueTreeNode;
        private TreeNodeLeaf iVoltageValueTreeNode;
        private TreeNodeLeaf eCurrentValueTreeNode;
        private TreeNodeLeaf condPlusValueTreeNode;
        private TreeNodeLeaf condMinusValueTreeNode;
        private TreeNodeLeaf detectorVoltageValueTreeNode;
        private TreeNodeLeaf hCurrentValueTreeNode;

        private TreeNodeLeaf turboSpeedValueTreeNode;
        private TreeNodeLeaf turboCurrentValueTreeNode;
        private TreeNodeLeaf pumpTemperatureValueTreeNode;
        private TreeNodeLeaf driveTemperatureValueTreeNode;
        private TreeNodeLeaf pwmValueTreeNode;
        private TreeNodeLeaf operationTimeValueTreeNode;
        
        private void populateStatusTreeView()
        {
            TreeNodePlus infoNode;

            TreeNodePair systemStateTextTreeNode;
            TreeNodePair vacuumStateTextTreeNode;
            TreeNodePair forPumpOnTextTreeNode;
            TreeNodePair turboPumpOnTextTreeNode;
            TreeNodePair forVacuumTextTreeNode;
            TreeNodePair highVacuumTextTreeNode;
            TreeNodePair hardwareBlockTextTreeNode;
            TreeNodePair vGate1TextTreeNode;
            TreeNodePair vGate2TextTreeNode;

            TreeNodePlus extraNode;

            TreeNodePair f1VoltageTextTreeNode;
            TreeNodePair f2VoltageTextTreeNode;
            TreeNodePair scanVoltageTextTreeNode;
            TreeNodePair iVoltageTextTreeNode;
            TreeNodePair eCurrentTextTreeNode;
            TreeNodePair condPlusTextTreeNode;
            TreeNodePair condMinusTextTreeNode;
            TreeNodePair detectorVoltageTextTreeNode;
            TreeNodePair hCurrentTextTreeNode;
        
            TreeNodePlus turboPumpNode;

            TreeNodePair turboSpeedTextTreeNode;
            TreeNodePair turboCurrentTextTreeNode;
            TreeNodePair pumpTemperatureTextTreeNode;
            TreeNodePair driveTemperatureTextTreeNode;
            TreeNodePair pwmTextTreeNode;
            TreeNodePair operationTimeTextTreeNode;

            systemStateValueTreeNode = new TreeNodeLeaf();
            systemStateTextTreeNode = new TreeNodePair("Состояние системы", systemStateValueTreeNode);
            vacuumStateValueTreeNode = new TreeNodeLeaf();
            vacuumStateTextTreeNode = new TreeNodePair("Состояние вакуума", vacuumStateValueTreeNode);
            forPumpOnValueTreeNode = new TreeNodeLeaf();
            forPumpOnTextTreeNode = new TreeNodePair("Форвакуумный насос", forPumpOnValueTreeNode);
            turboPumpOnValueTreeNode = new TreeNodeLeaf();
            turboPumpOnTextTreeNode = new TreeNodePair("Турбомолекулярный насос", turboPumpOnValueTreeNode);
            forVacuumValueTreeNode = new TreeNodeLeaf();
            forVacuumTextTreeNode = new TreeNodePair("Уровень вакуума (фор)", forVacuumValueTreeNode);
            highVacuumValueTreeNode = new TreeNodeLeaf();
            highVacuumTextTreeNode = new TreeNodePair("Уровень вакуума (высок)", highVacuumValueTreeNode);
            hardwareBlockValueTreeNode = new TreeNodeLeaf();
            hardwareBlockTextTreeNode = new TreeNodePair("Высокое напряжение", hardwareBlockValueTreeNode);
            vGate1ValueTreeNode = new TreeNodeLeaf();
            vGate1TextTreeNode = new TreeNodePair("Вакуумный вентиль 1", vGate1ValueTreeNode);
            vGate2ValueTreeNode = new TreeNodeLeaf();
            vGate2TextTreeNode = new TreeNodePair("Вакуумный вентиль 2", vGate2ValueTreeNode);

            infoNode = new TreeNodePlus("Информация о системе",
                new TreeNode[] { systemStateTextTreeNode, vacuumStateTextTreeNode, forPumpOnTextTreeNode, turboPumpOnTextTreeNode, 
                    forVacuumTextTreeNode, highVacuumTextTreeNode, hardwareBlockTextTreeNode, vGate1TextTreeNode, vGate2TextTreeNode });

            f1VoltageValueTreeNode = new TreeNodeLeaf();
            f1VoltageTextTreeNode = new TreeNodePair("Фокусирующее напр. (1) В", f1VoltageValueTreeNode);
            f2VoltageValueTreeNode = new TreeNodeLeaf();
            f2VoltageTextTreeNode = new TreeNodePair("Фокусирующее напр. (2) В", f2VoltageValueTreeNode);
            scanVoltageValueTreeNode = new TreeNodeLeaf();
            scanVoltageTextTreeNode = new TreeNodePair("Напряжение развертки В", scanVoltageValueTreeNode);
            iVoltageValueTreeNode = new TreeNodeLeaf();
            iVoltageTextTreeNode = new TreeNodePair("Напряжение ионизации, В", iVoltageValueTreeNode);
            eCurrentValueTreeNode = new TreeNodeLeaf();
            eCurrentTextTreeNode = new TreeNodePair("Ток эмиссии, мкА", eCurrentValueTreeNode);
            condPlusValueTreeNode = new TreeNodeLeaf();
            condPlusTextTreeNode = new TreeNodePair("Напряжение конденсатора (+), В", condPlusValueTreeNode);
            condMinusValueTreeNode = new TreeNodeLeaf();
            condMinusTextTreeNode = new TreeNodePair("Напряжение конденсатора (-), В", condMinusValueTreeNode);
            detectorVoltageValueTreeNode = new TreeNodeLeaf();
            detectorVoltageTextTreeNode = new TreeNodePair("Напряжение на детекторе, В", detectorVoltageValueTreeNode);
            hCurrentValueTreeNode = new TreeNodeLeaf();
            hCurrentTextTreeNode = new TreeNodePair("Ток нагрева, А", hCurrentValueTreeNode);

            extraNode = new TreeNodePlus("Дополнительно",
                new TreeNode[] { f1VoltageTextTreeNode, f2VoltageTextTreeNode, scanVoltageTextTreeNode, iVoltageTextTreeNode, eCurrentTextTreeNode,
                    condPlusTextTreeNode, condMinusTextTreeNode, detectorVoltageTextTreeNode,  hCurrentTextTreeNode });

            turboSpeedValueTreeNode = new TreeNodeLeaf();
            turboSpeedTextTreeNode = new TreeNodePair("Скорость вращения, об./мин.", turboSpeedValueTreeNode);
            turboCurrentValueTreeNode = new TreeNodeLeaf();
            turboCurrentTextTreeNode = new TreeNodePair("Ток, мА", turboCurrentValueTreeNode);
            pumpTemperatureValueTreeNode = new TreeNodeLeaf();
            pumpTemperatureTextTreeNode = new TreeNodePair("Температура насоса", pumpTemperatureValueTreeNode);
            driveTemperatureValueTreeNode = new TreeNodeLeaf();
            driveTemperatureTextTreeNode = new TreeNodePair("Температура привода", driveTemperatureValueTreeNode);
            pwmValueTreeNode = new TreeNodeLeaf();
            pwmTextTreeNode = new TreeNodePair("pwm", pwmValueTreeNode);
            operationTimeValueTreeNode = new TreeNodeLeaf();
            operationTimeTextTreeNode = new TreeNodePair("Время работы", operationTimeValueTreeNode);

            turboPumpNode = new TreeNodePlus("Турбонасос", 
                new TreeNode[] { turboSpeedTextTreeNode, turboCurrentTextTreeNode, pumpTemperatureTextTreeNode, driveTemperatureTextTreeNode,
                    pwmTextTreeNode, operationTimeTextTreeNode });

            rootNode = new TreeNodePlus("Состояние системы",
                new TreeNode[] { infoNode, extraNode, turboPumpNode });
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
        private void monitorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Prec/Mon forms simultaneously howto?
            MonitorOptionsForm mForm = MonitorOptionsForm.getInstance();
            mForm.UpLevel = this;
            mForm.Show();
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
        private void prepareControlsOnMeasureStart()
        {
            overview_button.Enabled = false;
            sensmeasure_button.Enabled = false;
            monitorToolStripButton.Enabled = false;

            // put here code that only changes data source and refreshes
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

            etime_label.Text = Config.CommonOptions.eTimeReal.ToString();
            itime_label.Text = Config.CommonOptions.iTimeReal.ToString();
            iVolt_label.Text = Config.CommonOptions.iVoltageReal.ToString("f3");
            cp_label.Text = Config.CommonOptions.CPReal.ToString("f3");
            emCurLabel.Text = Config.CommonOptions.eCurrentReal.ToString("f3");
            heatCurLabel.Text = Config.CommonOptions.hCurrentReal.ToString("f3");
            f1_label.Text = Config.CommonOptions.fV1Real.ToString("f3");
            f2_label.Text = Config.CommonOptions.fV2Real.ToString("f3");
            scanProgressBar.Value = 0;
            scanProgressBar.Maximum = Config.ePoint - Config.sPoint;
            scanProgressBar.Step = 1;
            cancelScanButton.Enabled = true;
            cancelScanButton.Visible = true;
        }
        private void overview_button_Click(object sender, EventArgs e)
        {
            prepareControlsOnMeasureStart();

            startScanTextLabel.Visible = true;
            label18.Visible = true;
            firstStepLabel.Visible = true;
            lastStepLabel.Visible = true;
            label37.Visible = false;
            peakNumberLabel.Visible = false;

            firstStepLabel.Text = Config.sPoint.ToString();
            lastStepLabel.Text = Config.ePoint.ToString();

            Graph.ResetLoadedPointLists();
            gForm.setXScaleLimits();
            gForm.CreateGraph();
            Commander.OnScanCancelled += new Commander.ProgramEventHandler(InvokeCancelScan);
            gForm.specterSavingEnabled = false;
            Commander.Scan();
        }
        private void sensmeasure_button_Click(object sender, EventArgs e)
        {
            prepareControlsOnMeasureStart();

            startScanTextLabel.Visible = false;
            label18.Visible = false;
            firstStepLabel.Visible = false;
            lastStepLabel.Visible = false;
            label37.Visible = true;
            peakNumberLabel.Visible = true;

            scanProgressBar.Value = 0;
            scanProgressBar.Maximum = 0;
            foreach (Utility.PreciseEditorData ped in Config.PreciseData)
            {
                if (ped.Use)
                    scanProgressBar.Maximum += (2 * ped.Width + 1) * ped.Iterations;
            }
            scanProgressBar.Step = 1;

            Graph.ResetLoadedPointLists();
            gForm.setXScaleLimits(Config.PreciseData);
            Commander.OnScanCancelled += new Commander.ProgramEventHandler(InvokeCancelScan);
            gForm.specterSavingEnabled = false;
            Commander.Sense();
        }
        private void monitorToolStripButton_Click(object sender, EventArgs e)
        {
            prepareControlsOnMeasureStart();

            startScanTextLabel.Visible = false;
            label18.Visible = false;
            firstStepLabel.Visible = false;
            lastStepLabel.Visible = false;
            label37.Visible = true;
            peakNumberLabel.Visible = true;

            //!!! other values here
            scanProgressBar.Value = 0;
            scanProgressBar.Maximum = 0;
            foreach (Utility.PreciseEditorData ped in Config.PreciseData)
            {
                if (ped.Use)
                    scanProgressBar.Maximum += (2 * ped.Width + 1) * ped.Iterations;
            }
            scanProgressBar.Step = 1;

            Graph.ResetLoadedPointLists();
            gForm.setXScaleLimits(Config.PreciseData);
            Commander.OnScanCancelled += new Commander.ProgramEventHandler(InvokeCancelScan);
            gForm.specterSavingEnabled = false;
            Commander.Monitor();
        }

        private void InvokeProcessTurboPumpAlert(bool isFault, byte bits)
        {
            string msg = "Turbopump: ";
            msg += isFault ? "failure (" : "warning (";
            msg += bits.ToString("X2");
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
                        scanRealTimeLabel.Text = Config.CommonOptions.scanVoltageReal(Graph.LastPoint).ToString("f1");
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
                            //gForm.specterSavingEnabled = true;
                            
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
            parameterPanel.SuspendLayout();
            statusTreeView.BeginUpdate();
            switch (Device.sysState)
            {
                case (byte)Device.DeviceStates.Start:
                    systemStateValueTreeNode.Text = "Запуск";
                    systemStateValueTreeNode.State = TreeNodePlus.States.Warning;
                    break;
                case (byte)Device.DeviceStates.Init:
                    systemStateValueTreeNode.Text = "Инициализация";
                    systemStateValueTreeNode.State = TreeNodePlus.States.Warning;
                    break;
                case (byte)Device.DeviceStates.VacuumInit:
                    systemStateValueTreeNode.Text = "Инициализация вакуума";
                    systemStateValueTreeNode.State = TreeNodePlus.States.Warning;
                    break;
                case (byte)Device.DeviceStates.WaitHighVoltage:
                    systemStateValueTreeNode.Text = "Ожидание высокого напряжения";
                    systemStateValueTreeNode.State = TreeNodePlus.States.Ok;
                    break;
                case (byte)Device.DeviceStates.Ready:
                    systemStateValueTreeNode.Text = "Готова к измерению";
                    systemStateValueTreeNode.State = TreeNodePlus.States.Ok;
                    break;
                case (byte)Device.DeviceStates.Measuring:
                    systemStateValueTreeNode.Text = "Производятся измерения";
                    systemStateValueTreeNode.State = TreeNodePlus.States.Ok;
                    break;
                case (byte)Device.DeviceStates.Measured:
                    systemStateValueTreeNode.Text = "Измерения закончены";
                    systemStateValueTreeNode.State = TreeNodePlus.States.Ok;
                    break;
                case (byte)Device.DeviceStates.ShutdownInit:
                    systemStateValueTreeNode.Text = "Идициализация выключения";
                    systemStateValueTreeNode.State = TreeNodePlus.States.Warning;
                    break;
                case (byte)Device.DeviceStates.Shutdowning:
                    systemStateValueTreeNode.Text = "Идет выключение";
                    systemStateValueTreeNode.State = TreeNodePlus.States.Warning;
                    break;
                case (byte)Device.DeviceStates.Shutdowned:
                    systemStateValueTreeNode.Text = "Выключено";
                    systemStateValueTreeNode.State = TreeNodePlus.States.Warning;
                    break;
                case (byte)Device.DeviceStates.TurboPumpFailure:
                    systemStateValueTreeNode.Text = "Отказ турбонасоса";
                    systemStateValueTreeNode.State = TreeNodePlus.States.Error;
                    break;
                case (byte)Device.DeviceStates.VacuumCrash:
                    systemStateValueTreeNode.Text = "Потеря вакуума";
                    systemStateValueTreeNode.State = TreeNodePlus.States.Error;
                    break;
                case (byte)Device.DeviceStates.ConstantsWrite:
                    systemStateValueTreeNode.Text = "Запись констант";
                    systemStateValueTreeNode.State = TreeNodePlus.States.Warning;
                    break;
                default:
                    systemStateValueTreeNode.Text = "Неизвестно";
                    systemStateValueTreeNode.State = TreeNodePlus.States.Error;
                    break;
            }
            statusTreeView.EndUpdate();
            parameterPanel.ResumeLayout();
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
            parameterPanel.SuspendLayout();
            statusTreeView.BeginUpdate();

            turboSpeedValueTreeNode.Text = Device.TurboPump.Speed.ToString("f0");
            turboCurrentValueTreeNode.Text = Device.TurboPump.Current.ToString("f0");
            pwmValueTreeNode.Text = Device.TurboPump.pwm.ToString("f0");
            pumpTemperatureValueTreeNode.Text = Device.TurboPump.PumpTemperature.ToString("f0");
            driveTemperatureValueTreeNode.Text = Device.TurboPump.DriveTemperature.ToString("f0");
            operationTimeValueTreeNode.Text = Device.TurboPump.OperationTime.ToString("f0");
            
            statusTreeView.EndUpdate();
            parameterPanel.ResumeLayout();
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
            parameterPanel.SuspendLayout();
            statusTreeView.BeginUpdate();
            if (Device.fPumpOn)
            {
                forPumpOnValueTreeNode.State = TreeNodePlus.States.Ok;
                forPumpOnValueTreeNode.Text = "Включен";
            }
            else
            {
                forPumpOnValueTreeNode.State = TreeNodePlus.States.Error;
                forPumpOnValueTreeNode.Text = "Выключен";
            }
            if (Device.tPumpOn)
            {
                turboPumpOnValueTreeNode.State = TreeNodePlus.States.Ok;
                turboPumpOnValueTreeNode.Text = "Включен";
            }
            else
            {
                turboPumpOnValueTreeNode.State = TreeNodePlus.States.Error;
                turboPumpOnValueTreeNode.Text = "Выключен";
            }
            forVacuumValueTreeNode.Text = string.Format("{0:e3}", Device.fVacuumReal);
            highVacuumValueTreeNode.Text = string.Format("{0:e3}", Device.hVacuumReal);
            if (Device.highVoltageOn)
            {
                hardwareBlockValueTreeNode.State = TreeNodePlus.States.Ok;
                hardwareBlockValueTreeNode.Text = "Включено";
            }
            else
            {
                hardwareBlockValueTreeNode.State = TreeNodePlus.States.Warning;
                hardwareBlockValueTreeNode.Text = "Выключено";
            }
            if (Device.highVacuumValve)
            {
                vGate1ValueTreeNode.State = TreeNodePlus.States.Ok;
                vGate1ValueTreeNode.Text = "Открыт";
            }
            else
            {
                vGate1ValueTreeNode.State = TreeNodePlus.States.Warning;
                vGate1ValueTreeNode.Text = "Закрыт";
            }
            if (Device.probeValve)
            {
                vGate2ValueTreeNode.State = TreeNodePlus.States.Warning;
                vGate2ValueTreeNode.Text = "Открыт";
            }
            else
            {
                vGate2ValueTreeNode.State = TreeNodePlus.States.Ok;
                vGate2ValueTreeNode.Text = "Закрыт";
            }
            f1VoltageValueTreeNode.Text = Device.DeviceCommonData.fV1Real.ToString("f2");
            f2VoltageValueTreeNode.Text = Device.DeviceCommonData.fV2Real.ToString("f2");
            iVoltageValueTreeNode.Text = Device.DeviceCommonData.iVoltageReal.ToString("f2");
            detectorVoltageValueTreeNode.Text = Device.DeviceCommonData.dVoltageReal.ToString("f1");
            condPlusValueTreeNode.Text = Device.DeviceCommonData.cVPlusReal.ToString("f2");
            condMinusValueTreeNode.Text = Device.DeviceCommonData.cVMinReal.ToString("f2");
            scanVoltageValueTreeNode.Text = Device.DeviceCommonData.sVoltageReal.ToString("f1");
            eCurrentValueTreeNode.Text = Device.DeviceCommonData.eCurrentReal.ToString("f3");
            hCurrentValueTreeNode.Text = Device.DeviceCommonData.hCurrentReal.ToString("f3");
            turboSpeedValueTreeNode.Text = Device.TurboPump.Speed.ToString("f0");

            statusTreeView.EndUpdate();
            parameterPanel.ResumeLayout();
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
            parameterPanel.SuspendLayout();
            statusTreeView.BeginUpdate();
            switch (Device.vacState) 
            {
                case (byte)Device.VacuumStates.Idle:
                    vacuumStateValueTreeNode.Text = "Бездействие";
                    vacuumStateValueTreeNode.State = TreeNodePlus.States.Warning;
                    break;
                case (byte)Device.VacuumStates.Init:
                    vacuumStateValueTreeNode.Text = "Инициализация";
                    vacuumStateValueTreeNode.State = TreeNodePlus.States.Warning;
                    break;
                case (byte)Device.VacuumStates.StartingForvacuumPump:
                    vacuumStateValueTreeNode.Text = "Включение форнасоса";
                    vacuumStateValueTreeNode.State = TreeNodePlus.States.Warning;
                    break;
                case (byte)Device.VacuumStates.PumpingForvacuum:
                    vacuumStateValueTreeNode.Text = "Откачка форвакуума";
                    vacuumStateValueTreeNode.State = TreeNodePlus.States.Warning;
                    break;
                case (byte)Device.VacuumStates.DelayPumpingHighVacuumByForvac:
                    vacuumStateValueTreeNode.Text = "Задержка высокого вакуума из-за фор";
                    vacuumStateValueTreeNode.State = TreeNodePlus.States.Warning;
                    break;
                case (byte)Device.VacuumStates.PumpingHighVacuumByForvac:
                    vacuumStateValueTreeNode.Text = "Откачка высокого вакуума форнасосом";
                    vacuumStateValueTreeNode.State = TreeNodePlus.States.Warning;
                    break;
                case (byte)Device.VacuumStates.PumpingHighVacuumByTurbo:
                    vacuumStateValueTreeNode.Text = "Откачка турбо";
                    vacuumStateValueTreeNode.State = TreeNodePlus.States.Warning;
                    break;
                case (byte)Device.VacuumStates.Ready:
                    vacuumStateValueTreeNode.Text = "Готово";
                    vacuumStateValueTreeNode.State = TreeNodePlus.States.Ok;
                    break;
                case (byte)Device.VacuumStates.ShutdownInit:
                    vacuumStateValueTreeNode.Text = "Инициализация отключения";
                    vacuumStateValueTreeNode.State = TreeNodePlus.States.Warning;
                    break;
                case (byte)Device.VacuumStates.ShutdownDelay:
                    vacuumStateValueTreeNode.Text = "Задержка отключения";
                    vacuumStateValueTreeNode.State = TreeNodePlus.States.Warning;
                    break;
                case (byte)Device.VacuumStates.ShutdownPumpProbe:
                    vacuumStateValueTreeNode.Text = "Отключение датчика насоса";
                    vacuumStateValueTreeNode.State = TreeNodePlus.States.Warning;
                    break;
                case (byte)Device.VacuumStates.Shutdowned:
                    vacuumStateValueTreeNode.Text = "Отключено";
                    vacuumStateValueTreeNode.State = TreeNodePlus.States.Warning;
                    break;
                case (byte)Device.VacuumStates.ShutdownStartingTurboPump:
                    vacuumStateValueTreeNode.Text = "Откачка при выключении";
                    vacuumStateValueTreeNode.State = TreeNodePlus.States.Warning;
                    break;
                case (byte)Device.VacuumStates.BadHighVacuum:
                    vacuumStateValueTreeNode.Text = "Плохой высокий вакуум";
                    vacuumStateValueTreeNode.State = TreeNodePlus.States.Error;
                    break;
                case (byte)Device.VacuumStates.BadForvacuum:
                    vacuumStateValueTreeNode.Text = "Плохой форвакуум";
                    vacuumStateValueTreeNode.State = TreeNodePlus.States.Error;
                    break;
                case (byte)Device.VacuumStates.ForvacuumFailure:
                    vacuumStateValueTreeNode.Text = "Отказ форвакуума";
                    vacuumStateValueTreeNode.State = TreeNodePlus.States.Error;
                    break;
                case (byte)Device.VacuumStates.LargeLeak:
                    vacuumStateValueTreeNode.Text = "Большая течь";
                    vacuumStateValueTreeNode.State = TreeNodePlus.States.Error;
                    break;
                case (byte)Device.VacuumStates.SmallLeak:
                    vacuumStateValueTreeNode.Text = "Малая течь";
                    vacuumStateValueTreeNode.State = TreeNodePlus.States.Error;
                    break;
                case (byte)Device.VacuumStates.ThermoCoupleFailure:
                    vacuumStateValueTreeNode.Text = "Отказ термопары";
                    vacuumStateValueTreeNode.State = TreeNodePlus.States.Error;
                    break;
                case (byte)Device.VacuumStates.TurboPumpFailure:
                    vacuumStateValueTreeNode.Text = "Отказ турбонасоса";
                    vacuumStateValueTreeNode.State = TreeNodePlus.States.Error;
                    break;
                case (byte)Device.VacuumStates.VacuumShutdownProbeLeak:
                    vacuumStateValueTreeNode.Text = "Отключение датчика вакуума";
                    vacuumStateValueTreeNode.State = TreeNodePlus.States.Error;
                    break;
                default:
                    vacuumStateValueTreeNode.Text = "Неизвестное состояние";
                    vacuumStateValueTreeNode.State = TreeNodePlus.States.Error;
                    break;
            }
            statusTreeView.EndUpdate();
            parameterPanel.ResumeLayout();
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
                    unblock_butt.Text = "Снять блокировку";
                    unblock_butt.ForeColor = Color.Green;
                    break;
                case false:
                    unblock_butt.Text = "Включить блокировку";
                    unblock_butt.ForeColor = Color.Red;
                    break;
            }
            switch (Commander.pState)
            {
                case Commander.programStates.Start:
                    connectToolStripButton.Enabled = true;
                    if (Commander.deviceIsConnected)
                    {
                        connectToolStripButton.Text = "Разъединить";
                        connectToolStripButton.ForeColor = Color.Red;
                    }
                    else
                    {
                        connectToolStripButton.Text = "Соединить";
                        connectToolStripButton.ForeColor = Color.Green;
                    }

                    initSys_butt.Enabled = Commander.deviceIsConnected;//true;
                    shutSys_butt.Enabled = Commander.deviceIsConnected;//false;
                    unblock_butt.Enabled = Commander.deviceIsConnected && !Commander.hBlock;//разрешено для включения блокировки
                    overview_button.Enabled = false;
                    sensmeasure_button.Enabled = false;
                    monitorToolStripButton.Enabled = false;
                    
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
                    monitorToolStripButton.Enabled = false;

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
                    monitorToolStripButton.Enabled = false;

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
                    monitorToolStripButton.Enabled = false;

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
                    monitorToolStripButton.Enabled = true & !Commander.hBlock & precPointsExist;//&& smth else?;

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
                    monitorToolStripButton.Enabled = false;

                    connectToolStripMenuItem.Enabled = false;
                    measureToolStripMenuItem.Enabled = false;

                    measurePanelToolStripMenuItem.Enabled = true;

                    gForm.specterOpeningEnabled = false;
                    //gForm.specterSavingEnabled = false; 
                    break;
                case Commander.programStates.WaitShutdown:
                    connectToolStripButton.Enabled = false;

                    initSys_butt.Enabled = false;
                    shutSys_butt.Enabled = false;
                    unblock_butt.Enabled = false;
                    overview_button.Enabled = false;
                    sensmeasure_button.Enabled = false;
                    monitorToolStripButton.Enabled = false;

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
                    monitorToolStripButton.Enabled = false;

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
            gForm.specterSavingEnabled = true; 
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
            // and some action with measurePanel
        }

        private void ParameterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            parameterPanel.Visible = ParameterToolStripMenuItem.Checked;
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
                Config.loadConfig();
            }
            catch (Config.ConfigLoadException cle)
            {
                cle.visualise();
            }
        }

        private void saveConfigFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Config.saveAll();
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

        private void mainForm_MdiChildActivate(object sender, EventArgs e)
        {
            // stub
            // set data source of measurePanel according to spectrum displayed
            // and refresh it
        }
    }
}