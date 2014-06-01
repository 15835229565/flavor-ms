using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
// move to components..
using TreeNodeLeaf = Flavor.Common.TreeNodeLeaf;
using TreeNodePlus = Flavor.Common.TreeNodePlus;
using TreeNodePair = Flavor.Common.TreeNodePair;
// data model
using Graph = Flavor.Common.Data.Measure.Graph;
// divide into 2 parts
using Config = Flavor.Common.Settings.Config;
// controller
using ICommander = Flavor.Common.Commander;
using ProgramStates = Flavor.Common.ProgramStates;
using ProgramEventHandler = Flavor.Common.ProgramEventHandler;
using MessageHandler = Flavor.Common.MessageHandler;
using Device = Flavor.Common.Device;
using DeviceEventHandler = Flavor.Common.DeviceEventHandler;

namespace Flavor.Forms {
    partial class MainForm2: Form, IMSControl/*, IMeasured*/ {
        // TODO: move to resource file
        const string EXIT_CAPTION = "Предупреждение об отключении";
        const string EXIT_MESSAGE = "Следует дождаться отключения системы.\nОтключить программу, несмотря на предупреждение?";
        const string MODE_START_FAILURE_CAPTION = "Ошибка при старте режима измерения";
        const string PRECISE_MODE_START_FAILURE_MESSAGE = "Точный режим: нет измеряемых пиков.";
        const string MONITOR_MODE_START_FAILURE_MESSAGE = "Режим мониторинга: нет измеряемых пиков или ошибка формирования матрицы из библиотеки спектров.";
        const string SHUTDOWN_CAPTION = "Предупреждение об отключении";
        const string SHUTDOWN_MESSAGE = "Внимание!\n" +
                                                "Проверьте герметизацию системы ввода перед отключением прибора (установку заглушки).\n" +
                                                "При успешном окончании проверки нажмите OK. Для отмены отключения прибора нажмите Отмена.";

        const string ON_TEXT = "Включен";
        const string ON_TEXT1 = "Включено";
        const string OFF_TEXT = "Выключен";
        const string OFF_TEXT1 = "Выключено";
        const string OPENED_TEXT = "Открыт";
        const string CLOSED_TEXT = "Закрыт";

        EventHandler<CallBackEventArgs<bool, string>> _connect;
        public event EventHandler<CallBackEventArgs<bool, string>> Connect {
            add {
                if (value == null)
                    return;
                _connect += value;
                connectToolStripButton.Visible = true;
            }
            remove {
                var evt = _connect;
                evt -= value;
                if (evt == null)
                    return;
                connectToolStripButton.Visible = false;
            }
        }
        protected virtual void OnConnect(CallBackEventArgs<bool, string> e) {
            _connect.Raise(this, e);
        }
        EventHandler<CallBackEventArgs<bool>> _init;
        public event EventHandler<CallBackEventArgs<bool>> Init {
            add {
                if (value == null)
                    return;
                _init += value;
                initSys_butt.Visible = true;
            }
            remove {
                var evt = _init;
                evt -= value;
                if (evt == null)
                    return;
                initSys_butt.Visible = false;
            }
        }
        protected virtual void OnInit(CallBackEventArgs<bool> e) {
            _init.Raise(this, e);
        }
        EventHandler<CallBackEventArgs<bool>> _shutdown;
        public event EventHandler<CallBackEventArgs<bool>> Shutdown {
            add {
                if (value == null)
                    return;
                _shutdown += value;
                shutSys_butt.Visible = true;
            }
            remove {
                var evt = _shutdown;
                evt -= value;
                if (evt == null)
                    return;
                shutSys_butt.Visible = false;
            }
        }
        protected virtual void OnShutdown(CallBackEventArgs<bool> e) {
            _shutdown.Raise(this, e);
        }
        EventHandler<CallBackEventArgs<bool>> _unblock;
        public event EventHandler<CallBackEventArgs<bool>> Unblock {
            add {
                if (value == null)
                    return;
                _unblock += value;
                unblock_butt.Visible = true;
            }
            remove {
                var evt = _unblock;
                evt -= value;
                if (evt == null)
                    return;
                unblock_butt.Visible = false;
            }
        }
        protected virtual void OnUnblock(CallBackEventArgs<bool> e) {
            _unblock.Raise(this, e);
        }
        //IMeasured activeMeasureChild;
        MeasuredCollectorsForm collectorsForm = null;
        MeasuredCollectorsForm CollectorsForm {
            get {
                if (collectorsForm == null) {
                    collectorsForm = new MeasuredCollectorsForm();
                    collectorsForm.MdiParent = this;
                }
                return collectorsForm;
            }
        }
        MonitorForm monitorForm = null;
        MonitorForm MonitorForm {
            get {
                if (monitorForm == null) {
                    monitorForm = new MonitorForm();
                    monitorForm.MdiParent = this;
                }
                return monitorForm;
            }
        }
        GraphForm GForm {
            get {
                Form child = ActiveMdiChild;
                if (child == null)
                    return CollectorsForm;
                return child as GraphForm;
            }
        }
        OptionsForm2 oForm = null;
        readonly ICommander commander;
        public MainForm2(ICommander commander)
            : base() {
            this.commander = commander;
            InitializeComponent();
            connectToolStripButton.Visible = false;
            connectToolStripButton.Tag = false;
            initSys_butt.Visible = false;
            initSys_butt.Tag = false;
            shutSys_butt.Visible = false;
            unblock_butt.Visible = false;
            unblock_butt.Tag = false;
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
            base.OnLoad(e);

            populateStatusTreeView();
            CollectorsForm.Visible = true;
            // do not activate so early!
            //MonitorForm.Visible = false;
            var device = commander.device;
            if (device == null) {
                Device.OnDeviceStateChanged += InvokeRefreshDeviceState;
                Device.OnDeviceStatusChanged += InvokeRefreshDeviceStatus;
                Device.OnVacuumStateChanged += InvokeRefreshVacuumState;
                Device.OnTurboPumpStatusChanged += InvokeRefreshTurboPumpStatus;
                Device.OnTurboPumpAlert += InvokeProcessTurboPumpAlert;
                Device.Init();
            } else {
                // BAD!
                device.DeviceStateChanged += RefreshDeviceStateAsync;
            }
            RefreshDeviceState();
            RefreshVacuumState();

            commander.ProgramStateChanged += InvokeRefreshButtons;
        }
        protected sealed override void OnFormClosing(FormClosingEventArgs e) {
            if (commander.pState != ProgramStates.Start &&
                MessageBox.Show(this, EXIT_MESSAGE, EXIT_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) != DialogResult.Yes) {
                e.Cancel = true;
                return;
            }
            var device = commander.device;
            if (device == null) {
                Device.OnDeviceStateChanged -= InvokeRefreshDeviceState;
                Device.OnDeviceStatusChanged -= InvokeRefreshDeviceStatus;
                Device.OnVacuumStateChanged -= InvokeRefreshVacuumState;
                Device.OnTurboPumpStatusChanged -= InvokeRefreshTurboPumpStatus;
                Device.OnTurboPumpAlert -= InvokeProcessTurboPumpAlert;
            } else {
                device.DeviceStateChanged -= RefreshDeviceStateAsync;
            }

            commander.ProgramStateChanged -= InvokeRefreshButtons;

            base.OnFormClosing(e);
        }

        protected sealed override void OnShown(EventArgs e) {
            base.OnShown(e);
            Activate();
        }

        void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            Close();
        }
        void connectToolStripMenuItem_Click(object sender, EventArgs e) {
            if ((new ConnectOptionsForm(commander.AvailablePorts)).ShowDialog() == DialogResult.OK)
                commander.Reconnect();
        }

        void overviewToolStripMenuItem_Click(object sender, EventArgs e) {
            showOptionsForm<ScanOptionsForm>();
        }
        void senseToolStripMenuItem_Click(object sender, EventArgs e) {
            showOptionsForm<PreciseOptionsForm>();
        }
        void monitorToolStripMenuItem_Click(object sender, EventArgs e) {
            showOptionsForm<MonitorOptionsForm>();
        }
        void showOptionsForm<T>()
            where T: OptionsForm2, new() {
            if (oForm == null) {
                oForm = new T();
                oForm.Load += (s, a) => {
                    var args = a as OptionsForm2.LoadEventArgs;
                    commander.ProgramStateChanged += args.Method;
                    var state = commander.pState;
                    args.Enabled = (state == ProgramStates.Ready ||
                        state == ProgramStates.WaitHighVoltage ||
                        state == ProgramStates.Measure ||
                        state == ProgramStates.BackgroundMeasureReady ||
                        state == ProgramStates.WaitBackgroundMeasure);
                    args.NotRareModeRequested = commander.notRareModeRequested;
                };
                oForm.FormClosing += (s, a) => {
                    var args = a as OptionsForm2.ClosingEventArgs;
                    var ps = args.Parameters;
                    if (ps != null) {
                        Config.temp_saveGO((double)ps[0], (double)ps[1], (double)ps[2], (double)ps[3], (double)ps[4], (double)ps[5], (double)ps[6]);
                        //Config.saveGlobalCommonOptions(
                        //    (ushort)ps[0],
                        //    (ushort)ps[1],
                        //    (double)ps[2],
                        //    (double)ps[3],
                        //    (double)ps[4],
                        //    (double)ps[5],
                        //    (double)ps[6],
                        //    (double)ps[7]);
                    }
                    commander.ProgramStateChanged -= args.Method;
                    switch (oForm.DialogResult) {
                        case DialogResult.Yes:
                            commander.SendSettings();
                            commander.notRareModeRequested = args.NotRareModeRequested;
                            break;
                        case DialogResult.OK:
                            commander.notRareModeRequested = args.NotRareModeRequested;
                            break;
                        case DialogResult.Cancel:
                            break;
                    }
                };
                oForm.FormClosed += (s, a) => {
                    oForm = null;
                    RefreshButtons(commander.pState);
                };
                oForm.Show();
            } else if (oForm as T == null)
                return;
            else
                oForm.Activate();
            // TODO: disable other menu items or close already opened?
        }

        void connectToolStripButton_Click(object sender, EventArgs e) {
            connectToolStripButton.Enabled = false;
            bool old = (bool)connectToolStripButton.Tag;
            var ee = new CallBackEventArgs<bool, string>(old, null);
            OnConnect(ee);
            bool connected = ee.Value;
            if (old != connected) {
                var callBack = ee.Handler;
                if (connected) {
                    // BAD!
                    commander.AsyncReplyReceived += InvokeRefreshUserMessage;
                    connectToolStripButton.Text = "Разъединить";
                    connectToolStripButton.ForeColor = Color.Red;
                } else {
                    connectToolStripButton.Text = "Соединить";
                    connectToolStripButton.ForeColor = Color.Green;
                    // BAD!
                    commander.AsyncReplyReceived -= InvokeRefreshUserMessage;
                }
                connectToolStripButton.Tag = connected;
                // BAD!
                RefreshButtons(commander.pState);
            }
            connectToolStripButton.Enabled = true;
        }

        void ButtonClick(ToolStripButton button, Action<CallBackEventArgs<bool>> action) {
            button.Enabled = false;
            bool old = (bool)button.Tag;
            var ee = new CallBackEventArgs<bool>(old, (s, eee) => Invoke(new Action(() => {
                    button.Enabled = true;
                    button.Tag = old;
                }))
            );
            action(ee);
            bool res = ee.Value;
            if (old != res) {
                button.Tag = res;
            }
        }
        void initSys_butt_Click(object sender, EventArgs e) {
            ButtonClick(initSys_butt, OnInit);
        }
        void shutSys_butt_Click(object sender, EventArgs e) {
            if ((bool)initSys_butt.Tag) {
                if (MessageBox.Show(this, SHUTDOWN_MESSAGE, SHUTDOWN_CAPTION, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) != DialogResult.OK)
                    return;
            }
            ButtonClick(shutSys_butt, OnShutdown);
        }
        void unblock_butt_Click(object sender, EventArgs e) {
            ButtonClick(unblock_butt, OnUnblock);
        }

        void Commander_OnError(string msg) {
            MessageBox.Show(this, msg);
        }
        void overview_button_Click(object sender, EventArgs e) {
            commander.Scan();
            ChildFormInit(CollectorsForm, false);
        }
        void sensmeasure_button_Click(object sender, EventArgs e) {
            if (commander.Sense()) {
                ChildFormInit(CollectorsForm, true);
            } else {
                MessageBox.Show(this, MONITOR_MODE_START_FAILURE_MESSAGE, MODE_START_FAILURE_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        void monitorToolStripButton_Click(object sender, EventArgs e) {
            // lock PreciseData for modification
            bool? result = commander.Monitor();
            if (result.HasValue) {
                if (result == true) {
                    ChildFormInit(MonitorForm, true);
                }
            } else {
                MessageBox.Show(this, MONITOR_MODE_START_FAILURE_MESSAGE, MODE_START_FAILURE_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        void ChildFormInit(IMeasured form, bool isPrecise) {
            overview_button.Enabled = false;
            sensmeasure_button.Enabled = false;
            monitorToolStripButton.Enabled = false;

            form.MeasureCancelRequested += ChildForm_MeasureCancelRequested;
            // order is important here!
            form.initMeasure(commander.CurrentMeasureMode.StepsCount, isPrecise);

            commander.MeasureCancelled += InvokeCancelScan;
            commander.ErrorOccured += Commander_OnError;

            if (form == CollectorsForm)
                MonitorForm.Hide();
            else
                CollectorsForm.Hide();
        }
        void ChildForm_MeasureCancelRequested(object sender, EventArgs e) {
            (sender as IMeasured).MeasureCancelRequested -= ChildForm_MeasureCancelRequested;
            commander.MeasureCancelRequested = true;
        }
        //TODO: make 2 subscribers. one for logging, another for displaying.
        void InvokeProcessTurboPumpAlert(bool isFault, byte bits) {
            string msg = new StringBuilder("Turbopump: ")
                .Append(isFault ? "failure (" : "warning (")
                .AppendFormat("{0:X2}", bits)
                .Append(")").ToString();
            InvokeRefreshUserMessage(msg);
            Config.logTurboPumpAlert(msg);
        }

        void InvokeRefreshUserMessage(string msg) {
            if (this.InvokeRequired) {
                this.BeginInvoke(new MessageHandler(RefreshUserMessage), msg);
                return;
            }
            RefreshUserMessage(msg);
        }
        void RefreshUserMessage(string msg) {
            measure_StatusLabel.Text = msg;
        }

        void RefreshDeviceStateAsync(object sender, EventArgs<byte> e) {
            // TODO:!
            //BeginInvoke();
        }
        // TODO: Device state as method parameter (avoid thread run)
        void InvokeRefreshDeviceState() {
            BeginInvoke(new DeviceEventHandler(RefreshDeviceState));
        }
        // Device.DeviceState state
        void RefreshDeviceState() {
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
        void InvokeRefreshTurboPumpStatus() {
            BeginInvoke(new DeviceEventHandler(RefreshTurboPumpStatus));
        }
        void RefreshTurboPumpStatus() {
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
        void InvokeRefreshDeviceStatus() {
            BeginInvoke(new DeviceEventHandler(RefreshDeviceStatus));
        }
        void RefreshDeviceStatus() {
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
        void InvokeRefreshVacuumState() {
            BeginInvoke(new DeviceEventHandler(RefreshVacuumState));
        }
        // Device.VacuumStates state
        void RefreshVacuumState() {
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
        void InvokeRefreshButtons(ProgramStates state) {
            if (this.InvokeRequired) {
                this.BeginInvoke(new ProgramEventHandler(RefreshButtons), state);
                return;
            }
            RefreshButtons(state);
        }
        // bool block, ProgramStates state, bool connected, bool canDoPrecise
        // use setButtons signature..
        void RefreshButtons(ProgramStates state) {
            switch (state) {
                case ProgramStates.Start:
                    bool connected = (bool)connectToolStripButton.Tag;
                    setButtons(true, connected, connected, false, false, false, false, true);
                    break;
                case ProgramStates.Init:
                    setButtons(false, false, true, false, false, false, false, true);
                    break;
                case ProgramStates.WaitHighVoltage:
                    unblock_butt.Text = "Снять блокировку";
                    unblock_butt.ForeColor = Color.Green;
                    setButtons(false, false, true, true, false, false, false, true);
                    break;
                case ProgramStates.Ready:
                    unblock_butt.Text = "Включить блокировку";
                    unblock_butt.ForeColor = Color.Red;
                    bool canDoPrecise = commander.SomePointsUsed;
                    setButtons(false, false, true, true, true, canDoPrecise, canDoPrecise, true);
                    monitorToolStripButton.Text = "Режим мониторинга";
                    break;
                case ProgramStates.WaitBackgroundMeasure:
                    unblock_butt.Text = "Включить блокировку";
                    unblock_butt.ForeColor = Color.Red;
                    setButtons(false, false, true, true, false, false, false, false);
                    monitorToolStripButton.Text = "Измерение фона";
                    break;
                case ProgramStates.BackgroundMeasureReady:
                    unblock_butt.Text = "Включить блокировку";
                    unblock_butt.ForeColor = Color.Red;
                    setButtons(false, false, true, true, false, false, true, false);
                    monitorToolStripButton.Text = "Начать мониторинг";
                    break;
                case ProgramStates.Measure:
                    setButtons(false, false, true, true, false, false, false, false);
                    unblock_butt.Text = "Включить блокировку";
                    unblock_butt.ForeColor = Color.Red;
                    monitorToolStripButton.Text = "Режим мониторинга";
                    break;
                case ProgramStates.WaitInit:
                case ProgramStates.WaitShutdown:
                case ProgramStates.Shutdown:
                    setButtons(false, false, false, false, false, false, false, true);
                    break;
            }
        }
        void setButtons(bool connect, bool init, bool shutdown, bool block, bool scan, bool precise, bool monitor, bool measureOptions) {
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

        // TODO: another handler
        void InvokeCancelScan(ProgramStates state) {
            if (this.InvokeRequired) {
                this.BeginInvoke(new ProgramEventHandler(CancelScan), state);
                return;
            }
            CancelScan(state);
        }
        void CancelScan(ProgramStates state) {
            commander.MeasureCancelled -= InvokeCancelScan;
            commander.ErrorOccured -= Commander_OnError;
            
            if (CollectorsForm.Visible)
                CollectorsForm.deactivateOnMeasureStop();
            else if (MonitorForm.Visible)
                MonitorForm.deactivateOnMeasureStop();
        }

        void ToolBarToolStripMenuItem_Click(object sender, EventArgs e) {
            controlToolStrip.Visible = toolBarToolStripMenuItem.Checked;
        }

        void StatusBarToolStripMenuItem_Click(object sender, EventArgs e) {
            statusStrip.Visible = statusBarToolStripMenuItem.Checked;
        }

        void ParameterToolStripMenuItem_Click(object sender, EventArgs e) {
            parameterPanel.Visible = ParameterToolStripMenuItem.Checked;
        }

        void openConfigFileToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                Config.loadGlobalConfig();
            } catch (Config.ConfigLoadException cle) {
                cle.visualise();
            }
        }

        void saveConfigFileToolStripMenuItem_Click(object sender, EventArgs e) {
            Config.saveGlobalConfig();
        }

        void delaysToolStripMenuItem_Click(object sender, EventArgs e) {
            DelaysOptionsForm dForm = new DelaysOptionsForm();
            dForm.ShowDialog();
        }

        void openSpecterFileToolStripMenuItem_Click(object sender, EventArgs e) {
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

        void closeAllToolStripMenuItem_Click(object sender, EventArgs e) {
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