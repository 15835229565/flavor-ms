using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
// move to components..
using TreeNodeLeaf = Flavor.Common.TreeNodeLeaf;
using TreeNodePlus = Flavor.Common.TreeNodePlus;
using TreeNodePair = Flavor.Common.TreeNodePair;
// data model
using Graph = Flavor.Common.Graph;
// divide into 2 parts
using Config = Flavor.Common.Config;
// controller
using Commander = Flavor.Common.Commander;
using ProgramStates = Flavor.Common.ProgramStates;
using ProgramEventHandler = Flavor.Common.ProgramEventHandler;
using MessageHandler = Flavor.Common.MessageHandler;
using Device = Flavor.Common.Device;
using DeviceEventHandler = Flavor.Common.DeviceEventHandler;

namespace Flavor.Forms {
    [Obsolete]
    internal partial class mainForm: Form {
        // TODO: move to resource file
        private const string EXIT_CAPTION = "Предупреждение об отключении";
        private const string EXIT_MESSAGE = "Следует дождаться отключения системы.\nОтключить программу, несмотря на предупреждение?";
        private const string MODE_START_FAILURE_CAPTION = "Ошибка при старте режима измерения";
        private const string PRECISE_MODE_START_FAILURE_MESSAGE = "Точный режим: нет измеряемых пиков.";
        private const string MONITOR_MODE_START_FAILURE_MESSAGE = "Режим мониторинга: нет измеряемых пиков или ошибка формирования матрицы из библиотеки спектров.";
        private const string SHUTDOWN_CAPTION = "Предупреждение об отключении";
        private const string SHUTDOWN_MESSAGE = "Внимание!\n" +
                                                "Проверьте герметизацию системы ввода перед отключением прибора (установку заглушки).\n" +
                                                "При успешном окончании проверки нажмите OK. Для отмены отключения прибора нажмите Отмена.";

        private const string ON_TEXT = "Включен";
        private const string ON_TEXT1 = "Включено";
        private const string OFF_TEXT = "Выключен";
        private const string OFF_TEXT1 = "Выключено";
        private const string OPENED_TEXT = "Открыт";
        private const string CLOSED_TEXT = "Закрыт";

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
        private OptionsForm oForm = null;
        internal mainForm() {
            InitializeComponent();
            populateStatusTreeView();
            CollectorsForm.Visible = true;
            MonitorForm.Visible = false;

            Config.getInitialDirectory();

            Device.OnDeviceStateChanged += InvokeRefreshDeviceState;
            Device.OnDeviceStatusChanged += InvokeRefreshDeviceStatus;
            Device.OnVacuumStateChanged += InvokeRefreshVacuumState;
            Device.OnTurboPumpStatusChanged += InvokeRefreshTurboPumpStatus;
            Device.OnTurboPumpAlert += InvokeProcessTurboPumpAlert;
            Device.Init();
            RefreshDeviceState();
            RefreshVacuumState();

            Commander.ProgramStateChanged += InvokeRefreshButtons;
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
        protected sealed override void OnLoad(EventArgs e) {
            openConfigFileToolStripMenuItem_Click(this, e);
            base.OnLoad(e);
        }
        protected sealed override void OnShown(EventArgs e) {
            base.OnShown(e);
            Activate();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            Close();
        }
        private void connectToolStripMenuItem_Click(object sender, EventArgs e) {
            if ((new ConnectOptionsForm(Commander.AvailablePorts)).ShowDialog() == DialogResult.OK)
                Commander.reconnect();
        }

        private void overviewToolStripMenuItem_Click(object sender, EventArgs e) {
            showOptionsForm<ScanOptionsForm>();
        }
        private void senseToolStripMenuItem_Click(object sender, EventArgs e) {
            showOptionsForm<PreciseOptionsForm>();
        }
        private void monitorToolStripMenuItem_Click(object sender, EventArgs e) {
            showOptionsForm<MonitorOptionsForm>();
        }
        private void showOptionsForm<T>()
            where T: OptionsForm, new() {
            if (oForm == null) {
                oForm = new T();
                oForm.Load += (s, a) => {
                    var args = a as OptionsForm.LoadEventArgs;
                    Commander.ProgramStateChanged += args.Method;
                    var state = Commander.pState;
                    args.Enabled = (state == ProgramStates.Ready ||
                        state == ProgramStates.WaitHighVoltage ||
                        state == ProgramStates.Measure ||
                        state == ProgramStates.BackgroundMeasureReady ||
                        state == ProgramStates.WaitBackgroundMeasure);
                    args.NotRareModeRequested = Commander.notRareModeRequested;
                };
                oForm.FormClosing += (s, a) => {
                    var args = a as OptionsForm.ClosingEventArgs;
                    Commander.ProgramStateChanged -= args.Method;
                    switch (oForm.DialogResult) {
                        case DialogResult.Yes:
                            Commander.sendSettings();
                            Commander.notRareModeRequested = args.NotRareModeRequested;
                            break;
                        case DialogResult.OK:
                            Commander.notRareModeRequested = args.NotRareModeRequested;
                            break;
                        case DialogResult.Cancel:
                            break;
                    }
                };
                oForm.FormClosed += (s, a) => {
                    oForm = null;
                    RefreshButtons(Commander.pState);
                };
                oForm.Show();
            } else if (oForm as T == null)
                return;
            else
                oForm.Activate();
            // TODO: disable other menu items or close already opened?
        }
        private void initSys_butt_Click(object sender, EventArgs e) {
            initSys_butt.Enabled = false;
            Commander.Init();
        }
        private void shutSys_butt_Click(object sender, EventArgs e) {
            if (Commander.pState != ProgramStates.Start)
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

        private void Commander_OnError(string msg) {
            MessageBox.Show(this, msg);
        }
        private void overview_button_Click(object sender, EventArgs e) {
            Commander.Scan();
            ChildFormInit(CollectorsForm, false);
        }
        private void sensmeasure_button_Click(object sender, EventArgs e) {
            if (Commander.Sense()) {
                ChildFormInit(CollectorsForm, true);
            } else {
                MessageBox.Show(this, MONITOR_MODE_START_FAILURE_MESSAGE, MODE_START_FAILURE_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void monitorToolStripButton_Click(object sender, EventArgs e) {
            // lock PreciseData for modification
            bool? result = Commander.Monitor();
            if (result.HasValue) {
                if (result == true) {
                    ChildFormInit(MonitorForm, true);
                }
            } else {
                MessageBox.Show(this, MONITOR_MODE_START_FAILURE_MESSAGE, MODE_START_FAILURE_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ChildFormInit(IMeasured form, bool isPrecise) {
            overview_button.Enabled = false;
            sensmeasure_button.Enabled = false;
            monitorToolStripButton.Enabled = false;

            form.MeasureCancelRequested += ChildForm_MeasureCancelRequested;
            // order is important here!
            form.initMeasure(Commander.CurrentMeasureMode.StepsCount, isPrecise);

            Commander.MeasureCancelled += InvokeCancelScan;
            Commander.ErrorOccured += Commander_OnError;

            if (form == CollectorsForm)
                MonitorForm.Hide();
            else
                CollectorsForm.Hide();
        }
        private void ChildForm_MeasureCancelRequested(object sender, EventArgs e) {
            (sender as IMeasured).MeasureCancelRequested -= ChildForm_MeasureCancelRequested;
            Commander.measureCancelRequested = true;
        }

        //TODO: make 2 subscribers. one for logging, another for displaying.
        private void InvokeProcessTurboPumpAlert(bool isFault, byte bits) {
            string msg = new StringBuilder("Turbopump: ")
                .Append(isFault ? "failure (" : "warning (")
                .AppendFormat("{0:X2}", bits)
                .Append(")").ToString();
            InvokeRefreshUserMessage(msg);
            Config.logTurboPumpAlert(msg);
        }

        private void InvokeRefreshUserMessage(string msg) {
            if (this.InvokeRequired) {
                this.BeginInvoke(new MessageHandler(RefreshUserMessage), msg);
                return;
            }
            RefreshUserMessage(msg);
        }
        private void RefreshUserMessage(string msg) {
            measure_StatusLabel.Text = msg;
        }

        // TODO: Device state as method parameter (avoid thread run)
        private void InvokeRefreshDeviceState() {
            if (this.InvokeRequired) {
                this.BeginInvoke(new DeviceEventHandler(RefreshDeviceState));
                return;
            }
            RefreshDeviceState();
        }
        // Device.DeviceState state
        private void RefreshDeviceState() {
            parameterPanel.SuspendLayout();
            statusTreeView.BeginUpdate();
            switch (Device.sysState) {
                case Device.DeviceStates.Start:
                    systemStateValueTreeNode.Text = "Запуск";
                    systemStateValueTreeNode.State = TreeNodePlus.States.Warning;
                    break;
                case Device.DeviceStates.Init:
                    systemStateValueTreeNode.Text = "Инициализация";
                    systemStateValueTreeNode.State = TreeNodePlus.States.Warning;
                    break;
                case Device.DeviceStates.VacuumInit:
                    systemStateValueTreeNode.Text = "Инициализация вакуума";
                    systemStateValueTreeNode.State = TreeNodePlus.States.Warning;
                    break;
                case Device.DeviceStates.WaitHighVoltage:
                    systemStateValueTreeNode.Text = "Ожидание высокого напряжения";
                    systemStateValueTreeNode.State = TreeNodePlus.States.Ok;
                    break;
                case Device.DeviceStates.Ready:
                    systemStateValueTreeNode.Text = "Готова к измерению";
                    systemStateValueTreeNode.State = TreeNodePlus.States.Ok;
                    break;
                case Device.DeviceStates.Measuring:
                    systemStateValueTreeNode.Text = "Производятся измерения";
                    systemStateValueTreeNode.State = TreeNodePlus.States.Ok;
                    break;
                case Device.DeviceStates.Measured:
                    systemStateValueTreeNode.Text = "Измерения закончены";
                    systemStateValueTreeNode.State = TreeNodePlus.States.Ok;
                    break;
                case Device.DeviceStates.ShutdownInit:
                    systemStateValueTreeNode.Text = "Инициализация выключения";
                    systemStateValueTreeNode.State = TreeNodePlus.States.Warning;
                    break;
                case Device.DeviceStates.Shutdowning:
                    systemStateValueTreeNode.Text = "Идет выключение";
                    systemStateValueTreeNode.State = TreeNodePlus.States.Warning;
                    break;
                case Device.DeviceStates.Shutdowned:
                    systemStateValueTreeNode.Text = "Выключено";
                    systemStateValueTreeNode.State = TreeNodePlus.States.Warning;
                    break;
                case Device.DeviceStates.TurboPumpFailure:
                    systemStateValueTreeNode.Text = "Отказ турбонасоса";
                    systemStateValueTreeNode.State = TreeNodePlus.States.Error;
                    break;
                case Device.DeviceStates.VacuumCrash:
                    systemStateValueTreeNode.Text = "Потеря вакуума";
                    systemStateValueTreeNode.State = TreeNodePlus.States.Error;
                    break;
                case Device.DeviceStates.ConstantsWrite:
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

        // TODO: turbo pump state as method parameter (avoid thread run)
        private void InvokeRefreshTurboPumpStatus() {
            if (this.InvokeRequired) {
                this.BeginInvoke(new DeviceEventHandler(RefreshTurboPumpStatus));
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

        // TODO: Device status as method parameter (avoid thread run)
        private void InvokeRefreshDeviceStatus() {
            if (this.InvokeRequired) {
                this.BeginInvoke(new DeviceEventHandler(RefreshDeviceStatus));
                return;
            }
            RefreshDeviceStatus();
        }
        private void RefreshDeviceStatus() {
            parameterPanel.SuspendLayout();
            statusTreeView.BeginUpdate();
            if (Device.fPumpOn) {
                forPumpOnValueTreeNode.State = TreeNodePlus.States.Ok;
                forPumpOnValueTreeNode.Text = ON_TEXT;
            } else {
                forPumpOnValueTreeNode.State = TreeNodePlus.States.Error;
                forPumpOnValueTreeNode.Text = OFF_TEXT;
            }
            if (Device.tPumpOn) {
                turboPumpOnValueTreeNode.State = TreeNodePlus.States.Ok;
                turboPumpOnValueTreeNode.Text = ON_TEXT;
            } else {
                turboPumpOnValueTreeNode.State = TreeNodePlus.States.Error;
                turboPumpOnValueTreeNode.Text = OFF_TEXT;
            }
            forVacuumValueTreeNode.Text = string.Format("{0:e3}", Device.fVacuumReal);
            highVacuumValueTreeNode.Text = string.Format("{0:e3}", Device.hVacuumReal);
            if (Device.highVoltageOn) {
                hardwareBlockValueTreeNode.State = TreeNodePlus.States.Ok;
                hardwareBlockValueTreeNode.Text = ON_TEXT1;
            } else {
                hardwareBlockValueTreeNode.State = TreeNodePlus.States.Warning;
                hardwareBlockValueTreeNode.Text = OFF_TEXT1;
            }
            if (Device.highVacuumValve) {
                vGate1ValueTreeNode.State = TreeNodePlus.States.Ok;
                vGate1ValueTreeNode.Text = OPENED_TEXT;
            } else {
                vGate1ValueTreeNode.State = TreeNodePlus.States.Warning;
                vGate1ValueTreeNode.Text = CLOSED_TEXT;
            }
            if (Device.probeValve) {
                vGate2ValueTreeNode.State = TreeNodePlus.States.Warning;
                vGate2ValueTreeNode.Text = OPENED_TEXT;
            } else {
                vGate2ValueTreeNode.State = TreeNodePlus.States.Ok;
                vGate2ValueTreeNode.Text = CLOSED_TEXT;
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

        // TODO: vacuum state as method parameter (avoid thread run)
        private void InvokeRefreshVacuumState() {
            if (this.InvokeRequired) {
                this.BeginInvoke(new DeviceEventHandler(RefreshVacuumState));
                return;
            }
            RefreshVacuumState();
        }
        // Device.VacuumStates state
        private void RefreshVacuumState() {
            parameterPanel.SuspendLayout();
            statusTreeView.BeginUpdate();
            switch (Device.vacState) {
                case Device.VacuumStates.Idle:
                    vacuumStateValueTreeNode.Text = "Бездействие";
                    vacuumStateValueTreeNode.State = TreeNodePlus.States.Warning;
                    break;
                case Device.VacuumStates.Init:
                    vacuumStateValueTreeNode.Text = "Инициализация";
                    vacuumStateValueTreeNode.State = TreeNodePlus.States.Warning;
                    break;
                case Device.VacuumStates.StartingForvacuumPump:
                    vacuumStateValueTreeNode.Text = "Включение форнасоса";
                    vacuumStateValueTreeNode.State = TreeNodePlus.States.Warning;
                    break;
                case Device.VacuumStates.PumpingForvacuum:
                    vacuumStateValueTreeNode.Text = "Откачка форвакуума";
                    vacuumStateValueTreeNode.State = TreeNodePlus.States.Warning;
                    break;
                case Device.VacuumStates.DelayPumpingHighVacuumByForvac:
                    vacuumStateValueTreeNode.Text = "Задержка высокого вакуума из-за фор";
                    vacuumStateValueTreeNode.State = TreeNodePlus.States.Warning;
                    break;
                case Device.VacuumStates.PumpingHighVacuumByForvac:
                    vacuumStateValueTreeNode.Text = "Откачка высокого вакуума форнасосом";
                    vacuumStateValueTreeNode.State = TreeNodePlus.States.Warning;
                    break;
                case Device.VacuumStates.PumpingHighVacuumByTurbo:
                    vacuumStateValueTreeNode.Text = "Откачка турбо";
                    vacuumStateValueTreeNode.State = TreeNodePlus.States.Warning;
                    break;
                case Device.VacuumStates.Ready:
                    vacuumStateValueTreeNode.Text = "Готово";
                    vacuumStateValueTreeNode.State = TreeNodePlus.States.Ok;
                    break;
                case Device.VacuumStates.ShutdownInit:
                    vacuumStateValueTreeNode.Text = "Инициализация отключения";
                    vacuumStateValueTreeNode.State = TreeNodePlus.States.Warning;
                    break;
                case Device.VacuumStates.ShutdownDelay:
                    vacuumStateValueTreeNode.Text = "Задержка отключения";
                    vacuumStateValueTreeNode.State = TreeNodePlus.States.Warning;
                    break;
                case Device.VacuumStates.ShutdownPumpProbe:
                    vacuumStateValueTreeNode.Text = "Отключение датчика насоса";
                    vacuumStateValueTreeNode.State = TreeNodePlus.States.Warning;
                    break;
                case Device.VacuumStates.Shutdowned:
                    vacuumStateValueTreeNode.Text = "Отключено";
                    vacuumStateValueTreeNode.State = TreeNodePlus.States.Warning;
                    break;
                case Device.VacuumStates.ShutdownStartingTurboPump:
                    vacuumStateValueTreeNode.Text = "Откачка при выключении";
                    vacuumStateValueTreeNode.State = TreeNodePlus.States.Warning;
                    break;
                case Device.VacuumStates.BadHighVacuum:
                    vacuumStateValueTreeNode.Text = "Плохой высокий вакуум";
                    vacuumStateValueTreeNode.State = TreeNodePlus.States.Error;
                    break;
                case Device.VacuumStates.BadForvacuum:
                    vacuumStateValueTreeNode.Text = "Плохой форвакуум";
                    vacuumStateValueTreeNode.State = TreeNodePlus.States.Error;
                    break;
                case Device.VacuumStates.ForvacuumFailure:
                    vacuumStateValueTreeNode.Text = "Отказ форвакуума";
                    vacuumStateValueTreeNode.State = TreeNodePlus.States.Error;
                    break;
                case Device.VacuumStates.LargeLeak:
                    vacuumStateValueTreeNode.Text = "Большая течь";
                    vacuumStateValueTreeNode.State = TreeNodePlus.States.Error;
                    break;
                case Device.VacuumStates.SmallLeak:
                    vacuumStateValueTreeNode.Text = "Малая течь";
                    vacuumStateValueTreeNode.State = TreeNodePlus.States.Error;
                    break;
                case Device.VacuumStates.ThermoCoupleFailure:
                    vacuumStateValueTreeNode.Text = "Отказ термопары";
                    vacuumStateValueTreeNode.State = TreeNodePlus.States.Error;
                    break;
                case Device.VacuumStates.TurboPumpFailure:
                    vacuumStateValueTreeNode.Text = "Отказ турбонасоса";
                    vacuumStateValueTreeNode.State = TreeNodePlus.States.Error;
                    break;
                case Device.VacuumStates.VacuumShutdownProbeLeak:
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

        // TODO: program state as method parameter (avoid thread run)
        internal void InvokeRefreshButtons(ProgramStates state) {
            if (this.InvokeRequired) {
                this.BeginInvoke(new ProgramEventHandler(RefreshButtons), state);
                return;
            }
            RefreshButtons(state);
        }
        // bool block, Commander.programStates state, bool connected, bool canDoPrecise
        // use setButtons signature..
        private void RefreshButtons(ProgramStates state) {
            bool block = !Commander.hBlock;
            if (block) {
                unblock_butt.Text = "Включить блокировку";
                unblock_butt.ForeColor = Color.Red;
            } else {
                unblock_butt.Text = "Снять блокировку";
                unblock_butt.ForeColor = Color.Green;
            }
            switch (state) {
                case ProgramStates.Start:
                    bool connected = Commander.DeviceIsConnected;
                    if (connected) {
                        connectToolStripButton.Text = "Разъединить";
                        connectToolStripButton.ForeColor = Color.Red;
                    } else {
                        connectToolStripButton.Text = "Соединить";
                        connectToolStripButton.ForeColor = Color.Green;
                    }
                    setButtons(true, connected, connected, connected && block, false, false, false, true);
                    break;
                case ProgramStates.Init:
                    setButtons(false, false, true, false, false, false, false, true);
                    break;
                case ProgramStates.WaitHighVoltage:
                    setButtons(false, false, true, true, false, false, false, true);
                    break;
                case ProgramStates.Ready:
                    bool canDoPrecise = block && Commander.SomePointsUsed;
                    setButtons(false, false, true, true, block, canDoPrecise, canDoPrecise, true);
                    monitorToolStripButton.Text = "Режим мониторинга";
                    break;
                case ProgramStates.WaitBackgroundMeasure:
                    setButtons(false, false, true, true, false, false, false, false);
                    monitorToolStripButton.Text = "Измерение фона";
                    break;
                case ProgramStates.BackgroundMeasureReady:
                    setButtons(false, false, true, true, false, false, true, false);
                    monitorToolStripButton.Text = "Начать мониторинг";
                    break;
                case ProgramStates.Measure:
                    setButtons(false, false, true, true, false, false, false, false);
                    monitorToolStripButton.Text = "Режим мониторинга";
                    break;
                case ProgramStates.WaitInit:
                case ProgramStates.WaitShutdown:
                case ProgramStates.Shutdown:
                    setButtons(false, false, false, false, false, false, false, true);
                    break;
            }
        }
        private void setButtons(bool connect, bool init, bool shutdown, bool block, bool scan, bool precise, bool monitor, bool measureOptions) {
            connectToolStripButton.Enabled = connect;

            initSys_butt.Enabled = init;
            shutSys_butt.Enabled = shutdown;
            unblock_butt.Enabled = block;
            overview_button.Enabled = scan;
            sensmeasure_button.Enabled = precise;
            monitorToolStripButton.Enabled = monitor;

            connectToolStripMenuItem.Enabled = connect;
            measureToolStripMenuItem.Enabled = measureOptions;
        }

        private void InvokeCancelScan(ProgramStates state) {
            if (this.InvokeRequired) {
                this.BeginInvoke(new ProgramEventHandler(CancelScan), state);
                return;
            }
            CancelScan(state);
        }
        private void CancelScan(ProgramStates state) {
            Commander.MeasureCancelled -= InvokeCancelScan;
            Commander.ErrorOccured -= Commander_OnError;
            
            if (CollectorsForm.Visible)
                CollectorsForm.deactivateOnMeasureStop();
            else if (MonitorForm.Visible)
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

        protected sealed override void OnFormClosing(FormClosingEventArgs e) {
            if (Commander.pState != ProgramStates.Start &&
                MessageBox.Show(this, EXIT_MESSAGE, EXIT_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) != DialogResult.Yes) {
                e.Cancel = true;
                return;
            }
            if (Commander.DeviceIsConnected)
                Commander.Disconnect();
            Device.OnDeviceStateChanged -= InvokeRefreshDeviceState;
            Device.OnDeviceStatusChanged -= InvokeRefreshDeviceStatus;
            Device.OnVacuumStateChanged -= InvokeRefreshVacuumState;
            Device.OnTurboPumpStatusChanged -= InvokeRefreshTurboPumpStatus;
            Device.OnTurboPumpAlert -= InvokeProcessTurboPumpAlert;

            Commander.ProgramStateChanged -= InvokeRefreshButtons;
            
            base.OnFormClosing(e);
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
                Commander.AsyncReplyReceived -= InvokeRefreshUserMessage;
            } else {
                Commander.AsyncReplyReceived += InvokeRefreshUserMessage;
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
                    // buggy workaround to refresh MdiWindowListItem
                    form.TextChanged += form_TextChanged;
                    form.Show();
                } catch (Config.ConfigLoadException cle) {
                    cle.visualise();
                }
            }
        }
        void form_TextChanged(object sender, EventArgs e) {
            //this.MainMenuStrip.MdiWindowListItem.Invalidate();
            Form form = sender as Form;
            // buggy workaround to refresh MdiWindowListItem
            form.Hide();
            form.Show();
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