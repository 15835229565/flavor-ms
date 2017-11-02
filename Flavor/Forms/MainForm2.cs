using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Flavor.Common;
using Flavor.Common.Data.Controlled;
// data model
using Graph = Flavor.Common.Data.Measure.Graph;
// divide into 2 parts
using Config = Flavor.Common.Settings.Config;

namespace Flavor.Forms {
    using AlertLevel = StatusTreeNode.AlertLevel;
    partial class MainForm2: Form, IMSControl/*, IMeasured*/ {
        const bool USING_COMPLEX_INLET = true;

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

        const string SYSTEM_START_TEXT = "Запуск";
        const string SYSTEM_INIT_TEXT = "Инициализация системы";
        const string VACUUM_INIT_TEXT = "Инициализация вакуума";
        const string WAIT_HVOLTAGE_TEXT = "Ожидание высокого напряжения";
        const string SYSTEM_READY_TEXT = "Готова к измерению";
        const string SYSTEM_ERROR_TEXT = "Ошибка";

        const string ON_TEXT = "Включен";
        const string ON_TEXT1 = "Включено";
        const string OFF_TEXT = "Выключен";
        const string OFF_TEXT1 = "Выключено";
        const string OPENED_TEXT = "Открыт";
        const string CLOSED_TEXT = "Закрыт";

        EventHandler<CallBackEventArgs<bool, string>> _connect;
        public event EventHandler<CallBackEventArgs<bool, string>> Connect {
            add {
                _connect += value;
                connectToolStripButton.Visible = _connect != null;
            }
            remove {
                _connect -= value;
                connectToolStripButton.Visible = _connect != null;
            }
        }
        protected virtual void OnConnect(CallBackEventArgs<bool, string> e) {
            _connect.Raise(this, e);
        }
        EventHandler<CallBackEventArgs<bool>> _init;
        public event EventHandler<CallBackEventArgs<bool>> Init {
            add {
                _init += value;
                initSys_butt.Visible = _init != null;
            }
            remove {
                _init -= value;
                initSys_butt.Visible = _init != null;
            }
        }
        protected virtual void OnInit(CallBackEventArgs<bool> e) {
            _init.Raise(this, e);
        }
        EventHandler<CallBackEventArgs<bool>> _shutdown;
        public event EventHandler<CallBackEventArgs<bool>> Shutdown {
            add {
                _shutdown += value;
                shutSys_butt.Visible = _shutdown != null;
            }
            remove {
                _shutdown -= value;
                shutSys_butt.Visible = _shutdown != null;
            }
        }
        protected virtual void OnShutdown(CallBackEventArgs<bool> e) {
            _shutdown.Raise(this, e);
        }
        EventHandler<CallBackEventArgs<bool>> _unblock;
        public event EventHandler<CallBackEventArgs<bool>> Unblock {
            add {
                _unblock += value;
                unblock_butt.Visible = _unblock != null;
            }
            remove {
                _unblock -= value;
                unblock_butt.Visible = _unblock != null;
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
                    collectorsForm = new MeasuredCollectorsForm { MdiParent = this };
                }
                return collectorsForm;
            }
        }
        MonitorForm monitorForm = null;
        MonitorForm MonitorForm {
            get {
                if (monitorForm == null) {
                    monitorForm = new MonitorForm { MdiParent = this };
                }
                return monitorForm;
            }
        }
        OptionsForm2 oForm = null;
        readonly Commander commander;
        public MainForm2(Commander commander)
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
        readonly StatusTreeNode systemStateTreeNode = StatusTreeNode.newLeaf();
        readonly StatusTreeNode vacuumStateTreeNode = StatusTreeNode.newLeaf();
        readonly StatusTreeNode forPumpOnTreeNode = StatusTreeNode.newLeaf();
        readonly StatusTreeNode turboPumpOnTreeNode = StatusTreeNode.newLeaf();
        readonly StatusTreeNode forVacuumTreeNode = StatusTreeNode.newLeaf();
        readonly StatusTreeNode highVacuumTreeNode = StatusTreeNode.newLeaf();
        readonly StatusTreeNode highVOnTreeNode = StatusTreeNode.newLeaf();
        readonly StatusTreeNode vGate1TreeNode = StatusTreeNode.newLeaf();
        readonly StatusTreeNode vGate2TreeNode = StatusTreeNode.newLeaf();

        readonly StatusTreeNode fV1TreeNode = StatusTreeNode.newLeaf();
        readonly StatusTreeNode fV2TreeNode = StatusTreeNode.newLeaf();
        readonly StatusTreeNode scanVoltageTreeNode = StatusTreeNode.newLeaf();
        readonly StatusTreeNode iVoltageTreeNode = StatusTreeNode.newLeaf();
        readonly StatusTreeNode eCurrentTreeNode = StatusTreeNode.newLeaf();
        readonly StatusTreeNode capVPlusTreeNode = StatusTreeNode.newLeaf();
        readonly StatusTreeNode capVMinusTreeNode = StatusTreeNode.newLeaf();
        readonly StatusTreeNode detectorVTreeNode = StatusTreeNode.newLeaf();
        readonly StatusTreeNode hCurrentTreeNode = StatusTreeNode.newLeaf();

        readonly StatusTreeNode turboSpeedTreeNode = StatusTreeNode.newLeaf();
        readonly StatusTreeNode turboCurrentTreeNode = StatusTreeNode.newLeaf();
        readonly StatusTreeNode pumpTTreeNode = StatusTreeNode.newLeaf();
        readonly StatusTreeNode driveTTreeNode = StatusTreeNode.newLeaf();
        readonly StatusTreeNode pwmTreeNode = StatusTreeNode.newLeaf();
        readonly StatusTreeNode operationTimeTreeNode = StatusTreeNode.newLeaf();

        void populateStatusTreeViewNew() {
            var root = StatusTreeNode.newNode("Информация о приборе",
                StatusTreeNode.newNode("Режим работы",
                    StatusTreeNode.newNode("Состояние системы", systemStateTreeNode),
                    StatusTreeNode.newNode("Основной вентиль", vacuumStateTreeNode),//SEMV1
                    StatusTreeNode.newNode("Турбомолекулярный насос", turboPumpOnTreeNode), 
                    StatusTreeNode.newNode("Высокое напряжение", highVOnTreeNode)
                ),
                StatusTreeNode.newNode("Модуль электроники",
                    StatusTreeNode.newNode("Фокусирующее напр. (1) В", fV1TreeNode),
                    StatusTreeNode.newNode("Фокусирующее напр. (2) В", fV2TreeNode),
                    StatusTreeNode.newNode("Напряжение развертки, В", scanVoltageTreeNode),
                    StatusTreeNode.newNode("Напряжение ионизации, В", iVoltageTreeNode),
                    StatusTreeNode.newNode("Ток эмиссии, мкА", eCurrentTreeNode),
                    StatusTreeNode.newNode("Напряжение конденсатора (+), В", capVPlusTreeNode),
                    StatusTreeNode.newNode("Напряжение конденсатора (-), В", capVMinusTreeNode),
                    StatusTreeNode.newNode("Напряжение на детекторе 1, В", detectorVTreeNode),//d1V
                    StatusTreeNode.newNode("Напряжение на детекторе 2, В", forVacuumTreeNode),//d2V
                    StatusTreeNode.newNode("Напряжение на детекторе 3, В", highVacuumTreeNode)//d3V
                ),
                StatusTreeNode.newNode("Система напуска",
                    StatusTreeNode.newNode("Прокачивающий насос", forPumpOnTreeNode),
                    StatusTreeNode.newNode("Вентиль капилляра 2", vGate1TreeNode),
                    StatusTreeNode.newNode("Вентиль капилляра 3", vGate2TreeNode),
                    StatusTreeNode.newNode("Температура нагрева, C", hCurrentTreeNode),//hT
                    StatusTreeNode.newNode("Напряжение натекателя, В", turboSpeedTreeNode)//inV
                ));
            root.ExpandAll();

            statusTreeView.Nodes.Add(root);
        }

        void populateStatusTreeView() {
            var root = StatusTreeNode.newNode("Состояние системы",
                StatusTreeNode.newNode("Информация о системе",
                    StatusTreeNode.newNode("Состояние системы", systemStateTreeNode),
                    StatusTreeNode.newNode("Состояние вакуума", vacuumStateTreeNode),
                    StatusTreeNode.newNode("Форвакуумный насос", forPumpOnTreeNode),
                    StatusTreeNode.newNode("Турбомолекулярный насос", turboPumpOnTreeNode), 
                    StatusTreeNode.newNode("Уровень вакуума (фор)", forVacuumTreeNode),
                    StatusTreeNode.newNode("Уровень вакуума (высок)", highVacuumTreeNode),
                    StatusTreeNode.newNode("Высокое напряжение", highVOnTreeNode),
                    StatusTreeNode.newNode("Вакуумный вентиль 1", vGate1TreeNode),
                    StatusTreeNode.newNode("Вакуумный вентиль 2", vGate2TreeNode)),
                StatusTreeNode.newNode("Дополнительно",
                    StatusTreeNode.newNode("Фокусирующее напр. (1) В", fV1TreeNode),
                    StatusTreeNode.newNode("Фокусирующее напр. (2) В", fV2TreeNode),
                    StatusTreeNode.newNode("Напряжение развертки, В", scanVoltageTreeNode),
                    StatusTreeNode.newNode("Напряжение ионизации, В", iVoltageTreeNode),
                    StatusTreeNode.newNode("Ток эмиссии, мкА", eCurrentTreeNode),
                    StatusTreeNode.newNode("Напряжение конденсатора (+), В", capVPlusTreeNode),
                    StatusTreeNode.newNode("Напряжение конденсатора (-), В", capVMinusTreeNode),
                    StatusTreeNode.newNode("Напряжение на детекторе, В", detectorVTreeNode),
                    StatusTreeNode.newNode("Ток нагрева, А", hCurrentTreeNode)),
                StatusTreeNode.newNode("Турбонасос",
                    StatusTreeNode.newNode("Скорость вращения, об./мин.", turboSpeedTreeNode),
                    StatusTreeNode.newNode("Ток, мА", turboCurrentTreeNode),
                    StatusTreeNode.newNode("Температура насоса", pumpTTreeNode),
                    StatusTreeNode.newNode("Температура привода", driveTTreeNode),
                    StatusTreeNode.newNode("pwm", pwmTreeNode),
                    StatusTreeNode.newNode("Время работы, ч.", operationTimeTreeNode)));
            root.ExpandAll();

            statusTreeView.Nodes.Add(root);
        }
        #endregion
        protected sealed override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            CollectorsForm.Visible = true;
            // do not activate so early!
            //MonitorForm.Visible = false;
            var device = commander.device;
            if (device == null) {
                populateStatusTreeView();
                Device.OnDeviceStateChanged += InvokeRefreshDeviceState;
                Device.OnDeviceStatusChanged += InvokeRefreshDeviceStatus;
                Device.OnVacuumStateChanged += InvokeRefreshVacuumState;
                Device.OnTurboPumpStatusChanged += InvokeRefreshTurboPumpStatus;
                Device.OnTurboPumpAlert += InvokeProcessTurboPumpAlert;
                Device.Init();

                RefreshDeviceState();
                RefreshVacuumState();
            } else {
                populateStatusTreeViewNew();
                // BAD!
                device.DeviceStateChanged += RefreshDeviceStateAsync;
                device.DeviceStatusChanged += RefreshDeviceStatusAsync;
                device.VacuumStateChanged += RefreshVacuumStateAsync;
            }
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
                device.DeviceStatusChanged -= RefreshDeviceStatusAsync;
                device.VacuumStateChanged -= RefreshVacuumStateAsync;
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
            if (new ConnectOptionsForm(commander.AvailablePorts).ShowDialog() == DialogResult.OK)
                commander.Reconnect();
        }
        // TODO: different callbacks for different forms as parameters
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
                Action<bool, bool> tempMethod = (enabled, canApply) => { };
                ProgramEventHandler method = state => {
                    bool enabled = (state != ProgramStates.Measure ||
                        state != ProgramStates.BackgroundMeasureReady ||
                        state != ProgramStates.WaitBackgroundMeasure);
                    bool canApply = (state == ProgramStates.Ready
                        // || state == ProgramStates.WaitHighVoltage
                        );
                    tempMethod(enabled, canApply);
                };
                oForm.Load += (s, a) => {
                    var args = a.As<OptionsForm2.LoadEventArgs>();
                    tempMethod += args.Method;
                    commander.ProgramStateChanged += method;
                    method(commander.pState);
                    args.NotRareModeRequested = commander.notRareModeRequested;
                    args.CommonOptions = Config.CommonOptions;
                };
                oForm.SaveFileButtonClick += (s, a) => {
                    var ps = a.Parameters;
                    if (ps.Length == 0)
                        return;
                    Config.temp_saveCO(a.FileName, (double)ps[0], (double)ps[1], (double)ps[2], (double)ps[3], (double)ps[4], (double)ps[5], (double)ps[6], (double)ps[7], (double)ps[8], (double)ps[9]);
                    // TODO: unsubscribe
                };
                oForm.FormClosing += (s, a) => {
                    var args = (OptionsForm2.ClosingEventArgs)a;
                    commander.ProgramStateChanged -= method;
                    tempMethod -= args.Method;
                    var ps = args.Parameters;
                    if (ps != null) {
                        Config.temp_saveGO((double)ps[0], (double)ps[1], (double)ps[2], (double)ps[3], (double)ps[4], (double)ps[5], (double)ps[6], (double)ps[7], (double)ps[8], (double)ps[9]);
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
            } else
                oForm.Activate();
            // TODO: disable other menu items or close already opened?
        }

        void connectToolStripButton_Click(object sender, EventArgs e) {
            connectToolStripButton.Enabled = false;
            bool old = (bool)connectToolStripButton.Tag;
            var ee = new CallBackEventArgs<bool, string> { Value = old, Handler = null };
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
            var e = new CallBackEventArgs<bool> {
                Value = old,
                Handler = (s, ee) => Invoke(new Action(() => {
                    button.Enabled = true;
                    button.Tag = old;
                }))
            };
            action(e);
            bool res = e.Value;
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
            BeginInvoke(new Action(() => MessageBox.Show(this, msg)));
        }
        void overview_button_Click(object sender, EventArgs e) {
            commander.Scan(1);
            ChildFormInit(CollectorsForm, false);
        }
        void sparseModeToolStripMenuItem_Click(object sender, EventArgs e) {
            // TODO: select ratio in narrow limits
            commander.Scan(2);
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
            var tag = monitorToolStripButton.Tag;
            bool? result;
            if (tag == null) {
                monitorToolStripButton.Tag = 1;
                monitorToolStripButton.Text = LabelTextGen;
                result = commander.Monitor();
            } else {
                int i = (int)tag;
                result = commander.Monitor(tag);
                MonitorForm.AddLabel(i);
                ++i;
                monitorToolStripButton.Tag = i;
                monitorToolStripButton.Text = LabelTextGen;
            }
            //bool? result = tag == null ? commander.Monitor() : commander.Monitor(tag);
            if (result.HasValue) {
                if (result == true) {
                    ChildFormInit(MonitorForm, true);
                } else {
                    if (tag == null)
                        MonitorForm.AddLabel(0);
                }
            } else {
                monitorToolStripButton.Tag = null;
                MessageBox.Show(this, MONITOR_MODE_START_FAILURE_MESSAGE, MODE_START_FAILURE_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        void ChildFormInit(IMeasured form, bool isPrecise) {
            // disabled
            
            //overview_button.Enabled = false;
            //sensmeasure_button.Enabled = false;
            //monitorToolStripButton.Enabled = false;

            form.MeasureCancelRequested += ChildForm_MeasureCancelRequested;
            // order is important here!
            form.initMeasure(commander.CurrentMeasureMode.StepsCount, isPrecise);

            commander.MeasureCancelled += InvokeCancelScan;
            commander.ErrorOccured += Commander_OnError;

            if (form == CollectorsForm) {
                if (monitorForm != null)
                    MonitorForm.Hide();
            } else
                CollectorsForm.Hide();
        }
        void ChildForm_MeasureCancelRequested(object sender, EventArgs e) {
            ((IMeasured)sender).MeasureCancelRequested -= ChildForm_MeasureCancelRequested;
            commander.MeasureCancelRequested = true;
        }
        //TODO: make 2 subscribers. one for logging, another for displaying.
        void InvokeProcessTurboPumpAlert(bool isFault, byte bits) {
            string msg = new StringBuilder("Turbopump: ")
                .Append(isFault ? "failure (" : "warning (")
                .AppendFormat("{0:X2})", bits)
                .ToString();
            InvokeRefreshUserMessage(msg);
            Config.logTurboPumpAlert(msg);
        }

        void InvokeRefreshUserMessage(string msg) {
            BeginInvoke(new MessageHandler(RefreshUserMessage), msg);
        }
        void RefreshUserMessage(string msg) {
            measure_StatusLabel.Text = msg;
        }

        void RefreshDeviceStateAsync(object sender, EventArgs<byte> e) {
            byte state = e.Value;
            AlertLevel system, vGate1, turbo, hV;
            string systemText, vGate1Text, turboText, hVText;
            if (state >= 128) {
                system = AlertLevel.Error;
                systemText = SYSTEM_ERROR_TEXT;
                // TODO: log alert
            } else if (state >= 64) {
                system = AlertLevel.Ok;
                systemText = SYSTEM_READY_TEXT;
            } else {
                system = AlertLevel.Warning;
                if (state > 32) {
                    systemText = WAIT_HVOLTAGE_TEXT;
                } else if (state > 1) {
                    systemText = VACUUM_INIT_TEXT;
                } else if (state == 1) {
                    systemText = SYSTEM_INIT_TEXT;
                } else {
                    systemText = SYSTEM_START_TEXT;
                }
            }
            state >>= 1;
            if ((state & 0x1) == 1) {
                vGate1 = AlertLevel.Ok;
                vGate1Text = OPENED_TEXT;
            } else {
                vGate1 = AlertLevel.Error;
                vGate1Text = CLOSED_TEXT;
            }
            state >>= 1;
            if ((state & 0x1) == 1) {
                turbo = AlertLevel.Ok;
                turboText = ON_TEXT;
            } else {
                turbo = AlertLevel.Error;
                turboText = OFF_TEXT;
            }
            state >>= 4;
            if ((state & 0x1) == 1) {
                hV = AlertLevel.Ok;
                hVText = ON_TEXT1;
            } else {
                hV = AlertLevel.Warning;
                hVText = OFF_TEXT1;
            }

            // TODO: make separate method to avoid closure
            BeginInvoke(new Action(() => {
                statusTreeView.BeginUpdate();
                systemStateTreeNode.State = system;
                systemStateTreeNode.Text = systemText;
                vacuumStateTreeNode.State = vGate1;
                vacuumStateTreeNode.Text = vGate1Text;
                turboPumpOnTreeNode.State = turbo;
                turboPumpOnTreeNode.Text = turboText;
                highVOnTreeNode.State = hV;
                highVOnTreeNode.Text = hVText;
                statusTreeView.EndUpdate();
            }));
        }
        // TODO: Device state as method parameter (avoid thread run)
        void InvokeRefreshDeviceState() {
            BeginInvoke(new DeviceEventHandler(RefreshDeviceState));
        }
        // Device.DeviceState state
        void RefreshDeviceState() {
            statusTreeView.BeginUpdate();
            switch (Device.sysState) {
                case Device.DeviceStates.Start:
                    systemStateTreeNode.Text = SYSTEM_START_TEXT;
                    systemStateTreeNode.State = AlertLevel.Warning;
                    break;
                case Device.DeviceStates.Init:
                    systemStateTreeNode.Text = SYSTEM_INIT_TEXT;
                    systemStateTreeNode.State = AlertLevel.Warning;
                    break;
                case Device.DeviceStates.VacuumInit:
                    systemStateTreeNode.Text = VACUUM_INIT_TEXT;
                    systemStateTreeNode.State = AlertLevel.Warning;
                    break;
                case Device.DeviceStates.WaitHighVoltage:
                    systemStateTreeNode.Text = WAIT_HVOLTAGE_TEXT;
                    systemStateTreeNode.State = AlertLevel.Ok;
                    break;
                case Device.DeviceStates.Ready:
                    systemStateTreeNode.Text = SYSTEM_READY_TEXT;
                    systemStateTreeNode.State = AlertLevel.Ok;
                    break;
                case Device.DeviceStates.Measuring:
                    systemStateTreeNode.Text = "Производятся измерения";
                    systemStateTreeNode.State = AlertLevel.Ok;
                    break;
                case Device.DeviceStates.Measured:
                    systemStateTreeNode.Text = "Измерения закончены";
                    systemStateTreeNode.State = AlertLevel.Ok;
                    break;
                case Device.DeviceStates.ShutdownInit:
                    systemStateTreeNode.Text = "Инициализация выключения";
                    systemStateTreeNode.State = AlertLevel.Warning;
                    break;
                case Device.DeviceStates.Shutdowning:
                    systemStateTreeNode.Text = "Идет выключение";
                    systemStateTreeNode.State = AlertLevel.Warning;
                    break;
                case Device.DeviceStates.Shutdowned:
                    systemStateTreeNode.Text = "Выключено";
                    systemStateTreeNode.State = AlertLevel.Warning;
                    break;
                case Device.DeviceStates.TurboPumpFailure:
                    systemStateTreeNode.Text = "Отказ турбонасоса";
                    systemStateTreeNode.State = AlertLevel.Error;
                    break;
                case Device.DeviceStates.VacuumCrash:
                    systemStateTreeNode.Text = "Потеря вакуума";
                    systemStateTreeNode.State = AlertLevel.Error;
                    break;
                case Device.DeviceStates.ConstantsWrite:
                    systemStateTreeNode.Text = "Запись констант";
                    systemStateTreeNode.State = AlertLevel.Warning;
                    break;
                default:
                    systemStateTreeNode.Text = "Неизвестно";
                    systemStateTreeNode.State = AlertLevel.Error;
                    break;
            }
            statusTreeView.EndUpdate();
        }

        // TODO: turbo pump state as method parameter (avoid thread run)
        void InvokeRefreshTurboPumpStatus() {
            BeginInvoke(new DeviceEventHandler(RefreshTurboPumpStatus));
        }
        void RefreshTurboPumpStatus() {
            statusTreeView.BeginUpdate();

            turboSpeedTreeNode.Text = Device.TurboPump.Speed.ToString("f0");
            turboCurrentTreeNode.Text = Device.TurboPump.Current.ToString("f0");
            pwmTreeNode.Text = Device.TurboPump.pwm.ToString("f0");
            pumpTTreeNode.Text = Device.TurboPump.PumpTemperature.ToString("f0");
            driveTTreeNode.Text = Device.TurboPump.DriveTemperature.ToString("f0");
            operationTimeTreeNode.Text = Device.TurboPump.OperationTime.ToString("f0");

            statusTreeView.EndUpdate();
        }

        void RefreshDeviceStatusAsync(object sender, EventArgs<ValueType[]> e) {
            var data = e.Value;
            bool microPumpOn = (bool)data[2];
            string pumpText = microPumpOn ? ON_TEXT : OFF_TEXT;
            bool SEMV2 = (bool)data[0];
            string vGate2Text = SEMV2 ? OPENED_TEXT : CLOSED_TEXT;
            bool SEMV3 = (bool)data[1];
            string vGate3Text = SEMV3 ? OPENED_TEXT : CLOSED_TEXT;

            string fV1Text, fV2Text, iVText, d1VText, d2VText, d3VText, cPText, cMText, sVText, eIText, inVText, hTText;
            AlertLevel pump, vGate2, vGate3, inV, hT;
            if (data.Length == 16) {
                fV1Text = ((double)data[5]).ToString("f2");
                fV2Text = ((double)data[6]).ToString("f2");
                iVText = ((double)data[4]).ToString("f2");
                d1VText = ((double)data[7]).ToString("f1");
                d2VText = ((double)data[8]).ToString("f1");
                d3VText = ((double)data[9]).ToString("f1");
                cPText = ((double)data[10]).ToString("f2");
                cMText = ((double)data[11]).ToString("f2");
                sVText = ((double)data[12]).ToString("f1");
                eIText = ((double)data[3]).ToString("f3");

                pump = microPumpOn ? AlertLevel.Ok : AlertLevel.Warning;

                double temp = (double)data[15];
                hTText = temp.ToString("f1");

                double inletV = (double)data[14];
                inVText = inletV.ToString("f1");

                if (USING_COMPLEX_INLET) {
                    // TODO: move comparison up
                    bool inletOpened = inletV > (double)minInletV;
                    
                    if (SEMV2 == SEMV3) {
                        if (SEMV2) {
                            // capillary in use
                            vGate2 = AlertLevel.Ok;
                            vGate3 = AlertLevel.Ok;
                            // inlet must be closed in case of capillary use
                            inV = inletOpened ? AlertLevel.Error : AlertLevel.Ok;
                            // heat temperature is not important
                            hT = AlertLevel.NA;
                        } else {
                            if (inletOpened) {
                                // membrane inlet in use
                                vGate2 = AlertLevel.Ok;
                                vGate3 = AlertLevel.Ok;
                                inV = AlertLevel.Ok;
                                // heat temperature is important
                                // TODO: move comparison up, use 3-state flag instead
                                if (temp < (double)minTemp)
                                    hT = AlertLevel.Warning;
                                else if (temp > (double)maxTemp)
                                    hT = AlertLevel.Error;
                                else
                                    hT = AlertLevel.Ok;
                            } else {
                                // all inputs are closed
                                vGate2 = AlertLevel.Warning;
                                vGate3 = AlertLevel.Warning;
                                inV = AlertLevel.Warning;
                                // heat temperature is not important
                                hT = AlertLevel.NA;
                            }
                        }
                    } else {
                        // capillary error
                        vGate2 = inletOpened == SEMV2 ? AlertLevel.Error : AlertLevel.Ok;
                        vGate3 = inletOpened == SEMV3 ? AlertLevel.Error : AlertLevel.Ok;
                        // heat temperature and membrane inlet state are not important in case of capillary error
                        inV = AlertLevel.NA;
                        hT = AlertLevel.NA;
                    }
                } else {
                    inV = AlertLevel.NA;
                    if (SEMV2 == SEMV3) {
                        if (SEMV2) {
                            // inlet is opened
                            vGate2 = AlertLevel.Ok;
                            vGate3 = AlertLevel.Ok;
                            // heat temperature is important
                            // TODO: move comparison up, use 3-state flag instead
                            if (temp < (double)minTemp)
                                hT = AlertLevel.Warning;
                            else if (temp > (double)maxTemp)
                                hT = AlertLevel.Error;
                            else
                                hT = AlertLevel.Ok;
                        } else {
                            // inlet is closed
                            vGate2 = AlertLevel.Warning;
                            vGate3 = AlertLevel.Warning;
                            // heat temperature is not important
                            hT = AlertLevel.NA;
                        }
                    } else {
                        // error: only one gauge is opened
                        vGate2 = SEMV2 ? AlertLevel.Error : AlertLevel.Ok;
                        vGate3 = SEMV3 ? AlertLevel.Error : AlertLevel.Ok;
                        hT = AlertLevel.NA;
                    }
                }

            } else {
                // high voltage off
                const string NA = "---";

                fV1Text = NA;
                fV2Text = NA;
                iVText = NA;
                d1VText = NA;
                d2VText = NA;
                d3VText = NA;
                cPText = NA;
                cMText = NA;
                sVText = NA;
                eIText = NA;

                // micro pump is off when high voltage is off
                pump = AlertLevel.NA;

                if (SEMV2 == SEMV3) {
                    vGate2 = SEMV2 ? AlertLevel.Warning : AlertLevel.Ok;
                    vGate3 = vGate2;
                } else {
                    vGate2 = SEMV2 ? AlertLevel.Error : AlertLevel.Ok;
                    vGate3 = SEMV3 ? AlertLevel.Error : AlertLevel.Ok;
                }
                hT = AlertLevel.NA;
                hTText = NA;
                inV = AlertLevel.NA;
                inVText = NA;
            }

            // TODO: make separate method to avoid closure
            BeginInvoke(new Action(() => {
                statusTreeView.BeginUpdate();

                forPumpOnTreeNode.State = pump;
                forPumpOnTreeNode.Text = pumpText;

                vGate1TreeNode.State = vGate2;
                vGate1TreeNode.Text = vGate2Text;
                vGate2TreeNode.State = vGate3;
                vGate2TreeNode.Text = vGate3Text;

                fV1TreeNode.Text = fV1Text;
                fV2TreeNode.Text = fV2Text;
                iVoltageTreeNode.Text = iVText;
                capVPlusTreeNode.Text = cPText;
                capVMinusTreeNode.Text = cMText;
                scanVoltageTreeNode.Text = sVText;
                eCurrentTreeNode.Text = eIText;
                // detectors actually
                detectorVTreeNode.Text = d1VText;
                forVacuumTreeNode.Text = d2VText;
                highVacuumTreeNode.Text = d3VText;
                // inlet voltage atually
                turboSpeedTreeNode.State = inV;
                turboSpeedTreeNode.Text = inVText;
                // heat temperature actually
                hCurrentTreeNode.State = hT;
                hCurrentTreeNode.Text = hTText;

                statusTreeView.EndUpdate();
            }));
        }
        // TODO: Device status as method parameter (avoid thread run)
        void InvokeRefreshDeviceStatus() {
            BeginInvoke(new DeviceEventHandler(RefreshDeviceStatus));
        }
        void RefreshDeviceStatus() {
            statusTreeView.BeginUpdate();
            if (Device.fPumpOn) {
                forPumpOnTreeNode.State = AlertLevel.Ok;
                forPumpOnTreeNode.Text = ON_TEXT;
            } else {
                forPumpOnTreeNode.State = AlertLevel.Error;
                forPumpOnTreeNode.Text = OFF_TEXT;
            }
            if (Device.tPumpOn) {
                turboPumpOnTreeNode.State = AlertLevel.Ok;
                turboPumpOnTreeNode.Text = ON_TEXT;
            } else {
                turboPumpOnTreeNode.State = AlertLevel.Error;
                turboPumpOnTreeNode.Text = OFF_TEXT;
            }
            forVacuumTreeNode.Text = Device.fVacuumReal.ToString("e3");
            highVacuumTreeNode.Text = Device.hVacuumReal.ToString("e3");
            if (Device.highVoltageOn) {
                highVOnTreeNode.State = AlertLevel.Ok;
                highVOnTreeNode.Text = ON_TEXT1;
            } else {
                highVOnTreeNode.State = AlertLevel.Warning;
                highVOnTreeNode.Text = OFF_TEXT1;
            }
            if (Device.highVacuumValve) {
                vGate1TreeNode.State = AlertLevel.Ok;
                vGate1TreeNode.Text = OPENED_TEXT;
            } else {
                vGate1TreeNode.State = AlertLevel.Warning;
                vGate1TreeNode.Text = CLOSED_TEXT;
            }
            if (Device.probeValve) {
                vGate2TreeNode.State = AlertLevel.Warning;
                vGate2TreeNode.Text = OPENED_TEXT;
            } else {
                vGate2TreeNode.State = AlertLevel.Ok;
                vGate2TreeNode.Text = CLOSED_TEXT;
            }
            {
                var cd = Device.DeviceCommonData;
                fV1TreeNode.Text = cd.fV1Real.ToString("f2");
                fV2TreeNode.Text = cd.fV2Real.ToString("f2");
                iVoltageTreeNode.Text = cd.iVoltageReal.ToString("f2");
                detectorVTreeNode.Text = cd.dVoltageReal.ToString("f1");
                capVPlusTreeNode.Text = cd.cVPlusReal.ToString("f2");
                capVMinusTreeNode.Text = cd.cVMinReal.ToString("f2");
                scanVoltageTreeNode.Text = cd.sVoltageReal.ToString("f1");
                eCurrentTreeNode.Text = cd.eCurrentReal.ToString("f3");
                hCurrentTreeNode.Text = cd.hCurrentReal.ToString("f3");
            }
            turboSpeedTreeNode.Text = Device.TurboPump.Speed.ToString("f0");

            statusTreeView.EndUpdate();
        }

        void RefreshVacuumStateAsync(object sender, EventArgs<ValueType[]> e) {
            // TODO:!
            //BeginInvoke();
        }
        // TODO: vacuum state as method parameter (avoid thread run)
        void InvokeRefreshVacuumState() {
            BeginInvoke(new DeviceEventHandler(RefreshVacuumState));
        }
        // Device.VacuumStates state
        void RefreshVacuumState() {
            statusTreeView.BeginUpdate();
            switch (Device.vacState) {
                case Device.VacuumStates.Idle:
                    vacuumStateTreeNode.Text = "Бездействие";
                    vacuumStateTreeNode.State = AlertLevel.Warning;
                    break;
                case Device.VacuumStates.Init:
                    vacuumStateTreeNode.Text = "Инициализация";
                    vacuumStateTreeNode.State = AlertLevel.Warning;
                    break;
                case Device.VacuumStates.StartingForvacuumPump:
                    vacuumStateTreeNode.Text = "Включение форнасоса";
                    vacuumStateTreeNode.State = AlertLevel.Warning;
                    break;
                case Device.VacuumStates.PumpingForvacuum:
                    vacuumStateTreeNode.Text = "Откачка форвакуума";
                    vacuumStateTreeNode.State = AlertLevel.Warning;
                    break;
                case Device.VacuumStates.DelayPumpingHighVacuumByForvac:
                    vacuumStateTreeNode.Text = "Задержка высокого вакуума из-за фор";
                    vacuumStateTreeNode.State = AlertLevel.Warning;
                    break;
                case Device.VacuumStates.PumpingHighVacuumByForvac:
                    vacuumStateTreeNode.Text = "Откачка высокого вакуума форнасосом";
                    vacuumStateTreeNode.State = AlertLevel.Warning;
                    break;
                case Device.VacuumStates.PumpingHighVacuumByTurbo:
                    vacuumStateTreeNode.Text = "Откачка турбо";
                    vacuumStateTreeNode.State = AlertLevel.Warning;
                    break;
                case Device.VacuumStates.Ready:
                    vacuumStateTreeNode.Text = "Готово";
                    vacuumStateTreeNode.State = AlertLevel.Ok;
                    break;
                case Device.VacuumStates.ShutdownInit:
                    vacuumStateTreeNode.Text = "Инициализация отключения";
                    vacuumStateTreeNode.State = AlertLevel.Warning;
                    break;
                case Device.VacuumStates.ShutdownDelay:
                    vacuumStateTreeNode.Text = "Задержка отключения";
                    vacuumStateTreeNode.State = AlertLevel.Warning;
                    break;
                case Device.VacuumStates.ShutdownPumpProbe:
                    vacuumStateTreeNode.Text = "Отключение датчика насоса";
                    vacuumStateTreeNode.State = AlertLevel.Warning;
                    break;
                case Device.VacuumStates.Shutdowned:
                    vacuumStateTreeNode.Text = "Отключено";
                    vacuumStateTreeNode.State = AlertLevel.Warning;
                    break;
                case Device.VacuumStates.ShutdownStartingTurboPump:
                    vacuumStateTreeNode.Text = "Откачка при выключении";
                    vacuumStateTreeNode.State = AlertLevel.Warning;
                    break;
                case Device.VacuumStates.BadHighVacuum:
                    vacuumStateTreeNode.Text = "Плохой высокий вакуум";
                    vacuumStateTreeNode.State = AlertLevel.Error;
                    break;
                case Device.VacuumStates.BadForvacuum:
                    vacuumStateTreeNode.Text = "Плохой форвакуум";
                    vacuumStateTreeNode.State = AlertLevel.Error;
                    break;
                case Device.VacuumStates.ForvacuumFailure:
                    vacuumStateTreeNode.Text = "Отказ форвакуума";
                    vacuumStateTreeNode.State = AlertLevel.Error;
                    break;
                case Device.VacuumStates.LargeLeak:
                    vacuumStateTreeNode.Text = "Большая течь";
                    vacuumStateTreeNode.State = AlertLevel.Error;
                    break;
                case Device.VacuumStates.SmallLeak:
                    vacuumStateTreeNode.Text = "Малая течь";
                    vacuumStateTreeNode.State = AlertLevel.Error;
                    break;
                case Device.VacuumStates.ThermoCoupleFailure:
                    vacuumStateTreeNode.Text = "Отказ термопары";
                    vacuumStateTreeNode.State = AlertLevel.Error;
                    break;
                case Device.VacuumStates.TurboPumpFailure:
                    vacuumStateTreeNode.Text = "Отказ турбонасоса";
                    vacuumStateTreeNode.State = AlertLevel.Error;
                    break;
                case Device.VacuumStates.VacuumShutdownProbeLeak:
                    vacuumStateTreeNode.Text = "Отключение датчика вакуума";
                    vacuumStateTreeNode.State = AlertLevel.Error;
                    break;
                default:
                    vacuumStateTreeNode.Text = "Неизвестное состояние";
                    vacuumStateTreeNode.State = AlertLevel.Error;
                    break;
            }
            statusTreeView.EndUpdate();
        }

        void InvokeRefreshButtons(ProgramStates state) {
            if (InvokeRequired) {
                BeginInvoke(new ProgramEventHandler(RefreshButtons), state);
                return;
            }
            RefreshButtons(state);
        }
        string LabelTextGen {
            get {
                // TODO: move to resource file
                const string LABEL_TEXT = "Метка №";
                var tag = monitorToolStripButton.Tag;
                return tag == null ? LABEL_TEXT : LABEL_TEXT + (int)tag;
            }
        }
        // bool block, ProgramStates state, bool connected, bool canDoPrecise
        // use setButtons signature..
        void RefreshButtons(ProgramStates state) {
            // TODO: move to resource file
            const string MONITOR_BUTTON_TEXT = "Режим мониторинга";
            const string BACKGROUND_TEXT = "Измерение фона";
            const string START_MONITOR = "Начать мониторинг";
            const string HBLOCK_ON = "Включить блокировку";
            const string HBLOCK_OFF = "Снять блокировку";
            switch (state) {
                case ProgramStates.Start:
                    bool connected = (bool)connectToolStripButton.Tag;
                    setButtons(true, connected, connected, false, false, false, false, true);
                    break;
                case ProgramStates.Init:
                    setButtons(false, false, true, false, false, false, false, true);
                    break;
                case ProgramStates.WaitHighVoltage:
                    unblock_butt.Text = HBLOCK_OFF;
                    unblock_butt.ForeColor = Color.Green;
                    setButtons(false, false, true, true, false, false, false, true);
                    break;
                case ProgramStates.Ready:
                    unblock_butt.Text = HBLOCK_ON;
                    unblock_butt.ForeColor = Color.Red;
                    bool canDoPrecise = commander.SomePointsUsed;
                    setButtons(false, false, true, true, true, canDoPrecise, canDoPrecise, true);
                    monitorToolStripButton.Text = MONITOR_BUTTON_TEXT;
                    // undo init
                    monitorToolStripButton.Tag = null;
                    break;
                case ProgramStates.WaitBackgroundMeasure:
                    unblock_butt.Text = HBLOCK_ON;
                    unblock_butt.ForeColor = Color.Red;
                    setButtons(false, false, true, true, false, false, false, false);
                    monitorToolStripButton.Text = BACKGROUND_TEXT;
                    // undo init
                    monitorToolStripButton.Tag = null;
                    break;
                case ProgramStates.BackgroundMeasureReady:
                    unblock_butt.Text = HBLOCK_ON;
                    unblock_butt.ForeColor = Color.Red;
                    setButtons(false, false, true, true, false, false, true, false);
                    monitorToolStripButton.Text = START_MONITOR;
                    // undo init
                    monitorToolStripButton.Tag = null;
                    break;
                case ProgramStates.Measure:
                    unblock_butt.Text = HBLOCK_ON;
                    unblock_butt.ForeColor = Color.Red;
                    // BAD!
                    if (commander.CurrentMeasureMode is Flavor.Common.Data.Measure.MeasureMode.Precise.Monitor) {
                        setButtons(false, false, true, true, false, false, true, false);
                        monitorToolStripButton.Text = LabelTextGen;
                    } else {
                        setButtons(false, false, true, true, false, false, false, false);
                        monitorToolStripButton.Text = MONITOR_BUTTON_TEXT;
                    }
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

            //test
            inletToolStripButton.Enabled = scan;
            
            overviewSplitButton.Enabled = scan;
            sensmeasure_button.Enabled = precise;
            monitorToolStripButton.Enabled = monitor;
            if (!monitor)
                monitorToolStripButton.Tag = null;

            connectToolStripMenuItem.Enabled = connect;
            measureToolStripMenuItem.Enabled = measureOptions;
        }

        // TODO: another handler
        void InvokeCancelScan(ProgramStates state) {
            if (InvokeRequired) {
                BeginInvoke(new ProgramEventHandler(CancelScan), state);
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
            new DelaysOptionsForm().ShowDialog();
        }

        void openSpecterFileToolStripMenuItem_Click(object sender, EventArgs e) {
            openSpecterFileDialog.Filter = string.Format("{0}|{1}", Config.SPECTRUM_FILE_DIALOG_FILTER, Config.PRECISE_SPECTRUM_FILE_DIALOG_FILTER);
            if (openSpecterFileDialog.ShowDialog() == DialogResult.OK) {
                foreach (Form childForm in MdiChildren) {
                    if (childForm is ILoaded && string.Equals(((ILoaded)childForm).FileName, openSpecterFileDialog.FileName)) {
                        childForm.Activate();
                        return;
                    }
                }
                try {
                    bool hint = (openSpecterFileDialog.FilterIndex == 1);
                    Graph graph;
                    string fileName = openSpecterFileDialog.FileName;
                    bool res = !Config.openSpectrumFile(fileName, hint, out graph);
                    var form = new LoadedCollectorsForm(graph, fileName, res);
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
            Form form = (Form)sender;
            // buggy workaround to refresh MdiWindowListItem
            form.Hide();
            form.Show();
        }

        void closeAllToolStripMenuItem_Click(object sender, EventArgs e) {
            foreach (Form childForm in MdiChildren) {
                if (childForm != collectorsForm && childForm != monitorForm)
                    childForm.Close();
            }
        }
        // temporary solution. move values from form to proper place!
        decimal minTemp = 40;
        decimal maxTemp = 50;
        decimal minInletV = 2400;
        // TODO: use form polymorphism. move most of code below inside forms.
        void inletToolStripButton_Click(object sender, EventArgs e) {
            if (USING_COMPLEX_INLET)
                complexInletSettings(e);
            else
                doubleMembraneInletSettings(e);
        }
        void complexInletSettings(EventArgs e) {
            var form = new Almazov.InletControlForm();
            form.Load += (s, ee) => {
                if (ee is ParamsEventArgs<decimal>) {
                    var args = (ParamsEventArgs<decimal>)ee;
                    // TODO: use some current values to feed form
                    // temporary solution. move values from form to proper place
                    args.Parameters = new[] { 2500, 3000, 2700, minTemp, maxTemp, maxTemp };
                }
            };
            form.FormClosing += (s, ee) => {
                if (form.DialogResult != DialogResult.OK)
                    return;
                if (ee is Almazov.InletControlForm.ClosingEventArgs) {
                    var args = (Almazov.InletControlForm.ClosingEventArgs)ee;

                    bool? useCapillary = args.UseCapillary;
                    var ps = args.Parameters;
                    var inletData = new List<ushort>(2);
                    if (useCapillary.HasValue && useCapillary.Value == false) {
                        inletData.Add(limitBy12Bits(ps[0] / 3000));
                    }
                    if (ps.Length > 1) {
                        inletData.Add(limitBy12Bits((ps[1] + 273) / 500));
                    }
                    ((AlmazovCommander)commander).SendInletSettings(useCapillary, inletData.ToArray());
                } 
            };
            form.ShowDialog();
        }
        void doubleMembraneInletSettings(EventArgs e) {
            var form = new Almazov.DoubleMembraneInletControlForm();
            form.Load += (s, ee) => {
                if (ee is ParamsEventArgs<decimal>) {
                    var args = (ParamsEventArgs<decimal>)ee;
                    // TODO: use some current values to feed form
                    // temporary solution. move values from form to proper place
                    args.Parameters = new[] { minTemp, maxTemp, maxTemp };
                }
            };
            form.FormClosing += (s, ee) => {
                if (form.DialogResult != DialogResult.OK)
                    return;
                if (ee is Almazov.DoubleMembraneInletControlForm.ClosingEventArgs) {
                    var args = (Almazov.DoubleMembraneInletControlForm.ClosingEventArgs)ee;

                    var ps = args.Parameters;
                    var inletData = new List<ushort>(1);
                    if (ps.Length > 0) {
                        inletData.Add(limitBy12Bits((ps[0] + 273) / 500));
                    }
                    ((AlmazovCommander)commander).SendInletSettings(args.Open, inletData.ToArray());
                }
            };
            form.ShowDialog();
        }
        // TODO: move to device section
        ushort limitBy12Bits(decimal value) {
            ushort res = (ushort)(value * 4096);
            if (res > 4095)
                res = 4095;
            return res;
        }
    }
}