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
            Device.Init();

            Graph.OnNewGraphData += new Graph.GraphEventHandler(InvokeRefreshGraph);
            
            Commander.OnProgramStateChanged += new Commander.ProgramEventHandler(InvokeRefreshButtons);
            Commander.pState = Commander.programStates.Start;
            Commander.pStatePrev = Commander.pState;
        }

        private void mainForm_Load(object sender, EventArgs e)
        {
            Config.LoadConfig();
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
            this.gForm.CreateGraph(gForm.collect1_graph, gForm.collect2_graph); 
            this.gForm.collect1_graph.GraphPane.XAxis.Scale.Min = Config.sPoint;
            this.gForm.collect1_graph.GraphPane.XAxis.Scale.Max = Config.ePoint;
            this.gForm.collect2_graph.GraphPane.XAxis.Scale.Min = Config.sPoint;
            this.gForm.collect2_graph.GraphPane.XAxis.Scale.Max = Config.ePoint;
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
            this.gForm.collect1_graph.GraphPane.XAxis.Scale.Min = 0;
            this.gForm.collect1_graph.GraphPane.XAxis.Scale.Max = 1056;
            this.gForm.collect2_graph.GraphPane.XAxis.Scale.Min = 0;
            this.gForm.collect2_graph.GraphPane.XAxis.Scale.Max = 1056;
            Commander.OnScanCancelled += new Commander.ProgramEventHandler(InvokeCancelScan);
            Commander.Sense();
        }

        void InvokeRefreshUserMessage(string msg)
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

        public void InvokeRefreshGraph(bool fromFile, bool recreate)
        {
            if (this.InvokeRequired)
            {
                Graph.GraphEventHandler InvokeDelegate = new Graph.GraphEventHandler(RefreshGraph);
                this.Invoke(InvokeDelegate, fromFile, recreate);
            }
            else
            {
                RefreshGraph(fromFile, recreate);
            }
        }

        private void RefreshGraph(bool fromFile, bool recreate)
        {
            if (fromFile)
            {
                if (recreate)
                    gForm.DisplayLoadedSpectrum(gForm.collect1_graph, gForm.collect2_graph);
                else
                    gForm.RefreshGraph();
            }
            else
            {
                if (recreate)
                    gForm.CreateGraph(gForm.collect1_graph, gForm.collect2_graph);
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
            }
        }

        public void InvokeRefreshDeviceState()
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
                    this.systemState_label.Text = "Запуск";
                    this.systemState_label.ForeColor = Color.Red;
                    break;
                case (byte)Device.DeviceStates.Init:
                    this.systemState_label.Text = "Инициализация";
                    this.systemState_label.ForeColor = Color.Blue;
                    break;
                case (byte)Device.DeviceStates.VacuumInit:
                    this.systemState_label.Text = "Инициализация вакуума";
                    this.systemState_label.ForeColor = Color.Blue;
                    break;
                case (byte)Device.DeviceStates.WaitHighVoltage:
                    this.systemState_label.Text = "Ожидание высокого напряжения";
                    this.systemState_label.ForeColor = Color.Green;
                    break;
                case (byte)Device.DeviceStates.Ready:
                    this.systemState_label.Text = "Готова к измерению";
                    this.systemState_label.ForeColor = Color.Green;
                    break;
                case (byte)Device.DeviceStates.Measuring:
                    this.systemState_label.Text = "Производятся измерения";
                    this.systemState_label.ForeColor = Color.Green;
                    break;
                case (byte)Device.DeviceStates.Measured:
                    this.systemState_label.Text = "Измерения закончены";
                    this.systemState_label.ForeColor = Color.Green;
                    break;
                case (byte)Device.DeviceStates.Shutdowning:
                    this.systemState_label.Text = "Идет выключение";
                    this.systemState_label.ForeColor = Color.Blue;
                    break;
                case (byte)Device.DeviceStates.Shutdown:
                    this.systemState_label.Text = "Выключение";
                    this.systemState_label.ForeColor = Color.Red;
                    break;
                case (byte)Device.DeviceStates.TurboPumpFailure:
                    this.systemState_label.Text = "Отказ турбонасоса";
                    this.systemState_label.ForeColor = Color.Red;
                    break;
                case (byte)Device.DeviceStates.VacuumCrash:
                    this.systemState_label.Text = "Потеря вакуума";
                    this.systemState_label.ForeColor = Color.Red;
                    break;
                default:
                    this.systemState_label.Text = "Неизвестно";
                    this.systemState_label.ForeColor = Color.Red;
                    break;
            }
        }

        public void InvokeRefreshTurboPumpStatus() 
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

        public void InvokeRefreshDeviceStatus()
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
                this.forPumpOn_label.ForeColor = Color.Green;
                this.forPumpOn_label.Text = "Включен";
            }
            else
            {
                this.forPumpOn_label.ForeColor = Color.Red;
                this.forPumpOn_label.Text = "Выключен";
            }
            if (Device.tPumpOn)
            {
                this.tPumpOn_label.ForeColor = Color.Green;
                this.tPumpOn_label.Text = "Включен";
            }
            else
            {
                this.tPumpOn_label.ForeColor = Color.Red;
                this.tPumpOn_label.Text = "Выключен";
            }
            this.forVacuum_label.Text = string.Format("{0:e3}", Device.fVacuumReal);
            this.hVacuum_label.Text = string.Format("{0:e3}", Device.hVacuumReal);
            if (Device.highVoltageOn)
            {
                this.hardwareBlock_label.ForeColor = Color.Green;
                this.hardwareBlock_label.Text = "Включено";
            }
            else
            {
                this.hardwareBlock_label.ForeColor = Color.Red;
                this.hardwareBlock_label.Text = "Выключено";
            }
            if (Device.highVacuumValve)
            {
                this.vGate_label.ForeColor = Color.Green;
                this.vGate_label.Text = "Открыт";
            }
            else
            {
                this.vGate_label.ForeColor = Color.Red;
                this.vGate_label.Text = "Закрыт";
            }
            if (Device.probeValve)
            {
                this.hGate_label.ForeColor = Color.Green;
                this.hGate_label.Text = "Открыт";
            }
            else
            {
                this.hGate_label.ForeColor = Color.Red;
                this.hGate_label.Text = "Закрыт";
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

        public void InvokeRefreshVacuumState()
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
                    this.VacuumState_label.Text = "Бездействие";
                    this.VacuumState_label.ForeColor = Color.Blue;
                    break;
                case (byte)Device.VacuumStates.Init:
                    this.VacuumState_label.Text = "Инициализация";
                    this.VacuumState_label.ForeColor = Color.Blue;
                    break;
                case (byte)Device.VacuumStates.StartingForvacuumPump:
                    this.VacuumState_label.Text = "Включение форнасоса";
                    this.VacuumState_label.ForeColor = Color.Blue;
                    break;
                case (byte)Device.VacuumStates.PumpingForvacuum:
                    this.VacuumState_label.Text = "Откачка форвакуума";
                    this.VacuumState_label.ForeColor = Color.Blue;
                    break;
                case (byte)Device.VacuumStates.DelayPumpingHighVacuumByForvac:
                    this.VacuumState_label.Text = "Задержка высокого вакуума из-за фор";
                    this.VacuumState_label.ForeColor = Color.Blue;
                    break;
                case (byte)Device.VacuumStates.PumpingHighVacuumByForvac:
                    this.VacuumState_label.Text = "Откачка высокого вакуума форнасосом";
                    this.VacuumState_label.ForeColor = Color.Blue;
                    break;
                case (byte)Device.VacuumStates.PumpingHighVacuumByTurbo:
                    this.VacuumState_label.Text = "Откачка турбо";
                    this.VacuumState_label.ForeColor = Color.Blue;
                    break;
                case (byte)Device.VacuumStates.Ready:
                    this.VacuumState_label.Text = "Готово";
                    this.VacuumState_label.ForeColor = Color.Green;
                    break;
                case (byte)Device.VacuumStates.ShutdownInit:
                    this.VacuumState_label.Text = "Инициализация отключения";
                    this.VacuumState_label.ForeColor = Color.Blue;
                    break;
                case (byte)Device.VacuumStates.ShutdownDelay:
                    this.VacuumState_label.Text = "Задержка отключения";
                    this.VacuumState_label.ForeColor = Color.Blue;
                    break;
                case (byte)Device.VacuumStates.ShutdownPumpProbe:
                    this.VacuumState_label.Text = "Отключение датчика насоса";
                    this.VacuumState_label.ForeColor = Color.Blue;
                    break;
                case (byte)Device.VacuumStates.Shutdowned:
                    this.VacuumState_label.Text = "Отключено";
                    this.VacuumState_label.ForeColor = Color.Red;
                    break;
                case (byte)Device.VacuumStates.ShutdownTurboPump:
                    this.VacuumState_label.Text = "Откачка при выключении";
                    this.VacuumState_label.ForeColor = Color.Blue;
                    break;
                case (byte)Device.VacuumStates.BadHighVacuum:
                    this.VacuumState_label.Text = "Плохой высокий вакуум";
                    this.VacuumState_label.ForeColor = Color.Red;
                    break;
                case (byte)Device.VacuumStates.BadForvacuum:
                    this.VacuumState_label.Text = "Плохой форвакуум";
                    this.VacuumState_label.ForeColor = Color.Red;
                    break;
                case (byte)Device.VacuumStates.ForvacuumFailure:
                    this.VacuumState_label.Text = "Отказ форвакуума";
                    this.VacuumState_label.ForeColor = Color.Red;
                    break;
                case (byte)Device.VacuumStates.LargeLeak:
                    this.VacuumState_label.Text = "Большая течь";
                    this.VacuumState_label.ForeColor = Color.Red;
                    break;
                case (byte)Device.VacuumStates.SmallLeak:
                    this.VacuumState_label.Text = "Малая течь";
                    this.VacuumState_label.ForeColor = Color.Red;
                    break;
                case (byte)Device.VacuumStates.ThermoCoupleFailure:
                    this.VacuumState_label.Text = "Отказ термопары";
                    this.VacuumState_label.ForeColor = Color.Red;
                    break;
                case (byte)Device.VacuumStates.VacuumShutdownProbeLeak:
                    this.VacuumState_label.Text = "Отключение датчика вакуума";
                    this.VacuumState_label.ForeColor = Color.Red;
                    break;
                default:
                    this.VacuumState_label.Text = "Неизвестное состояние";
                    this.VacuumState_label.ForeColor = Color.Red;
                    break;
            }
        }

        public void InvokeRefreshButtons() 
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

        public void RefreshButtons()
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

        public void InvokeCancelScan()
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
            Config.LoadConfig();
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
    }
}