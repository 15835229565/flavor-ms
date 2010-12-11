using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Timers;
using Flavor.Common;
using Flavor.Common.Measuring;
using Flavor.Controls;

namespace Flavor.Forms {
    internal partial class mainForm: Form {
        private const string EXIT_CAPTION = "Предупреждение об отключении";
        private const string EXIT_MESSAGE = "Следует дождаться отключения системы.\nОтключить программу, несмотря на предупреждение?";
        private const string SHUTDOWN_CAPTION = "Предупреждение об отключении";
        private const string SHUTDOWN_MESSAGE = "Внимание!\n" +
                                                "Проверьте герметизацию системы ввода перед отключением прибора (установку заглушки).\n" +
                                                "При успешном окончании проверки нажмите OK. Для отмены отключения прибора нажмите Отмена.";

        private MeasuredCollectorsForm collectorsForm = null;
        private MeasuredCollectorsForm CollectorsForm {
            get {
                if (collectorsForm == null) {
                    collectorsForm = new MeasuredCollectorsForm();
                    collectorsForm.MdiParent = this;
                }
                return collectorsForm;
            }
        }
        private MonitorForm monitorForm = null;
        private MonitorForm MonitorForm {
            get {
                if (monitorForm == null) {
                    monitorForm = new MonitorForm();
                    monitorForm.MdiParent = this;
                }
                return monitorForm;
            }
        }
        private GraphForm GForm {
            get {
                Form child = ActiveMdiChild;
                if (child == null)
                    return CollectorsForm;
                return child as GraphForm;
            }
        }
		
        internal mainForm() {
            InitializeComponent();
            populateStatusTreeView();
            CollectorsForm.Visible = true;
            MonitorForm.Visible = false;

            Config.getInitialDirectory();

            Device.OnDeviceStateChanged += new DeviceEventHandler(InvokeRefreshDeviceState);
            Device.OnDeviceStatusChanged += new DeviceEventHandler(InvokeRefreshDeviceStatus);
            Device.OnVacuumStateChanged += new DeviceEventHandler(InvokeRefreshVacuumState);
            Device.OnTurboPumpStatusChanged += new DeviceEventHandler(InvokeRefreshTurboPumpStatus);
            Device.OnTurboPumpAlert += new TurboPumpAlertEventHandler(InvokeProcessTurboPumpAlert);
            Device.Init();

            Commander.OnProgramStateChanged += new Commander.ProgramEventHandler(InvokeRefreshButtons);
            Commander.setProgramStateWithoutUndo(Commander.programStates.Start);
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

        private void populateStatusTreeView() {
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
            operationTimeTextTreeNode = new TreeNodePair("Время работы, ч.", operationTimeValueTreeNode);

            turboPumpNode = new TreeNodePlus("Турбонасос",
                new TreeNode[] { turboSpeedTextTreeNode, turboCurrentTextTreeNode, pumpTemperatureTextTreeNode, driveTemperatureTextTreeNode,
                    pwmTextTreeNode, operationTimeTextTreeNode });

            rootNode = new TreeNodePlus("Состояние системы",
                new TreeNode[] { infoNode, extraNode, turboPumpNode });
            rootNode.ExpandAll();

            statusTreeView.Nodes.AddRange(new TreeNode[] { rootNode });
        }
        #endregion
        private void mainForm_Shown(object sender, EventArgs e) {
            openConfigFileToolStripMenuItem_Click(sender, e);
            Activate();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            Close();
        }
        private void connectToolStripMenuItem_Click(object sender, EventArgs e) {
            (new ConnectOptionsForm()).ShowDialog();
        }

        private void overviewToolStripMenuItem_Click(object sender, EventArgs e) {
            (new ScanOptionsForm()).ShowDialog();
        }
        private void senseToolStripMenuItem_Click(object sender, EventArgs e) {
            PreciseOptionsForm pForm = PreciseOptionsForm.getInstance();
            pForm.UpLevel = this;
            pForm.Show();
        }
        private void monitorToolStripMenuItem_Click(object sender, EventArgs e) {
            // Prec/Mon forms simultaneously howto?
            MonitorOptionsForm mForm = MonitorOptionsForm.getInstance();
            mForm.UpLevel = this;
            mForm.Show();
        }

        private void initSys_butt_Click(object sender, EventArgs e) {
            initSys_butt.Enabled = false;
            Commander.Init();
        }
        private void shutSys_butt_Click(object sender, EventArgs e) {
            if (Commander.pState != Commander.programStates.Start)
            {
                if (MessageBox.Show(this, SHUTDOWN_MESSAGE, SHUTDOWN_CAPTION, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) != DialogResult.OK)
                    return;
            }
            shutSys_butt.Enabled = false;
            Commander.Shutdown();
        }
        private void unblock_butt_Click(object sender, EventArgs e) {
            unblock_butt.Enabled = false;
            Commander.Unblock();
        }
        private void prepareControlsOnMeasureStart(IMeasured form) {
            overview_button.Enabled = false;
            sensmeasure_button.Enabled = false;
            monitorToolStripButton.Enabled = false;

            form.prepareControlsOnMeasureStart();

            Commander.OnScanCancelled += new Commander.ProgramEventHandler(InvokeCancelScan);
            Commander.OnError += new Commander.ErrorHandler(Commander_OnError);
        }

        void Commander_OnError(string msg) {
            MessageBox.Show(this, msg);
        }
        private void overview_button_Click(object sender, EventArgs e) {
            Commander.Scan();
            // order is important here!
            CollectorsForm.initMeasure(false);
            prepareControlsOnMeasureStart(CollectorsForm);
            MonitorForm.Hide();
        }
        private void sensmeasure_button_Click(object sender, EventArgs e) {
            Commander.Sense();
            // order is important here!
            CollectorsForm.initMeasure(true);
            prepareControlsOnMeasureStart(CollectorsForm);
            MonitorForm.Hide();
        }
        private void monitorToolStripButton_Click(object sender, EventArgs e) {
            Commander.Monitor();
            MonitorForm.initMeasure(true);
            prepareControlsOnMeasureStart(MonitorForm);
            CollectorsForm.Hide();
        }

        private void InvokeProcessTurboPumpAlert(bool isFault, byte bits) {
            string msg = "Turbopump: ";
            msg += isFault ? "failure (" : "warning (";
            msg += bits.ToString("X2");
            msg += ")";
            InvokeRefreshUserMessage(msg);
            Config.logTurboPumpAlert(msg);
        }

        private void InvokeRefreshUserMessage(string msg) {
            if (this.InvokeRequired) {
                this.Invoke(new Commander.AsyncReplyHandler(RefreshUserMessage), msg);
                return;
            }
            RefreshUserMessage(msg);
        }
        private void RefreshUserMessage(string msg) {
            measure_StatusLabel.Text = msg;
        }

        private void InvokeRefreshDeviceState() {
            if (this.InvokeRequired) {
                this.Invoke(new DeviceEventHandler(RefreshDeviceState));
                return;
            }
            RefreshDeviceState();
        }
        private void RefreshDeviceState() {
            parameterPanel.SuspendLayout();
            statusTreeView.BeginUpdate();
            switch (Device.sysState) {
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

        private void InvokeRefreshTurboPumpStatus() {
            if (this.InvokeRequired) {
                this.Invoke(new DeviceEventHandler(RefreshTurboPumpStatus));
                return;
            }
            RefreshTurboPumpStatus();
        }
        private void RefreshTurboPumpStatus() {
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

        private void InvokeRefreshDeviceStatus() {
            if (this.InvokeRequired) {
                this.Invoke(new DeviceEventHandler(RefreshDeviceStatus));
                return;
            }
            RefreshDeviceStatus();
        }
        private void RefreshDeviceStatus() {
            parameterPanel.SuspendLayout();
            statusTreeView.BeginUpdate();
            if (Device.fPumpOn) {
                forPumpOnValueTreeNode.State = TreeNodePlus.States.Ok;
                forPumpOnValueTreeNode.Text = "Включен";
            } else {
                forPumpOnValueTreeNode.State = TreeNodePlus.States.Error;
                forPumpOnValueTreeNode.Text = "Выключен";
            }
            if (Device.tPumpOn) {
                turboPumpOnValueTreeNode.State = TreeNodePlus.States.Ok;
                turboPumpOnValueTreeNode.Text = "Включен";
            } else {
                turboPumpOnValueTreeNode.State = TreeNodePlus.States.Error;
                turboPumpOnValueTreeNode.Text = "Выключен";
            }
            forVacuumValueTreeNode.Text = string.Format("{0:e3}", Device.fVacuumReal);
            highVacuumValueTreeNode.Text = string.Format("{0:e3}", Device.hVacuumReal);
            if (Device.highVoltageOn) {
                hardwareBlockValueTreeNode.State = TreeNodePlus.States.Ok;
                hardwareBlockValueTreeNode.Text = "Включено";
            } else {
                hardwareBlockValueTreeNode.State = TreeNodePlus.States.Warning;
                hardwareBlockValueTreeNode.Text = "Выключено";
            }
            if (Device.highVacuumValve) {
                vGate1ValueTreeNode.State = TreeNodePlus.States.Ok;
                vGate1ValueTreeNode.Text = "Открыт";
            } else {
                vGate1ValueTreeNode.State = TreeNodePlus.States.Warning;
                vGate1ValueTreeNode.Text = "Закрыт";
            }
            if (Device.probeValve) {
                vGate2ValueTreeNode.State = TreeNodePlus.States.Warning;
                vGate2ValueTreeNode.Text = "Открыт";
            } else {
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

        private void InvokeRefreshVacuumState() {
            if (this.InvokeRequired) {
                this.Invoke(new DeviceEventHandler(RefreshVacuumState));
                return;
            }
            RefreshVacuumState();
        }
        private void RefreshVacuumState() {
            parameterPanel.SuspendLayout();
            statusTreeView.BeginUpdate();
            switch (Device.vacState) {
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

        internal void InvokeRefreshButtons() {
            if (this.InvokeRequired) {
                this.Invoke(new Commander.ProgramEventHandler(RefreshButtons));
                return;
            }
            RefreshButtons();
        }
        private void RefreshButtons() {
            bool precPointsExist = Commander.SomePointsUsed;
            if (Commander.hBlock) {
                unblock_butt.Text = "Снять блокировку";
                unblock_butt.ForeColor = Color.Green;
            } else {
                unblock_butt.Text = "Включить блокировку";
                unblock_butt.ForeColor = Color.Red;
            }
            switch (Commander.pState) {
                case Commander.programStates.Start:
                    connectToolStripButton.Enabled = true;
                    bool connected = Commander.DeviceIsConnected;
                    if (connected) {
                        connectToolStripButton.Text = "Разъединить";
                        connectToolStripButton.ForeColor = Color.Red;
                    } else {
                        connectToolStripButton.Text = "Соединить";
                        connectToolStripButton.ForeColor = Color.Green;
                    }
                    initSys_butt.Enabled = connected;//true;
                    shutSys_butt.Enabled = connected;//false;
                    unblock_butt.Enabled = connected && !Commander.hBlock;//разрешено для включения блокировки
                    overview_button.Enabled = false;
                    sensmeasure_button.Enabled = false;
                    monitorToolStripButton.Enabled = false;

                    connectToolStripMenuItem.Enabled = true;
                    measureToolStripMenuItem.Enabled = true;
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
                    break;
            }
        }

        private void InvokeCancelScan() {
            if (this.InvokeRequired) {
                this.Invoke(new Commander.ProgramEventHandler(CancelScan));
                return;
            }
            CancelScan();
        }
        private void CancelScan() {
            Commander.OnScanCancelled -= new Commander.ProgramEventHandler(InvokeCancelScan);
            Commander.OnError -= new Commander.ErrorHandler(Commander_OnError);
            
            if (CollectorsForm.Visible)
                CollectorsForm.deactivateOnMeasureStop();
            if (MonitorForm.Visible)
                MonitorForm.deactivateOnMeasureStop();
        }

        private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e) {
            controlToolStrip.Visible = toolBarToolStripMenuItem.Checked;
        }

        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e) {
            statusStrip.Visible = statusBarToolStripMenuItem.Checked;
        }

        private void ParameterToolStripMenuItem_Click(object sender, EventArgs e) {
            parameterPanel.Visible = ParameterToolStripMenuItem.Checked;
        }

        private void mainForm_FormClosing(object sender, FormClosingEventArgs e) {
            if (Commander.pState != Commander.programStates.Start) {
                if (MessageBox.Show(this, EXIT_MESSAGE, EXIT_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) != DialogResult.Yes) {
                    e.Cancel = true;
                    return;
                }
            }
            if (Commander.DeviceIsConnected)
                Commander.Disconnect();
            Device.OnDeviceStateChanged -= new DeviceEventHandler(InvokeRefreshDeviceState);
            Device.OnDeviceStatusChanged -= new DeviceEventHandler(InvokeRefreshDeviceStatus);
            Device.OnVacuumStateChanged -= new DeviceEventHandler(InvokeRefreshVacuumState);
            Device.OnTurboPumpStatusChanged -= new DeviceEventHandler(InvokeRefreshTurboPumpStatus);
            Device.OnTurboPumpAlert -= new TurboPumpAlertEventHandler(InvokeProcessTurboPumpAlert);

            Commander.OnProgramStateChanged -= new Commander.ProgramEventHandler(InvokeRefreshButtons);
        }

        private void openConfigFileToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                Config.loadGlobalConfig();
            } catch (Config.ConfigLoadException cle) {
                cle.visualise();
            }
        }

        private void saveConfigFileToolStripMenuItem_Click(object sender, EventArgs e) {
            Config.saveGlobalConfig();
        }

        private void connectToolStripButton_Click(object sender, EventArgs e) {
            if (Commander.DeviceIsConnected) {
                Commander.Disconnect();
                Commander.OnAsyncReply -= new Commander.AsyncReplyHandler(InvokeRefreshUserMessage);
            } else {
                Commander.OnAsyncReply += new Commander.AsyncReplyHandler(InvokeRefreshUserMessage);
                Commander.Connect();
            }
        }

        private void delaysToolStripMenuItem_Click(object sender, EventArgs e) {
            DelaysOptionsForm dForm = new DelaysOptionsForm();
            dForm.ShowDialog();
        }

        private void openSpecterFileToolStripMenuItem_Click(object sender, EventArgs e) {
            openSpecterFileDialog.Filter = string.Format("{0}|{1}", Config.SPECTRUM_FILE_DIALOG_FILTER, Config.PRECISE_SPECTRUM_FILE_DIALOG_FILTER);
            if (openSpecterFileDialog.ShowDialog() == DialogResult.OK) {
                foreach (Form childForm in MdiChildren) {
                    if (childForm is ILoaded) {
                        if ((childForm as ILoaded).FileName == openSpecterFileDialog.FileName) {
                            childForm.Activate();
                            return;
                        }
                    }
                }
                try {
                    bool hint = (openSpecterFileDialog.FilterIndex == 1);
                    Graph graph;
                    string fileName = openSpecterFileDialog.FileName;
                    bool res = !Config.openSpectrumFile(fileName, hint, out graph);
                    LoadedCollectorsForm form = new LoadedCollectorsForm(graph, fileName, res);
                    form.MdiParent = this;
                    form.Show();
                } catch (Config.ConfigLoadException cle) {
                    cle.visualise();
                }
            }
        }

        private void closeAllToolStripMenuItem_Click(object sender, EventArgs e) {
            foreach (Form childForm in MdiChildren) {
                if (childForm == collectorsForm)
                    continue;
                if (childForm == monitorForm)
                    continue;
                childForm.Close();
            }
        }
    }
}